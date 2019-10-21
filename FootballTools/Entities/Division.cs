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

        //public Team this[string teamName] => FindTeam(teamName);

        public Team this[int teamId] => FindTeam(teamId);

        public Team FindTeam(int teamId)
        {
            foreach (Team team in Teams)
            {
                if (team.Id == teamId)
                {
                    return team;
                }
            }

            return null;
        }

        public GameList AllGames
        {
            get
            {
                GameList games = new GameList();
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

                games.Sort();

                return games;
            }
        }
    }
}
