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
            "Server=Localhost;Database=Lab4;Trusted_Connection=True";

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
    }
}
