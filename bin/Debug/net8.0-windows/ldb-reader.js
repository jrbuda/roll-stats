const { Level } = require('level');
const path = require('path');
const fs = require('fs');

async function readLevelDB(dbPath, outputFilePath) {
    try {
        // Check if the directory exists
        if (!fs.existsSync(dbPath)) {
            console.log(`Database directory not found: ${dbPath}`);
            process.exit(1); // Exit with error code
        }

        // Open the LevelDB database with valueEncoding set to 'utf8'
        const db = new Level(dbPath, { valueEncoding: 'utf8' });

        // Array to collect all entries
        const entries = [];

        // Iterate over all key-value pairs using async iterator
        for await (const [key, value] of db.iterator()) {
            let parsedValue;
            try {
                // Parse the main value as JSON
                parsedValue = JSON.parse(value);
            } catch (e) {
                // If parsing fails, keep the original value
                parsedValue = value;
            }

            // Extract timestamp before converting it
            let timestampValue = null;

            // Function to recursively parse nested JSON strings and convert timestamps
            function parseNestedJSON(obj) {
                for (const prop in obj) {
                    if (typeof obj[prop] === 'string') {
                        try {
                            const parsed = JSON.parse(obj[prop]);
                            obj[prop] = parsed;
                            parseNestedJSON(parsed); // Recursively parse nested objects
                        } catch (e) {
                            // If parsing fails, check for 'timestamp' fields
                            if (prop === 'timestamp') {
                                const timestamp = Number(obj[prop]);
                                if (!isNaN(timestamp)) {
                                    obj[prop] = new Date(timestamp).toISOString();
                                    timestampValue = timestampValue || timestamp;
                                }
                            }
                            // Leave the value as is if parsing fails and it's not a timestamp
                        }
                    } else if (typeof obj[prop] === 'number') {
                        if (prop === 'timestamp') {
                            timestampValue = timestampValue || obj[prop];
                            obj[prop] = new Date(obj[prop]).toISOString();
                        }
                    } else if (typeof obj[prop] === 'object' && obj[prop] !== null) {
                        parseNestedJSON(obj[prop]); // Recurse into nested objects
                    }
                }
            }

            // Parse any nested JSON strings and convert timestamps within the parsed value
            if (typeof parsedValue === 'object' && parsedValue !== null) {
                parseNestedJSON(parsedValue);
            }

            // Add the entry to the array with its timestamp
            entries.push({
                key,
                value: parsedValue,
                timestamp: timestampValue
            });
        }

        await db.close(); // Close the database

        // Sort the entries array by timestamp
        entries.sort((a, b) => {
            // Handle missing timestamps by placing them at the end
            if (a.timestamp === null || a.timestamp === undefined) return 1;
            if (b.timestamp === null || b.timestamp === undefined) return -1;
            return a.timestamp - b.timestamp;
        });

        // Create a write stream to save the output to a file
        const outputStream = fs.createWriteStream(outputFilePath, { flags: 'w' });

        // Write the sorted entries to the file
        for (const entry of entries) {
            const output = `Key: ${entry.key}\nValue:\n${JSON.stringify(entry.value, null, 2)}\n\n`;
            outputStream.write(output);
        }

        outputStream.end(); // Close the file stream
        console.log('Read complete. Output saved to:', outputFilePath);
    } catch (e) {
        console.error(`An error occurred: ${e}`);
        process.exit(1); // Exit with error code
    }
}

// Get the database path and output file path from command-line arguments
const args = process.argv.slice(2);
if (args.length < 2) {
    console.error('Usage: node ldb-reader.js <dbPath> <outputFilePath>');
    process.exit(1); // Exit with error code
}

const dbPath = args[0];
const outputFilePath = args[1];

(async () => {
    await readLevelDB(dbPath, outputFilePath);
})();
