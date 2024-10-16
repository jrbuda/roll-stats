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
        private static readonly HashSet<string> PlayerNames = new HashSet<string> { "vanta", "thomas ramore", "ruby", "naakopraan", "keke", "jinx", "jimothy", "bimothy", "timothy" };
        private static readonly Regex rollLineRegex = new Regex(
            @"^(?<rollExpr>.+?)\s*=\s*(?<outcomes>.+?)\s*=\s*(?<total>\d+(\.\d+)?)$",
            RegexOptions.Compiled);

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnGetInputFile_Click(object sender, EventArgs e)
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
                    await Task.Run(() => getRollStats(openFileDialog.FileName));
                }
            }
        }

        private void getRollStats(string inputFilepath)
        {
            try
            {
                // Determine if debugging is enabled
                bool isDebug = chbDebug.Checked;

                // Only create debug lists if debugging is enabled
                Dictionary<string, Dictionary<string, List<double>>> playerRollOutcomes = new Dictionary<string, Dictionary<string, List<double>>>();
                List<string> skippedLines = isDebug ? new List<string>() : null;
                List<string> inputReadItems = isDebug ? new List<string>() : null;

                int totalLines = File.ReadLines(inputFilepath).Count();
                int linesProcessed = 0;
                int currentLineNumber = 0;
                int lastProgress = 0;

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
                            ProcessChunk(chunkLines, playerRollOutcomes, isDebug, skippedLines, inputReadItems, currentLineNumber - chunkLines.Count);
                            chunkLines.Clear();
                        }
                        else
                        {
                            chunkLines.Add(currentLine);
                        }

                        // Update progress bar more frequently to reflect ongoing progress
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
                        ProcessChunk(chunkLines, playerRollOutcomes, isDebug, skippedLines, inputReadItems, currentLineNumber - chunkLines.Count);
                    }
                }

                // Update UI lists only if debugging is enabled
                if (isDebug)
                {
                    Invoke(new Action(() =>
                    {
                        lstSkippedRolls.Items.AddRange(skippedLines.Select(line => new ListViewItem(line)).ToArray());
                        lstInputRead.Items.AddRange(inputReadItems.Select(item => new ListViewItem(item)).ToArray());
                    }));
                }

                UpdateRollStatsGrid(playerRollOutcomes);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void ProcessChunk(List<string> chunkLines, Dictionary<string, Dictionary<string, List<double>>> playerRollOutcomes, bool isDebug, List<string> skippedLines = null, List<string> inputReadItems = null, int startingLineNumber = 0)
        {
            if (chunkLines.Count == 0) return;

            string name = ExtractPlayerName(chunkLines[0]);

            if (string.IsNullOrEmpty(name) || !PlayerNames.Contains(name)) return;

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

        private string ExtractPlayerName(string line)
        {
            int index = line.LastIndexOf("]");
            if (index != -1 && index + 1 < line.Length)
            {
                return line.Substring(index + 1).Trim().ToLower();
            }
            return string.Empty;
        }

        private bool ParseRollLine(string line, Dictionary<string, List<double>> playerRolls, bool isDebug, int lineNumber, List<string> skippedLines = null, List<string> inputReadItems = null)
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

        private void UpdateRollStatsGrid(Dictionary<string, Dictionary<string, List<double>>> playerRollOutcomes)
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

        private double GetMedian(List<double> sortedNumbers)
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

        private double GetMode(List<double> numbers)
        {
            return numbers.GroupBy(n => n)
                          .OrderByDescending(g => g.Count())
                          .ThenBy(g => g.Key)
                          .Select(g => g.Key)
                          .FirstOrDefault();
        }

        private double GetMaxRollValue(string roll)
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
            return 0; // Return 0 if unable to parse
        }
    }
}
