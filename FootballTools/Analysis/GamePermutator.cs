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

        public void PermutateGames(List<Game> games, Action<List<Game>,List<string>> callback)
        {
            Thread thread = new Thread(delegate ()
            {
                doPermutateGames(games, callback);
            });
            thread.Start();
        }

        private void doPermutateGames(List<Game> games, Action<List<Game>, List<string>> callback)
        {
            CompletedPermutations = 0;

            int totalGames = 0;
            List<string> winners = new List<string>();
            foreach (Game game in games)
            {
                //Setup the list of already completed games
                if (game.GameAlreadyPlayed)
                {
                    winners.Add(game.Winner);
                }

                totalGames++;
            }

            TotalPermutations = Math.Pow(2, totalGames - winners.Count);

            Console.WriteLine($"{totalGames} games to analyze ({winners.Count} completed)");

            //Try calculating all possible results (BIG RECURSIVE CALL)
            Explore(games, winners.Count, winners, callback);

            Console.WriteLine("Done permuting games");

            callback(games, null);
        }

        private void Explore(List<Game> games, int startIndex, List<string> winners, Action<List<Game>, List<string>> callback)
        {
            LastUpdateCounter++;
            if (LastUpdateCounter > (TotalPermutations/20))
            {
                Console.WriteLine($"{(int)Math.Round(CompletedPermutations / TotalPermutations * 100)}% done permuting");
                LastUpdateCounter = 0;
            }

            if (startIndex >= games.Count)
            {
                //Reached an endpoint, output a permutation
                callback(games, new List<string>(winners));
                return;
            }

            CompletedPermutations++;

            Game game = games[startIndex];

            //Recursively call to explore 1) home team winning and 2) away team winning
            winners.Add(game.home_team);
            Explore(games, startIndex + 1, winners, callback);
            winners.RemoveAt(winners.Count - 1);

            winners.Add(game.away_team);
            Explore(games, startIndex + 1, winners, callback);
            winners.RemoveAt(winners.Count - 1);
        }
    }
}
