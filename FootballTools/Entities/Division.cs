using System.Collections.Generic;
using System.Diagnostics;

namespace FootballTools.Entities
{
    [DebuggerDisplay("Division: {Name}")]
    public class Division
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ConferenceName { get; set; }
        public List<Team> Teams { get; set; }

        public Division(int id, string name, string conferenceName)
        {
            Id = id;
            Name = name;
            ConferenceName = conferenceName;
            Teams = new List<Team>();
        }

        public Team this[string teamName] => FindTeam(teamName);

        public Team this[int index] => Teams[index];

        public Team FindTeam(string teamName)
        {
            foreach (Team team in Teams)
            {
                if (team.Name.Equals(teamName))
                {
                    return team;
                }
            }

            return null;
        }

        public List<Game> AllGames
        {
            get
            {
                List<Game> games = new List<Game>();
                foreach (Team team in Teams)
                {
                    foreach (Game game in team.Schedule)
                    {
                        if (!games.Contains(game))
                        {
                            games.Add(game);
                        }
                    }
                }

                Game.SortGameList(games);

                return games;
            }
        }
    }
}
