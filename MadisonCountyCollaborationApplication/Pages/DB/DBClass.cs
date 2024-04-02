﻿using MadisonCountyCollaborationApplication.Pages.DataClasses;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata;
using static MadisonCountyCollaborationApplication.Pages.Dataset.ViewDataModel;
using static Plotly.NET.StyleParam.DrawingStyle;

namespace MadisonCountyCollaborationApplication.Pages.DB
{
    public class DBClass
    {

        // Connection Object at Data Field Level
        public static SqlConnection MainDBconnection = new SqlConnection();

        // Connection String - How to find and connect to DB
        private static readonly String? MainDBconnString =
            "Server=Localhost;Database=MainDB;Trusted_Connection=True";
        // Connection String for AUTH database for Hashed Credentials
        private static readonly String? AuthConnString =
            "Server=Localhost;Database=AUTH;Trusted_Connection=True";


        //GENERAL READER STATEMENT -- PARKER T. SHORT
        public static SqlDataReader GeneralReaderQuery(string sqlQuery)
        {
            var connection = new SqlConnection(MainDBconnString);
            var cmdGeneralRead = new SqlCommand(sqlQuery, connection);
            connection.Open();
            return cmdGeneralRead.ExecuteReader(CommandBehavior.CloseConnection);
        }
        //GENERAL INSERT STATEMENT -- PARKER T. SHORT 
        public static void GeneralInsertQuery(string sqlQuery)
        {
            using (var connection = new SqlConnection(MainDBconnString))
            {
                using (var cmdGeneralRead = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    cmdGeneralRead.ExecuteNonQuery();
                }
            }
        }



        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //PROCESS SECTION           

        //Functions for displaying tables
        public static SqlDataReader ProcessReader()
        {
            SqlCommand cmdProcessRead = new SqlCommand();
            cmdProcessRead.Connection = MainDBconnection;
            cmdProcessRead.Connection.ConnectionString = MainDBconnString;
            cmdProcessRead.CommandText = "SELECT * FROM Process";
            cmdProcessRead.Connection.Open();

            SqlDataReader tempReader = cmdProcessRead.ExecuteReader();

            return tempReader;
        }


        public static SqlDataReader ProcessGetName(int ProcessID)
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT processName FROM Process WHERE ProcessID = @ProcessID";
            cmdContentRead.Parameters.AddWithValue("@ProcessID", ProcessID);
            cmdContentRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdContentRead.ExecuteReader();

            return tempReader;
        }

        public static bool ProcessExist(int ProcessID)
        {
            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.CommandType = System.Data.CommandType.StoredProcedure;
            cmdProductRead.Parameters.AddWithValue("@ProcessID", ProcessID);
            cmdProductRead.CommandText = "sp_processExist";
            cmdProductRead.Connection.Open();
            if (((int)cmdProductRead.ExecuteScalar()) > 0)
            {
                return true;
            }

            return false;
        }

        public static void CreateProcess(MadisonCountyCollaborationApplication.Pages.DataClasses.Process process)
        {
            // SQL query now only includes processName and notesAndInfo fields
            String sqlQuery = "INSERT INTO Process (processName, notesAndInfo) VALUES(@ProcessName, @NotesAndInfo);";

            using (SqlCommand cmdProductRead = new SqlCommand(sqlQuery, MainDBconnection))
            {
                // Adding parameters to prevent SQL injection
                cmdProductRead.Parameters.AddWithValue("@ProcessName", process.ProcessName);
                cmdProductRead.Parameters.AddWithValue("@NotesAndInfo", process.NotesAndInfo ?? string.Empty); // Handling potential nulls

                MainDBconnection.ConnectionString = MainDBconnString;
                if (MainDBconnection.State != System.Data.ConnectionState.Open)
                {
                    MainDBconnection.Open();
                }

                cmdProductRead.ExecuteNonQuery();
            }
        }



        public static SqlDataReader ProcessDatasetReader(int ProcessID)
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT DataAssists.datasetID, DataSet.dataSetName FROM DataAssists " +
                "LEFT JOIN DataSet ON DataAssists.datasetID = DataSet.datasetID" +
                " WHERE ProcessID = @ProcessID";
            cmdContentRead.Parameters.AddWithValue("@ProcessID", ProcessID);
            cmdContentRead.Connection.Open();

            SqlDataReader tempReader = cmdContentRead.ExecuteReader();

            return tempReader;
        }



        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //DATASET SECTION

        public static void InsertIntoDataAssists(int selectedCollabID, int datasetID)
        {
            string sqlQuery = "INSERT INTO DataAssists (collabID, datasetID) VALUES (@CollabID, @DatasetID)";

            using (var connection = new SqlConnection(MainDBconnString))
            {
                using (var cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@CollabID", selectedCollabID);
                    cmd.Parameters.AddWithValue("@DatasetID", datasetID);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

      
        public static bool DatasetExist(int dataID)
        {

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.Parameters.AddWithValue("@datasetID", dataID);
            cmdProductRead.CommandText = "Select Count(*) FROM DataSet WHERE dataSetID = @datasetID";
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
            cmdKnowledgeRead.CommandText = "SELECT * FROM DataSet";
            cmdKnowledgeRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdKnowledgeRead.ExecuteReader();

            return tempReader;
        }


        
        
        
        
        public static int ExtractDatasetID()
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT TOP 1 datasetID FROM DataSet ORDER BY datasetID DESC";
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
            cmdContentRead.CommandText = "SELECT dataSetName FROM DataSet WHERE datasetID = @dataID";
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

            DataTable dataTable = new DataTable();
            string query = $"SELECT * FROM [{tableName}]"; 

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

        //functions for adding values to 
        public static void CreateDataset(String title)
        {
            String sqlQuery = "INSERT INTO DataSet (dataSetName) VALUES('";
            sqlQuery += title + "')";

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




        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //USERS SECTION


        //Is user admin?
        public static bool CheckAdmin(int userID)
        {
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = MainDBconnection;
                sqlCommand.Connection.ConnectionString = MainDBconnString;
                sqlCommand.CommandText = "SELECT COUNT(*) FROM Users WHERE userID = @userID AND userType = 'admin';";
                sqlCommand.Parameters.AddWithValue("@userID", userID);
                sqlCommand.Connection.Open();
                int count = (int)sqlCommand.ExecuteScalar();

                return (count > 0);
            }
        }


 
        //Converts session UserName to a UserID to display processes that only a user is in
        public static int UserNameIDConverter(string username)
        {
            int userID = 0; // Default value indicating not found or invalid
            string userQuery = "SELECT userID FROM Users WHERE userName = @username;";

            using (var connection = new SqlConnection(MainDBconnString))
            {
                using (var command = new SqlCommand(userQuery, connection))
                {
                    // Use parameterized query to prevent SQL injection
                    command.Parameters.AddWithValue("@username", username);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userID = reader.GetInt32(0); // Assuming userID is the first column
                        }
                    }
                }
            }
            return userID;
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
