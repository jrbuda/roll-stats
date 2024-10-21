using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics; // For Process class
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Roll_stats
{
    public partial class Form1 : Form
    {
        private Dictionary<string, PlayerData> allPlayerRollOutcomes = new Dictionary<string, PlayerData>();

        public Form1()
        {
            InitializeComponent();
            clbCharacters.ItemCheck += clbCharacters_ItemCheck;
            dgvRollStats.SelectionChanged += dgvRollStats_SelectionChanged;
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogError(e.Exception);
            MessageBox.Show("An unexpected error occurred. Please check the error.log file for details.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            LogError(ex);
            MessageBox.Show("A critical error occurred. Please check the error.log file for details.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

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
                // Ignored
            }
        }

        private async void btnGetInputFile_Click(object sender, EventArgs e)
        {
            try
            {
                // Ask the user to choose between LevelDB directory or output.txt file
                DialogResult result = MessageBox.Show(
                    "Do you want to select a LevelDB directory?\n\nClick Yes to select a LevelDB directory.\nClick No to select an existing output.txt file.",
                    "Select Input Type", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // User wants to select LevelDB directory
                    using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                    {
                        folderBrowserDialog.Description = "Select the FoundryVTT 'messages' LevelDB directory";
                        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                        {
                            string dbPath = folderBrowserDialog.SelectedPath;
                            txtInputFile.Text = dbPath;

                            Invoke(new Action(() =>
                            {
                                clbCharacters.Items.Clear();
                                dgvRollStats.Rows.Clear();
                                dgvRollReasons.Rows.Clear();
                                lstSkippedRolls.Items.Clear();
                                lstInputRead.Items.Clear();
                                clbCharacters.Enabled = false;
                                pgbStatus.Value = 0;
                            }));

                            await Task.Run(() => RunNodeScriptAndParseData(dbPath));

                            Invoke(new Action(() =>
                            {
                                clbCharacters.Enabled = true;
                                pgbStatus.Value = 100; // Ensure progress bar reaches 100%
                            }));
                        }
                    }
                }
                else if (result == DialogResult.No)
                {
                    // User wants to select output.txt file
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.InitialDirectory = "e:\\";
                        openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                        openFileDialog.FilterIndex = 1;
                        openFileDialog.RestoreDirectory = true;

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string outputFilePath = openFileDialog.FileName;
                            txtInputFile.Text = outputFilePath;

                            Invoke(new Action(() =>
                            {
                                clbCharacters.Items.Clear();
                                dgvRollStats.Rows.Clear();
                                dgvRollReasons.Rows.Clear();
                                lstSkippedRolls.Items.Clear();
                                lstInputRead.Items.Clear();
                                clbCharacters.Enabled = false;
                                pgbStatus.Value = 0;
                            }));

                            await Task.Run(() => ParseFileAndExtractStats(outputFilePath));

                            Invoke(new Action(() =>
                            {
                                clbCharacters.Enabled = true;
                                pgbStatus.Value = 100; // Ensure progress bar reaches 100%
                            }));
                        }
                    }
                }
                else
                {
                    // User canceled the operation
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("An error occurred while selecting the input. Please check the error.log file for details.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RunNodeScriptAndParseData(string dbPath)
        {
            try
            {
                // Path to your Node.js script in the same directory as the executable
                string nodeScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ldb-reader.js");

                // Check if the script exists
                if (!File.Exists(nodeScriptPath))
                {
                    throw new FileNotFoundException("Node.js script file not found in the application directory.", nodeScriptPath);
                }

                // Define the output file path in the same directory
                string outputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.txt");

                // Build the arguments to pass to the script
                string args = $"\"{nodeScriptPath}\" \"{dbPath}\" \"{outputFilePath}\"";

                // Start the Node.js process
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "node",
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    // Event handlers for output and error data
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            // Optionally handle standard output
                        }
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            LogError(new Exception(e.Data));
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception("Node.js script exited with a non-zero exit code.");
                    }
                }

                // After the Node.js script has finished, parse the output.txt file
                ParseFileAndExtractStats(outputFilePath);
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("An error occurred while running the Node.js script. Please check the error.log file for details.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ParseFileAndExtractStats(string inputFilepath)
        {
            try
            {
                bool isDebug = chbDebug.Checked;
                List<string> skippedLines = isDebug ? new List<string>() : null;
                List<string> inputReadItems = isDebug ? new List<string>() : null;

                allPlayerRollOutcomes = new Dictionary<string, PlayerData>();

                // Read all lines into a list
                List<string> lines = File.ReadAllLines(inputFilepath).ToList();
                long totalLines = lines.Count;
                long currentLine = 0;

                while (currentLine < totalLines)
                {
                    string line = lines[(int)currentLine].Trim();

                    if (line.StartsWith("Key:"))
                    {
                        currentLine++; // Move to "Value:" line
                        if (currentLine >= totalLines)
                        {
                            if (isDebug && skippedLines != null)
                            {
                                skippedLines.Add($"Unexpected end of file after 'Key:' at line {currentLine}");
                            }
                            break;
                        }

                        line = lines[(int)currentLine].Trim();
                        if (!line.StartsWith("Value:"))
                        {
                            if (isDebug && skippedLines != null)
                            {
                                skippedLines.Add($"Expected 'Value:' after 'Key:' at line {currentLine}");
                            }
                            continue;
                        }

                        currentLine++; // Move to JSON lines
                        StringBuilder jsonBuilder = new StringBuilder();
                        int braceCount = 0;
                        bool jsonStarted = false;

                        while (currentLine < totalLines)
                        {
                            line = lines[(int)currentLine];
                            jsonBuilder.AppendLine(line);

                            // Count braces to detect end of JSON object
                            braceCount += line.Count(c => c == '{');
                            braceCount -= line.Count(c => c == '}');

                            if (line.Contains("{"))
                            {
                                jsonStarted = true;
                            }

                            currentLine++;

                            if (jsonStarted && braceCount == 0)
                            {
                                // JSON object is complete
                                break;
                            }
                        }

                        string json = jsonBuilder.ToString();
                        if (!string.IsNullOrWhiteSpace(json) && IsValidJson(json))
                        {
                            // Process the JSON
                            ProcessJson(json, isDebug, skippedLines, inputReadItems);
                        }
                        else if (isDebug && skippedLines != null)
                        {
                            skippedLines.Add($"Invalid JSON format starting at line {currentLine}");
                        }
                    }
                    else
                    {
                        if (isDebug && skippedLines != null)
                        {
                            skippedLines.Add($"Skipped non-JSON line: {line}");
                        }
                        currentLine++;
                    }

                    // Update progress bar
                    int progress = (int)(((double)currentLine / totalLines) * 100);
                    Invoke(new Action(() =>
                    {
                        pgbStatus.Value = progress;
                    }));
                }

                var characterNames = new HashSet<string>(allPlayerRollOutcomes.Keys);

                Invoke(new Action(() =>
                {
                    clbCharacters.Items.Clear();
                    clbCharacters.Items.AddRange(characterNames.OrderBy(n => n).ToArray());
                }));

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
                MessageBox.Show("An error occurred while parsing the file. Please check the error.log file for details.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessJson(string json, bool isDebug, List<string> skippedLines, List<string> inputReadItems)
        {
            try
            {
                JObject parsedJson = JObject.Parse(json);

                // Extract player name
                string playerName = null;
                if (parsedJson.TryGetValue("speaker", out JToken speakerToken))
                {
                    if (speakerToken.Type == JTokenType.Object)
                    {
                        playerName = speakerToken["alias"]?.ToString()?.ToLower();
                    }
                    else
                    {
                        // Skip entries where 'speaker' is not an object
                        return;
                    }
                }
                else
                {
                    // Skip entries without 'speaker'
                    return;
                }

                if (string.IsNullOrEmpty(playerName))
                {
                    // Skip entries with missing alias
                    return;
                }

                if (!allPlayerRollOutcomes.TryGetValue(playerName, out PlayerData playerData))
                {
                    playerData = new PlayerData { PlayerName = playerName };
                    allPlayerRollOutcomes[playerName] = playerData;
                }

                JArray rolls = parsedJson["rolls"] as JArray;
                if (rolls != null)
                {
                    foreach (var roll in rolls)
                    {
                        string flavor = roll["options"]?["flavor"]?.ToString() ?? "Unknown";

                        JArray terms = roll["terms"] as JArray;
                        if (terms == null) continue;

                        foreach (var term in terms)
                        {
                            if (term["class"]?.ToString() == "Die")
                            {
                                int number = term["number"]?.ToObject<int>() ?? 1;
                                int faces = term["faces"]?.ToObject<int>() ?? 0;
                                JArray modifiersArray = term["modifiers"] as JArray;
                                string modifiers = modifiersArray != null && modifiersArray.Count > 0
                                    ? string.Join("", modifiersArray.Select(m => m.ToString()))
                                    : "";

                                string diceRollKey = $"{number}d{faces}{modifiers}";

                                if (!playerData.RollDataByKey.TryGetValue(diceRollKey, out RollData rollData))
                                {
                                    rollData = new RollData { DiceRollKey = diceRollKey };
                                    playerData.RollDataByKey[diceRollKey] = rollData;
                                }

                                JArray results = term["results"] as JArray;
                                if (results == null) continue;

                                foreach (var result in results)
                                {
                                    if (result["active"]?.ToObject<bool>() == true)
                                    {
                                        double rollResult = result["result"]?.ToObject<double>() ?? 0;
                                        rollData.Results.Add(new RollResult { Flavor = flavor, Value = rollResult });

                                        if (isDebug && inputReadItems != null)
                                        {
                                            inputReadItems.Add($"{playerName}: {diceRollKey} ({flavor}) rolled {rollResult}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                if (isDebug && skippedLines != null)
                {
                    skippedLines.Add($"Error parsing JSON: {ex.Message}");
                }
            }
        }

        private bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || // For object
                (strInput.StartsWith("[") && strInput.EndsWith("]")))   // For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        private void clbCharacters_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                try
                {
                    UpdateRollStatsGridForSelectedCharacters();
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    MessageBox.Show("An error occurred while updating the stats. Please check the error.log file for details.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private void UpdateRollStatsGridForSelectedCharacters()
        {
            try
            {
                var selectedCharacters = clbCharacters.CheckedItems.Cast<string>().ToList();
                var displayData = new Dictionary<string, PlayerData>();

                foreach (var character in selectedCharacters)
                {
                    if (allPlayerRollOutcomes.TryGetValue(character, out PlayerData playerData))
                    {
                        displayData[character] = playerData;
                    }
                }

                UpdateRollStatsGrid(displayData);
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("An error occurred while preparing the data for display. Please check the error.log file for details.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateRollStatsGrid(Dictionary<string, PlayerData> playerRollOutcomes)
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

                    foreach (var player in playerRollOutcomes.Values)
                    {
                        foreach (var rollData in player.RollDataByKey.Values)
                        {
                            List<double> outcomes = rollData.Results.Select(r => r.Value).ToList();
                            List<double> sortedOutcomes = outcomes.OrderBy(o => o).ToList();

                            double average = outcomes.Average();
                            double median = GetMedian(sortedOutcomes);
                            double mode = GetMode(sortedOutcomes);
                            int countOnes = outcomes.Count(o => o == 1);
                            double maxRollValue = GetMaxRollValue(rollData.DiceRollKey);
                            int countMax = outcomes.Count(o => o == maxRollValue);
                            int totalRolls = outcomes.Count;

                            dgvRollStats.Rows.Add(player.PlayerName, rollData.DiceRollKey, totalRolls, average, median, mode, countOnes, countMax);
                        }
                    }

                    dgvRollStats.Sort(dgvRollStats.Columns["Player"], System.ComponentModel.ListSortDirection.Ascending);
                    dgvRollStats.ResumeLayout();
                }));
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("An error occurred while updating the grid. Please check the error.log file for details.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvRollStats_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRollStats.SelectedRows.Count > 0)
            {
                var selectedRow = dgvRollStats.SelectedRows[0];
                string playerName = selectedRow.Cells["Player"].Value.ToString();
                string diceRollKey = selectedRow.Cells["Roll"].Value.ToString();

                UpdateRollReasonsGrid(playerName, diceRollKey);
            }
            else
            {
                // Clear the reasons grid if no row is selected
                dgvRollReasons.Rows.Clear();
                dgvRollReasons.Columns.Clear();
            }
        }

        private void UpdateRollReasonsGrid(string playerName, string diceRollKey)
        {
            try
            {
                Invoke(new Action(() =>
                {
                    dgvRollReasons.SuspendLayout();
                    dgvRollReasons.Rows.Clear();
                    dgvRollReasons.Columns.Clear();

                    dgvRollReasons.Columns.Add("Flavor", "Flavor");
                    dgvRollReasons.Columns.Add("Total Rolls", "Total Rolls");
                    dgvRollReasons.Columns.Add("Average", "Average");
                    dgvRollReasons.Columns.Add("Median", "Median");
                    dgvRollReasons.Columns.Add("Mode", "Mode");
                    dgvRollReasons.Columns.Add("1s Rolled", "1s Rolled");
                    dgvRollReasons.Columns.Add("Max Rolls", "Max Rolls");

                    if (allPlayerRollOutcomes.TryGetValue(playerName, out PlayerData playerData))
                    {
                        if (playerData.RollDataByKey.TryGetValue(diceRollKey, out RollData rollData))
                        {
                            var flavorGroups = rollData.Results.GroupBy(r => r.Flavor);

                            foreach (var flavorGroup in flavorGroups)
                            {
                                List<double> outcomes = flavorGroup.Select(r => r.Value).ToList();
                                List<double> sortedOutcomes = outcomes.OrderBy(o => o).ToList();

                                double average = outcomes.Average();
                                double median = GetMedian(sortedOutcomes);
                                double mode = GetMode(sortedOutcomes);
                                int countOnes = outcomes.Count(o => o == 1);
                                double maxRollValue = GetMaxRollValue(diceRollKey);
                                int countMax = outcomes.Count(o => o == maxRollValue);
                                int totalRolls = outcomes.Count;

                                dgvRollReasons.Rows.Add(flavorGroup.Key, totalRolls, average, median, mode, countOnes, countMax);
                            }
                        }
                    }

                    dgvRollReasons.ResumeLayout();
                }));
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("An error occurred while updating the reasons grid. Please check the error.log file for details.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // Match the roll pattern including number of dice and modifiers
                var match = Regex.Match(roll, @"^(?<num>\d+)d(?<faces>\d+)(?<modifiers>.*)$");
                if (match.Success)
                {
                    int numDice = int.Parse(match.Groups["num"].Value);
                    int faces = int.Parse(match.Groups["faces"].Value);
                    string modifiers = match.Groups["modifiers"].Value;

                    // Calculate maximum possible value considering modifiers
                    double maxRollValue = numDice * faces;

                    // Handle "kh" and "kl" modifiers
                    if (modifiers.Contains("kh") || modifiers.Contains("kl"))
                    {
                        // For simplicity, assume that "kh" or "kl" will keep one die
                        maxRollValue = faces;
                    }

                    return maxRollValue;
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
                lstSkippedRolls.Visible = chbDebug.Checked;
                lstInputRead.Visible = chbDebug.Checked;
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }
    }
}
