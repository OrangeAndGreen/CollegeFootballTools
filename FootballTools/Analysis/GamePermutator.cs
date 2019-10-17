using FootballTools.Entities;
using System;
using System.Collections.Generic;
using System.Threading;

namespace FootballTools.Analysis
{
    /// <summary>
    /// Takes a season in progress and generates all permutations of possible game outcomes
    /// </summary>
    public class GamePermutator
    {
        public double TotalPermutations { get; private set; }
        public int CompletedPermutations { get; private set; }
        public int LastUpdateCounter { get; private set; }

        public void PermutateGames(List<Game> games, Action<List<Game>,List<bool>> callback)
        {
            Thread thread = new Thread(delegate ()
            {
                doPermutateGames(games, callback);
            });
            thread.Start();
        }

        private void doPermutateGames(List<Game> games, Action<List<Game>, List<bool>> callback)
        {
            CompletedPermutations = 0;

            int totalGames = 0;
            List<bool> homeWinners = new List<bool>();
            foreach (Game game in games)
            {
                //Setup the list of already completed games
                if (game.GameAlreadyPlayed)
                {
                    homeWinners.Add(game.HomeWin.Value);
                }

                totalGames++;
            }

            TotalPermutations = Math.Pow(2, totalGames - homeWinners.Count);

            Console.WriteLine($"{totalGames} games to analyze ({homeWinners.Count} completed)");

            //Try calculating all possible results (BIG RECURSIVE CALL)
            Explore(games, homeWinners.Count, homeWinners, callback);

            callback(games, null);
        }

        private void Explore(List<Game> games, int startIndex, List<bool> homeWinners, Action<List<Game>, List<bool>> callback)
        {
            LastUpdateCounter++;
            if (LastUpdateCounter > (TotalPermutations/20))
            {
                Console.WriteLine($"{(int)Math.Round(CompletedPermutations / TotalPermutations * 100)}% done permutating");
                LastUpdateCounter = 0;
            }

            if (startIndex >= games.Count)
            {
                //Reached an endpoint, output a permutation
                callback(games, new List<bool>(homeWinners));
                return;
            }

            CompletedPermutations++;

            //Recursively call to explore 1) home team winning and 2) away team winning
            homeWinners.Add(true);
            Explore(games, startIndex + 1, homeWinners, callback);
            homeWinners.RemoveAt(homeWinners.Count - 1);

            homeWinners.Add(false);
            Explore(games, startIndex + 1, homeWinners, callback);
            homeWinners.RemoveAt(homeWinners.Count - 1);
        }
    }
}
