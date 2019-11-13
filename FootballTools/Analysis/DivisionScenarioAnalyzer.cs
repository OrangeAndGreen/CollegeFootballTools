using FootballTools.Entities;
using System;
using System.Collections.Generic;
using System.Threading;

namespace FootballTools.Analysis
{
    /*
     * Brainstorming:
     * Want to play out every scenario for the remaining games
     *
     * New ideas: 20191113
     *  -Have two ways to run the calculations: fast and detailed
     *      Fast is the existing multi-threaded way
     *      Detailed is single-threaded, but supports more results
     *          Single-team analysis
     *  -If a team is selected, analyze must-win scenarios in detailed mode
     *
     */

     /// <summary>
     /// Analyzes all potential end-of-season scenarios for a division
     /// </summary>
    public class DivisionScenarioAnalyzer
    {
        private League mLeague = null;
        public List<int> TeamIds = null;
        private Team mTeam = null;
        

        private GameList mGames = null;
        private Division mDivision = null;
        private DateTime mStartTime = DateTime.Now;

        private DivisionWinnerCalculator mJudge = null;

        private double TotalCalculations = 0;
        private long JudgementsDone = 0;
        public int TieOutcomes = 0;
        public Dictionary<int, int> WinnerCounts = null;
        public Dictionary<int, int> TieCounts = null;
        private Dictionary<int, int> mCommonGamesForTargetDivisionWinner = null; //Key=GameId, Value=WinnerId

        private Action<long, double, TimeSpan> mProgressCallback = null;
        private Action<List<string>, TimeSpan> mFinishedCallback = null;

        private int mUpdateCounter = 0;

        public List<int> ScenariosCalculated { get; set; }
        public List<double> ScenarioTimes { get; set; }

        public static DivisionScenarioAnalyzer Analyze(League league, string conferenceName, string divisionName, string teamName, bool fastMode, Action<long, double, TimeSpan> progressCallback, Action<List<string>, TimeSpan> finishedCallback)
        {
            return new DivisionScenarioAnalyzer(league, conferenceName, divisionName, teamName, fastMode, progressCallback, finishedCallback);
        }

        private DivisionScenarioAnalyzer(League league, string conferenceName, string divisionName, string teamName, bool fastMode, Action<long,double,TimeSpan> progressCallback, Action<List<string>, TimeSpan> finishedCallback)
        {
            mLeague = league;
            mTeam = teamName != null ? league.FindTeam(teamName) : null;

            mProgressCallback = progressCallback;
            mFinishedCallback = finishedCallback;

            ScenariosCalculated = new List<int> { 0, 0, 0 };
            ScenarioTimes = new List<double> { 0, 0, 0 };

            mDivision = league.FindDivision(conferenceName, divisionName);

            //Prepare the list of team Ids and division winner counts
            TeamIds = Team.GetTeamIds(mDivision.Teams);
            WinnerCounts = new Dictionary<int, int>();
            TieCounts = new Dictionary<int, int>();
            lock (WinnerCounts)
            {
                foreach (int teamId in TeamIds)
                {
                    //Entries for outright winner, 1/2/3/etc.-way tie-breakers, and no-win ties
                    WinnerCounts.Add(teamId, 0);
                    TieCounts.Add(teamId, 0);
                }
            }

            //Get all games involving division teams
            mGames = mDivision.AllGames;

            if (fastMode)
            {
                mJudge = new DivisionWinnerCalculator(league, mDivision, mGames, JudgmentReady);
            }

            //Explore every permutation of game scenarios
            GamePermutator.PermutateGames(mGames, PermutationReady);
        }

        private void PermutationReady(GameList allGames, List<int> allWinners, List<TeamResult> teamResults, int completedPermutations, double totalPermutations)
        {
            TotalCalculations = totalPermutations;

            mUpdateCounter++;
            if (mUpdateCounter > totalPermutations / 100)
            {
                Console.WriteLine($"{(int)Math.Round(completedPermutations / totalPermutations * 100)}% done permuting");
                mUpdateCounter = 0;
            }

            //Check for final callback (signaled by homeWinners == null)
            if (allWinners == null)
            {
                //Wait for the judge to catch up
                if (mJudge != null)
                {
                    mJudge.FinishSession((a, b) => { Finish(mFinishedCallback); });
                }
                else
                {
                    Finish(mFinishedCallback);
                }

                return;
            }

            if (mJudge != null)
            {
                //Throttle so we don't overload the judge
                while (mJudge.Backlog > 1000000)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }

                //Determine who won the permutation (async)
                mJudge.Judge(allWinners, teamResults);
            }
            else
            {
                DivisionResult result = DivisionWinnerCalculator.JudgeSync(mGames, mDivision, TeamIds, allWinners, teamResults);
                JudgmentReady(result.WinnerId, result.DivisionFinalistIds, result.Milliseconds, allWinners);
            }
        }

