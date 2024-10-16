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
            ((System.ComponentModel.ISupportInitialize)dgvRollStats).BeginInit();
            SuspendLayout();
            // 
            // ofdInputFile
            // 
            ofdInputFile.FileName = "openFileDialog1";
            // 
            // btnGetInputFile
            // 
            btnGetInputFile.Location = new Point(17, 20);
            btnGetInputFile.Margin = new Padding(4, 5, 4, 5);
            btnGetInputFile.Name = "btnGetInputFile";
            btnGetInputFile.Size = new Size(216, 38);
            btnGetInputFile.TabIndex = 0;
            btnGetInputFile.Text = "Get Input File";
            btnGetInputFile.UseVisualStyleBackColor = true;
            btnGetInputFile.Click += btnGetInputFile_Click;
            // 
            // txtInputFile
            // 
            txtInputFile.Enabled = false;
            txtInputFile.Location = new Point(241, 22);
            txtInputFile.Margin = new Padding(4, 5, 4, 5);
            txtInputFile.Name = "txtInputFile";
            txtInputFile.Size = new Size(280, 31);
            txtInputFile.TabIndex = 1;
            // 
            // dgvRollStats
            // 
            dgvRollStats.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRollStats.Location = new Point(17, 68);
            dgvRollStats.Margin = new Padding(4, 5, 4, 5);
            dgvRollStats.Name = "dgvRollStats";
            dgvRollStats.RowHeadersWidth = 62;
            dgvRollStats.Size = new Size(1593, 527);
            dgvRollStats.TabIndex = 2;
            // 
            // lstInputRead
            // 
            lstInputRead.FormattingEnabled = true;
            lstInputRead.ItemHeight = 25;
            lstInputRead.Location = new Point(17, 605);
            lstInputRead.Margin = new Padding(4, 5, 4, 5);
            lstInputRead.Name = "lstInputRead";
            lstInputRead.Size = new Size(543, 154);
            lstInputRead.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1623, 884);
            Controls.Add(lstInputRead);
            Controls.Add(dgvRollStats);
            Controls.Add(txtInputFile);
            Controls.Add(btnGetInputFile);
            Margin = new Padding(4, 5, 4, 5);
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
    }
}
