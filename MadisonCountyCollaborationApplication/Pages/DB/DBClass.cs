using MadisonCountyCollaborationApplication.Pages.DataClasses;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata;

namespace MadisonCountyCollaborationApplication.Pages.DB
{
    public class DBClass
    {

        // Connection Object at Data Field Level
        public static SqlConnection MainDBconnection = new SqlConnection();

        // Connection String - How to find and connect to DB
        private static readonly String? MainDBconnString =
            "Server=Localhost;Database=Lab4;Trusted_Connection=True";
        // Connection String for AUTH database for Hashed Credentials
        private static readonly String? AuthConnString =
            "Server=Localhost;Database=AUTH;Trusted_Connection=True";


        public static bool StoredProcedureLogin(string Username, string Password)
        {

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.CommandType = System.Data.CommandType.StoredProcedure;
            cmdProductRead.Parameters.AddWithValue("@Username", Username);
            cmdProductRead.Parameters.AddWithValue("@Password", Password);
            cmdProductRead.CommandText = "sp_Lab4Login";
            cmdProductRead.Connection.Open();
            if (((int)cmdProductRead.ExecuteScalar()) > 0)
            {
                return true;
            }

            return false;
        }

        //Functions for displaying tables
        public static SqlDataReader CollaborationReader()
        {
            SqlCommand cmdCollaborationRead = new SqlCommand();
            cmdCollaborationRead.Connection = MainDBconnection;
            cmdCollaborationRead.Connection.ConnectionString = MainDBconnString;
            cmdCollaborationRead.CommandText = "SELECT * FROM Collaboration";
            cmdCollaborationRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdCollaborationRead.ExecuteReader();

            return tempReader;
        }
        public static SqlDataReader CollaborationGetName(int collabID)
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT collabName FROM Collaboration WHERE collabID = @collabID";
            cmdContentRead.Parameters.AddWithValue("@collabID", collabID);
            cmdContentRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdContentRead.ExecuteReader();

            return tempReader;
        }
        public static SqlDataReader CollaborationGetAllNames()
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT collabName FROM Collaboration";
            cmdContentRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdContentRead.ExecuteReader();

