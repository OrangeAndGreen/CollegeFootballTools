using System.Collections.Generic;
using System.Diagnostics;

namespace FootballTools.Entities
{
    [DebuggerDisplay("Team: {Name}")]
    public class Team
    {
        public string Name { get; set; }
        public int OverallWins { get; set; }
        public int OverallLosses { get; set; }
        public int OverallTies { get; set; }
        public int ConferenceWins { get; set; }
        public int ConferenceLosses { get; set; }
        public int ConferenceTies { get; set; }

        public string OverallRecord => $"{OverallWins}-{OverallLosses}";
        public string ConferenceRecord => $"{ConferenceWins}-{ConferenceLosses}";
        public string ComboRecord => $"{OverallRecord} ({ConferenceRecord})";

        public List<Game> Schedule { get; set; }

        public Team(string name)
        {
            Name = name;
            OverallWins = 0;
            OverallLosses = 0;
            OverallTies = 0;
            ConferenceWins = 0;
            ConferenceLosses = 0;
            ConferenceTies = 0;
            Schedule = new List<Game>();
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
