using FootballTools.Entities;
using System;
using System.Collections.Generic;

namespace FootballTools.Analysis
{
    /*
     * Brainstorming:
     * Want to play out every scenario for the remaining games
     *
     */
    public class DivisionScenarioAnalyzer
    {
        public List<string> TeamNames = null;
        public Dictionary<string, List<int>> WinnerCounts = null;

        private DivisionWinnerCalculator mJudge = null;

        private double TotalCalculations = 0;
        private long CalculationsDone = 0;
        private long JudgementsDone = 0;
        private int LastUpdateCounter = 0;
        public int TieOutcomes = 0;

        public void Analyze(League league, string conferenceName, string divisionName, DivisionWinnerCalculator judge, Action<string,double> progressCallback, Action<List<string>> finishedCallback)
        {
            mJudge = judge;
            Division division = league.FindDivision(conferenceName, divisionName);
            //Prepare the list of team names and division winner counts
            TeamNames = new List<string>();
            WinnerCounts = new Dictionary<string, List<int>>();
            lock (WinnerCounts)
            {
                foreach(Team team in division.Teams)
                {
                    TeamNames.Add(team.Name);
                    //Entries for outright winner, 1/2/3/etc.-way tie-breakers, and no-win ties
                    List<int> counts = new List<int>();
                    for (int i = 0; i < Constants.NumTiebreakers + 2; i++)
                    {
                        counts.Add(0);
                    }
                    WinnerCounts.Add(team.Name, counts);
                }
            }

            //Filter down to conference games
            List<Game> games = new List<Game>();
            int completedGames = 0;
            foreach (Game game in division.AllGames)
            {
                if (game.ConferenceGame)
                {
                    games.Add(game);
                    if(game.GameAlreadyPlayed)
                    {
                        completedGames++;
                    }
                }
            }

            TotalCalculations = Math.Pow(2, games.Count - completedGames);

            //Explore every permutation of game scenarios
            GamePermutator permutator = new GamePermutator();
            permutator.PermutateGames(games, (allGames, homeWinners) =>
            {
                CalculationsDone++;
                //Check for final callback (signalled by homeWinners == null)
                if (homeWinners == null)
                {
                    //Wait for the judge to catch up
                    mJudge.Signal((a,b) =>
                    {
                        List<string> results = new List<string>();
                        lock (WinnerCounts)
                        {
                            results.Add($"Analyzed {TotalCalculations} possible outcomes");
                            results.Add($"{TieOutcomes/TotalCalculations*100:0.00}% result in unbreakable tie");
                            foreach (string teamName in WinnerCounts.Keys)
                            {
                                int totalWins = 0;
                                for(int i=0; i<WinnerCounts[teamName].Count - 1; i++)
                                {
                                    totalWins += WinnerCounts[teamName][i];
                                }
                                results.Add($"{teamName}: {WinnerCounts[teamName][0]/TotalCalculations * 100:0.00}% outright winner, member of {WinnerCounts[teamName][WinnerCounts[teamName].Count - 1] / (double)TieOutcomes * 100:0.00}% of tie scenarios");
                            }

                        }

                        finishedCallback(results);
                    });

                    return;
                }

                //Determine who won the permutation (async)
                mJudge.Judge(division, allGames, homeWinners, (winner, tiedTeams) =>
                {
                    JudgementsDone++;
                    lock (WinnerCounts)
                    {
                        if (winner != null)
                        {
                            WinnerCounts[winner][tiedTeams.Count - 1]++;
                        }
                        else
                        {
                            TieOutcomes++;
                            foreach(string teamName in tiedTeams)
                            {
                                WinnerCounts[teamName][WinnerCounts[teamName].Count - 1]++;
                            }
                        }

                        //Log the current counts once in a while
                        LastUpdateCounter++;
                        if (LastUpdateCounter > 1000)
                        {
                            progressCallback($"Judging {JudgementsDone} of {TotalCalculations} outcomes", JudgementsDone / TotalCalculations);
                            LastUpdateCounter = 0;
                        }
                    }
                });
            });
        }
    }
}
