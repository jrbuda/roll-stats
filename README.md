# Roll Stats

Welcome to **Roll Stats**, a utility for breaking down and analyzing FoundryVTT (Virtual Tabletop) JSON databases (ldb) to provide an easily digestible format for game masters and players alike.

## Features

- Parses FoundryVTT ldb files to generate meaningful statistics.
- Displays data in an easy-to-read format for quick insights.
- Helps game masters understand trends in dice rolls, player activity, and more.

## Installation

1. Clone this repository to your local machine using:
   ```bash
   git clone https://github.com/jrbuda/roll-stats.git
   ```

2. Install the required dependencies:
   ```bash
   npm install
   ```

   **Note:** Ensure you have Node.js installed. The `level` library is required for `ldb-reader.js` to work properly. The `package.json` file includes this dependency, so running `npm install` will handle it for you.

3. Run the program:
   ```bash
   npm start
   ```

## Usage

- Ensure you have a `.ldb` file exported from FoundryVTT.
- Provide the `.ldb` file as an input to Roll Stats.
- View the detailed breakdown of statistics in the output.

## Example Output

After running Roll Stats, you will receive a report that includes:
- Total number of rolls made.
- Breakdown of rolls by player.
- Frequency of natural 1s and 20s.
- Average rolls and critical success rates.

This makes it easier for you to analyze gameplay behavior and ensure everyone has a fair and enjoyable experience.

## Contributing

Contributions are welcome! If you'd like to help improve Roll Stats, feel free to:
- Fork this repository.
- Create a new branch for your feature or bug fix.
- Submit a pull request with a detailed explanation of your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

If you encounter any issues or have suggestions, please open an issue on the GitHub repo or reach out to me directly via GitHub.

### Acknowledgments

Special thanks to the FoundryVTT community for inspiring this project and providing valuable feedback.