using FootballTools.Analysis.DivisionTiebreakers;
using FootballTools.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        //Threading stuff
        private static readonly int NumWorkerThreads = 8;
        private List<Thread> mWorkerThreads = null;
        private List<ConcurrentQueue<DivisionWinnerCalculatorCommand>> mWorkerQueues = null;
        private int mWorkerIndex = 0;

        //Passed in data
        private League mLeague = null;
        private Division mDivision = null;
        private List<int> mTeamIds = null;
        private GameList mGames = null;
        private Action<int, List<int>, double, List<int>> mJudgmentCallback = null;

        private static Dictionary<string, ITiebreaker> mTiebreakers = new Dictionary<string, ITiebreaker>();

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        //public List<int> ScenariosCalculated { get; set; }
        //public List<double> ScenarioTimes { get; set; }

        public DivisionWinnerCalculator(League league, Division division, GameList games, Action<int, List<int>, double, List<int>> callback)
        {
            mLeague = league;
            mDivision = division;
            mGames = games;
            mJudgmentCallback = callback;

            mTeamIds = Team.GetTeamIds(mDivision.Teams);

            //ScenariosCalculated = new List<int> {0, 0, 0};
            //ScenarioTimes = new List<double> { 0, 0, 0 };

            mWorkerThreads = new List<Thread>();
            mWorkerQueues = new List<ConcurrentQueue<DivisionWinnerCalculatorCommand>>();
            for (int i = 0; i < NumWorkerThreads; i++)
            {
                mWorkerQueues.Add(new ConcurrentQueue<DivisionWinnerCalculatorCommand>());
                Thread thread = new Thread(workerLoop);
                thread.Start();
                mWorkerThreads.Add(thread);
            }

            StartTime = DateTime.Now;
        }

        private static ITiebreaker GetTiebreaker(string conferenceName)
        {
            lock (mTiebreakers)
            {
                ITiebreaker tiebreaker = null;

                if (mTiebreakers.ContainsKey(conferenceName))
                {
                    return mTiebreakers[conferenceName];
                }

                switch (conferenceName)
                {
                    case "ACC":
                        tiebreaker = new AccTiebreaker();
                        break;
                    case "SEC":
                        tiebreaker = new SecTiebreaker();
                        break;
                    case "Mountain West":
                        tiebreaker = new MwcTiebreaker();
                        break;
                    case "Pac-12":
                        tiebreaker = new Pac12Tiebreaker();
                        break;
                    case "Mid-American":
                        tiebreaker = new MacTiebreaker();
                        break;
                    case "Conference USA":
                        tiebreaker = new CUsaTiebreaker();
                        break;
                    case "Sun Belt":
                        tiebreaker = new SunBeltTiebreaker();
                        break;
                    case "Big Ten":
                        tiebreaker = new Big10Tiebreaker();
                        break;
                    case "Big 12":
                        tiebreaker = new Big12Tiebreaker();
                        break;
                    default: throw new Exception("Not ready for this case");
                }

                mTiebreakers[conferenceName] = tiebreaker;

                return tiebreaker;
            }
        }

        public int Backlog
        {
            get
            {
                lock (mWorkerThreads)
                {
                    int backlog = 0;
                    foreach (var queue in mWorkerQueues)
                    {
                        backlog += queue.Count;
                    }

                    return backlog;
                }
            }
        }

        public void Judge(List<int> winners, List<TeamResult> teamResults)
        {
            //Queue a command to judge the outcome
            lock (mWorkerThreads)
            {
                //Hand the jobs out to the workers circularly
                ConcurrentQueue<DivisionWinnerCalculatorCommand> queue = mWorkerQueues[mWorkerIndex];
                mWorkerIndex = (mWorkerIndex + 1) % NumWorkerThreads;

                queue.Enqueue(DivisionWinnerCalculatorCommand.CreateJudgeCommand(winners, teamResults));
            }
        }

        public static DivisionResult JudgeSync(GameList games, Division division, List<int> teamIds, List<int> winnerIds ,List<TeamResult> teamResults)
        {
            DateTime start = DateTime.Now;
            //List<TeamResult> teamResults = TeamResult.FilterByTeams(command.TeamResults, mTeamIds);

            //Have a list of games and a list of winners, determine who won the division
            //Find the teams with the most wins
            List<int> divisionFinalistIds = TeamResult.GetWinningTeamIds(teamResults, RecordType.Conference, teamIds);
            //divisionFinalistIds = mGames.GetWinnersFromTeams(mTeamIds, true, command.WinnerIds);

            int finalWinner = -1;

            //switch (divisionFinalistIds.Count)
            //{
            //    case 1:
            //        finalWinner = divisionFinalistIds[0];
            //        break;
            //    case 2:
            //        {
            //            //See who won the head-to-head matchup
            //            int gameIndex = games.FindMatchupIndex(teamIds[0], teamIds[1]);
            //            if (winnerIds != null && gameIndex < winnerIds.Count)
            //            {
            //                finalWinner = winnerIds[gameIndex];
            //            }
            //            else
            //            {
            //                Game game = games[gameIndex];
            //                if (game.GameAlreadyPlayed)
            //                {
            //                    finalWinner = game.WinnerId;
            //                }
            //            }

            //            break;
            //        }
            //    default:
            //    {
                    ITiebreaker tiebreaker = GetTiebreaker(division.ConferenceName);
                    finalWinner = tiebreaker.BreakTie(games, winnerIds, teamResults, divisionFinalistIds, division);
            //        break;
            //    }
            //}

            //if(finalWinner == 1)
            //{
            //    TeamResult miamiResult = command.TeamResults.Where(result => result.TeamId == 1).ElementAt(0);

            //    if (miamiResult.ConferenceRecord.Wins != 5)
            //    {
            //        Console.WriteLine("Miami wins");
            //        foreach (TeamResult result in TeamResult.FilterByTeams(command.TeamResults, mTeamIds))
            //        {
            //            Console.WriteLine($"     {result.TeamName}: {result.OverallRecord.Wins}-{result.OverallRecord.Losses} ({result.ConferenceRecord.Wins}-{result.ConferenceRecord.Losses})");
            //        }
            //    }
            //}

            return new DivisionResult
            {
                WinnerId = finalWinner,
                DivisionFinalistIds = divisionFinalistIds,
                Milliseconds = (DateTime.Now - start).TotalMilliseconds
            };
        }

        public void FinishSession(Action<int, List<int>> finishedCallback)
        {
            lock(mWorkerThreads)
            {
                //Tell all the workers to finish
                foreach (ConcurrentQueue<DivisionWinnerCalculatorCommand> queue in mWorkerQueues)
                {
                    queue.Enqueue(DivisionWinnerCalculatorCommand.CreateFinishSessionCommand());
                }

                //Wait for the worker threads to join
                foreach (Thread thread in mWorkerThreads)
                {
                    thread.Join();
                }

                mWorkerQueues.Clear();
                mWorkerThreads.Clear();
            }

            EndTime = DateTime.Now;

            finishedCallback?.Invoke(-1, null);
        }

        private void workerLoop()
        {
            bool quitter = false;
            while (!quitter)
            {
                //Try to dequeue a command
                if (mWorkerQueues[mWorkerThreads.IndexOf(Thread.CurrentThread)].TryDequeue(out DivisionWinnerCalculatorCommand command))
                {
                    switch (command.CommandType)
                    {
                        case DivisionWinnerCalculatorCommandType.Judge:
                        {
                            DivisionResult result = JudgeSync(mGames, mDivision, mTeamIds, command.WinnerIds, command.TeamResults);

                            mJudgmentCallback(result.WinnerId, result.DivisionFinalistIds, result.Milliseconds, command.WinnerIds);

                            break;
                        }
                        case DivisionWinnerCalculatorCommandType.Finish:
                            quitter = true;
                            break;
                        default:
                            throw new Exception("Not ready for this case");
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }
    }
}
