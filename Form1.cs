using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Roll_stats
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetInputFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                ofdInputFile.InitialDirectory = "c:\\";
                ofdInputFile.Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt";
                ofdInputFile.FilterIndex = 1;
                ofdInputFile.RestoreDirectory = true;

                if (ofdInputFile.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of specified file
                    txtInputFile.Text = ofdInputFile.FileName;
                    getRollStats(ofdInputFile.FileName);
                }
            }
        }

        private void getRollStats(string inputFilepath)
        {
            try
            {
                string fileContents = File.ReadAllText(inputFilepath);
                string[] playerNames = { "vanta", "thomas ramore", "ruby", "naakopraan", "keke", "jinx", "jimothy", "bimothy", "timothy" };
                // Explode the file contents into chunks
                string[] chatChunks = fileContents.Split(new string[] { "---------------------------" }, StringSplitOptions.RemoveEmptyEntries);

                // Dictionary to store rolls per player
                Dictionary<string, Dictionary<string, List<int>>> playerRollOutcomes = new Dictionary<string, Dictionary<string, List<int>>>();

                // Regex to identify rolls (e.g., 3d6, 1d20)
                Regex rollRegex = new Regex(@"\b\d+d\d+\b");

                foreach (string chunk in chatChunks)
                {
                    string[] lines = chunk.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    string name = "";
                    bool containsValidRoll = false;

                    // Check if the chunk contains a valid roll line
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Count(c => c == '=') > 1)
                        {
                            containsValidRoll = true;
                            break;
                        }
                    }

                    if (!containsValidRoll) continue;

                    if (lines.Length > 0)
                    {
                        name = lines[0].Substring(lines[0].LastIndexOf("]") + 1).Trim().ToLower();
                    }

                    if (!Array.Exists(playerNames, element => element.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    // Ensure the player has an entry in the dictionary
                    if (!playerRollOutcomes.ContainsKey(name))
                    {
                        playerRollOutcomes[name] = new Dictionary<string, List<int>>();
                    }

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Count(c => c == '=') > 1)
                        {
                            // Split the line on '=' to separate rolls from outcomes
                            string[] parts = lines[i].Split('=');

                            // Extract the rolls from the first part
                            string[] rolls = parts[0].Split(new[] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            // Extract the outcomes from the second part
                            string[] outcomes = parts[1].Split(new[] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            for (int j = 0; j < rolls.Length; j++)
                            {
                                string roll = rolls[j];
                                if (rollRegex.IsMatch(roll))
                                {
                                    int outcome = int.Parse(outcomes[j]);

                                    // Store the roll and its outcome in the player's dictionary
                                    if (!playerRollOutcomes[name].ContainsKey(roll))
                                    {
                                        playerRollOutcomes[name][roll] = new List<int>();
                                    }
                                    playerRollOutcomes[name][roll].Add(outcome);

                                    // Add "1d20" rolls to lstInputItems
                                    if (roll == "1d20")
                                    {
                                        lstInputRead.Items.Add($"{name}: Rolled a 1d20, Outcome: {outcome}");
                                    }
                                }
                            }
                        }
                    }
                }

                // Display the results (for example, in a DataGridView)
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
                        List<int> outcomes = playerRollOutcomes[player][roll];
                        double average = outcomes.Average();
                        double median = GetMedian(outcomes);
                        int mode = GetMode(outcomes);
                        int countOnes = outcomes.Count(o => o == 1);
                        int maxRollValue = GetMaxRollValue(roll);
                        int countMax = outcomes.Count(o => o == maxRollValue);
                        int totalRolls = outcomes.Count;

                        dgvRollStats.Rows.Add(player, roll, totalRolls, average, median, mode, countOnes, countMax);
                    }
                }

                // Sort the rows in alphabetical order based on the "Player" column
                dgvRollStats.Sort(dgvRollStats.Columns["Player"], System.ComponentModel.ListSortDirection.Ascending);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while reading the file: " + ex.Message);
            }
        }

        private double GetMedian(List<int> numbers)
        {
            var sortedNumbers = numbers.OrderBy(n => n).ToList();
            int count = sortedNumbers.Count;
            if (count % 2 == 0)
            {
                // Even number of elements
                return (sortedNumbers[count / 2 - 1] + sortedNumbers[count / 2]) / 2.0;
            }
            else
            {
                // Odd number of elements
                return sortedNumbers[count / 2];
            }
        }

        private int GetMode(List<int> numbers)
        {
            return numbers.GroupBy(n => n)
                          .OrderByDescending(g => g.Count())
                          .ThenBy(g => g.Key)
                          .Select(g => g.Key)
                          .FirstOrDefault();
        }

        private int GetMaxRollValue(string roll)
        {
            // Extract the maximum value from the roll (e.g., for "1d20", return 20)
            return int.Parse(roll.Split('d')[1]);
        }
    }
}