        private void JudgmentReady(int winner, List<int> tiedTeams, double milliseconds, List<int> allWinners)
        {
            TimeSpan elapsed = DateTime.Now - mStartTime;

            JudgementsDone++;
            lock (WinnerCounts)
            {
                ScenariosCalculated[Math.Min(2, tiedTeams.Count - 1)]++;
                ScenarioTimes[Math.Min(2, tiedTeams.Count - 1)] += milliseconds;

                //Record the winner and tie-ers
                if (winner > 0)
                {
                    WinnerCounts[winner]++;
                }
                else
                {
                    TieOutcomes++;
                    foreach (int teamId in tiedTeams)
                    {
                        TieCounts[teamId]++;
                    }
                }

                if (mTeam != null)
                {
                    if (winner == mTeam.Id || winner <= 0 && tiedTeams.Contains(mTeam.Id))
                    {
                        //private Dictionary<int, int> mCommonGamesForTargetDivisionWinner = null; //Key=GameId, Value=WinnerId
                        if (mCommonGamesForTargetDivisionWinner == null)
                        {
                            //Initialize the list
                            mCommonGamesForTargetDivisionWinner = new Dictionary<int, int>();
                            for (int i = 0; i < mGames.Count; i++)
                            {
                                if (!mGames[i].GameAlreadyPlayed)
                                {
                                    mCommonGamesForTargetDivisionWinner[mGames[i].Id] = allWinners[i];
                                }
                            }
                        }
                        else
                        {
                            //Remove any games from the common list that don't match with this outcome
                            for (int i = 0; i < mGames.Count; i++)
                            {
                                if (mCommonGamesForTargetDivisionWinner.ContainsKey(mGames[i].Id))
                                {
                                    int commonWinner = mCommonGamesForTargetDivisionWinner[mGames[i].Id];
                                    if (commonWinner != allWinners[i])
                                    {
                                        mCommonGamesForTargetDivisionWinner.Remove(mGames[i].Id);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            mProgressCallback(JudgementsDone, TotalCalculations, elapsed);
        }

        private void Finish(Action<List<string>, TimeSpan> finishedCallback)
        {
            TimeSpan elapsed = DateTime.Now - mStartTime;

            List<string> results = new List<string>();
            lock (WinnerCounts)
            {
                results.Add($"Analyzed {TotalCalculations} possible outcomes in {elapsed}");
                results.Add($"{TieOutcomes / TotalCalculations * 100:0.00}% result in unbreakable tie ({TieOutcomes} outcomes)");
                for (int i = 0; i < ScenariosCalculated.Count; i++)
                {
                    results.Add($"Spent {ScenariosCalculated[i] / 1000} s on {ScenariosCalculated[i]} {i + 1}-winner scenarios ({ScenarioTimes[i] / ScenariosCalculated[i]} ms/calc)");
                }
                results.Add("Team,OutrightWinner,TieMember");
                foreach (int teamId in WinnerCounts.Keys)
                {
                    Team team = mLeague.FindTeam(teamId);
                    results.Add($"{team.Name},{WinnerCounts[teamId] / TotalCalculations * 100:0.00}%,{TieCounts[teamId] / (double)TieOutcomes * 100:0.00}%");
                }

                if (mTeam != null)
                {
                    results.Add(string.Empty);
                    results.Add($"{mTeam.Name} required game outcomes:");
                    foreach (int gameId in mCommonGamesForTargetDivisionWinner.Keys)
                    {
                        Game game = mGames.FindMatchup(gameId);
                        int winnerId = mCommonGamesForTargetDivisionWinner[gameId];
                        Team winner = mLeague.FindTeam(winnerId);
                        int loserId = winnerId == game.HomeTeamId ? game.AwayTeamId : game.HomeTeamId;
                        Team loser = mLeague.FindTeam(loserId);
                        results.Add($"{winner.Name} beats {loser.Name}");
                    }
                }
            }

            finishedCallback(results, elapsed);
        }
    }
}
