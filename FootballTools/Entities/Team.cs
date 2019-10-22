using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace FootballTools.Entities
{
    [DebuggerDisplay("Team: {Name} ({ConferenceName})")]
    [DataContract]
    public class Team
    {
        [DataMember(IsRequired = false, Name = "id")]
        public int Id;
        [DataMember(IsRequired = false, Name = "school")]
        public string Name;
        [DataMember(IsRequired = false, Name = "mascot")]
        public string Mascot;
        [DataMember(IsRequired = false, Name = "abbreviation")]
        public string Abbreviation;
        [DataMember(IsRequired = false, Name = "alt_name_1")]
        public string Name2;
        [DataMember(IsRequired = false, Name = "alt_name_2")]
        public string Name3;
        [DataMember(IsRequired = false, Name = "alt_name_3")]
        public string Name4;
        [DataMember(IsRequired = false, Name = "conference")]
        public string ConferenceName;
        [DataMember(IsRequired = false, Name = "division")]
        public string DivisionName;
        [DataMember(IsRequired = false, Name = "color")]
        public string Color;
        [DataMember(IsRequired = false, Name = "alt_color")]
        public string Colo2;
        [DataMember(IsRequired = false, Name = "logos")]
        public List<string> Logos;

        public int OverallWins { get; set; }
        public int OverallLosses { get; set; }
        public int OverallTies { get; set; }
        public int ConferenceWins { get; set; }
        public int ConferenceLosses { get; set; }
        public int ConferenceTies { get; set; }

        public string OverallRecord => $"{OverallWins}-{OverallLosses}";
        public string ConferenceRecord => $"{ConferenceWins}-{ConferenceLosses}";
        public string ComboRecord => $"{OverallRecord} ({ConferenceRecord})";

        public GameList Schedule { get; set; }

        public Team(int id, string name, string divisionName, string conferenceName)
        {
            Id = id;
            Name = name;
            DivisionName = divisionName;
            ConferenceName = conferenceName;
            OverallWins = 0;
            OverallLosses = 0;
            OverallTies = 0;
            ConferenceWins = 0;
            ConferenceLosses = 0;
            ConferenceTies = 0;
            Schedule = new GameList();
        }

        //public static List<string> GetTeamNames(List<Team> teams)
        //{
        //    List<string> ret = new List<string>();
        //    foreach (Team team in teams)
        //    {
        //        ret.Add(team.Name);
        //    }

        //    return ret;
        //}

        public static List<int> GetTeamIds(List<Team> teams)
        {
            List<int> ret = new List<int>();
            foreach (Team team in teams)
            {
                ret.Add(team.Id);
            }

            return ret;
        }

        public static void SortTeamList(List<Team> teams, bool sortByTotalOrConferenceWins)
        {
            teams.Sort(
                    delegate (Team t1, Team t2)
                    {
                        //Sort by most wins, then least losses, then alphabetical name
                        int val1 = sortByTotalOrConferenceWins ? t1.OverallWins : t1.ConferenceWins;
                        int val2 = sortByTotalOrConferenceWins ? t2.OverallWins : t2.ConferenceWins;
                        int compare = -val1.CompareTo(val2);
                        if (compare == 0)
                        {
                            val1 = sortByTotalOrConferenceWins ? t1.OverallLosses : t1.ConferenceLosses;
                            val2 = sortByTotalOrConferenceWins ? t2.OverallLosses : t2.ConferenceLosses;
                            compare = val1.CompareTo(val2);
                            if (compare == 0)
                            {
                                return t1.Name.CompareTo(t2.Name);
                            }
                        }
                        return compare;
                    }
                );
        }

        public void RecordWin(bool conferenceGame)
        {
            OverallWins++;
            if (conferenceGame)
            {
                ConferenceWins++;
            }
        }

        public void RecordLoss(bool conferenceGame)
        {
            OverallLosses++;
            if (conferenceGame)
            {
                ConferenceLosses++;
            }
        }

        public void RecordTie(bool conferenceGame)
        {
            OverallTies++;
            if (conferenceGame)
            {
                ConferenceTies++;
            }
        }
    }
}
