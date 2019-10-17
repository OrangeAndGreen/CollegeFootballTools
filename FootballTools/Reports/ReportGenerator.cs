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
                    teams = new List<Team> { league.FindTeam(teamName) };
                    break;
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

            ret.Add("Records:");
            foreach (Team team in sortedTeams)
            {
                ret.Add($"{team.Name}: {team.ComboRecord}");
            }

            ret.Add(string.Empty);

            ret.Add("All games:");
            List<string> teamNames = new List<string>();
            foreach(Team team in sortedTeams)
            {
                teamNames.Add(team.Name);
            }
            List<Game> games = Game.FilterGamesByTeams(league.AllGames, teamNames);
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
        public static void PopulateMatrix(League league, DisplayType displayType, DataGridView grid, string conferenceName, string divisionName, string teamName)
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

            PopulateMatrix(grid, matrix);
        }

        /// <summary>
        /// Builds a matrix of team head-to-head matchups given a list of teams
        /// </summary>
        public static DataMatrix GenerateTeamMatrix(League league, List<Team> teams, bool sortByTotalOrConferenceWins)
        {
            List<Team> sortedTeams = new List<Team>(teams);
            Team.SortTeamList(sortedTeams, sortByTotalOrConferenceWins);

            string[,] labels = new string[sortedTeams.Count + 1, sortedTeams.Count + 1];
            Color[,] colors = new Color[sortedTeams.Count + 1, sortedTeams.Count + 1];

            for (int teamIndex = 0; teamIndex < sortedTeams.Count; teamIndex++)
            {
                Team team = sortedTeams[teamIndex];

                string teamLabel = $"{team.Name}{Constants.Newline}{team.ComboRecord}";
                labels[0, teamIndex + 1] = teamLabel;
                colors[0, teamIndex + 1] = Constants.LossColor;
                labels[teamIndex + 1, 0] = teamLabel;
                colors[teamIndex + 1, 0] = Constants.WinColor;

                for (int rivalIndex = 0; rivalIndex < sortedTeams.Count; rivalIndex++)
                {
                    if (teamIndex != rivalIndex)
                    {
                        Team rival = sortedTeams[rivalIndex];

                        foreach (Game game in league.AllGames)
                        {
                            if ((game.home_team.Equals(team.Name) && game.away_team.Equals(rival.Name)) ||
                                (game.home_team.Equals(rival.Name) && game.away_team.Equals(team.Name)))
                            {
                                string entry = game.GameDate.ToString("MMMM dd");
                                Color rowColor = Color.White;
                                Color colColor = Color.White;
                                if (game.home_points.HasValue && game.away_points.HasValue)
                                {
                                    entry += $"{Constants.Newline}{game.home_points}-{game.away_points}";
                                    bool rowWin = game.home_team.Equals(team.Name) == (game.home_points > game.away_points);
                                    rowColor = rowWin ? Constants.WinColor : Constants.LossColor;
                                    colColor = rowWin ? Constants.LossColor : Constants.WinColor;
                                }

                                labels[teamIndex + 1, rivalIndex + 1] = entry;
                                colors[teamIndex + 1, rivalIndex + 1] = rowColor;

                                labels[rivalIndex + 1, teamIndex + 1] = entry;
                                colors[rivalIndex + 1, teamIndex + 1] = colColor;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < sortedTeams.Count + 1; i++)
                for (int j = 0; j < sortedTeams.Count + 1; j++)
                {
                    if (labels[i, j] == null)
                    {
                        labels[i, j] = string.Empty;
                    }
                }


            return new DataMatrix(labels, colors);
        }

        /// <summary>
        /// Populates a DataGridView given a DataMatrix
        /// </summary>
        private static void PopulateMatrix(DataGridView grid, DataMatrix matrix)
        {
            grid.DataSource = null;
            grid.Rows.Clear();

            if (matrix == null)
            {
                return;
            }

            //Setup the grid
            grid.DefaultCellStyle.SelectionBackColor = Color.LightGray;
            grid.ColumnCount = matrix.Labels.GetLength(1);
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            for (int c = 0; c < matrix.Labels.GetLength(1); c++)
            {
                grid.Columns[c].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                grid.Columns[c].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //Add the data and BG color to each cell
            for (int r = 0; r < matrix.Labels.GetLength(0); r++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(grid);

                for (int c = 0; c < matrix.Labels.GetLength(1); c++)
                {
                    row.Cells[c].Value = matrix.Labels[r, c];
                    row.Cells[c].Style.BackColor = matrix.Colors[r, c];
                }

                grid.Rows.Add(row);
            }

            grid.ClearSelection();
        }
    }
}
