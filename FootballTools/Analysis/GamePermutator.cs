using FootballTools.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FootballTools.Analysis
{
    /// <summary>
    /// Takes a season in progress and generates all permutations of possible game outcomes
    /// </summary>
    public class GamePermutator
    {
        private double TotalPermutations { get; set; }
        private int CompletedPermutations { get; set; }

        public static void PermutateGames(GameList games, Action<GameList,List<int>, List<TeamResult>, int, double> callback)
        {
            GamePermutator permutator = new GamePermutator();
            Thread thread = new Thread(delegate ()
            {
                permutator.doPermutateGames(games, callback);
            });
            thread.Start();
        }

        private void doPermutateGames(GameList games, Action<GameList, List<int>, List<TeamResult>, int, double> callback)
        {
            CompletedPermutations = 0;

            int completedGames = games.Sum(game => game.GameAlreadyPlayed ? 1 : 0);

            TotalPermutations = Math.Pow(2, games.Count - completedGames);

            Console.WriteLine($"{games.Count} games to analyze ({completedGames} completed)");

            //Try calculating all possible results (BIG RECURSIVE CALL)
            Explore(games, completedGames, callback);

            Console.WriteLine("Done permuting games");

            callback(games, null, null, CompletedPermutations, TotalPermutations);
        }

        private void Explore(GameList games, int startIndex, Action<GameList, List<int>, List<TeamResult>, int, double> callback)
        {
            if (startIndex >= games.Count)
            {
                //Reached an endpoint, output a permutation
                CompletedPermutations++;
                
                List<int> winners = new List<int>();
                foreach(Game winnerGame in games)
                {
                    winners.Add(winnerGame.WinnerId);
                }

                List<TeamResult> results = new List<TeamResult>();
                foreach(TeamResult result in games.Results.Values)
                {
                    results.Add(new TeamResult(result));
                }

                callback(games, winners, results, CompletedPermutations, TotalPermutations);
                return;
            }

            //Recursively call to explore 1) home team winning and 2) away team winning
            Game game = games[startIndex];
            
            games.SetProposedGameWinner(startIndex, game.HomeTeamId);
            Explore(games, startIndex + 1, callback);
            
            games.SetProposedGameWinner(startIndex, game.AwayTeamId);
            Explore(games, startIndex + 1, callback);
            
            games.SetProposedGameWinner(startIndex, null);
        }
    }
}
