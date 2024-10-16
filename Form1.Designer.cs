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
            ofdInputFile = new OpenFileDialog();
            btnGetInputFile = new Button();
            txtInputFile = new TextBox();
            dgvRollStats = new DataGridView();
            lstInputRead = new ListBox();
            lstSkippedRolls = new ListView();
            pgbStatus = new ProgressBar();
            chbDebug = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)dgvRollStats).BeginInit();
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
            dgvRollStats.Size = new Size(1115, 316);
            dgvRollStats.TabIndex = 2;
            // 
            // lstInputRead
            // 
            lstInputRead.FormattingEnabled = true;
            lstInputRead.ItemHeight = 15;
            lstInputRead.Location = new Point(12, 363);
            lstInputRead.Name = "lstInputRead";
            lstInputRead.Size = new Size(381, 94);
            lstInputRead.TabIndex = 3;
            // 
            // lstSkippedRolls
            // 
            lstSkippedRolls.Location = new Point(399, 363);
            lstSkippedRolls.Name = "lstSkippedRolls";
            lstSkippedRolls.Size = new Size(725, 242);
            lstSkippedRolls.TabIndex = 4;
            lstSkippedRolls.UseCompatibleStateImageBehavior = false;
            // 
            // pgbStatus
            // 
            pgbStatus.Location = new Point(12, 463);
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
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1136, 617);
            Controls.Add(chbDebug);
            Controls.Add(pgbStatus);
            Controls.Add(lstSkippedRolls);
            Controls.Add(lstInputRead);
            Controls.Add(dgvRollStats);
            Controls.Add(txtInputFile);
            Controls.Add(btnGetInputFile);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dgvRollStats).EndInit();
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
    }
}
