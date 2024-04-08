using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using MadisonCountyCollaborationApplication.Pages.DataClasses;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata;

namespace MadisonCountyCollaborationApplication.Pages.DB
{
    public class Datasets_DBClass
    {
        // Connection Object at Data Field Level
        public static SqlConnection MainDBconnection = new SqlConnection();

        // Connection String - How to find and connect to DB
        private static readonly String? MainDBconnString =
             "Server=tcp:upstreamconsultingdata.database.windows.net,1433;Initial Catalog=MainDB;Persist Security Info=False;User ID=SQLAdmin;Password=GoDukes1$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        public static List<T> AttributeReader<T>(string column, string dataSet)
        {
            List<T> values = new List<T>();
            using (SqlConnection connection = new SqlConnection(MainDBconnString))
            {
                using (SqlCommand cmdContentRead = new SqlCommand($"SELECT {column} FROM {dataSet}", connection))
                {
                    connection.Open();
                    using (SqlDataReader tempReader = cmdContentRead.ExecuteReader())
                    {
                        while (tempReader.Read())
                        {
                            values.Add((T)Convert.ChangeType(tempReader[column], typeof(T)));
                        }
                    }
                }
            }
            return values;
        }
        public static (List<List<double>> independents, List<double> dependent) FetchRegressionData(string[] independentColumns, string dependentColumn, string dataSet)
        {
            var independentValues = new List<List<double>>();
            foreach (var column in independentColumns)
            {
                independentValues.Add(new List<double>()); // Initialize each list for independent variables
            }
            var dependentValues = new List<double>();

            using (SqlConnection connection = new SqlConnection(MainDBconnString))
            {
                // Build the SELECT statement dynamically to include all independent columns and the dependent column
                string columns = string.Join(", ", independentColumns) + ", " + dependentColumn;
                using (SqlCommand cmd = new SqlCommand($"SELECT {columns} FROM {dataSet}", connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < independentColumns.Length; i++)
                            {
                                // Assuming all data is numeric and convertible to double
                                independentValues[i].Add(Convert.ToDouble(reader[independentColumns[i]]));
                            }
                            dependentValues.Add(Convert.ToDouble(reader[dependentColumn]));
                        }
                    }
                }
            }
            return (independentValues, dependentValues);
        }
        public static SqlDataReader AttributeReader(string datasetName)
        {
            SqlCommand attributeRead = new SqlCommand();
            attributeRead.Connection = MainDBconnection;
            attributeRead.Connection.ConnectionString = MainDBconnString;
            attributeRead.CommandText = $"SELECT _Account FROM {datasetName}";
            attributeRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = attributeRead.ExecuteReader();

            return tempReader;
        }
        public static string AccountReader(string datasetName, string independentVariable)
        {
            SqlCommand attributeRead = new SqlCommand();
            attributeRead.Connection = MainDBconnection;
            attributeRead.Connection.ConnectionString = MainDBconnString;
            attributeRead.CommandText = $"SELECT {independentVariable} FROM {datasetName}";
            Console.WriteLine(attributeRead.CommandText.ToString());
            Console.WriteLine("Dataset: " + datasetName);
            Console.WriteLine("Var: " + independentVariable);
            attributeRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = attributeRead.ExecuteReader();
            List<double> values = new List<double>();
            bool firstRow = true;
            while (tempReader.Read())
            {
                values.Add(Convert.ToDouble(tempReader[independentVariable].ToString().Replace("(","-").Replace(",","").Replace("$","").Replace(")","")));

            }
            return values.ToArray().Average().ToString();
        }
        public static string AccountDeviation(string datasetName, string independentVariable)
        {
            SqlCommand attributeRead = new SqlCommand();
            attributeRead.Connection = MainDBconnection;
            attributeRead.Connection.ConnectionString = MainDBconnString;
            attributeRead.CommandText = $"SELECT {independentVariable} FROM {datasetName}";
            Console.WriteLine("Dataset: " + datasetName);
            Console.WriteLine("Var: " + independentVariable);
            attributeRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = attributeRead.ExecuteReader();
            List<double> values = new List<double>();
            bool firstRow = true;
            while (tempReader.Read())
            {
                values.Add(Convert.ToDouble(tempReader[independentVariable].ToString().Replace("(", "-").Replace(",", "").Replace("$", "").Replace(")", "")));

            }
            double[] dev = values.ToArray();
            double average = dev.Average();
            double sumOfSquaresOfDifferences = dev.Select(val => (val - average) * (val - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / dev.Length);
            return sd.ToString();
        }
    }
}

