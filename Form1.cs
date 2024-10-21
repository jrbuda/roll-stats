using System;
using System.Collections.Generic;
using System.Diagnostics; // For Process class
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks; // For Task.Run
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.VisualBasic; // For InputBox

namespace Roll_stats
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// A dictionary storing all player roll outcomes, keyed by player name.
        /// </summary>
        private Dictionary<string, PlayerData> allPlayerRollOutcomes = new Dictionary<string, PlayerData>();

        public Form1()
        {
            InitializeComponent();

            // Assign event handlers to controls
            chbDebug.CheckedChanged += chbDebug_CheckedChanged;
            toolStripButtonGetInputFile.Click += btnGetInputFile_Click;
            toolStripButtonReset.Click += toolStripButtonReset_Click;

            // Set DataGridView properties
            dgvRollStats.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

            // Handle unhandled exceptions
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Initialize status bar
            toolStripStatusLabel.Text = "Ready";
            toolStripProgressBar.Value = 0;
        }

        #region Error Handling

        /// <summary>
        /// Handles unhandled thread exceptions.
        /// </summary>
        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogError(e.Exception);
            MessageBox.Show("An unexpected error occurred. Please check the error.log file for details.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Handles unhandled domain exceptions.
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            LogError(ex);
            MessageBox.Show("A critical error occurred. Please check the error.log file for details.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Logs exceptions to a file.
        /// </summary>
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

        #endregion

        #region Input File Processing

        /// <summary>
        /// Handles the click event for the "Get Input File" button.
        /// </summary>
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

                            // Reset UI and disable controls
                            Invoke(new Action(() =>
                            {
                                ResetUI();
                                clbCharacters.Enabled = false;
                                toolStripStatusLabel.Text = "Processing...";
                            }));

                            // Run processing in a background task
                            await Task.Run(() => RunNodeScriptAndParseData(dbPath));

                            // Re-enable controls and update UI
                            Invoke(new Action(() =>
                            {
                                clbCharacters.Enabled = true;
                                toolStripProgressBar.Value = 100; // Ensure progress bar reaches 100%
                                toolStripStatusLabel.Text = "Completed";

                                // Redirect to Roll Statistics tab
                                tabControl.SelectedTab = tabRollStatistics;
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

                            // Reset UI and disable controls
                            Invoke(new Action(() =>
                            {
                                ResetUI();
                                clbCharacters.Enabled = false;
                                toolStripStatusLabel.Text = "Processing...";
                            }));

                            // Run processing in a background task
                            await Task.Run(() => ParseFileAndExtractStats(outputFilePath));

                            // Re-enable controls and update UI
                            Invoke(new Action(() =>
                            {
                                clbCharacters.Enabled = true;
                                toolStripProgressBar.Value = 100; // Ensure progress bar reaches 100%
                                toolStripStatusLabel.Text = "Completed";

                                // Redirect to Roll Statistics tab
                                tabControl.SelectedTab = tabRollStatistics;
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

        /// <summary>
        /// Resets the UI elements to their default state.
        /// </summary>
        private void ResetUI()
        {
            clbCharacters.Items.Clear();
            dgvRollStats.Rows.Clear();
            dgvRollReasons.Rows.Clear();
            lstSkippedRolls.Items.Clear();
            lstInputRead.Items.Clear();
            toolStripProgressBar.Value = 0;
            toolStripStatusLabel.Text = "Ready";
            txtInputFile.Text = string.Empty;
        }

        /// <summary>
        /// Handles the click event for the "Reset" button.
        /// </summary>
        private void toolStripButtonReset_Click(object sender, EventArgs e)
        {
            // Clear all controls and reset the form
            ResetUI();
        }

        /// <summary>
        /// Runs the Node.js script to read LevelDB data and parses the output.
        /// </summary>
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

        /// <summary>
        /// Parses the output file and extracts roll statistics.
        /// </summary>
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

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        currentLine++;
                        continue; // Skip empty lines
                    }

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

                            if (string.IsNullOrWhiteSpace(line))
                            {
                                currentLine++;
                                continue; // Skip empty lines
                            }

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
                        toolStripProgressBar.Value = progress;
                        toolStripStatusLabel.Text = $"Processing... {progress}%";
                    }));
                }

                var characterNames = new HashSet<string>(allPlayerRollOutcomes.Keys);

                // Update the character list on the UI
                Invoke(new Action(() =>
                {
                    clbCharacters.Items.Clear();
                    clbCharacters.Items.AddRange(characterNames.OrderBy(n => n).ToArray());
                }));

                if (isDebug)
                {
                    Invoke(new Action(() =>
                    {
                        lstSkippedRolls.Items.Clear();
                        lstInputRead.Items.Clear();

                        lstSkippedRolls.Items.AddRange(skippedLines.Select(line => new ListViewItem(line)).ToArray());
                        lstInputRead.Items.AddRange(inputReadItems.ToArray());
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

        /// <summary>
        /// Processes a JSON string to extract roll data.
        /// </summary>
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

                        double dieRollSum = 0;
                        double modifierSum = 0;

                        foreach (var term in terms)
                        {
                            string termClass = term["class"]?.ToString();

                            if (termClass == "Die")
                            {
                                JArray results = term["results"] as JArray;
                                if (results == null) continue;

                                foreach (var result in results)
                                {
                                    if (result["active"]?.ToObject<bool>() == true)
                                    {
                                        double rollResult = result["result"]?.ToObject<double>() ?? 0;
                                        dieRollSum += rollResult;
                                    }
                                }
                            }
                            else if (termClass == "NumericTerm" && term["evaluated"]?.ToObject<bool>() == true)
                            {
                                double modifierValue = term["number"]?.ToObject<double>() ?? 0;
                                modifierSum += modifierValue;
                            }
                            // Handle other term types if necessary
                        }

                        double totalValue = roll["total"]?.ToObject<double>() ?? dieRollSum + modifierSum;

                        // Extract the full formula
                        string fullFormula = roll["formula"]?.ToString() ?? "Unknown";

                        // Extract the base roll
                        string diceRollKey = ExtractBaseRoll(fullFormula);

                        if (string.IsNullOrWhiteSpace(diceRollKey))
                        {
                            // Log and skip if diceRollKey is empty
                            LogError(new Exception($"ProcessJson: diceRollKey is null or empty for formula '{fullFormula}'."));
                            continue;
                        }

                        // Use full formula as the roll sub-key (includes modifiers)
                        string rollSubKey = fullFormula;

                        if (!playerData.RollDataByKey.TryGetValue(diceRollKey, out RollData rollData))
                        {
                            rollData = new RollData { DiceRollKey = diceRollKey };
                            playerData.RollDataByKey[diceRollKey] = rollData;
                        }

                        // Create a new RollResult with the extracted values
                        var rollResultObj = new RollResult
                        {
                            Flavor = flavor,
                            DieRollValue = dieRollSum,
                            ModifierValue = modifierSum,
                            TotalValue = totalValue,
                            FullFormula = fullFormula
                        };

                        rollData.Results.Add(rollResultObj);

                        if (isDebug && inputReadItems != null)
                        {
                            inputReadItems.Add($"{playerName}: {fullFormula} ({flavor}) rolled {dieRollSum} + {modifierSum} = {totalValue}");
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

        /// <summary>
        /// Extracts the base roll (dice expression) from a formula by removing modifiers.
        /// </summary>
        private string ExtractBaseRoll(string formula)
        {
            if (string.IsNullOrWhiteSpace(formula))
            {
                // Return null if formula is null or empty
                return null;
            }

            // Remove any whitespaces
            formula = formula.Replace(" ", "");

            // Regular expression to match dice expressions
            var dicePattern = @"\b(?:max\(0,)?(?<num>\d*)d(?<faces>\d+)(?<keep>k(?:h|l|\d+))?\)?\b";
            var matches = Regex.Matches(formula, dicePattern);

            if (matches.Count == 0)
            {
                // Return null if no dice expressions are found
                return null;
            }

            // Combine all matched dice expressions to form the base roll key
            var baseRoll = string.Join(" + ", matches.Cast<Match>().Select(m => m.Value));
            return baseRoll;
        }


        /// <summary>
        /// Checks if a string is valid JSON.
        /// </summary>
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

        #endregion

        #region Character Selection and Merging

        /// <summary>
        /// Handles the ItemCheck event for the character checklist, updating the roll stats grid.
        /// </summary>
        private void clbCharacters_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Use BeginInvoke to ensure the ItemCheck event completes before updating
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

        /// <summary>
        /// Updates the roll stats grid based on the selected characters.
        /// </summary>
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

        /// <summary>
        /// Handles the click event for the "Merge Characters" button.
        /// Allows users to merge roll statistics of multiple characters.
        /// </summary>
        private void btnMergeCharacters_Click(object sender, EventArgs e)
        {
            try
            {
                // Get selected characters
                var selectedCharacters = clbCharacters.CheckedItems.Cast<string>().ToList();

                if (selectedCharacters.Count < 2)
                {
                    MessageBox.Show("Please select at least two characters to merge.", "Merge Characters", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Prompt user for a name for the merged character
                string mergedCharacterName = PromptForMergedCharacterName(selectedCharacters);

                if (string.IsNullOrEmpty(mergedCharacterName))
                {
                    // User canceled or didn't provide a name
                    return;
                }

                // Check if a character with this name already exists
                if (allPlayerRollOutcomes.ContainsKey(mergedCharacterName))
                {
                    MessageBox.Show("A character with this name already exists. Please choose a different name.", "Merge Characters", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Merge the player data
                PlayerData mergedPlayerData = MergePlayerData(selectedCharacters, mergedCharacterName);

                // Add the merged player data to the dictionary
                allPlayerRollOutcomes[mergedCharacterName] = mergedPlayerData;

                // Add the merged character to the clbCharacters list
                clbCharacters.Items.Add(mergedCharacterName, true);

                // Optionally, uncheck the original characters
                foreach (string character in selectedCharacters)
                {
                    int index = clbCharacters.Items.IndexOf(character);
                    if (index >= 0)
                    {
                        clbCharacters.SetItemChecked(index, false);
                    }
                }

                // Update the roll stats grid
                UpdateRollStatsGridForSelectedCharacters();
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("An error occurred while merging characters. Please check the error.log file for details.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Prompts the user for a name for the merged character.
        /// </summary>
        private string PromptForMergedCharacterName(List<string> selectedCharacters)
        {
            // Default name for the merged character
            string defaultName = $"{string.Join(", ", selectedCharacters)} (Merged)";
            string promptMessage = "Enter a name for the merged character:";
            string title = "Merge Characters";

            // Use InputBox to get the name
            string input = Microsoft.VisualBasic.Interaction.InputBox(promptMessage, title, defaultName);

            return input.Trim();
        }

        /// <summary>
        /// Merges the roll data of selected characters into a new PlayerData object.
        /// </summary>
        private PlayerData MergePlayerData(List<string> selectedCharacters, string mergedCharacterName)
        {
            PlayerData mergedPlayerData = new PlayerData
            {
                PlayerName = mergedCharacterName,
                RollDataByKey = new Dictionary<string, RollData>()
            };

            foreach (string character in selectedCharacters)
            {
                if (allPlayerRollOutcomes.TryGetValue(character, out PlayerData playerData))
                {
                    foreach (var kvp in playerData.RollDataByKey)
                    {
                        string rollKey = kvp.Key;
                        RollData rollData = kvp.Value;

                        if (!mergedPlayerData.RollDataByKey.TryGetValue(rollKey, out RollData mergedRollData))
                        {
                            mergedRollData = new RollData { DiceRollKey = rollKey };
                            mergedPlayerData.RollDataByKey[rollKey] = mergedRollData;
                        }

                        // Merge the roll results
                        mergedRollData.Results.AddRange(rollData.Results);
                    }
                }
            }

            return mergedPlayerData;
        }

        #endregion

        #region Roll Statistics Grid

        /// <summary>
        /// Updates the roll statistics grid with the provided player roll outcomes.
        /// Displays overall stats per base roll (without modifiers).
        /// </summary>
        private void UpdateRollStatsGrid(Dictionary<string, PlayerData> playerRollOutcomes)
        {
            try
            {
                Invoke(new Action(() =>
                {
                    dgvRollStats.SuspendLayout();
                    dgvRollStats.Rows.Clear();
                    dgvRollStats.Columns.Clear();

                    // Define columns
                    dgvRollStats.Columns.Add("Player", "Player");
                    dgvRollStats.Columns.Add("Roll", "Roll");
                    dgvRollStats.Columns.Add("TotalRolls", "Total Rolls");
                    dgvRollStats.Columns.Add("Average", "Average");
                    dgvRollStats.Columns.Add("Median", "Median");
                    dgvRollStats.Columns.Add("Mode", "Mode");
                    dgvRollStats.Columns.Add("OnesRolled", "1's Rolled");
                    dgvRollStats.Columns.Add("MaxRolls", "Max Rolls");

                    // Populate rows
                    foreach (var player in playerRollOutcomes.Values)
                    {
                        // Group RollData by base roll (DiceRollKey)
                        var rollDataGroups = player.RollDataByKey.Values.GroupBy(rd => rd.DiceRollKey);

                        foreach (var rollGroup in rollDataGroups)
                        {
                            string baseRoll = rollGroup.Key;

                            // Collect all die roll values across all RollData with the same base roll
                            List<double> dieRolls = rollGroup.SelectMany(rd => rd.Results.Select(r => r.DieRollValue)).ToList();

                            if (dieRolls.Count == 0) continue;

                            double avgDieRoll = dieRolls.Average();
                            double median = GetMedian(dieRolls.OrderBy(r => r).ToList());
                            var modeResult = GetMode(dieRolls);
                            string modeFormatted = $"{modeResult.Item1} | {modeResult.Item2} times";
                            int onesRolled = dieRolls.Count(r => r == 1);
                            double maxRollValue = GetMaxDieRollValue(baseRoll);
                            int maxRolls = dieRolls.Count(r => r == maxRollValue);
                            int totalRollsCount = dieRolls.Count;

                            dgvRollStats.Rows.Add(
                                player.PlayerName,
                                baseRoll,
                                totalRollsCount,
                                Math.Round(avgDieRoll, 2),
                                median,
                                modeFormatted,
                                onesRolled,
                                maxRolls
                            );
                        }
                    }

                    // Sort the grid by player name
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

        /// <summary>
        /// Handles the SelectionChanged event for the roll stats grid.
        /// Updates the roll reasons grid based on the selected roll.
        /// </summary>
        private void dgvRollStats_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvRollStats.SelectedRows.Count == 1)
                {
                    var selectedRow = dgvRollStats.SelectedRows[0];

                    // Ensure the selected row is a data row and not a new row placeholder
                    if (!selectedRow.IsNewRow)
                    {
                        // Access cells safely
                        var playerCell = selectedRow.Cells["Player"];
                        var rollCell = selectedRow.Cells["Roll"];

                        if (playerCell?.Value != null && rollCell?.Value != null)
                        {
                            string playerName = playerCell.Value.ToString();
                            string baseRoll = rollCell.Value.ToString();

                            UpdateRollReasonsGrid(playerName, baseRoll);
                        }
                        else
                        {
                            // Clear the reasons grid if cell values are null
                            dgvRollReasons.Rows.Clear();
                            dgvRollReasons.Columns.Clear();
                        }
                    }
                    else
                    {
                        // Clear the reasons grid if the selected row is not valid
                        dgvRollReasons.Rows.Clear();
                        dgvRollReasons.Columns.Clear();
                    }
                }
                else
                {
                    // Clear the reasons grid if no row or multiple rows are selected
                    dgvRollReasons.Rows.Clear();
                    dgvRollReasons.Columns.Clear();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("An error occurred while processing the selection. Please check the error.log file for details.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Roll Reasons Grid

        /// <summary>
        /// Updates the roll reasons grid based on the selected player and base roll.
        /// Displays detailed stats per roll reason (flavor), including modifiers.
        /// </summary>
        private void UpdateRollReasonsGrid(string playerName, string baseRoll)
        {
            try
            {
                Invoke(new Action(() =>
                {
                    dgvRollReasons.SuspendLayout();
                    dgvRollReasons.Rows.Clear();
                    dgvRollReasons.Columns.Clear();

                    // Define columns
                    dgvRollReasons.Columns.Add("Roll", "Roll");
                    dgvRollReasons.Columns.Add("TotalRolls", "Total Rolls");
                    dgvRollReasons.Columns.Add("AvgDieRoll", "Average (no mod)");
                    dgvRollReasons.Columns.Add("AvgTotalRoll", "Average (w/ mod)");
                    dgvRollReasons.Columns.Add("MedianDieRoll", "Median (no mod)");
                    dgvRollReasons.Columns.Add("ModeDieRoll", "Mode (no mod)");
                    dgvRollReasons.Columns.Add("OnesRolled", "1's Rolled");
                    dgvRollReasons.Columns.Add("MaxRolls", "Max Rolls");
                    dgvRollReasons.Columns.Add("GrandTotal", "Grand Total");

                    if (allPlayerRollOutcomes.TryGetValue(playerName, out PlayerData playerData))
                    {
                        // Get all RollData with the specified base roll
                        var rollDatas = playerData.RollDataByKey.Values.Where(rd => rd.DiceRollKey == baseRoll).ToList();

                        if (rollDatas.Count > 0)
                        {
                            // Collect all RollResults
                            var allResults = rollDatas.SelectMany(rd => rd.Results).ToList();

                            // Group by Flavor (Roll Reason)
                            var flavorGroups = allResults.GroupBy(r => r.Flavor);

                            foreach (var flavorGroup in flavorGroups)
                            {
                                List<double> dieRolls = flavorGroup.Select(r => r.DieRollValue).ToList();
                                List<double> totalRolls = flavorGroup.Select(r => r.TotalValue).ToList();

                                if (dieRolls.Count == 0) continue;

                                // Calculations based on die rolls only
                                double avgDieRoll = dieRolls.Average();
                                double medianDieRoll = GetMedian(dieRolls.OrderBy(r => r).ToList());
                                var modeDieRollResult = GetMode(dieRolls);
                                string modeDieRollFormatted = $"{modeDieRollResult.Item1} | {modeDieRollResult.Item2} times";
                                int onesRolled = dieRolls.Count(r => r == 1);
                                double maxDieRollValue = GetMaxDieRollValue(baseRoll);
                                int maxRolls = dieRolls.Count(r => r == maxDieRollValue);

                                // Calculations including modifiers
                                double avgTotalRoll = totalRolls.Average();
                                double grandTotal = totalRolls.Sum();
                                int totalRollsCount = flavorGroup.Count();

                                dgvRollReasons.Rows.Add(
                                    flavorGroup.Key,                                // Roll (Flavor)
                                    totalRollsCount,                                // Total Rolls
                                    Math.Round(avgDieRoll, 2),                      // Average (no mod)
                                    Math.Round(avgTotalRoll, 2),                    // Average (w/ mod)
                                    medianDieRoll,                                  // Median (no mod)
                                    modeDieRollFormatted,                           // Mode (no mod)
                                    onesRolled,                                     // 1's Rolled
                                    maxRolls,                                       // Max Rolls (die roll only)
                                    grandTotal                                      // Grand Total (includes modifiers)
                                );
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

        #endregion

        #region Helper Methods

        /// <summary>
        /// Calculates the median of a sorted list of numbers.
        /// </summary>
        private double GetMedian(List<double> sortedNumbers)
        {
            try
            {
                int count = sortedNumbers.Count;
                if (count == 0) return 0;

                if (count % 2 == 0)
                {
                    // Even number of elements
                    double a = sortedNumbers[count / 2 - 1];
                    double b = sortedNumbers[count / 2];
                    return (a + b) / 2.0;
                }
                else
                {
                    // Odd number of elements
                    return sortedNumbers[count / 2];
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return 0;
            }
        }

        /// <summary>
        /// Calculates the mode and its frequency from a list of numbers.
        /// </summary>
        private Tuple<double, int> GetMode(List<double> numbers)
        {
            try
            {
                if (numbers.Count == 0) return Tuple.Create(0.0, 0);

                var grouped = numbers.GroupBy(n => n)
                                     .Select(g => new { Value = g.Key, Count = g.Count() })
                                     .OrderByDescending(g => g.Count)
                                     .ThenBy(g => g.Value)
                                     .ToList();

                var modeGroup = grouped.FirstOrDefault();

                if (modeGroup != null)
                {
                    return Tuple.Create(modeGroup.Value, modeGroup.Count);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return Tuple.Create(0.0, 0);
        }

        /// <summary>
        /// Calculates the maximum possible die roll value based on the base roll (without modifiers).
        /// </summary>
        private double GetMaxDieRollValue(string baseRoll)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(baseRoll))
                {
                    // Return 0 if baseRoll is empty or null
                    return 0;
                }

                // Split the base roll into individual dice expressions
                var diceExpressions = baseRoll.Split(new[] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                double maxRollValue = 0;

                foreach (var diceExpr in diceExpressions)
                {
                    if (string.IsNullOrWhiteSpace(diceExpr))
                    {
                        continue; // Skip empty expressions
                    }

                    var match = Regex.Match(diceExpr, @"^(?:max\(0,)?(?<num>\d*)d(?<faces>\d+)(?<keep>k(?:h|l|\d+))?\)?$");
                    if (match.Success)
                    {
                        string numStr = match.Groups["num"].Value;
                        string facesStr = match.Groups["faces"].Value;

                        int numDice = 1; // Default to 1 if numStr is empty
                        if (!string.IsNullOrEmpty(numStr))
                        {
                            if (!int.TryParse(numStr, out numDice))
                            {
                                // Continue if numDice is not a valid integer
                                continue;
                            }
                        }

                        if (!int.TryParse(facesStr, out int faces))
                        {
                            // Continue if faces is not a valid integer
                            continue;
                        }

                        string keepOption = match.Groups["keep"].Value;

                        // For dice expressions with keep options
                        int diceKept = numDice;

                        if (!string.IsNullOrEmpty(keepOption))
                        {
                            if (keepOption.StartsWith("kh"))
                            {
                                diceKept = numDice == 0 ? 1 : numDice; // Keep all dice if no number specified
                            }
                            else if (keepOption.StartsWith("kl"))
                            {
                                diceKept = numDice == 0 ? 1 : numDice; // Keep all dice if no number specified
                            }
                            else if (keepOption.StartsWith("k"))
                            {
                                string keepNumStr = keepOption.Substring(1);
                                if (int.TryParse(keepNumStr, out int keepNum))
                                {
                                    diceKept = keepNum;
                                }
                                else
                                {
                                    diceKept = 1; // Default to 1 if parsing fails
                                }
                            }
                        }

                        maxRollValue += diceKept * faces;
                    }
                    else
                    {
                        // Unable to parse, skip this dice expression
                        continue;
                    }
                }

                return maxRollValue;
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return 0; // Return 0 if unable to parse
        }


        /// <summary>
        /// Handles the CheckedChanged event for the debug checkbox.
        /// Shows or hides debug information.
        /// </summary>
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

        #endregion
    }
}
