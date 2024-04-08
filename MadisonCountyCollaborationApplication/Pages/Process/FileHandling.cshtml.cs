using CsvHelper.Configuration;
using CsvHelper;
using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Dynamic;
using System.Globalization;
using System.Data.SqlClient;


namespace MadisonCountyCollaborationApplication.Pages.Process
{

    public class FileHandlingModel : PageModel
    {
        private static readonly string MainDBconnString = "Server=tcp:upstreamconsultingdata.database.windows.net,1433;Initial Catalog=MainDB;Persist Security Info=False;User ID=SQLAdmin;Password=GoDukes1$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        [BindProperty(SupportsGet = true)]
        public string FilePath { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FileName { get; set; }


        // Method to check session before accessing the page
        public IActionResult OnGetSessionCheck()
        {
            if (HttpContext.Session.GetString("username") == null)
            { // If user is not logged in, set an error message and redirect to login page
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                string filePath = HttpContext.Request.Query["filePath"];
                string fileName = HttpContext.Request.Query["newFileName"];

                return RedirectToPage("/Login/HashedLogin");

            }
            else
            {
                return Page(); // If logged in, allow access to the page
            }
        }
        // Method to handle GET requests for processing a CSV file
        public async Task<IActionResult> OnGetAsync(string filePath, string newFileName)
        {
            int ProcessID = (int)HttpContext.Session.GetInt32("processID");

            if (string.IsNullOrWhiteSpace(filePath))
            {
                ModelState.AddModelError("", "File path is not provided.");
                return Page();
            }

            if (!System.IO.File.Exists(filePath)) // error message if the file does not exist 
            {
                ModelState.AddModelError("", "File does not exist.");
                Console.WriteLine("File Exists");
                return Page();
            }

            try
            {
                await ProcessCsvFile(filePath, newFileName);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error processing file: {ex.Message}"); // error message if there is an error processing file
                Console.WriteLine("Error processing");
                return Page();
            }

            // Redirect to a success page and optionally pass the table name for confirmation.
            TempData["TableName"] = Path.GetFileNameWithoutExtension(filePath);
            return RedirectToPage("./Index", new { ProcessID = ProcessID });
        }
        // Method to process a CSV file asynchronously
        private async Task ProcessCsvFile(string filePath, string fileName)
        {
            

            DBClass.MainDBconnection.Close();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ",", HeaderValidated = null, MissingFieldFound = null }))
            {
                var records = new List<IDictionary<string, object>>();
                csv.Read();
                csv.ReadHeader();
                var headers = csv.Context.Reader.HeaderRecord;
                var noSpacesHeaders = headers.Select(header => "_" + header.Replace(" ", "_")).ToArray();

                while (csv.Read())
                {
                    var record = new ExpandoObject() as IDictionary<string, object>;
                    foreach (string columnHeader in headers)
                    {
                        string modifiedHeader = "_" + columnHeader.Replace(" ", "_");
                        record[modifiedHeader] = csv.GetField(columnHeader);
                    }
                    records.Add(record);
                }

                await CreateTableFromCsv(fileName, noSpacesHeaders, records);
                await AddFileToDataSet(fileName); // Assume this inserts the dataset name into DataSets
            }

            // After CSV processing and dataset insertion
            int datasetID = await GetDatasetID(fileName); // Retrieve the newly inserted dataset ID. Implement this method based on prior instructions.
            int ProcessID = (int)HttpContext.Session.GetInt32("processID"); // Retrieve ProcessID from session

            await InsertIntoDatasetProcess(ProcessID, datasetID); // Insert the association into DataAssist. Implement this method based on prior instructions.
        }


        private async Task<int> GetDatasetID(string tableName)
        {
            string sqlQuery = @"SELECT datasetID FROM DataSet WHERE dataSetName = @TableName;";
            using (var connection = new SqlConnection(MainDBconnString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(0); // Assuming datasetID is the first column
                        }
                        else
                        {
                            throw new InvalidOperationException("Dataset not found after upload.");
                        }
                    }
                }
            }
        }

        private async Task InsertIntoDatasetProcess(int ProcessID, int datasetID)
        {
            string sqlQuery = @"INSERT INTO DatasetProcess (processID, datasetID) VALUES (@ProcessID, @DatasetID);";
            using (var connection = new SqlConnection(MainDBconnString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@ProcessID", ProcessID);
                    command.Parameters.AddWithValue("@DatasetID", datasetID);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }



        // Method to create table from CSV data
        private async Task CreateTableFromCsv(string tableName, string[] headers, IEnumerable<IDictionary<string, object>> records)
        {
            using (var connection = new SqlConnection(MainDBconnString))
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
            string sqlQuery = "SELECT FileName FROM DataSet"; // Adjust if your actual table name differs
            using (var connection = new SqlConnection(MainDBconnString)) // Use the actual connection string variable
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
            //Get UserID to put in table
            string username = HttpContext.Session.GetString("username");
            var userID = DBClass.UserNameIDConverter(username);

            // Assuming the DataSet table has a column named "TableName"
            string addFileQuery = @"INSERT INTO DataSet (dataSetName, userID) VALUES (@FileName, " + userID + ");";

            using (var connection = new SqlConnection(MainDBconnString))
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
