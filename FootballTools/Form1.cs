using FootballTools.Analysis;
using FootballTools.CFBData;
using FootballTools.Entities;
using FootballTools.Reports;
using System;
using System.Collections.Generic;
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
     *      
     */
    public partial class Form1 : Form
    {
        private readonly string DivisionsFilename = "Divisions.txt";

        private bool mLoadedStorage = false;
        private Dictionary<string, TabPage> mTabs = new Dictionary<string, TabPage>();

        private League mLeague = null;
        private DivisionWinnerCalculator mJudge = new DivisionWinnerCalculator();

        public Form1()
        {
            InitializeComponent();

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
            reportTextbox.Text = "Loading data";

            List<Game> games = CfbDownloader.Retrieve(int.Parse(yearTextbox.Text), forceDownload);
            mLeague = new League(games, DivisionsFilename);

            reportTextbox.Text = "Finished loading";

            //Populating the selector will trigger a UI update
            PopulateConferenceSelector();
        }

        private void UpdateDisplay(bool withAnalysis)
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
                    ReportGenerator.PopulateMatrix(mLeague, CurrentDisplay, matrixGrid, SelectedConference, SelectedDivision, SelectedTeam);
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
                DivisionScenarioAnalyzer analyzer = new DivisionScenarioAnalyzer();

                analyzer.Analyze(mLeague, SelectedConference, SelectedDivision, mJudge,
                    (status, progress) =>
                    {
                        UpdateStatus(status, progress);
                    },
                    (outputLines) =>
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
                UpdateDisplay(false);
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
                UpdateDisplay(false);
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
                UpdateDisplay(false);
            }
        }

        private void teamSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplay(false);
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
                    progressBar.Value = (int)Math.Round(percentage * 100);
                });
            }
            else
            {
                statusText.Text = status;
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
            UpdateDisplay(false);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mJudge.Quit();
        }
    }
}
