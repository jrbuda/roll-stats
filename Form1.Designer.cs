namespace Roll_stats
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            toolStrip = new ToolStrip();
            toolStripButtonGetInputFile = new ToolStripButton();
            toolStripButtonReset = new ToolStripButton();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            toolStripProgressBar = new ToolStripProgressBar();
            tabControl = new TabControl();
            tabInputSettings = new TabPage();
            groupBoxInput = new GroupBox();
            chbDebug = new CheckBox();
            txtInputFile = new TextBox();
            btnGetInputFile = new Button();
            tabRollStatistics = new TabPage();
            splitContainerMain = new SplitContainer();
            splitContainerCharacters = new SplitContainer();
            clbCharacters = new CheckedListBox();
            btnMergeCharacters = new Button();
            splitContainerStats = new SplitContainer();
            dgvRollStats = new DataGridView();
            dgvRollReasons = new DataGridView();
            tabDetails = new TabPage();
            groupBoxSkippedRolls = new GroupBox();
            lstSkippedRolls = new ListView();
            groupBoxInputRead = new GroupBox();
            lstInputRead = new ListBox();
            toolStrip.SuspendLayout();
            statusStrip1.SuspendLayout();
            tabControl.SuspendLayout();
            tabInputSettings.SuspendLayout();
            groupBoxInput.SuspendLayout();
            tabRollStatistics.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerCharacters).BeginInit();
            splitContainerCharacters.Panel1.SuspendLayout();
            splitContainerCharacters.Panel2.SuspendLayout();
            splitContainerCharacters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerStats).BeginInit();
            splitContainerStats.Panel1.SuspendLayout();
            splitContainerStats.Panel2.SuspendLayout();
            splitContainerStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRollStats).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvRollReasons).BeginInit();
            tabDetails.SuspendLayout();
            groupBoxSkippedRolls.SuspendLayout();
            groupBoxInputRead.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip
            // 
            toolStrip.Items.AddRange(new ToolStripItem[] { toolStripButtonGetInputFile, toolStripButtonReset });
            toolStrip.Location = new Point(0, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new Size(1356, 25);
            toolStrip.TabIndex = 0;
            toolStrip.Text = "toolStrip1";
            // 
            // toolStripButtonGetInputFile
            // 
            toolStripButtonGetInputFile.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonGetInputFile.Image = (Image)resources.GetObject("toolStripButtonGetInputFile.Image");
            toolStripButtonGetInputFile.ImageTransparentColor = Color.Magenta;
            toolStripButtonGetInputFile.Name = "toolStripButtonGetInputFile";
            toolStripButtonGetInputFile.Size = new Size(23, 22);
            toolStripButtonGetInputFile.Text = "Get Input File";
            // 
            // toolStripButtonReset
            // 
            toolStripButtonReset.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonReset.Image = (Image)resources.GetObject("toolStripButtonReset.Image");
            toolStripButtonReset.ImageTransparentColor = Color.Magenta;
            toolStripButtonReset.Name = "toolStripButtonReset";
            toolStripButtonReset.Size = new Size(23, 22);
            toolStripButtonReset.Text = "Reset";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel, toolStripProgressBar });
            statusStrip1.Location = new Point(0, 595);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1356, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(39, 17);
            toolStripStatusLabel.Text = "Ready";
            // 
            // toolStripProgressBar
            // 
            toolStripProgressBar.Alignment = ToolStripItemAlignment.Right;
            toolStripProgressBar.Name = "toolStripProgressBar";
            toolStripProgressBar.Size = new Size(100, 16);
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabInputSettings);
            tabControl.Controls.Add(tabRollStatistics);
            tabControl.Controls.Add(tabDetails);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 25);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1356, 570);
            tabControl.TabIndex = 2;
            // 
            // tabInputSettings
            // 
            tabInputSettings.Controls.Add(groupBoxInput);
            tabInputSettings.Location = new Point(4, 24);
            tabInputSettings.Name = "tabInputSettings";
            tabInputSettings.Padding = new Padding(3);
            tabInputSettings.Size = new Size(1348, 542);
            tabInputSettings.TabIndex = 0;
            tabInputSettings.Text = "Input & Settings";
            tabInputSettings.UseVisualStyleBackColor = true;
            // 
            // groupBoxInput
            // 
            groupBoxInput.Controls.Add(chbDebug);
            groupBoxInput.Controls.Add(txtInputFile);
            groupBoxInput.Controls.Add(btnGetInputFile);
            groupBoxInput.Dock = DockStyle.Fill;
            groupBoxInput.Location = new Point(3, 3);
            groupBoxInput.Name = "groupBoxInput";
            groupBoxInput.Size = new Size(1342, 536);
            groupBoxInput.TabIndex = 0;
            groupBoxInput.TabStop = false;
            groupBoxInput.Text = "Input & Settings";
            // 
            // chbDebug
            // 
            chbDebug.AutoSize = true;
            chbDebug.Location = new Point(393, 25);
            chbDebug.Name = "chbDebug";
            chbDebug.Size = new Size(95, 19);
            chbDebug.TabIndex = 2;
            chbDebug.Text = "Debug Mode";
            chbDebug.UseVisualStyleBackColor = true;
            chbDebug.CheckedChanged += chbDebug_CheckedChanged;
            // 
            // txtInputFile
            // 
            txtInputFile.Enabled = false;
            txtInputFile.Location = new Point(87, 23);
            txtInputFile.Name = "txtInputFile";
            txtInputFile.Size = new Size(300, 23);
            txtInputFile.TabIndex = 1;
            // 
            // btnGetInputFile
            // 
            btnGetInputFile.Location = new Point(6, 22);
            btnGetInputFile.Name = "btnGetInputFile";
            btnGetInputFile.Size = new Size(75, 23);
            btnGetInputFile.TabIndex = 0;
            btnGetInputFile.Text = "Get Input File";
            btnGetInputFile.UseVisualStyleBackColor = true;
            btnGetInputFile.Click += btnGetInputFile_Click;
            // 
            // tabRollStatistics
            // 
            tabRollStatistics.Controls.Add(splitContainerMain);
            tabRollStatistics.Location = new Point(4, 24);
            tabRollStatistics.Name = "tabRollStatistics";
            tabRollStatistics.Padding = new Padding(3);
            tabRollStatistics.Size = new Size(1348, 542);
            tabRollStatistics.TabIndex = 1;
            tabRollStatistics.Text = "Roll Statistics";
            tabRollStatistics.UseVisualStyleBackColor = true;
            // 
            // splitContainerMain
            // 
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.Location = new Point(3, 3);
            splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.Controls.Add(splitContainerCharacters);
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(splitContainerStats);
            splitContainerMain.Size = new Size(1342, 536);
            splitContainerMain.SplitterDistance = 248;
            splitContainerMain.TabIndex = 0;
            // 
            // splitContainerCharacters
            // 
            splitContainerCharacters.Dock = DockStyle.Fill;
            splitContainerCharacters.Location = new Point(0, 0);
            splitContainerCharacters.Name = "splitContainerCharacters";
            splitContainerCharacters.Orientation = Orientation.Horizontal;
            // 
            // splitContainerCharacters.Panel1
            // 
            splitContainerCharacters.Panel1.Controls.Add(clbCharacters);
            // 
            // splitContainerCharacters.Panel2
            // 
            splitContainerCharacters.Panel2.Controls.Add(btnMergeCharacters);
            splitContainerCharacters.Size = new Size(248, 536);
            splitContainerCharacters.SplitterDistance = 494;
            splitContainerCharacters.TabIndex = 1;
            // 
            // clbCharacters
            // 
            clbCharacters.Dock = DockStyle.Fill;
            clbCharacters.FormattingEnabled = true;
            clbCharacters.Location = new Point(0, 0);
            clbCharacters.Name = "clbCharacters";
            clbCharacters.Size = new Size(248, 494);
            clbCharacters.TabIndex = 0;
            clbCharacters.ItemCheck += clbCharacters_ItemCheck;
            // 
            // btnMergeCharacters
            // 
            btnMergeCharacters.Dock = DockStyle.Fill;
            btnMergeCharacters.Location = new Point(0, 0);
            btnMergeCharacters.Name = "btnMergeCharacters";
            btnMergeCharacters.Size = new Size(248, 38);
            btnMergeCharacters.TabIndex = 0;
            btnMergeCharacters.Text = "Merge Characters";
            btnMergeCharacters.UseVisualStyleBackColor = true;
            btnMergeCharacters.Click += btnMergeCharacters_Click;
            // 
            // splitContainerStats
            // 
            splitContainerStats.Dock = DockStyle.Fill;
            splitContainerStats.Location = new Point(0, 0);
            splitContainerStats.Name = "splitContainerStats";
            splitContainerStats.Orientation = Orientation.Horizontal;
            // 
            // splitContainerStats.Panel1
            // 
            splitContainerStats.Panel1.Controls.Add(dgvRollStats);
            // 
            // splitContainerStats.Panel2
            // 
            splitContainerStats.Panel2.Controls.Add(dgvRollReasons);
            splitContainerStats.Size = new Size(1090, 536);
            splitContainerStats.SplitterDistance = 297;
            splitContainerStats.TabIndex = 0;
            // 
            // dgvRollStats
            // 
            dgvRollStats.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dgvRollStats.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRollStats.Dock = DockStyle.Fill;
            dgvRollStats.Location = new Point(0, 0);
            dgvRollStats.Name = "dgvRollStats";
            dgvRollStats.Size = new Size(1090, 297);
            dgvRollStats.TabIndex = 0;
            dgvRollStats.SelectionChanged += dgvRollStats_SelectionChanged;
            // 
            // dgvRollReasons
            // 
            dgvRollReasons.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRollReasons.Dock = DockStyle.Fill;
            dgvRollReasons.Location = new Point(0, 0);
            dgvRollReasons.Name = "dgvRollReasons";
            dgvRollReasons.Size = new Size(1090, 235);
            dgvRollReasons.TabIndex = 0;
            // 
            // tabDetails
            // 
            tabDetails.Controls.Add(groupBoxSkippedRolls);
            tabDetails.Controls.Add(groupBoxInputRead);
            tabDetails.Location = new Point(4, 24);
            tabDetails.Name = "tabDetails";
            tabDetails.Padding = new Padding(3);
            tabDetails.Size = new Size(1348, 542);
            tabDetails.TabIndex = 2;
            tabDetails.Text = "Details";
            tabDetails.UseVisualStyleBackColor = true;
            // 
            // groupBoxSkippedRolls
            // 
            groupBoxSkippedRolls.Controls.Add(lstSkippedRolls);
            groupBoxSkippedRolls.Dock = DockStyle.Fill;
            groupBoxSkippedRolls.Location = new Point(3, 203);
            groupBoxSkippedRolls.Name = "groupBoxSkippedRolls";
            groupBoxSkippedRolls.Size = new Size(1342, 336);
            groupBoxSkippedRolls.TabIndex = 1;
            groupBoxSkippedRolls.TabStop = false;
            groupBoxSkippedRolls.Text = "Skipped Rolls";
            // 
            // lstSkippedRolls
            // 
            lstSkippedRolls.Dock = DockStyle.Fill;
            lstSkippedRolls.Location = new Point(3, 19);
            lstSkippedRolls.Name = "lstSkippedRolls";
            lstSkippedRolls.Size = new Size(1336, 314);
            lstSkippedRolls.TabIndex = 0;
            lstSkippedRolls.UseCompatibleStateImageBehavior = false;
            // 
            // groupBoxInputRead
            // 
            groupBoxInputRead.Controls.Add(lstInputRead);
            groupBoxInputRead.Dock = DockStyle.Top;
            groupBoxInputRead.Location = new Point(3, 3);
            groupBoxInputRead.Name = "groupBoxInputRead";
            groupBoxInputRead.Size = new Size(1342, 200);
            groupBoxInputRead.TabIndex = 0;
            groupBoxInputRead.TabStop = false;
            groupBoxInputRead.Text = "Input Read";
            // 
            // lstInputRead
            // 
            lstInputRead.Dock = DockStyle.Fill;
            lstInputRead.FormattingEnabled = true;
            lstInputRead.ItemHeight = 15;
            lstInputRead.Location = new Point(3, 19);
            lstInputRead.Name = "lstInputRead";
            lstInputRead.Size = new Size(1336, 178);
            lstInputRead.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1356, 617);
            Controls.Add(tabControl);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Roll Stats!";
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            tabControl.ResumeLayout(false);
            tabInputSettings.ResumeLayout(false);
            groupBoxInput.ResumeLayout(false);
            groupBoxInput.PerformLayout();
            tabRollStatistics.ResumeLayout(false);
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            splitContainerCharacters.Panel1.ResumeLayout(false);
            splitContainerCharacters.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerCharacters).EndInit();
            splitContainerCharacters.ResumeLayout(false);
            splitContainerStats.Panel1.ResumeLayout(false);
            splitContainerStats.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerStats).EndInit();
            splitContainerStats.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRollStats).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvRollReasons).EndInit();
            tabDetails.ResumeLayout(false);
            groupBoxSkippedRolls.ResumeLayout(false);
            groupBoxInputRead.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip;
        private ToolStripButton toolStripButtonGetInputFile;
        private ToolStripButton toolStripButtonReset;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel;
        private ToolStripProgressBar toolStripProgressBar;
        private TabControl tabControl;
        private TabPage tabInputSettings;
        private TabPage tabRollStatistics;
        private TabPage tabDetails;
        private GroupBox groupBoxInput;
        private CheckBox chbDebug;
        private TextBox txtInputFile;
        private Button btnGetInputFile;
        private SplitContainer splitContainerMain;
        private CheckedListBox clbCharacters;
        private SplitContainer splitContainerStats;
        private DataGridView dgvRollStats;
        private DataGridView dgvRollReasons;
        private GroupBox groupBoxSkippedRolls;
        private ListView lstSkippedRolls;
        private GroupBox groupBoxInputRead;
        private ListBox lstInputRead;
        private SplitContainer splitContainerCharacters;
        private Button btnMergeCharacters;
    }
}
