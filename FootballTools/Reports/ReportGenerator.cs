using FootballTools.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FootballTools.Reports
{
    public static class ReportGenerator
    {
        /// <summary>
        /// Generates a report given the conference/division/team selections
        /// </summary>
        public static List<string> GenerateReport(League league, DisplayType displayType, string conferenceName, string divisionName, string teamName)
        {
            List<Team> teams;
            bool sortByTotalOrConferenceWins = false;
            switch (displayType)
            {
                case DisplayType.League:
                    sortByTotalOrConferenceWins = true;
                    teams = league.AllTeams;
                    break;
                case DisplayType.Conference:
                    teams = league[conferenceName].AllTeams;
                    break;
                case DisplayType.Division:
                    teams = league.FindDivision(conferenceName, divisionName).Teams;
                    break;
                case DisplayType.Team:
                    {
                        Team team = league.FindTeam(teamName);
                        teams = new List<Team> { league.FindTeam(team.Id) };
                        break;
                    }
                default:
                    throw new Exception("Unknown display type");
            }

            List<string> ret = GenerateTeamListReport(league, teams, sortByTotalOrConferenceWins);
            ret.Add(string.Empty);

            switch (displayType)
            {
                case DisplayType.League:
                    ret.AddRange(LeagueReport.Generate(league));
                    break;
                case DisplayType.Conference:
                    ret.AddRange(ConferenceReport.Generate(conferenceName, league));
                    break;
                case DisplayType.Division:
                    ret.AddRange(DivisionReport.Generate(conferenceName, divisionName, league));
                    break;
                case DisplayType.Team:
                    ret.AddRange(TeamReport.Generate(conferenceName, divisionName, teamName, league));
                    break;
                default:
                    throw new Exception("Unknown display type");
            }

            return ret;
        }

        public static List<string> GenerateTeamListReport(League league, List<Team> teams, bool sortByTotalOrConferenceWins)
        {
            List<string> ret = new List<string>();

            List<Team> sortedTeams = new List<Team>(teams);
            Team.SortTeamList(sortedTeams, sortByTotalOrConferenceWins);

            if (sortedTeams.Count > 0)
            {
                ret.Add("Records:");
                foreach (Team team in sortedTeams)
                {
                    ret.Add($"{team.Name}: {team.ComboRecord}");
                }

                ret.Add(string.Empty);
            }

            ret.Add("All games:");
            List<int> teamIds = Team.GetTeamIds(sortedTeams);
            GameList games = league.AllGames.FilterByTeams(teamIds);
            foreach (Game game in games)
            {
                string entry = $"{game.home_team} v. {game.away_team}";
                if (game.home_points.HasValue && game.away_points.HasValue)
                {
                    entry += $" ({game.home_points}-{game.away_points})";
                }
                ret.Add(entry);
            }

            return ret;
        }


        /// <summary>
        /// Populates a grid with a team head-to-head matrix given the conference/division/team selections
        /// </summary>
        public static DataMatrix GenerateTeamMatrix(League league, DisplayType displayType, string conferenceName, string divisionName, string teamName)
        {
            DataMatrix matrix;
            switch (displayType)
            {
                case DisplayType.League:
                    matrix = GenerateTeamMatrix(league, league.AllTeams, true);
                    break;
                case DisplayType.Conference:
                    {
                        Conference conference = league[conferenceName];
                        matrix = GenerateTeamMatrix(league, conference.AllTeams, false);
                        break;
                    }
                case DisplayType.Division:
                    {
                        Division division = league.FindDivision(conferenceName, divisionName);
                        matrix = GenerateTeamMatrix(league, division.Teams, false);
                        break;
                    }
                case DisplayType.Team:
                    matrix = null;
                    break;
                default:
                    throw new Exception("Unknown display type");
            }

            return matrix;
        }

        /// <summary>
        /// Builds a matrix of team head-to-head matchups given a list of teams
        /// </summary>
        public static DataMatrix GenerateTeamMatrix(League league, List<Team> teams, bool sortByTotalOrConferenceWins)
        {
            List<Team> sortedTeams = new List<Team>(teams);
            Team.SortTeamList(sortedTeams, sortByTotalOrConferenceWins);

            string[] headers = new string[sortedTeams.Count];
            string[,] labels = new string[sortedTeams.Count, sortedTeams.Count];
            Color[,] colors = new Color[sortedTeams.Count, sortedTeams.Count];

            for (int teamIndex = 0; teamIndex < sortedTeams.Count; teamIndex++)
            {
                Team team = sortedTeams[teamIndex];

                string teamLabel = $"{team.Name}{Constants.Newline}{team.ComboRecord}";
                headers[teamIndex] = teamLabel;

                for (int rivalIndex = 0; rivalIndex < sortedTeams.Count; rivalIndex++)
                {
                    if (teamIndex != rivalIndex)
                    {
                        Team rival = sortedTeams[rivalIndex];

                        Game game = league.AllGames.FindMatchup(team.Id, rival.Id);

                        if (game != null)
                        {
                            string entry = game.GameDate.ToString("MMM dd");
                            Color rowColor = Color.White;
                            Color colColor = Color.White;
                            if (game.home_points.HasValue && game.away_points.HasValue)
                            {
                                entry += $"{Constants.Newline}{game.home_points}-{game.away_points}";
                                bool rowWin = (game.HomeTeamId == team.Id) == (game.home_points > game.away_points);
                                rowColor = rowWin ? Constants.WinColor : Constants.LossColor;
                                colColor = rowWin ? Constants.LossColor : Constants.WinColor;
                            }

                            labels[teamIndex, rivalIndex] = entry;
                            colors[teamIndex, rivalIndex] = rowColor;

                            labels[rivalIndex, teamIndex] = entry;
                            colors[rivalIndex, teamIndex] = colColor;
                        }
                    }
                    else
                    {
                        colors[teamIndex, rivalIndex] = Color.Black;
                    }
                }
            }

            for (int i = 0; i < sortedTeams.Count; i++)
                for (int j = 0; j < sortedTeams.Count; j++)
                {
                    if (labels[i, j] == null)
                    {
                        labels[i, j] = string.Empty;
                    }
                }


            return new DataMatrix(headers, headers, labels, colors);
        }
    }
}