            return tempReader;
        }
        public static bool CollaborationExist(int collabID)
        {

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.CommandType = System.Data.CommandType.StoredProcedure;
            cmdProductRead.Parameters.AddWithValue("@collabID", collabID);
            cmdProductRead.CommandText = "sp_collabExist";
            cmdProductRead.Connection.Open();
            if (((int)cmdProductRead.ExecuteScalar()) > 0)
            {
                return true;
            }

            return false;
        }
        public static bool DatasetExist(int dataID)
        {

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.Parameters.AddWithValue("@datasetID", dataID);
            cmdProductRead.CommandText = "Select Count(*) FROM DataSets WHERE dataSetID = @datasetID";
            cmdProductRead.Connection.Open();
            if (((int)cmdProductRead.ExecuteScalar()) > 0)
            {
                return true;
            }

            return false;
        }

        public static bool KnowledgeExist(int dataID)
        {

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.Parameters.AddWithValue("@knowledgeID", dataID);
            cmdProductRead.CommandText = "Select Count(*) FROM KnowledgeItems WHERE knowledgeItemID = @knowledgeID";
            cmdProductRead.Connection.Open();
            if (((int)cmdProductRead.ExecuteScalar()) > 0)
            {
                return true;
            }

            return false;
        }


        public static SqlDataReader DatasetReader()
        {
            SqlCommand cmdKnowledgeRead = new SqlCommand();
            cmdKnowledgeRead.Connection = MainDBconnection;
            cmdKnowledgeRead.Connection.ConnectionString = MainDBconnString;
            cmdKnowledgeRead.CommandText = "SELECT * FROM DataSets";
            cmdKnowledgeRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdKnowledgeRead.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader DocReader()
        {
            SqlCommand cmdKnowledgeRead = new SqlCommand();
            cmdKnowledgeRead.Connection = MainDBconnection;
            cmdKnowledgeRead.Connection.ConnectionString = MainDBconnString;
            cmdKnowledgeRead.CommandText = "SELECT * FROM Documents";
            cmdKnowledgeRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdKnowledgeRead.ExecuteReader();

            return tempReader;
        }

        

        public static SqlDataReader UsersReader()
        {
            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = MainDBconnection;
            cmdUserRead.Connection.ConnectionString = MainDBconnString;
            cmdUserRead.CommandText = "SELECT * FROM Users";
            cmdUserRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdUserRead.ExecuteReader();

            return tempReader;
        }
        public static SqlDataReader PlansReader(int CollabID)
        {
            SqlCommand cmdPlanRead = new SqlCommand();
            cmdPlanRead.Connection = MainDBconnection;
            cmdPlanRead.Connection.ConnectionString = MainDBconnString;
            cmdPlanRead.CommandText = "SELECT * FROM Plans WHERE collabID = @CollabID";
            cmdPlanRead.Parameters.AddWithValue("@CollabID", CollabID);
            cmdPlanRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdPlanRead.ExecuteReader();

            return tempReader;
        }
        public static SqlDataReader ContentsReader(int PlanID)
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT * FROM PlanContents WHERE planID = @PlanID";
            cmdContentRead.Parameters.AddWithValue("@PlanID", PlanID);
            cmdContentRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdContentRead.ExecuteReader();

            return tempReader;
        }
        
        public static SqlDataReader CollabDatasetReader(int CollabID)
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT DataAssists.datasetID, DataSets.dataSetName FROM DataAssists " +
                "LEFT JOIN DataSets ON DataAssists.datasetID = DataSets.datasetID" +
                " WHERE collabID = @CollabID";
            cmdContentRead.Parameters.AddWithValue("@CollabID", CollabID);
            cmdContentRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdContentRead.ExecuteReader();

            return tempReader;
        }
        
        
        public static int ExtractDatasetID()
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT TOP 1 datasetID FROM DataSets ORDER BY datasetID DESC";
            cmdContentRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdContentRead.ExecuteReader();
            if (tempReader.Read())
            {
                return (int)tempReader["datasetID"];
            }
            else return 0;
        }
        
        public static string ExtractDatasetName(int ID)
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT dataSetName FROM DataSets WHERE datasetID = @dataID";
            cmdContentRead.Parameters.AddWithValue("@dataID", ID);
            cmdContentRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdContentRead.ExecuteReader();
            if (tempReader.Read())
            {
                return (string)tempReader["dataSetName"];
            }
            else return "null";
        }
        public static DataTable FetchDataForTable(string tableName)
        {
            // WARNING: Directly using user input in SQL queries can lead to SQL injection.
            // Ensure tableName is validated against a list of known, safe table names or use another form of verification.
            DataTable dataTable = new DataTable();
            string query = $"SELECT * FROM [{tableName}]"; // Unsafe: Do not use as is.

            using (var connection = new SqlConnection(MainDBconnString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            return dataTable;
        }
        
        
        //functions for adding values to databases



        public static void CreateDataset(String title)
        {
            String sqlQuery = "INSERT INTO DataSets (dataSetName) VALUES('";
            sqlQuery += title + "')";

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.CommandText = sqlQuery;
            cmdProductRead.Connection.Open();
            cmdProductRead.ExecuteNonQuery();
        }
        

        
        public static void CreatePlanContents(MadisonCountyCollaborationApplication.Pages.DataClasses.Contents Con)
        {
            String sqlQuery = "INSERT INTO PlanContents (contents, step, sequenceNumber, planID) VALUES('";
            sqlQuery += Con.planContents + "','";
            sqlQuery += Con.planStep + "','";
            sqlQuery += Con.sequenceNumber + "','";
            sqlQuery += Con.planID + "')";

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.CommandText = sqlQuery;
            cmdProductRead.Connection.Open();
            cmdProductRead.ExecuteNonQuery();
        }
        public static void CreatePlan(MadisonCountyCollaborationApplication.Pages.DataClasses.Plans Plan)
        {
            String sqlQuery = "INSERT INTO Plans (planName, planDesc, dateCreated, collabID) VALUES('";
            sqlQuery += Plan.planName + "','";
            sqlQuery += Plan.planDesc + "','";
            sqlQuery += Plan.dateCreated + "','";
            sqlQuery += Plan.collabID + "')";

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.CommandText = sqlQuery;
            cmdProductRead.Connection.Open();
            cmdProductRead.ExecuteNonQuery();
        }
        public static void CreateCollab(MadisonCountyCollaborationApplication.Pages.DataClasses.Collaboration Collab)
        {
            String sqlQuery = "INSERT INTO Collaboration (collabName, notesAndInfo, dateCreated, CollabType) VALUES('";
            sqlQuery += Collab.collabName + "','";
            sqlQuery += Collab.notesAndInfo + "','";
            sqlQuery += Collab.dateCreated + "','";
            sqlQuery += Collab.collabType + "')";

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.CommandText = sqlQuery;
            cmdProductRead.Connection.Open();
            cmdProductRead.ExecuteNonQuery();
        }
        
        
        public static void CreateDataAssist(MadisonCountyCollaborationApplication.Pages.DataClasses.DatasetAssist Assist)
        {
            String sqlQuery = "INSERT INTO DataAssists (datasetID, collabID) VALUES('";
            sqlQuery += Assist.datasetID + "','";
            sqlQuery += Assist.collabID + "')";

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.CommandText = sqlQuery;
            cmdProductRead.Connection.Open();
            cmdProductRead.ExecuteNonQuery();
        }
        
        public static SqlDataReader AddUsersReader()
        {
            SqlCommand cmdPlanRead = new SqlCommand();
            cmdPlanRead.Connection = MainDBconnection;
            cmdPlanRead.Connection.ConnectionString = MainDBconnString;
            cmdPlanRead.CommandText = @"SELECT 
                                            u.firstName, u.lastName, u.userName, u.email
                                        FROM 
                                            Collaboration c, Contributes con, Users u
                                        WHERE 
                                            c.collabID = con.collabID AND u.userID = con.userID";
            cmdPlanRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdPlanRead.ExecuteReader();

            return tempReader;
        }
        public static int GetUserIDFromUserNameOrEmail(string input)
        {
            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = MainDBconnection;
            cmdUserRead.Connection.ConnectionString = MainDBconnString;
            cmdUserRead.CommandText = "SELECT userID FROM Users WHERE userName = @input OR email = @input;";
            cmdUserRead.Parameters.AddWithValue("@input", input);
            cmdUserRead.Connection.Open();
            SqlDataReader tempReader = cmdUserRead.ExecuteReader();
            if (tempReader.Read())
            {
                return (int)tempReader["userID"];
            }
            else return 0;
        }
        public static bool UserExistInCollab(int userID, int collabID)
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = MainDBconnection;
            sqlCommand.Connection.ConnectionString = MainDBconnString;
            sqlCommand.CommandText = "SELECT COUNT(*) FROM Contributes WHERE userID = @userID AND collabID = @collabID;";
            sqlCommand.Parameters.AddWithValue("@userID", userID);
            sqlCommand.Parameters.AddWithValue("@collabID", collabID);
            sqlCommand.Connection.Open();
            int count = (int)sqlCommand.ExecuteScalar();
            if (count == 0)
            {
                return true;
            }

            return false;

        }
        public static bool AddUserToCollab(int userID, int collabID)
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = MainDBconnection;
            sqlCommand.Connection.ConnectionString = MainDBconnString;
            sqlCommand.CommandText = "INSERT INTO Contributes (userID, collabID) VALUES (@userID, @collabID);";
            sqlCommand.Parameters.AddWithValue("@userID", userID);
            sqlCommand.Parameters.AddWithValue("@collabID", collabID);
            sqlCommand.Connection.Open();
            int rowsAffected = sqlCommand.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                return true;
            }

            return false;

        }
        public static bool HashedParameterLogin(string Username, string Password)
        {
            string loginQuery =
                "SELECT Password FROM HashedCredentials WHERE Username = @Username";

            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = MainDBconnection;
            cmdLogin.Connection.ConnectionString = AuthConnString;

            cmdLogin.CommandText = loginQuery;
            cmdLogin.Parameters.AddWithValue("@Username", Username);

            cmdLogin.Connection.Open();

            // ExecuteScalar() returns back data type Object
            // Use a typecast to convert this to an int.
            // Method returns first column of first row.
            SqlDataReader hashReader = cmdLogin.ExecuteReader();
            if (hashReader.Read())
            {
                string correctHash = hashReader["Password"].ToString();

                if (PasswordHash.ValidatePassword(Password, correctHash))
                {
                    return true;
                }
            }

            return false;
        }
        

        public static SqlDataReader GetPlansFromCollabReader(int collabID)
        {
            SqlCommand cmdQuery = new SqlCommand();
            cmdQuery.Connection = MainDBconnection;
            cmdQuery.Connection.ConnectionString = MainDBconnString;
            cmdQuery.CommandText = @"SELECT 
                                         planID, planID, planName, planDesc, dateCreated
                                     FROM 
                                         Plans
                                    WHERE 
                                         collabID = @collabID";
            cmdQuery.Parameters.AddWithValue("@collabID", collabID);

            cmdQuery.Connection.Open();

            SqlDataReader resultReader = cmdQuery.ExecuteReader();

            return resultReader;
        }

        public static SqlDataReader GetUsersFromCollabReader(int collabID)
        {
            // Create a new connection instance specifically for this operation.
            SqlConnection conn = new SqlConnection(MainDBconnString);

            // Create the command object with the SQL query and associate it with the connection.
            SqlCommand cmdQuery = new SqlCommand();
            cmdQuery.Connection = conn; // Use the newly created connection
            cmdQuery.CommandText = @"SELECT 
                                        u.userID, u.firstName, u.lastName, u.email, u.userName
                                     FROM 
                                        Users u, Contributes con, Collaboration c
                                     WHERE 
                                        u.userID = con.userID AND con.collabID = c.collabID AND c.collabID = @collabID";
            cmdQuery.Parameters.AddWithValue("@collabID", collabID);

            // Open the connection right before executing the reader.
            conn.Open();

            // Execute the reader with CommandBehavior.CloseConnection to ensure the connection closes when the reader is closed.
            SqlDataReader resultReader = cmdQuery.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the reader to the caller.
            return resultReader;
        }



        

        // Combined Hashed Password and Insert User Method
        public static void CreateAndHashUser(MadisonCountyCollaborationApplication.Pages.DataClasses.Users newUser)
        {
            // Hash the password once
            var hashedPassword = PasswordHash.HashPassword(newUser.userPassword);

            // First, insert into the authentication database
            using (var authDbConnection = new SqlConnection(AuthConnString))
            {
                var authCommand = new SqlCommand("INSERT INTO HashedCredentials (Username,Password) values (@Username, @Password)", authDbConnection);
                authCommand.Parameters.AddWithValue("@Username", newUser.userName);
                authCommand.Parameters.AddWithValue("@Password", hashedPassword);

                authDbConnection.Open();
                authCommand.ExecuteNonQuery();
            }

            // Then, insert into the user database
            using (var userDbConnection = new SqlConnection(MainDBconnString))
            {
                var userCommand = new SqlCommand("sp_CreateUser", userDbConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                userCommand.Parameters.AddWithValue("@Username", newUser.userName);
                userCommand.Parameters.AddWithValue("@Password", hashedPassword); // Use hashed password
                userCommand.Parameters.AddWithValue("@firstName", newUser.firstName);
                userCommand.Parameters.AddWithValue("@lastName", newUser.lastName);
                userCommand.Parameters.AddWithValue("@email", newUser.email);
                userCommand.Parameters.AddWithValue("@phone", newUser.phone);
                userCommand.Parameters.AddWithValue("@type", newUser.userType);
                userCommand.Parameters.AddWithValue("@street", newUser.street);
                userCommand.Parameters.AddWithValue("@city", newUser.city);
                userCommand.Parameters.AddWithValue("@state", newUser.userState);
                userCommand.Parameters.AddWithValue("@zip", newUser.zip);

                userDbConnection.Open();
                userCommand.ExecuteNonQuery();
            }
        }

   
    }
}
