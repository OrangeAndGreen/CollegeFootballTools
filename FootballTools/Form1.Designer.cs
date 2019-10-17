namespace FootballTools
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.conferenceSelector = new System.Windows.Forms.ComboBox();
            this.conferenceLabel = new System.Windows.Forms.Label();
            this.reportTextbox = new System.Windows.Forms.TextBox();
            this.yearTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.divisionLabel = new System.Windows.Forms.Label();
            this.divisionSelector = new System.Windows.Forms.ComboBox();
            this.downloadButton = new System.Windows.Forms.Button();
            this.analyzeButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.textTab = new System.Windows.Forms.TabPage();
            this.leagueTab = new System.Windows.Forms.TabPage();
            this.conferenceTab = new System.Windows.Forms.TabPage();
            this.divisionTab = new System.Windows.Forms.TabPage();
            this.matrixGrid = new System.Windows.Forms.DataGridView();
            this.teamTab = new System.Windows.Forms.TabPage();
            this.teamLabel = new System.Windows.Forms.Label();
            this.teamSelector = new System.Windows.Forms.ComboBox();
            this.matrixTab = new System.Windows.Forms.TabPage();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1.SuspendLayout();
            this.textTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.matrixGrid)).BeginInit();
            this.matrixTab.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // conferenceSelector
            // 
            this.conferenceSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.conferenceSelector.FormattingEnabled = true;
            this.conferenceSelector.Location = new System.Drawing.Point(117, 29);
            this.conferenceSelector.Name = "conferenceSelector";
            this.conferenceSelector.Size = new System.Drawing.Size(150, 21);
            this.conferenceSelector.TabIndex = 0;
            this.conferenceSelector.SelectedIndexChanged += new System.EventHandler(this.conferenceSelector_SelectedIndexChanged);
            // 
            // conferenceLabel
            // 
            this.conferenceLabel.AutoSize = true;
            this.conferenceLabel.Location = new System.Drawing.Point(114, 13);
            this.conferenceLabel.Name = "conferenceLabel";
            this.conferenceLabel.Size = new System.Drawing.Size(62, 13);
            this.conferenceLabel.TabIndex = 1;
            this.conferenceLabel.Text = "Conference";
            // 
            // reportTextbox
            // 
            this.reportTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.reportTextbox.Location = new System.Drawing.Point(9, 6);
            this.reportTextbox.Multiline = true;
            this.reportTextbox.Name = "reportTextbox";
            this.reportTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.reportTextbox.Size = new System.Drawing.Size(968, 416);
            this.reportTextbox.TabIndex = 2;
            // 
            // yearTextbox
            // 
            this.yearTextbox.Location = new System.Drawing.Point(16, 29);
            this.yearTextbox.Name = "yearTextbox";
            this.yearTextbox.Size = new System.Drawing.Size(80, 20);
            this.yearTextbox.TabIndex = 3;
            this.yearTextbox.Text = "2019";
            this.yearTextbox.TextChanged += new System.EventHandler(this.yearTextbox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Year";
            // 
            // divisionLabel
            // 
            this.divisionLabel.AutoSize = true;
            this.divisionLabel.Location = new System.Drawing.Point(287, 13);
            this.divisionLabel.Name = "divisionLabel";
            this.divisionLabel.Size = new System.Drawing.Size(44, 13);
            this.divisionLabel.TabIndex = 6;
            this.divisionLabel.Text = "Division";
            // 
            // divisionSelector
            // 
            this.divisionSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.divisionSelector.FormattingEnabled = true;
            this.divisionSelector.Location = new System.Drawing.Point(290, 29);
            this.divisionSelector.Name = "divisionSelector";
            this.divisionSelector.Size = new System.Drawing.Size(150, 21);
            this.divisionSelector.TabIndex = 5;
            this.divisionSelector.SelectedIndexChanged += new System.EventHandler(this.divisionSelector_SelectedIndexChanged);
            // 
            // downloadButton
            // 
            this.downloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadButton.Location = new System.Drawing.Point(901, 28);
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(75, 23);
            this.downloadButton.TabIndex = 7;
            this.downloadButton.Text = "Update";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
            // 
            // analyzeButton
            // 
            this.analyzeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.analyzeButton.Location = new System.Drawing.Point(820, 29);
            this.analyzeButton.Name = "analyzeButton";
            this.analyzeButton.Size = new System.Drawing.Size(75, 23);
            this.analyzeButton.TabIndex = 8;
            this.analyzeButton.Text = "Analyze";
            this.analyzeButton.UseVisualStyleBackColor = true;
            this.analyzeButton.Click += new System.EventHandler(this.analyzeButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.textTab);
            this.tabControl1.Controls.Add(this.matrixTab);
            this.tabControl1.Controls.Add(this.leagueTab);
            this.tabControl1.Controls.Add(this.conferenceTab);
            this.tabControl1.Controls.Add(this.divisionTab);
            this.tabControl1.Controls.Add(this.teamTab);
            this.tabControl1.Location = new System.Drawing.Point(0, 56);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(991, 305);
            this.tabControl1.TabIndex = 9;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // textTab
            // 
            this.textTab.Controls.Add(this.reportTextbox);
            this.textTab.Location = new System.Drawing.Point(4, 22);
            this.textTab.Name = "textTab";
            this.textTab.Padding = new System.Windows.Forms.Padding(3);
            this.textTab.Size = new System.Drawing.Size(983, 425);
            this.textTab.TabIndex = 1;
            this.textTab.Text = "Text";
            this.textTab.UseVisualStyleBackColor = true;
            // 
            // leagueTab
            // 
            this.leagueTab.Location = new System.Drawing.Point(4, 22);
            this.leagueTab.Name = "leagueTab";
            this.leagueTab.Padding = new System.Windows.Forms.Padding(3);
            this.leagueTab.Size = new System.Drawing.Size(983, 425);
            this.leagueTab.TabIndex = 0;
            this.leagueTab.Text = "League";
            this.leagueTab.UseVisualStyleBackColor = true;
            // 
            // conferenceTab
            // 
            this.conferenceTab.Location = new System.Drawing.Point(4, 22);
            this.conferenceTab.Name = "conferenceTab";
            this.conferenceTab.Size = new System.Drawing.Size(983, 425);
            this.conferenceTab.TabIndex = 2;
            this.conferenceTab.Text = "Conference";
            this.conferenceTab.UseVisualStyleBackColor = true;
            // 
            // divisionTab
            // 
            this.divisionTab.Location = new System.Drawing.Point(4, 22);
            this.divisionTab.Name = "divisionTab";
            this.divisionTab.Size = new System.Drawing.Size(983, 425);
            this.divisionTab.TabIndex = 3;
            this.divisionTab.Text = "Division";
            this.divisionTab.UseVisualStyleBackColor = true;
            // 
            // matrixGrid
            // 
            this.matrixGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.matrixGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.matrixGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.matrixGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.matrixGrid.ColumnHeadersVisible = false;
            this.matrixGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.matrixGrid.Location = new System.Drawing.Point(3, 3);
            this.matrixGrid.Name = "matrixGrid";
            this.matrixGrid.RowHeadersVisible = false;
            this.matrixGrid.ShowCellErrors = false;
            this.matrixGrid.ShowEditingIcon = false;
            this.matrixGrid.ShowRowErrors = false;
            this.matrixGrid.Size = new System.Drawing.Size(977, 273);
            this.matrixGrid.TabIndex = 0;
            // 
            // teamTab
            // 
            this.teamTab.Location = new System.Drawing.Point(4, 22);
            this.teamTab.Name = "teamTab";
            this.teamTab.Size = new System.Drawing.Size(983, 425);
            this.teamTab.TabIndex = 4;
            this.teamTab.Text = "Team";
            this.teamTab.UseVisualStyleBackColor = true;
            // 
            // teamLabel
            // 
            this.teamLabel.AutoSize = true;
            this.teamLabel.Location = new System.Drawing.Point(461, 13);
            this.teamLabel.Name = "teamLabel";
            this.teamLabel.Size = new System.Drawing.Size(34, 13);
            this.teamLabel.TabIndex = 11;
            this.teamLabel.Text = "Team";
            // 
            // teamSelector
            // 
            this.teamSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.teamSelector.FormattingEnabled = true;
            this.teamSelector.Location = new System.Drawing.Point(464, 29);
            this.teamSelector.Name = "teamSelector";
            this.teamSelector.Size = new System.Drawing.Size(150, 21);
            this.teamSelector.TabIndex = 10;
            this.teamSelector.SelectedIndexChanged += new System.EventHandler(this.teamSelector_SelectedIndexChanged);
            // 
            // matrixTab
            // 
            this.matrixTab.Controls.Add(this.matrixGrid);
            this.matrixTab.Location = new System.Drawing.Point(4, 22);
            this.matrixTab.Name = "matrixTab";
            this.matrixTab.Size = new System.Drawing.Size(983, 279);
            this.matrixTab.TabIndex = 5;
            this.matrixTab.Text = "Matrix";
            this.matrixTab.UseVisualStyleBackColor = true;
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText,
            this.progressBar});
            this.statusBar.Location = new System.Drawing.Point(0, 360);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(988, 22);
            this.statusBar.TabIndex = 12;
            this.statusBar.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // statusText
            // 
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(840, 17);
            this.statusText.Spring = true;
            this.statusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 382);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.teamLabel);
            this.Controls.Add(this.teamSelector);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.analyzeButton);
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.divisionLabel);
            this.Controls.Add(this.divisionSelector);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.yearTextbox);
            this.Controls.Add(this.conferenceLabel);
            this.Controls.Add(this.conferenceSelector);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.tabControl1.ResumeLayout(false);
            this.textTab.ResumeLayout(false);
            this.textTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.matrixGrid)).EndInit();
            this.matrixTab.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox conferenceSelector;
        private System.Windows.Forms.Label conferenceLabel;
        private System.Windows.Forms.TextBox reportTextbox;
        private System.Windows.Forms.TextBox yearTextbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label divisionLabel;
        private System.Windows.Forms.ComboBox divisionSelector;
        private System.Windows.Forms.Button downloadButton;
        private System.Windows.Forms.Button analyzeButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage leagueTab;
        private System.Windows.Forms.TabPage textTab;
        private System.Windows.Forms.TabPage conferenceTab;
        private System.Windows.Forms.TabPage divisionTab;
        private System.Windows.Forms.TabPage teamTab;
        private System.Windows.Forms.Label teamLabel;
        private System.Windows.Forms.ComboBox teamSelector;
        private System.Windows.Forms.DataGridView matrixGrid;
        private System.Windows.Forms.TabPage matrixTab;
        private System.Windows.Forms.StatusStrip statusBar;
        public System.Windows.Forms.ToolStripStatusLabel statusText;
        public System.Windows.Forms.ToolStripProgressBar progressBar;
    }
}

