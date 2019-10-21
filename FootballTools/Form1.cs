using FootballTools.Analysis;
using FootballTools.CFBData;
using FootballTools.Entities;
using FootballTools.Reports;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FootballTools
{
    /*
     * Idea:
     *  Download and analyze college football data
     *  Found a few APIs to download data from
     *      (NCAA stopped updating in 2019, so exclusively using https://api.collegefootballdata.com)
     *  Show current info about season (or final results from previous seasons), i.e.:
     *      -Win/loss records
     *      -Schedules and results
     *  Also advanced analysis:
     *      -Calculate odds of teams winning division
     *      -Identify critical games (i.e. winner wins division, or loser mathematically eliminated)
     * LATER:
     *  Could turn this into a live score tracker if the source APIs update real-time

     * TODOs:
     *      -Show Division Rankings with each team's overall/conference win/loss, points forced/allowed
     *      -Show remaining conference games
     *      -Matrix improvements
     *      -Idea: Analyze earlier weeks in the background and cache results
     *              Idea is to eventually have division scenarios for every season/week/division so we can graph them
     *              Only cache completed weeks
     *              Be ready to save progress if program quits while analysis is under way
     *              Then restore the saved progress and continue the analysis later
     *      
     */
    public partial class Form1 : Form
    {
        private readonly string DivisionsFilename = "Divisions.txt";

        private bool mLoadedStorage = false;
        private Dictionary<string, TabPage> mTabs = new Dictionary<string, TabPage>();

        private League mLeague = null;
        private DataMatrix mDataMatrix = null;
        private int mUpdateCounter = 0;

        private List<int> mSelectedMatrixRows { get; set; }
        private List<int> mSelectedMatrixCellRows { get; set; }
        private List<int> mSelectedMatrixCellColumns { get; set; }
        private List<Point> mHighlightedCells { get; set; }

        public Form1()
        {
            InitializeComponent();

            mSelectedMatrixRows = new List<int>();
            mSelectedMatrixCellRows = new List<int>();
            mSelectedMatrixCellColumns = new List<int>();
            mHighlightedCells = new List<Point>();

            LoadData(false);
        }

        private void LoadFromStorage()
        {
            yearTextbox.Text = Properties.Settings.Default.Year.ToString();
            conferenceSelector.SelectedItem = Properties.Settings.Default.Conference;
            divisionSelector.SelectedItem = Properties.Settings.Default.Division;
            teamSelector.SelectedItem = Properties.Settings.Default.Team;

            Size = new System.Drawing.Size(Properties.Settings.Default.WindowWidth, Properties.Settings.Default.WindowHeight);
        }

        private void SaveStorage()
        {
            Properties.Settings.Default.Year = int.Parse(yearTextbox.Text);
            Properties.Settings.Default.Conference = (string)conferenceSelector.SelectedItem;
            Properties.Settings.Default.Division = (string)divisionSelector.SelectedItem;
            Properties.Settings.Default.Team = (string)teamSelector.SelectedItem;

            Properties.Settings.Default.WindowWidth = Size.Width;
            Properties.Settings.Default.WindowHeight = Size.Height;

            Properties.Settings.Default.Save();
        }

        private string SelectedConference => (string) conferenceSelector.SelectedItem;
        private string SelectedDivision => (string)divisionSelector.SelectedItem;
        private string SelectedTeam => (string)teamSelector.SelectedItem;

        private void LoadData(bool forceDownload)
        {
            UpdateStatus("Loading data", 0);

            GameList games = CfbDownloader.Retrieve(int.Parse(yearTextbox.Text), forceDownload);
            mLeague = new League(games, DivisionsFilename);

            UpdateStatus("Finished loading", 0);

            //Populating the selector will trigger a UI update
            PopulateConferenceSelector();
        }

        private void UpdateDisplay()
        {
            //Load the user inputs from storage the first time, and save them every time after that
            if(!mLoadedStorage)
            {
                LoadFromStorage();
                mLoadedStorage = true;
            }
            else
            {
                SaveStorage();
            }
            
            SetUIVisibility();
            
            switch(tabControl1.SelectedIndex)
            {
                //Text, Matrix, League/Conference/Division/Team
                case 0:
                    reportTextbox.Text = string.Join(Constants.Newline, ReportGenerator.GenerateReport(mLeague, CurrentDisplay, SelectedConference, SelectedDivision, SelectedTeam));
                    break;
                case 1:
                    mDataMatrix = ReportGenerator.GenerateTeamMatrix(mLeague, CurrentDisplay, SelectedConference, SelectedDivision, SelectedTeam);
                    PopulateMatrix(matrixGrid, mDataMatrix);

                    mSelectedMatrixRows.Clear();
                    mSelectedMatrixCellRows.Clear();
                    mSelectedMatrixCellColumns.Clear();

                    break;
                case 2:
                    break;
            }
        }

        private DisplayType CurrentDisplay
        {
            get
            {
                bool conferenceSelected = !SelectedConference.Equals(Constants.AllText);
                if(!conferenceSelected)
                {
                    return DisplayType.League;
                }

                bool divisionSelected = conferenceSelected && SelectedDivision != null && !SelectedDivision.Equals(Constants.AllText);
                if (conferenceSelected && !divisionSelected)
                {
                    return DisplayType.Conference;
                }

                bool teamSelected = divisionSelected && SelectedTeam != null && !SelectedTeam.Equals(Constants.AllText);
                if (divisionSelected && !teamSelected)
                {
                    return DisplayType.Division;
                }
                if(teamSelected)
                {
                    return DisplayType.Team;
                }

                return DisplayType.Unknown;
            }
        }

        private void SetUIVisibility()
        {
            DisplayType currentDisplay = CurrentDisplay;

            bool conferenceSelected = !SelectedConference.Equals(Constants.AllText);
            divisionLabel.Visible = conferenceSelected;
            divisionSelector.Visible = conferenceSelected;

            bool divisionSelected = conferenceSelected && SelectedDivision != null && !SelectedDivision.Equals(Constants.AllText);
            teamLabel.Visible = conferenceSelected && divisionSelected;
            teamSelector.Visible = conferenceSelected && divisionSelected;

            analyzeButton.Enabled = divisionSelected;
            
            ShowHideTab(leagueTab, currentDisplay == DisplayType.League);
            ShowHideTab(conferenceTab, currentDisplay == DisplayType.Conference);
            ShowHideTab(divisionTab, currentDisplay == DisplayType.Division);
            ShowHideTab(teamTab, currentDisplay == DisplayType.Team);
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            LoadData(true);
        }

        private void yearTextbox_TextChanged(object sender, EventArgs e)
        {
            LoadData(false);
        }

        private void analyzeButton_Click(object sender, EventArgs e)
        {
            if (!SelectedConference.Equals(Constants.AllText))
            {
                EnableDisableAnalyzeButton(false);

                //Start an async job to calculate the division scenarios
                DivisionScenarioAnalyzer analyzer = DivisionScenarioAnalyzer.Analyze(mLeague, SelectedConference, SelectedDivision,
                    (judgmentsDone, totalJudgments, elapsed) =>
                    {
                        mUpdateCounter++;
                        if(mUpdateCounter >= Constants.AnalysisUiUpdateInterval)
                        {
                            string status = $"Judging {judgmentsDone} of {totalJudgments} outcomes ({elapsed})";
                            double progress = judgmentsDone / totalJudgments;
                            UpdateStatus(status, progress);
                            mUpdateCounter = 0;
                        }
                    },
                    (outputLines, elapsed) =>
                    {
                        UpdateStatus("Analysis complete", 0);
                        UpdateReport("Division Scenarios:" + Constants.Newline + string.Join(Constants.Newline, outputLines) + Constants.Newline + Constants.Newline + reportTextbox.Text);
                        EnableDisableAnalyzeButton(true);
                    });
            }
        }

        private void PopulateConferenceSelector()
        {
            //Populate the conferenceSelector with the available conferences
            conferenceSelector.Items.Clear();
            conferenceSelector.Items.Add(Constants.AllText);
            foreach (Conference conference in mLeague.Conferences)
            {
                conferenceSelector.Items.Add(conference.Name);
            }
            if (conferenceSelector.SelectedIndex != 0)
            {
                conferenceSelector.SelectedIndex = 0;
            }
            else
            {
                UpdateDisplay();
            }
        }

        private void conferenceSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            divisionSelector.Items.Clear();
            divisionSelector.Items.Add(Constants.AllText);

            SetUIVisibility();

            if (!SelectedConference.Equals(Constants.AllText))
            {
                //Populate the divisionSelector with the divisions in the selected conference
                Conference selected = mLeague[SelectedConference];
                foreach (Division division in selected.Divisions)
                {
                    divisionSelector.Items.Add(division.Name);
                }
            }

            if (divisionSelector.SelectedIndex != 0)
            {
                divisionSelector.SelectedIndex = 0;
            }
            else
            {
                UpdateDisplay();
            }
        }

        private void divisionSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            teamSelector.Items.Clear();
            teamSelector.Items.Add(Constants.AllText);

            SetUIVisibility();

            if (!SelectedDivision.Equals(Constants.AllText))
            {
                //Populate the teamSelector with the teams in the selected division
                Division selected = mLeague.FindDivision(SelectedConference, SelectedDivision);
                foreach (Team team in selected.Teams)
                {
                    teamSelector.Items.Add(team.Name);
                }
            }

            if (teamSelector.SelectedIndex != 0)
            {
                teamSelector.SelectedIndex = 0;
            }
            else
            {
                UpdateDisplay();
            }
        }

        private void teamSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void ShowHideTab(TabPage tab, bool show)
        {
            if (show)
            {
                if (mTabs.ContainsKey(tab.Text))
                {
                    tabControl1.Controls.Add(mTabs[tab.Text]);
                    mTabs.Remove(tab.Text);
                }
            }
            else
            {
                if (!mTabs.ContainsKey(tab.Text))
                {
                    mTabs[tab.Text] = tab;
                    tabControl1.Controls.Remove(tab);
                }
            }
        }

        private void UpdateReport(string report)
        {
            if (reportTextbox.InvokeRequired)
            {
                reportTextbox.Invoke((MethodInvoker)delegate { reportTextbox.Text = report; });
            }
            else
            {
                reportTextbox.Text = report;
            }
        }

        private void UpdateStatus(string status, double percentage)
        {
            if (statusBar.InvokeRequired)
            {
                statusBar.Invoke((MethodInvoker)delegate
                {
                    statusText.Text = status;
                    progressText.Text = percentage > 0 ? $"{(percentage*100):0.0}%" : string.Empty;
                    progressBar.Value = (int)Math.Round(percentage * 100);
                });
            }
            else
            {
                statusText.Text = status;
                progressText.Text = percentage > 0 ? $"{(percentage * 100):0.0}%" : string.Empty;
                progressBar.Value = (int)Math.Round(percentage * 100);
            }
        }

        private void EnableDisableAnalyzeButton(bool enable)
        {
            if (analyzeButton.InvokeRequired)
            {
                analyzeButton.Invoke((MethodInvoker)delegate { analyzeButton.Enabled = enable; });
            }
            else
            {
                analyzeButton.Enabled = enable;
            }
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            SaveStorage();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //mJudge.Quit();
        }

        private void matrixGrid_SelectionChanged(object sender, EventArgs e)
        {
            List<int> selectedRows = new List<int>();
            //List<int> deselectedRows = new List<int>();

            List<int> selectedCellRows = new List<int>();
            //List<int> deselectedCellRows = new List<int>();
            List<int> selectedCellColumns = new List<int>();
            //List<int> deselectedCellColumns = new List<int>();

            //Determine changes to the selection
            //New rows selected?
            //List<int> oldSelection = new List<int>(mSelectedMatrixRows);
            for (int i = 0; i < matrixGrid.SelectedRows.Count; i++)
            {
                if (!mSelectedMatrixRows.Contains(matrixGrid.SelectedRows[i].Index))
                {
                    //Console.WriteLine($"Selected row {matrixGrid.SelectedRows[i].Index}");
                    selectedRows.Add(matrixGrid.SelectedRows[i].Index);
                }
                //else
                //{
                //    oldSelection.Remove(matrixGrid.SelectedRows[i].Index);
                //}
            }

            //foreach (int index in oldSelection)
            //{
            //    //Console.WriteLine($"Deselected row {index}");
            //    deselectedRows.Add((index));
            //}

            //New cells selected?
            //List<int> oldCellRows = new List<int>(mSelectedMatrixCellRows);
            //List<int> oldCellColumns = new List<int>(mSelectedMatrixCellColumns);
            for (int i = 0; i < matrixGrid.SelectedCells.Count; i++)
            {
                int foundIndex = -1;
                for (int searchIndex = 0; searchIndex < mSelectedMatrixCellRows.Count; searchIndex++)
                {
                    if (matrixGrid.SelectedCells[i].RowIndex == mSelectedMatrixCellRows[searchIndex] &&
                        matrixGrid.SelectedCells[i].ColumnIndex == mSelectedMatrixCellColumns[searchIndex])
                    {
                        foundIndex = searchIndex;
                        break;
                    }
                }

                if (foundIndex >= 0)
                {
                    //oldCellRows.RemoveAt(foundIndex);
                    //oldCellColumns.RemoveAt(foundIndex);
                }
                else
                {
                    //Console.WriteLine($"Selected cell {matrixGrid.SelectedCells[i].RowIndex}-{matrixGrid.SelectedCells[i].ColumnIndex}");
                    selectedCellRows.Add(matrixGrid.SelectedCells[i].RowIndex);
                    selectedCellColumns.Add(matrixGrid.SelectedCells[i].ColumnIndex);
                }
            }

            //for (int i = 0; i < oldCellRows.Count; i++)
            //{
            //    //Console.WriteLine($"Deselected cell {oldCellRows[i]}-{oldCellColumns[i]}");
            //    deselectedCellRows.Add(oldCellRows[i]);
            //    deselectedCellColumns.Add(oldCellColumns[i]);
            //}

            int row = selectedRows.Count > 0 ? selectedRows[selectedRows.Count - 1] : -1;
            if (row >= 0)
            {
                HighlightMatrixRow(row);
            }
            else
            {
                int cellRow = selectedCellRows.Count > 0 ? selectedCellRows[selectedCellRows.Count - 1] : -1;
                int cellColumn = selectedCellColumns.Count > 0 ? selectedCellColumns[selectedCellColumns.Count - 1] : -1;
                if (cellRow >= 0 && cellColumn >= 0)
                {
                    HighlightMatrixCell(cellRow, cellColumn);
                }
            }
            
            //Remember the current selections
            mSelectedMatrixRows.Clear();
            mSelectedMatrixCellRows.Clear();
            mSelectedMatrixCellColumns.Clear();
            for (int i = 0; i < selectedRows.Count; i++)
            {
                mSelectedMatrixRows.Add(selectedRows[i]);
            }
            for (int i = 0; i < selectedCellRows.Count; i++)
            {
                mSelectedMatrixCellRows.Add(selectedCellRows[i]);
                mSelectedMatrixCellColumns.Add(selectedCellColumns[i]);
            }
        }

        private void HighlightMatrixRow(int rowIndex)
        {
            mHighlightedCells.Clear();
            if(rowIndex >= 0)
            {
                //Console.WriteLine($"Highlighting row {rowIndex}");
                for (int i = 0; i < matrixGrid.ColumnCount; i++)
                {
                    mHighlightedCells.Add(new Point(i, rowIndex));
                }
            }

            matrixGrid.Refresh();
        }

        private void HighlightMatrixCell(int cellRowIndex, int cellColumnIndex)
        {
            mHighlightedCells.Clear();
            if (cellRowIndex >= 0 && cellColumnIndex >= 0)
            {
                //Console.WriteLine($"Highlighting cell {cellRowIndex}-{cellColumnIndex}");
                mHighlightedCells.Add(new Point(cellRowIndex, cellColumnIndex));
            }

            //bool rowMode = mHighlightedCells.Count > 1;
            //string label = mDataMatrix.Labels[e.RowIndex, e.ColumnIndex];
            //Color bgColor = mDataMatrix.Colors[e.RowIndex, e.ColumnIndex];

            //Highlight all cells with the same data
            string label = mDataMatrix.Labels[cellRowIndex, cellColumnIndex].Split('\n')[0].Trim();

            for(int i=0; i<mDataMatrix.Labels.GetLength(0); i++)
            for (int j = 0; j < mDataMatrix.Labels.GetLength(1); j++)
            {
                if (i != cellRowIndex && j != cellColumnIndex)
                {
                    string searchLabel = mDataMatrix.Labels[i, j].Split('\n')[0].Trim();
                    if (label.Equals(searchLabel))
                    {
                        mHighlightedCells.Add(new Point(i, j));
                    }
                }
            }

            matrixGrid.Refresh();
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

        /// <summary>
        /// Handles special formatting for highlighted cells
        /// </summary>
        private void matrixGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1 && e.ColumnIndex != e.RowIndex)
            {
                //Console.WriteLine($"{e.RowIndex}-{e.ColumnIndex}: NumHighlighted= {mHighlightedCells.Count}");
                if (mHighlightedCells.Contains(new Point(e.ColumnIndex, e.RowIndex)))
                {
                    //Console.WriteLine($"Drawing border on cell {e.RowIndex}-{e.ColumnIndex}");
                    using (Brush borderBrush = new SolidBrush(Color.Blue))
                    {
                        using (Pen borderPen = new Pen(borderBrush, 2))
                        {
                            Rectangle rectDimensions = e.CellBounds;
                            rectDimensions.Width -= 2;
                            rectDimensions.Height -= 2;
                            rectDimensions.X = rectDimensions.Left + 1;
                            rectDimensions.Y = rectDimensions.Top + 1;

                            e.Graphics.DrawRectangle(borderPen, rectDimensions);

                            e.Handled = true;
                        }
                    }
                }
            }
        }
    }
}
