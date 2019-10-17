using FootballTools.Entities;
using System.Collections.Generic;

namespace FootballTools.Reports
{
    public class TeamReport
    {
        public static List<string> Generate(string conferenceName, string divisionName, string teamName, League league)
        {
            return new List<string> { "Team Report" };
        }

        public static DataMatrix GenerateMatrix(string conferenceName, string divisionName, string teamName, League league)
        {
            return null;
        }
    }
}
