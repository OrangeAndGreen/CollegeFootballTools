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

                        foreach (Game game in league.AllGames)
                        {
                            if ((game.home_team.Equals(team.Name) && game.away_team.Equals(rival.Name)) ||
                                (game.home_team.Equals(rival.Name) && game.away_team.Equals(team.Name)))
                            {
                                string entry = game.GameDate.ToString("MMM dd");
                                Color rowColor = Color.White;
                                Color colColor = Color.White;
                                if (game.home_points.HasValue && game.away_points.HasValue)
                                {
                                    entry += $"{Constants.Newline}{game.home_points}-{game.away_points}";
                                    bool rowWin = game.home_team.Equals(team.Name) == (game.home_points > game.away_points);
                                    rowColor = rowWin ? Constants.WinColor : Constants.LossColor;
                                    colColor = rowWin ? Constants.LossColor : Constants.WinColor;
                                }

                                labels[teamIndex, rivalIndex] = entry;
                                colors[teamIndex, rivalIndex] = rowColor;

                                labels[rivalIndex, teamIndex] = entry;
                                colors[rivalIndex, teamIndex] = colColor;
                            }
                        }
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
            grid.AllowUserToResizeColumns = false;
            //Special code to hide the black arrow in the row header
            //grid.RowHeadersDefaultCellStyle.Padding = new Padding(grid.RowHeadersWidth);
            //grid.RowPostPaint += (sender, e) =>
            //{
            //    object o = grid.Rows[e.RowIndex].HeaderCell.Value;

            //    e.Graphics.FillRectangle(new SolidBrush(grid.RowHeadersDefaultCellStyle.BackColor),
            //        e.RowBounds.Left + 1, e.RowBounds.Top + 1,
            //        grid.Rows[e.RowIndex].HeaderCell.Size.Width - 2, grid.Rows[e.RowIndex].HeaderCell.Size.Height - 2);

            //    e.Graphics.DrawString(o != null ? o.ToString() : "",
            //        grid.Font, 
            //        new SolidBrush(grid.RowHeadersDefaultCellStyle.ForeColor),
            //        //new PointF((float)e.RowBounds.Left + 2, (float)e.RowBounds.Top + 4),
            //        new RectangleF(e.RowBounds.Left + 1, e.RowBounds.Top + 1,
            //            grid.Rows[e.RowIndex].HeaderCell.Size.Width - 2, grid.Rows[e.RowIndex].HeaderCell.Size.Height - 2),
            //        new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.LineLimit });
            //};

            for (int c = 0; c < matrix.Labels.GetLength(1); c++)
            {
                grid.Columns[c].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                grid.Columns[c].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (matrix.ColumnHeaders != null)
                {
                    grid.Columns[c].HeaderText = matrix.ColumnHeaders[c];
                }
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
                if (matrix.RowHeaders != null)
                {
                    row.HeaderCell.Value = matrix.RowHeaders[r];
                }
            }

            grid.ClearSelection();
        }
    }
}
