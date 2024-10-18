using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Roll_stats
{
    public partial class Form1 : Form
    {
        private const string ChunkDelimiter = "---------------------------";
        private static readonly Regex rollLineRegex = new Regex(
            @"^(?<rollExpr>.+?)\s*=\s*(?<outcomes>.+?)\s*=\s*(?<total>.+)$",
            RegexOptions.Compiled);

        // Class-level variable to store all stats
        private Dictionary<string, Dictionary<string, List<double>>> allPlayerRollOutcomes = new Dictionary<string, Dictionary<string, List<double>>>();

        public Form1()
        {
            InitializeComponent();
            // Subscribe to the ItemCheck event
            clbCharacters.ItemCheck += clbCharacters_ItemCheck;
            // Hide debug lists initially
            lstSkippedRolls.Visible = false;
            lstInputRead.Visible = false;

            // Set up global exception handlers
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        // Global exception handler for UI thread exceptions
        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogError(e.Exception);
            MessageBox.Show("An unexpected error occurred. Please check the error.log file for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Global exception handler for non-UI thread exceptions
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            LogError(ex);
            MessageBox.Show("A critical error occurred. Please check the error.log file for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Method to log errors to error.log file
        private void LogError(Exception ex)
        {
            try
            {
                string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
                string errorMessage = $"[{DateTime.Now}] {ex}\n";
                File.AppendAllText(logFilePath, errorMessage);
            }
            catch
            {
                // If logging fails, there's not much we can do
            }
        }

        private async void btnGetInputFile_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "e:\\";
                    openFileDialog.Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        txtInputFile.Text = openFileDialog.FileName;

                        // Clear previous data and disable controls during parsing
                        Invoke(new Action(() =>
                        {
                            clbCharacters.Items.Clear();
                            dgvRollStats.Rows.Clear();
                            lstSkippedRolls.Items.Clear();
                            lstInputRead.Items.Clear();
                            clbCharacters.Enabled = false;
                            pgbStatus.Value = 0;
                        }));

                        // Parse the file to extract character names and stats
                        await Task.Run(() => ParseFileAndExtractStats(openFileDialog.FileName));

                        // Enable controls after parsing
                        Invoke(new Action(() =>
                        {
                            clbCharacters.Enabled = true;
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("An error occurred while opening the file. Please check the error.log file for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ParseFileAndExtractStats(string inputFilepath)
        {
            try
            {
                bool isDebug = chbDebug.Checked;
                List<string> skippedLines = isDebug ? new List<string>() : null;
                List<string> inputReadItems = isDebug ? new List<string>() : null;

                int totalLines = File.ReadLines(inputFilepath).Count();
                int linesProcessed = 0;
                int currentLineNumber = 0;
                int lastProgress = 0;

                allPlayerRollOutcomes = new Dictionary<string, Dictionary<string, List<double>>>();

                using (var reader = new StreamReader(inputFilepath))
                {
                    List<string> chunkLines = new List<string>();
                    string currentLine;

                    while ((currentLine = reader.ReadLine()) != null)
                    {
                        currentLineNumber++;

                        currentLine = currentLine.Trim();
                        linesProcessed++;

                        if (currentLine == ChunkDelimiter)
                        {
                            ProcessChunk(chunkLines, allPlayerRollOutcomes, isDebug, skippedLines, inputReadItems, currentLineNumber - chunkLines.Count);
                            chunkLines.Clear();
                        }
                        else
                        {
                            chunkLines.Add(currentLine);
                        }

                        // Update progress bar
                        int progress = (int)((double)linesProcessed / totalLines * 100);
                        if (progress > lastProgress)
                        {
                            lastProgress = progress;
                            Invoke(new Action(() => pgbStatus.Value = progress));
                        }
                    }

                    // Process any remaining chunk
                    if (chunkLines.Count > 0)
                    {
                        ProcessChunk(chunkLines, allPlayerRollOutcomes, isDebug, skippedLines, inputReadItems, currentLineNumber - chunkLines.Count);
                    }
                }

                // Update the clbCharacters with the character names
                var characterNames = new HashSet<string>(allPlayerRollOutcomes.Keys);

                // Update UI elements on the main thread
                Invoke(new Action(() =>
                {
                    clbCharacters.Items.Clear();
                    clbCharacters.Items.AddRange(characterNames.OrderBy(n => n).ToArray());
                }));

                // Update debug lists if debugging is enabled
                if (isDebug)
                {
                    Invoke(new Action(() =>
                    {
                        lstSkippedRolls.Items.AddRange(skippedLines.Select(line => new ListViewItem(line)).ToArray());
                        lstInputRead.Items.AddRange(inputReadItems.Select(item => new ListViewItem(item)).ToArray());
                    }));
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("An error occurred while parsing the file. Please check the error.log file for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessChunk(List<string> chunkLines, Dictionary<string, Dictionary<string, List<double>>> playerRollOutcomes, bool isDebug, List<string> skippedLines = null, List<string> inputReadItems = null, int startingLineNumber = 0)
        {
            try
            {
                if (chunkLines.Count == 0) return;

                string name = ExtractPlayerName(chunkLines[0]);

                if (string.IsNullOrEmpty(name))
                {
                    return; // Skip processing if name is empty
                }

                if (!playerRollOutcomes.TryGetValue(name, out var playerRolls))
                {
                    playerRolls = new Dictionary<string, List<double>>();
                    playerRollOutcomes[name] = playerRolls;
                }

                for (int i = 0; i < chunkLines.Count; i++)
                {
                    if (!ParseRollLine(chunkLines[i], playerRolls, isDebug, startingLineNumber + i, skippedLines, inputReadItems))
                    {
                        if (isDebug && skippedLines != null)
                        {
                            skippedLines.Add($"{chunkLines[i]}\nSkipped at line {startingLineNumber + i}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                if (isDebug && skippedLines != null)
                {
                    skippedLines.Add($"Error processing chunk starting at line {startingLineNumber}: {ex.Message}");
                }
            }
        }

        private string ExtractPlayerName(string line)
        {
            try
            {
                int index = line.LastIndexOf("]");
                if (index != -1 && index + 1 < line.Length)
                {
                    return line.Substring(index + 1).Trim().ToLower();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return string.Empty;
        }

        private bool ParseRollLine(string line, Dictionary<string, List<double>> playerRolls, bool isDebug, int lineNumber, List<string> skippedLines = null, List<string> inputReadItems = null)
        {
            try
            {
                line = line.Trim();

                // Simplified regex to capture the roll expression and outcomes
                var pattern = @"^(?<rollExpr>.+?)\s*=\s*(?<outcomes>.+?)\s*=\s*(?<total>.+)$";
                var match = Regex.Match(line, pattern);
                if (!match.Success)
                {
                    if (isDebug && skippedLines != null)
                    {
                        skippedLines.Add($"{line}\nInvalid format at line {lineNumber}");
                    }
                    return false;
                }

                var rollExpr = match.Groups["rollExpr"].Value.Trim();       // e.g., "1d8"
                var outcomesStr = match.Groups["outcomes"].Value.Trim();    // e.g., "4"

                // Extract dice rolls from the roll expression
                var diceRollMatches = Regex.Matches(rollExpr, @"\b\d+d\d+(?:kh\d*|kl\d*)?\b");
                var diceRolls = diceRollMatches.Cast<Match>().Select(m => m.Value).ToList();

                // Extract numbers from the outcomes expression
                var outcomeMatches = Regex.Matches(outcomesStr, @"-?\d+(\.\d+)?");
                var outcomeValues = outcomeMatches.Cast<Match>().Select(m => m.Value).ToList();

                if (diceRolls.Count == 1 && outcomeValues.Count == 1)
                {
                    // Handle the case where there's a single roll and a single outcome
                    var diceRoll = diceRolls[0];
                    var outcomeStr = outcomeValues[0];

                    if (!double.TryParse(outcomeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double outcome))
                    {
                        if (isDebug && skippedLines != null)
                        {
                            skippedLines.Add($"Invalid outcome value at line {lineNumber}: {outcomeStr} in line: {line}");
                        }
                        return false;
                    }

                    if (!playerRolls.ContainsKey(diceRoll))
                    {
                        playerRolls[diceRoll] = new List<double>();
                    }
                    playerRolls[diceRoll].Add(outcome);

                    if (isDebug && inputReadItems != null)
                    {
                        inputReadItems.Add($"{diceRoll}: Outcome: {outcome}");
                    }

                    return true;
                }

                if (diceRolls.Count > outcomeValues.Count)
                {
                    if (isDebug && skippedLines != null)
                    {
                        skippedLines.Add($"Not enough outcomes for dice rolls at line {lineNumber}: {line}");
                    }
                    return false;
                }

                // Process each dice roll and corresponding outcome
                for (int i = 0; i < diceRolls.Count; i++)
                {
                    var diceRoll = diceRolls[i];
                    var outcomeStr = outcomeValues[i];

                    if (!double.TryParse(outcomeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double outcome))
                    {
                        if (isDebug && skippedLines != null)
                        {
                            skippedLines.Add($"Invalid outcome value at line {lineNumber}: {outcomeStr} in line: {line}");
                        }
                        return false;
                    }

                    if (!playerRolls.ContainsKey(diceRoll))
                    {
                        playerRolls[diceRoll] = new List<double>();
                    }
                    playerRolls[diceRoll].Add(outcome);

                    if (isDebug && inputReadItems != null)
                    {
                        inputReadItems.Add($"{diceRoll}: Outcome: {outcome}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                if (isDebug && skippedLines != null)
                {
                    skippedLines.Add($"Error parsing roll line at line {lineNumber}: {ex.Message}");
                }
                return false;
            }
        }

        private void clbCharacters_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Schedule the update after the event handler completes
            this.BeginInvoke((MethodInvoker)delegate {
                try
                {
                    UpdateRollStatsGridForSelectedCharacters();
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    MessageBox.Show("An error occurred while updating the stats. Please check the error.log file for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private void UpdateRollStatsGridForSelectedCharacters()
        {
            try
            {
                var selectedCharacters = clbCharacters.CheckedItems.Cast<string>().ToList();

                // Prepare data to display
                var displayData = new Dictionary<string, Dictionary<string, List<double>>>();

                foreach (var character in selectedCharacters)
                {
                    if (allPlayerRollOutcomes.TryGetValue(character, out var playerRolls))
                    {
                        displayData[character] = playerRolls;
                    }
                }

                // Now update the DataGridView with displayData
                UpdateRollStatsGrid(displayData);
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("An error occurred while preparing the data for display. Please check the error.log file for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateRollStatsGrid(Dictionary<string, Dictionary<string, List<double>>> playerRollOutcomes)
        {
            try
            {
                Invoke(new Action(() =>
                {
                    dgvRollStats.SuspendLayout();
                    dgvRollStats.Rows.Clear();
                    dgvRollStats.Columns.Clear();

                    dgvRollStats.Columns.Add("Player", "Player");
                    dgvRollStats.Columns.Add("Roll", "Roll");
                    dgvRollStats.Columns.Add("Total Rolls", "Total Rolls");
                    dgvRollStats.Columns.Add("Average", "Average");
                    dgvRollStats.Columns.Add("Median", "Median");
                    dgvRollStats.Columns.Add("Mode", "Mode");
                    dgvRollStats.Columns.Add("1s Rolled", "1s Rolled");
                    dgvRollStats.Columns.Add("Max Rolls", "Max Rolls");

                    foreach (var player in playerRollOutcomes.Keys)
                    {
                        foreach (var roll in playerRollOutcomes[player].Keys)
                        {
                            List<double> outcomes = playerRollOutcomes[player][roll];
                            List<double> sortedOutcomes = outcomes.OrderBy(o => o).ToList();

                            double average = outcomes.Average();
                            double median = GetMedian(sortedOutcomes);
                            double mode = GetMode(sortedOutcomes);
                            int countOnes = outcomes.Count(o => o == 1);
                            double maxRollValue = GetMaxRollValue(roll);
                            int countMax = outcomes.Count(o => o == maxRollValue);
                            int totalRolls = outcomes.Count;

                            dgvRollStats.Rows.Add(player, roll, totalRolls, average, median, mode, countOnes, countMax);
                        }
                    }

                    dgvRollStats.Sort(dgvRollStats.Columns["Player"], System.ComponentModel.ListSortDirection.Ascending);
                    dgvRollStats.ResumeLayout();
                }));
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("An error occurred while updating the grid. Please check the error.log file for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private double GetMedian(List<double> sortedNumbers)
        {
            try
            {
                int count = sortedNumbers.Count;
                if (count % 2 == 0)
                {
                    return (sortedNumbers[count / 2 - 1] + sortedNumbers[count / 2]) / 2.0;
                }
                else
                {
                    return sortedNumbers[count / 2];
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return 0;
            }
        }

        private double GetMode(List<double> numbers)
        {
            try
            {
                return numbers.GroupBy(n => n)
                              .OrderByDescending(g => g.Count())
                              .ThenBy(g => g.Key)
                              .Select(g => g.Key)
                              .FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogError(ex);
                return 0;
            }
        }

        private double GetMaxRollValue(string roll)
        {
            try
            {
                // Handling cases where roll may contain modifiers like "kh" or "kl"
                var match = Regex.Match(roll, @"\d+d(\d+)");
                if (match.Success)
                {
                    var rollValueStr = match.Groups[1].Value;
                    if (double.TryParse(rollValueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double rollValue))
                    {
                        return rollValue;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return 0; // Return 0 if unable to parse
        }

        private void chbDebug_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chbDebug.Checked)
                {
                    lstSkippedRolls.Visible = true;
                    lstInputRead.Visible = true;
                }
                else
                {
                    lstSkippedRolls.Visible = false;
                    lstInputRead.Visible = false;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }
    }
}
