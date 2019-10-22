using FootballTools.Entities;
using System.Collections.Generic;

namespace FootballTools.Reports
{
    public class TeamReport
    {
        public static List<string> Generate(string conferenceName, string divisionName, string teamName, League league)
        {
            List<string> ret = new List<string>();

            Team team = league.FindTeam(teamName);

            GameList games = league.AllGames.FilterByTeams(new List<int> {team.Id});

            TeamResult results = games.Results[team.Id];

            ret.Add("Records:");
            ret.Add($"Overall: {results.OverallRecord}");
            ret.Add($"Conference: {results.ConferenceRecord}");
            ret.Add($"Division: {results.DivisionRecord}");
            ret.Add(string.Empty);

            /*
                Cross-Conference win/loss
                Division chances
                Rankings
                PF/PA
             */
            return ret;
        }

        public static DataMatrix GenerateMatrix(string conferenceName, string divisionName, string teamName, League league)
        {
            return null;
        }
    }
}
