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
     */

     /// <summary>
     /// Analyzes all potential end-of-season scenarios for a division
     /// </summary>
    public class DivisionScenarioAnalyzer
    {
        private League mLeague = null;
        public List<int> TeamIds = null;
        public Dictionary<int, List<int>> WinnerCounts = null;

        private DivisionWinnerCalculator mJudge = null;

        private double TotalCalculations = 0;
        private long JudgementsDone = 0;
        public int TieOutcomes = 0;

        private int mUpdateCounter = 0;

        public static DivisionScenarioAnalyzer Analyze(League league, string conferenceName, string divisionName, Action<long, double, TimeSpan> progressCallback, Action<List<string>, TimeSpan> finishedCallback)
        {
            return new DivisionScenarioAnalyzer(league, conferenceName, divisionName, progressCallback, finishedCallback);
        }

        private DivisionScenarioAnalyzer(League league, string conferenceName, string divisionName, Action<long,double,TimeSpan> progressCallback, Action<List<string>, TimeSpan> finishedCallback)
        {
            mLeague = league;

            Division division = league.FindDivision(conferenceName, divisionName);

            //Prepare the list of team Ids and division winner counts
            TeamIds = Team.GetTeamIds(division.Teams);
            WinnerCounts = new Dictionary<int, List<int>>();
            lock (WinnerCounts)
            {
                List<int> counts = new List<int>(Constants.NumTiebreakers + 2);
                for (int i = 0; i < Constants.NumTiebreakers + 2; i++)
                {
                    counts.Add(0);
                }

                foreach (int teamId in TeamIds)
                {
                    //Entries for outright winner, 1/2/3/etc.-way tie-breakers, and no-win ties
                    WinnerCounts.Add(teamId, new List<int>(counts));
                }
            }

            //Get all games involving division teams
            GameList games = division.AllGames;

            mJudge = new DivisionWinnerCalculator(league, division, games, (winner, tiedTeams) =>
            {
                //This is the judgment callback
                JudgementsDone++;
                lock (WinnerCounts)
                {
                    //Record the winner and tie-ers
                    if (winner > 0)
                    {
                        WinnerCounts[winner][tiedTeams.Count - 1]++;
                    }
                    else
                    {
                        TieOutcomes++;
                        foreach (int teamId in tiedTeams)
                        {
                            WinnerCounts[teamId][WinnerCounts[teamId].Count - 1]++;
                        }
                    }
                }

                progressCallback(JudgementsDone, TotalCalculations, DateTime.Now - mJudge.StartTime);
            });

            //Explore every permutation of game scenarios
            GamePermutator.PermutateGames(games, (allGames, allWinners, teamResults, completedPermutations, totalPermutations) =>
            {
                TotalCalculations = totalPermutations;

                mUpdateCounter++;
                if (mUpdateCounter > totalPermutations / 100)
                {
                    Console.WriteLine($"{(int)Math.Round(completedPermutations / totalPermutations * 100)}% done permuting");
                    mUpdateCounter = 0;
                }

                //Check for final callback (signalled by homeWinners == null)
                if (allWinners == null)
                {
                    //Wait for the judge to catch up
                    mJudge.FinishSession((a,b) =>
                    {
                        Finish(finishedCallback);
                    });

                    return;
                }

                //Throttle so we don't overload the judge
                while (mJudge.Backlog > 1000000)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }

                //Determine who won the permutation (async)
                mJudge.Judge(allWinners, teamResults);
            });
        }

        private void Finish(Action<List<string>, TimeSpan> finishedCallback)
        {
            List<string> results = new List<string>();
            lock (WinnerCounts)
            {
                results.Add($"Analyzed {TotalCalculations} possible outcomes in {(mJudge.EndTime - mJudge.StartTime)}");
                results.Add($"{TieOutcomes / TotalCalculations * 100:0.00}% result in unbreakable tie");
                for (int i = 0; i < mJudge.ScenariosCalculated.Count; i++)
                {
                    results.Add($"Spent {mJudge.ScenariosCalculated[i] / 1000} s on {mJudge.ScenariosCalculated[i]} {i + 1}-winner scenarios ({mJudge.ScenarioTimes[i] / mJudge.ScenariosCalculated[i]} ms/calc)");
                }
                results.Add("Team,OutrightWinner,TieMember,[WinnerCounts]");
                foreach (int teamId in WinnerCounts.Keys)
                {
                    int totalWins = 0;
                    for (int i = 0; i < WinnerCounts[teamId].Count - 1; i++)
                    {
                        totalWins += WinnerCounts[teamId][i];
                    }
                    Team team = mLeague.FindTeam(teamId);
                    results.Add($"{team.Name},{totalWins / TotalCalculations * 100:0.00}%,{WinnerCounts[teamId][WinnerCounts[teamId].Count - 1] / (double)TieOutcomes * 100:0.00}%,{string.Join(", ", WinnerCounts[teamId])}");
                }
            }

            finishedCallback(results, mJudge.EndTime - mJudge.StartTime);
        }
    }
}
