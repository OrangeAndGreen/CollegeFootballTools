using System.Collections.Generic;
using System.Diagnostics;

namespace FootballTools.Entities
{
    [DebuggerDisplay("Conference: {Name}")]
    public class Conference
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Division> Divisions { get; set; }

        public Conference(int id, string name)
        {
            Id = id;
            Name = name;
            Divisions = new List<Division>();
        }

        public Division this[string divisionName] => FindDivision(divisionName);

        public Division this[int index] => Divisions[index];

        public Division FindDivision(string divisionName)
        {
            foreach (Division division in Divisions)
            {
                if (division.Name.Equals(divisionName))
                {
                    return division;
                }
            }

            return null;
        }

        public List<Team> AllTeams
        {
            get
            {
                List<Team> ret = new List<Team>();

                foreach (Division division in Divisions)
                {
                    ret.AddRange(division.Teams);
                }

                return ret;
            }
        }

        public List<Game> AllGames
        {
            get
            {
                List<Game> games = new List<Game>();
                foreach (Division division in Divisions)
                {
                    foreach (Team team in division.Teams)
                    {
                        foreach (Game game in team.Schedule)
                        {
                            if (!games.Contains(game))
                            {
                                games.Add(game);
                            }
                        }
                    }
                }

                Game.SortGameList(games);

                return games;
            }
        }
    }
}
