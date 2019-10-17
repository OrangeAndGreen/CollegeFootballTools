using System.Collections.Generic;
using System.Diagnostics;

namespace FootballTools.Entities
{
    [DebuggerDisplay("Division: {Name}")]
    public class Division
    {
        public string Name { get; set; }
        public List<Team> Teams { get; set; }

        public Division(string name)
        {
            Name = name;
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
