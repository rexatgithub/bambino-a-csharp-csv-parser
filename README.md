# bambino — C# CSV parser

A small C# utility that watches a target folder for CSV files, parses them, and inserts the data into a configured database.

Features
- Uses a background worker to keep the UI responsive and avoid crashes when processing large files.
- Uses Microsoft.VisualBasic.FileIO.TextFieldParser for robust CSV parsing (handles quoted fields and delimiters).
- Simple configuration for the watched directory and database connection.
- Basic error handling and logging to help diagnose malformed rows or connection issues.

Prerequisites
- Visual Studio (recommended) or another C#/.NET IDE
- .NET Framework/.NET SDK compatible with the project (see the project file for exact target)
- Access to the target database and permissions to insert data

Quick start
1. Clone the repository:
   git clone https://github.com/rexatgithub/bambino-a-csharp-csv-parser.git
2. Open the solution in Visual Studio.
3. Configure the application:
   - Open the application's configuration (app.config or the project settings). Set the directory to watch and the database connection string. If your project uses a different configuration source, update the appropriate settings.
4. Build the solution (Debug or Release).
5. Run the application. Drop CSV files into the configured directory — the app will detect and process them.

CSV expectations
- CSVs should be text files with values separated by commas (or the delimiter configured in the parser).
- It's recommended to include a header row whose column names match your database column names. If your CSV layout differs, adjust the mapping in code accordingly.

Configuration
- Target directory: the folder the app watches for new CSV files.
- Database connection: a connection string with insert permissions.
- Other options (poll interval, log location) may be available in the project settings — check the code or project documentation.

Troubleshooting
- If rows are skipped or parsing fails, ensure the CSV is well-formed (proper quoting and consistent columns).
- Check the application logs or console output for detailed errors and stack traces.

Contributing
Contributions are welcome. Please open an issue to discuss changes or submit a pull request.
