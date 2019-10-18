using FootballTools.Analysis.DivisionTiebreakers;
using FootballTools.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Resources;
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
        private readonly Dictionary<string, ITiebreaker> mTiebreakers;
        private List<int> mScenariosCalculated;
        private List<double> mScenarioTime;

        private League mLeague = null;
        private Division mDivision = null;
        private List<string> mTeamNames = null;
        private List<Game> mGames = null;
        private Action<string, List<string>> mCallback = null;

        public DivisionWinnerCalculator()
        {
            mTiebreakers = new Dictionary<string, ITiebreaker>
            {
                ["ACC"] = new AccTiebreaker(),
                ["SEC"] = new SecTiebreaker(),
                ["Mountain West"] = new MwcTiebreaker(),
                ["Pac-12"] = new Pac12Tiebreaker(),
                ["Mid-American"] = new MacTiebreaker(),
                ["Conference USA"] = new CUsaTiebreaker(),
                ["Sun Belt"] = new SunBeltTiebreaker(),
                ["Big Ten"] = new Big10Tiebreaker(),
                ["Big 12"] = new Big12Tiebreaker()
            };

            mScenariosCalculated = new List<int> {0,0,0};
            mScenarioTime = new List<double> { 0, 0, 0 };

            mThread = new Thread(mainLoop);
            mThread.Start();
        }

        public int Backlog => mCommandQueue.Count;

        public void SetData(League league, Division division, List<Game> games, Action<string, List<string>> callback)
        {
            mCommandQueue.Enqueue(DivisionWinnerCalculatorCommand.CreateSetDataCommand(league, division, games, callback));
        }
        
        public void Judge(List<string> winners)
        {
            mCommandQueue.Enqueue(DivisionWinnerCalculatorCommand.CreateJudgeCommand(winners));
        }

        public void Signal(Action<string, List<string>> callback)
        {
            mCommandQueue.Enqueue(DivisionWinnerCalculatorCommand.CreateSignalCommand(callback));
        }

        public void Quit()
        {
            mCommandQueue.Enqueue(DivisionWinnerCalculatorCommand.CreateShutdownCommand());

            mThread.Join();
            mThread = null;
        }

        private void mainLoop()
        {
            bool quitter = false;
            while (!quitter)
            {
                bool dequeued = mCommandQueue.TryDequeue(out DivisionWinnerCalculatorCommand command);
                if (dequeued)
                {
                    //if(mCommandQueue.Count > 0 && mCommandQueue.Count % 100000 == 0)
                    //{
                    //    Console.WriteLine($"Judgment queue: {mCommandQueue.Count}");
                    //}

                    switch (command.CommandType)
                    {
                        case DivisionWinnerCalculatorCommandType.SetData:
                            mLeague = command.ActiveLeague;
                            mDivision = command.ActiveDivision;
                            mGames = command.Games;
                            mCallback = command.Callback;

                            mTeamNames = new List<string>();
                            foreach (Team team in mDivision.Teams)
                            {
                                mTeamNames.Add(team.Name);
                            }

                            break;
                        case DivisionWinnerCalculatorCommandType.Judge:
                        {
                            DateTime start = DateTime.Now;
                            //Have a list of games and a list of winners, determine who won the division
                            //Find the teams with the most wins
                            List<string> winnerNames = Game.GetWinnersFromTeams(mTeamNames, mGames, command.Winners);

                            string finalWinner = null;

                            switch (winnerNames.Count)
                            {
                                case 1:
                                    finalWinner = winnerNames[0];
                                    break;
                                case 2:
                                {
                                    //See who won the head-to-head matchup
                                    Game matchup = Game.FindMatchup(mGames, mTeamNames[0], mTeamNames[1]);
                                    if (matchup != null && matchup.GameAlreadyPlayed)
                                    {
                                        finalWinner = matchup.Winner.Equals(mTeamNames[0]) ? mTeamNames[0] : mTeamNames[1];
                                    }

                                    break;
                                }
                                default:
                                    finalWinner = mTiebreakers[mDivision.ConferenceName].BreakTie(mGames, command.Winners, winnerNames, mDivision);
                                    break;
                            }

                            mCallback(finalWinner, winnerNames);
                            mScenariosCalculated[Math.Min(2, winnerNames.Count - 1)]++;
                            mScenarioTime[Math.Min(2, winnerNames.Count - 1)] += (DateTime.Now-start).TotalMilliseconds;

                                break;
                        }
                        case DivisionWinnerCalculatorCommandType.Signal:
                            for (int i = 0; i < mScenariosCalculated.Count; i++)
                            {
                                Console.WriteLine($"Spent {mScenarioTime[i] / 1000} s on {mScenariosCalculated[i]} {i+1}-winner scenarios ({mScenarioTime[i] / mScenariosCalculated[i]} ms/calc)");
                            }

                            mScenariosCalculated = new List<int> { 0, 0, 0 };
                            mScenarioTime = new List<double> { 0, 0, 0 };

                            command.Callback?.Invoke(null, null);
                            break;
                        case DivisionWinnerCalculatorCommandType.Shutdown:
                            quitter = true;
                            break;
                        default:
                            throw new Exception("Not ready for this case");
                    }
                }
            }
        }
    }
}
