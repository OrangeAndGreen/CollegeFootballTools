using FootballTools.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace FootballTools.Analysis
{
    /// <summary>
    /// Active object (separate worker thread)
    /// Determines the winner of a division given a scenario of winners
    /// </summary>
    public class DivisionWinnerCalculator
    {
        private Thread mThread { get; set; }
        private ConcurrentQueue<DivisionWinnerCalculatorCommand> mCommandQueue = new ConcurrentQueue<DivisionWinnerCalculatorCommand>();

        public DivisionWinnerCalculator()
        {
            mThread = new Thread(mainLoop);
            mThread.Start();
        }

        public void Quit()
        {
            mCommandQueue.Enqueue(new DivisionWinnerCalculatorCommand(null, null, null, null));

            mThread.Join();
            mThread = null;
        }

        public void Signal(Action<string, List<string>> callback)
        {
            mCommandQueue.Enqueue(new DivisionWinnerCalculatorCommand(null, null, null, callback));
        }

        public void Judge(Division division, List<Game> games, List<bool> homeWinners, Action<string, List<string>> callback)
        {
            mCommandQueue.Enqueue(new DivisionWinnerCalculatorCommand(division, games, homeWinners, callback));
        }

        private void mainLoop()
        {
            while (true)
            {
                bool dequeued = mCommandQueue.TryDequeue(out DivisionWinnerCalculatorCommand command);
                if (dequeued)
                {
                    if(mCommandQueue.Count > 0 && mCommandQueue.Count % 100000 == 0)
                    {
                        Console.WriteLine($"Judgment queue: {mCommandQueue.Count}");
                    }

                    if (command.Games == null)
                    {
                        if (command.Callback != null)
                        {
                            command.Callback(null, null);
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    //Have a list of games and a list of winners, determine who won the division

                    //Determine the teams we're interested in
                    List<string> teamNames = new List<string>();
                    foreach (Team team in command.ActiveDivision.Teams)
                    {
                        teamNames.Add(team.Name);
                    }

                    //Count the number of wins for each team
                    Dictionary<string, int> winnerCounts = new Dictionary<string, int>();
                    for (int i = 0; i < command.Games.Count; i++)
                    {
                        Game game = command.Games[i];
                        string winner = command.HomeWinners[i] ? game.home_team : game.away_team;
                        if (teamNames.Contains(winner))
                        {
                            if (winnerCounts.ContainsKey(winner))
                            {
                                winnerCounts[winner]++;
                            }
                            else
                            {
                                winnerCounts.Add(winner, 1);
                            }
                        }
                    }

                    //Determine the max number of wins
                    int maxWins = 0;
                    foreach (string key in winnerCounts.Keys)
                    {
                        maxWins = Math.Max(winnerCounts[key], maxWins);
                    }

                    //Determine how many teams tied for the most wins
                    int numWinners = 0;
                    List<string> winners = new List<string>();
                    foreach (string key in winnerCounts.Keys)
                    {
                        if (winnerCounts[key] == maxWins)
                        {
                            winners.Add(key);
                            numWinners++;
                        }
                    }

                    string finalWinner = null;
                    switch (numWinners)
                    {
                        case 1:
                            finalWinner = winners[0];
                            break;
                        case 2:
                            //See who won the head-to-head matchup
                            for (int i = 0; i < command.Games.Count; i++)
                            {
                                Game game = command.Games[i];
                                bool homeWinner = command.HomeWinners[i];
                                if (game.InvolvesTeam(winners[0]) && game.InvolvesTeam(winners[1]))
                                {
                                    finalWinner = homeWinner ? game.home_team : game.away_team;
                                    break;
                                }
                            }
                            break;
                        case 3:
                            //See if one team beat both of the others
                            List<string> headToHeadWinners = new List<string>();
                            for (int i = 0; i < command.Games.Count; i++)
                            {
                                Game game = command.Games[i];
                                bool homeWinner = command.HomeWinners[i];
                                int teamsInvolved = 0;
                                foreach(string winner in winners)
                                {
                                    if(game.InvolvesTeam(winner))
                                    {
                                        teamsInvolved++;
                                    }
                                }

                                if (teamsInvolved == 2)
                                {
                                    string headToHeadWinnner = homeWinner ? game.home_team : game.away_team;
                                    if(headToHeadWinners.Contains(headToHeadWinnner))
                                    {
                                        finalWinner = headToHeadWinnner;
                                        break;
                                    }
                                    headToHeadWinners.Add(headToHeadWinnner);
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    command.Callback(finalWinner, winners);
                }
            }
        }
    }
}
