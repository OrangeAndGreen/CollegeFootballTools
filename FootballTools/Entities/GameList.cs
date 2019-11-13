using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FootballTools.Entities
{
    [DebuggerDisplay("{mGames.Count} games")]
    public class GameList : IEnumerable<Game>
    {
        private List<Game> mGames { get; set; }

        public Dictionary<int,TeamResult> Results { get; set; }

        public GameList()
        {
            mGames = new List<Game>();
            Results = new Dictionary<int, TeamResult>();
        }

        public GameList(GameList games)
            : this()
        {
            foreach(Game game in games)
            {
                Add(game);
            }
        }

        public Game this[int index] => mGames[index];

        public int Count => mGames.Count;

        public void Add(Game game)
        {
            mGames.Add(game);

            List<int> teamIds = new List<int> { game.HomeTeamId, game.AwayTeamId };
            List<string> teamNames = new List<string> { game.home_team, game.away_team};
            for (int i = 0; i < teamIds.Count; i++)
            {
                int teamId = teamIds[i];
                
                if (teamId > 0 && !Results.ContainsKey(teamId))
                {
                    Results[teamId] = new TeamResult(teamId, teamNames[i]);
                }
            }

            if (game.WinnerId > 0)
            {
                bool conferenceGame = game.ConferenceGame;
                bool divisionGame = game.DivisionGame;

                TeamResult winnerResult = game.WinnerId > 0 ? Results[game.WinnerId] : null;
                if (winnerResult != null)
                {
                    winnerResult.AddWin(conferenceGame, divisionGame);
                }

                TeamResult loserResult = game.LoserId > 0 ? Results[game.LoserId] : null;
                if (loserResult != null)
                {
                    loserResult.AddLoss(conferenceGame, divisionGame);
                }
            }
        }

        public void SetProposedGameWinner(int gameIndex, int? winnerId)
        {
            Game game = mGames[gameIndex];
            bool conferenceGame = game.ConferenceGame;
            bool divisionGame = game.DivisionGame;

            if (game.ProposedWinnerId.HasValue)
            {
                //Changing or clearing previously proposed winner
                if(game.ProposedWinnerId.Value > 0) Results[game.ProposedWinnerId.Value].RemoveWin(conferenceGame, divisionGame);
                if (game.ProposedLoserId.Value > 0) Results[game.ProposedLoserId.Value].RemoveLoss(conferenceGame, divisionGame);
            }

            game.ProposedWinnerId = winnerId;

            if (game.ProposedWinnerId.HasValue)
            {
                //Count the new proposed winner and loser
                if (game.ProposedWinnerId.Value > 0) Results[game.ProposedWinnerId.Value].AddWin(conferenceGame, divisionGame);
                if (game.ProposedLoserId.Value > 0) Results[game.ProposedLoserId.Value].AddLoss(conferenceGame, divisionGame);
            }
        }

        public void AddRange(IEnumerable<Game> games)
        {
            foreach (Game game in games)
            {
                Add(game);
            }
        }

        public bool Contains(Game game)
        {
            return mGames.Contains(game);
        }

        public IEnumerator<Game> GetEnumerator()
        {
            return ((IEnumerable<Game>)mGames).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Game>)mGames).GetEnumerator();
        }

        public GameList FilterByTeams(List<int> teamIds)
        {
            GameList filtered = new GameList();

            foreach (Game game in mGames)
            {
                foreach (int teamId in teamIds)
                {
                    if (game.InvolvesTeam(teamId))
                    {
                        filtered.Add(game);
                        break;
                    }
                }
            }

            return filtered;
        }

        public void Sort()
        {
            mGames.Sort(
                    delegate (Game g1, Game g2)
                    {
                        int compareDate = g1.week.HasValue && g2.week.HasValue ? g1.week.Value.CompareTo(g2.week.Value) : 0;
                        if (compareDate == 0)
                        {
                            return g1.GameDate.CompareTo(g2.GameDate);
                        }
                        return compareDate;
                    }
                );
        }

        public Game FindMatchup(int team1, int team2)
        {
            int index = FindMatchupIndex(team1, team2);

            return index >= 0 ? mGames[index] : null;
        }

        public Game FindMatchup(int gameId)
        {
            for (int i = 0; i < mGames.Count; i++)
            {
                Game game = mGames[i];
                if (game.Id == gameId)
                {
                    return game;
                }
            }

            return null;
        }

        public int FindMatchupIndex(int team1, int team2)
        {
            for (int i = 0; i < mGames.Count; i++)
            {
                Game game = mGames[i];
                if (game.InvolvesTeam(team1) && game.InvolvesTeam(team2))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Given a list of teams, identifies the team(s) with the most head-to-head wins
        /// </summary>
        public List<int> GetWinnersFromTeams(List<int> teamIds, bool includeConferencegames, List<int> winners = null)
        {
            int[] headToHeadWins = new int[teamIds.Count];
            for (int i = 0; i < mGames.Count; i++)
            {
                Game game = mGames[i];
                int winnerId = winners?.Count > i ? winners[i] : game.WinnerId;

                if (winnerId > 0)
                {
                    for (int index1 = 0; index1 < teamIds.Count; index1++)
                        for (int index2 = index1 + 1; index2 < teamIds.Count; index2++)
                        {
                            int team1 = teamIds[index1];
                            int team2 = teamIds[index2];

                            bool team1Involved = game.InvolvesTeam(team1);
                            bool team2Involved = game.InvolvesTeam(team2);

                            if (!team1Involved || !team2Involved)
                            {
                                continue;
                            }

                            if ((team1Involved || team2Involved) && includeConferencegames && game.ConferenceGame)
                            {
                                int winnerIndex = winnerId == team1 ? index1 : index2;
                                headToHeadWins[winnerIndex]++;
                            }
                        }
                }
            }

            int maxWins = -1;
            List<int> tiedTeamIds = new List<int>();
            for (int i = 0; i < teamIds.Count; i++)
            {
                int headToHead = headToHeadWins[i];
                if (headToHead > maxWins)
                {
                    maxWins = headToHead;
                    tiedTeamIds.Clear();
                    tiedTeamIds.Add(teamIds[i]);
                }
                else if (headToHead == maxWins)
                {
                    tiedTeamIds.Add(teamIds[i]);
                }
            }

            return tiedTeamIds;
        }
    }
}
