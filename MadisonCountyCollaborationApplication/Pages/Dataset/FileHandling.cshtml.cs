using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Dynamic;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class FileHandlingModel : PageModel
    {
        private static readonly string CollabAppString = "server=Localhost;Database=Lab3;Trusted_Connection=True";
        // Method to check session before accessing the page
        public IActionResult OnGetSessionCheck()
        {
            if (HttpContext.Session.GetString("username") == null)
            { // If user is not logged in, set an error message and redirect to login page
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login/HashedLogin");
            }
            else
            {
                return Page(); // If logged in, allow access to the page
            }
        }
        // Method to handle GET requests for processing a CSV file
        public async Task<IActionResult> OnGetAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                ModelState.AddModelError("", "File path is not provided.");
                return Page();
            }

            if (!System.IO.File.Exists(filePath)) // error message if the file does not exist 
            {
                ModelState.AddModelError("", "File does not exist.");
                return Page();
            }

            try
            {
                await ProcessCsvFile(filePath);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error processing file: {ex.Message}"); // error message if their is an error processing file
                return Page();
            }

            // Redirect to a success page and optionally pass the table name for confirmation.
            TempData["TableName"] = Path.GetFileNameWithoutExtension(filePath);
            return RedirectToPage("/Datasets");
        }
        // Method to process a CSV file asynchronously
        private async Task ProcessCsvFile(string filePath)
        {
            string tableName = Path.GetFileNameWithoutExtension(filePath).Replace(" ", "_").Replace("-", "_");
            string tableID = (DBClass.ExtractDatasetID() + 1).ToString();
            DBClass.MainDBconnection.Close();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ",", HeaderValidated = null, MissingFieldFound = null }))
            {
                var records = new List<IDictionary<string, object>>();
                csv.Read();
                csv.ReadHeader();
                var headers = csv.Context.Reader.HeaderRecord;

                while (csv.Read())
                {
                    var record = new ExpandoObject() as IDictionary<string, object>;
                    foreach (string header in headers)
                    {
                        record[header] = csv.GetField(header);
                    }
                    records.Add(record);
                }

                await CreateTableFromCsv(tableName + tableID, headers, records);
                await AddFileToDataSet(tableName);
            }
        }
        // Method to create table from CSV data
        private async Task CreateTableFromCsv(string tableName, string[] headers, IEnumerable<IDictionary<string, object>> records)
        {
            using (var connection = new SqlConnection(CollabAppString))
            {
                await connection.OpenAsync();
                var createTableSql = GenerateTableCreationSql(tableName, headers);
                using (var command = new SqlCommand(createTableSql, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
                await InsertDataIntoTable(tableName, headers, records, connection);
            }
        }
        // Method to generate SQL for creating table
        private string GenerateTableCreationSql(string tableName, string[] headers)
        {
            var columnDefinitions = headers.Select(header => $"[{header}] NVARCHAR(MAX)");
            string sql = $@"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'{tableName}')
                            BEGIN
                                CREATE TABLE [{tableName}] ({string.Join(", ", columnDefinitions)});
                            END";
            return sql;
        }
        // Method to insert data into table
        private async Task InsertDataIntoTable(string tableName, string[] headers, IEnumerable<IDictionary<string, object>> records, SqlConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                foreach (var record in records)
                {
                    var columns = string.Join(", ", record.Keys.Select(key => $"[{key}]"));
                    var values = string.Join(", ", record.Values.Select(value => value == null ? "NULL" : $"'{value.ToString().Replace("'", "''")}'"));
                    var insertSql = $"INSERT INTO [{tableName}] ({columns}) VALUES ({values});";

                    using (var command = new SqlCommand(insertSql, connection, transaction))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
                transaction.Commit();
            }
        }
        // method to retrieve datasets
        public static List<string> FetchAllDatasets()
        {
            List<string> datasetNames = new List<string>();
            string sqlQuery = "SELECT FileName FROM DataSets"; // Adjust if your actual table name differs
            using (var connection = new SqlConnection(CollabAppString)) // Use the actual connection string variable
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string fileName = reader.GetString(0); // Assumes FileName is the first column
                        datasetNames.Add(fileName);
                    }
                }
            }
            return datasetNames;
        }



        private async Task AddFileToDataSet(string tableName)
        {
            // Assuming the DataSet table has a column named "TableName"
            string addFileQuery = @"INSERT INTO DataSets (dataSetName) VALUES (@FileName);";

            using (var connection = new SqlConnection(CollabAppString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(addFileQuery, connection))
                {
                    // Parameterization to prevent SQL injection
                    command.Parameters.AddWithValue("@FileName", tableName);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
