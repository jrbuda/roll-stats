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
            ofdInputFile = new OpenFileDialog();
            btnGetInputFile = new Button();
            txtInputFile = new TextBox();
            dgvRollStats = new DataGridView();
            lstInputRead = new ListBox();
            lstSkippedRolls = new ListView();
            pgbStatus = new ProgressBar();
            chbDebug = new CheckBox();
            lblProgress = new Label();
            clbCharacters = new CheckedListBox();
            lblCharacters = new Label();
            dgvRollReasons = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvRollStats).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvRollReasons).BeginInit();
            SuspendLayout();
            // 
            // ofdInputFile
            // 
            ofdInputFile.FileName = "openFileDialog1";
            // 
            // btnGetInputFile
            // 
            btnGetInputFile.Location = new Point(12, 12);
            btnGetInputFile.Name = "btnGetInputFile";
            btnGetInputFile.Size = new Size(151, 23);
            btnGetInputFile.TabIndex = 0;
            btnGetInputFile.Text = "Get Input File";
            btnGetInputFile.UseVisualStyleBackColor = true;
            btnGetInputFile.Click += btnGetInputFile_Click;
            // 
            // txtInputFile
            // 
            txtInputFile.Enabled = false;
            txtInputFile.Location = new Point(169, 13);
            txtInputFile.Name = "txtInputFile";
            txtInputFile.Size = new Size(197, 23);
            txtInputFile.TabIndex = 1;
            // 
            // dgvRollStats
            // 
            dgvRollStats.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRollStats.Location = new Point(12, 41);
            dgvRollStats.Name = "dgvRollStats";
            dgvRollStats.RowHeadersWidth = 62;
            dgvRollStats.Size = new Size(879, 316);
            dgvRollStats.TabIndex = 2;
            // 
            // lstInputRead
            // 
            lstInputRead.FormattingEnabled = true;
            lstInputRead.ItemHeight = 15;
            lstInputRead.Location = new Point(897, 289);
            lstInputRead.Name = "lstInputRead";
            lstInputRead.Size = new Size(230, 64);
            lstInputRead.TabIndex = 3;
            lstInputRead.Visible = false;
            // 
            // lstSkippedRolls
            // 
            lstSkippedRolls.Location = new Point(897, 41);
            lstSkippedRolls.Name = "lstSkippedRolls";
            lstSkippedRolls.Size = new Size(230, 242);
            lstSkippedRolls.TabIndex = 4;
            lstSkippedRolls.UseCompatibleStateImageBehavior = false;
            lstSkippedRolls.Visible = false;
            // 
            // pgbStatus
            // 
            pgbStatus.Location = new Point(120, 363);
            pgbStatus.Name = "pgbStatus";
            pgbStatus.Size = new Size(381, 40);
            pgbStatus.TabIndex = 5;
            // 
            // chbDebug
            // 
            chbDebug.AutoSize = true;
            chbDebug.Location = new Point(372, 15);
            chbDebug.Name = "chbDebug";
            chbDebug.Size = new Size(61, 19);
            chbDebug.TabIndex = 6;
            chbDebug.Text = "Debug";
            chbDebug.UseVisualStyleBackColor = true;
            chbDebug.CheckedChanged += chbDebug_CheckedChanged;
            // 
            // lblProgress
            // 
            lblProgress.AutoSize = true;
            lblProgress.Location = new Point(12, 375);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(105, 15);
            lblProgress.TabIndex = 7;
            lblProgress.Text = "File Read Progress:";
            // 
            // clbCharacters
            // 
            clbCharacters.BackColor = SystemColors.ScrollBar;
            clbCharacters.FormattingEnabled = true;
            clbCharacters.Location = new Point(1133, 41);
            clbCharacters.Name = "clbCharacters";
            clbCharacters.Size = new Size(218, 562);
            clbCharacters.TabIndex = 10;
            clbCharacters.ItemCheck += clbCharacters_ItemCheck;
            // 
            // lblCharacters
            // 
            lblCharacters.AutoSize = true;
            lblCharacters.Location = new Point(1170, 23);
            lblCharacters.Name = "lblCharacters";
            lblCharacters.Size = new Size(143, 15);
            lblCharacters.TabIndex = 11;
            lblCharacters.Text = "Select from the list below!";
            // 
            // dgvRollReasons
            // 
            dgvRollReasons.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRollReasons.Location = new Point(12, 409);
            dgvRollReasons.Name = "dgvRollReasons";
            dgvRollReasons.Size = new Size(1115, 194);
            dgvRollReasons.TabIndex = 12;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1356, 617);
            Controls.Add(dgvRollReasons);
            Controls.Add(lblCharacters);
            Controls.Add(clbCharacters);
            Controls.Add(lblProgress);
            Controls.Add(chbDebug);
            Controls.Add(pgbStatus);
            Controls.Add(lstSkippedRolls);
            Controls.Add(lstInputRead);
            Controls.Add(dgvRollStats);
            Controls.Add(txtInputFile);
            Controls.Add(btnGetInputFile);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Roll Stats!";
            ((System.ComponentModel.ISupportInitialize)dgvRollStats).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvRollReasons).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog ofdInputFile;
        private Button btnGetInputFile;
        private TextBox txtInputFile;
        private DataGridView dgvRollStats;
        private ListBox lstInputRead;
        private ListView lstSkippedRolls;
        private ProgressBar pgbStatus;
        private CheckBox chbDebug;
        private Label lblProgress;
        private CheckedListBox clbCharacters;
        private Label lblCharacters;
        private DataGridView dgvRollReasons;
    }
}
