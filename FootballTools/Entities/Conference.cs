using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace FootballTools.Entities
{
    [DebuggerDisplay("Conference: {Name}")]
    [DataContract]
    public class Conference
    {
        [DataMember(IsRequired = false, Name = "id")]
        public int Id;
        [DataMember(IsRequired = false, Name = "name")]
        public string Name;
        [DataMember(IsRequired = false, Name = "short_name")]
        public string NameShort;
        [DataMember(IsRequired = false, Name = "abbreviation")]
        public string Abbreviation;

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

        public GameList AllGames
        {
            get
            {
                GameList games = new GameList();
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

                games.Sort();

                return games;
            }
        }
    }
}
