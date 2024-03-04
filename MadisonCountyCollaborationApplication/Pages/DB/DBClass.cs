using MadisonCountyCollaborationApplication.Pages.DataClasses;
using System.Data;
using System.Data.SqlClient;

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

        public static SqlDataReader SWOTReader()
        {
            SqlCommand cmdKnowledgeRead = new SqlCommand();
            cmdKnowledgeRead.Connection = MainDBconnection;
            cmdKnowledgeRead.Connection.ConnectionString = MainDBconnString;
            cmdKnowledgeRead.CommandText = @"SELECT 
                                                s.swotID, 
                                                s.title, 
                                                s.category, 
                                                s.strengths, 
                                                s.weaknesses, 
                                                s.opportunities, 
                                                s.threats,
                                                CONCAT(u.firstName, ' ', u.lastName) AS author
                                            FROM 
                                                SWOT s
                                            LEFT JOIN
                                                SWOT_Author a ON s.swotID = a.swotID
                                            LEFT JOIN
                                                Users u ON a.userID = u.userID;";
            cmdKnowledgeRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdKnowledgeRead.ExecuteReader();

            return tempReader;
        }
        public static SqlDataReader SWOTDetails(int swotID)
        {
            SqlCommand cmdPlanRead = new SqlCommand();
            cmdPlanRead.Connection = MainDBconnection;
            cmdPlanRead.Connection.ConnectionString = MainDBconnString;
            cmdPlanRead.CommandText = @"SELECT 
                                        s.swotID, 
                                        s.title, 
                                        s.category, 
                                        s.strengths, 
                                        s.weaknesses, 
                                        s.opportunities, 
                                        s.threats,
	                                    s.swotDate,
                                        CONCAT(u.firstName, ' ', u.lastName) AS author
                                    FROM 
                                        SWOT s
                                    LEFT JOIN
                                        SWOT_Author a ON s.swotID = a.swotID
                                    LEFT JOIN
                                        Users u ON a.userID = u.userID
                                    WHERE
	                                    s.swotID = @swotID;";
            cmdPlanRead.Parameters.AddWithValue("@swotID", swotID);
            cmdPlanRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdPlanRead.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader KnowledgeItemsReader(string author = null, string keyword = null, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            SqlCommand cmdKnowledgeRead = new SqlCommand();
            cmdKnowledgeRead.Connection = MainDBconnection;
            cmdKnowledgeRead.Connection.ConnectionString = MainDBconnString;
            cmdKnowledgeRead.CommandText = @"SELECT 
                                                ki.knowledgeItemID, 
                                                ki.title, 
                                                ki.KISubject, 
                                                ki.category, 
                                                ki.information, 
                                                ki.KMDate, 
                                                CONCAT(u.firstName, ' ', u.lastName) AS author
                                            FROM 
                                                KnowledgeItems ki
                                            LEFT JOIN 
                                                Author a ON ki.knowledgeItemID = a.knowledgeItemID
                                            LEFT JOIN 
                                                Users u ON a.userID = u.userID;";
            cmdKnowledgeRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdKnowledgeRead.ExecuteReader();

            return tempReader;
        }
        public static SqlDataReader KnowledgeItemsReader()
        {
            SqlCommand cmdKnowledgeRead = new SqlCommand();
            cmdKnowledgeRead.Connection = MainDBconnection;
            cmdKnowledgeRead.Connection.ConnectionString = MainDBconnString;
            cmdKnowledgeRead.CommandText = @"SELECT 
                                                ki.knowledgeItemID, 
                                                ki.title, 
                                                ki.KISubject, 
                                                ki.category, 
                                                ki.information, 
                                                ki.KMDate, 
                                                CONCAT(u.firstName, ' ', u.lastName) AS author
                                            FROM 
                                                KnowledgeItems ki
                                            LEFT JOIN 
                                                Author a ON ki.knowledgeItemID = a.knowledgeItemID
                                            LEFT JOIN 
                                                Users u ON a.userID = u.userID;";
            cmdKnowledgeRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdKnowledgeRead.ExecuteReader();

            return tempReader;
        }
        public static SqlDataReader KnowledgeItemCategoryReader()
        {
            SqlCommand cmdKnowledgeRead = new SqlCommand();
            cmdKnowledgeRead.Connection = MainDBconnection;
            cmdKnowledgeRead.Connection.ConnectionString = MainDBconnString;
            cmdKnowledgeRead.CommandText = "SELECT DISTINCT(category) FROM KnowledgeItems";
            cmdKnowledgeRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdKnowledgeRead.ExecuteReader();

            return tempReader;
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

        public static SqlDataReader SearchKnowledgeItems(string author, string keyword, string category, DateOnly? dateFrom, DateOnly? dateTo)
        {
            List<string> conditions = new List<string>();

            // Start with the basic SELECT statement
            string query = @"SELECT 
                                ki.knowledgeItemID, 
                                ki.title, 
                                ki.KISubject, 
                                ki.category, 
                                ki.information, 
                                ki.KMDate, 
                                CONCAT(u.firstName, ' ', u.lastName) AS author
                            FROM 
                                KnowledgeItems ki
                            LEFT JOIN 
                                Author a ON ki.knowledgeItemID = a.knowledgeItemID
                            LEFT JOIN 
                                Users u ON a.userID = u.userID
                            WHERE 1=1"; // 1=1 (TRUE) added to allow for multiple AND statements to be added

            // Add conditions based on provided parameters
            if (!string.IsNullOrEmpty(author))
            {
                query += " AND CONCAT(u.firstName,' ',u.lastName) LIKE @author";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query += " AND ki.information LIKE @keyword";
            }
            if (!string.IsNullOrEmpty(category))
            {
                query += " AND ki.category LIKE @category";
            }

            MainDBconnection.Open();
            SqlCommand command = new SqlCommand(query, MainDBconnection);

            // Add parameter values if they exist
            if (!string.IsNullOrEmpty(author))
            {
                command.Parameters.AddWithValue("@author", "%" + author + "%");
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                command.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
            }
            if (!string.IsNullOrEmpty(category))
            {
                command.Parameters.AddWithValue("@category", "%" + category + "%");
            }

            // Execute the query and return the SqlDataReader
            return command.ExecuteReader();
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
        public static SqlDataReader MessageReader(int CollabID)
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT * FROM CollabChat WHERE collabID = @CollabID";
            cmdContentRead.Parameters.AddWithValue("@CollabID", CollabID);
            cmdContentRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdContentRead.ExecuteReader();

            return tempReader;
        }
        public static SqlDataReader CollabKnowledgeReader(int CollabID)
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT Assits.knowledgeItemID, KnowledgeItems.title, KnowledgeItems.KISubject FROM Assits " +
                "LEFT JOIN KnowledgeItems ON Assits.knowledgeItemID = KnowledgeItems.knowledgeItemID" +
                " WHERE collabID = @CollabID";
            cmdContentRead.Parameters.AddWithValue("@CollabID", CollabID);
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
        public static SqlDataReader CollabSWOTReader(int CollabID)
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT SWOT_Assits.swotID, SWOT.title, SWOT.strengths, SWOT.weaknesses, SWOT.opportunities, SWOT.threats, SWOT.category, SWOT.swotDATE FROM SWOT_Assits " +
                "LEFT JOIN SWOT ON SWOT_Assits.swotID = SWOT.swotID" +
                " WHERE collabID = @CollabID";
            cmdContentRead.Parameters.AddWithValue("@CollabID", CollabID);
            cmdContentRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdContentRead.ExecuteReader();

            return tempReader;
        }
        public static SqlDataReader AttributeFinder(int datasetID)
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT attributeID, attributeNAME FROM Attribute " +
                " WHERE datasetID = @datasetID";
            cmdContentRead.Parameters.AddWithValue("@datasetID", datasetID);
            cmdContentRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdContentRead.ExecuteReader();

            return tempReader;
        }
        public static int AttributeLength(int attributeID)
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT count(dataValue) as countData FROM AttributeValue " +
                " WHERE attributeID = @attributeID";
            cmdContentRead.Parameters.AddWithValue("@attributeID", attributeID);
            cmdContentRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdContentRead.ExecuteReader();
            if (tempReader.Read())
            {
                return (int)tempReader["countData"];
            }
            else return 0;
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
        public static int ExtractAttributeID(String column, int datasetID)
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT attributeID FROM Attribute " +
                "WHERE attributeNAME = @columnName " +
                "AND datasetID = @datasetID " +
                "ORDER BY datasetID DESC";
            cmdContentRead.Parameters.AddWithValue("@columnName", column);
            cmdContentRead.Parameters.AddWithValue("@datasetID", datasetID);

            cmdContentRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdContentRead.ExecuteReader();
            if (tempReader.Read())
            {
                return (int)tempReader["attributeID"];
            }
            else return 0;
        }
        public static SqlDataReader AttributeReader(int attributeID)
        {
            SqlCommand cmdContentRead = new SqlCommand();
            cmdContentRead.Connection = MainDBconnection;
            cmdContentRead.Connection.ConnectionString = MainDBconnString;
            cmdContentRead.CommandText = "SELECT dataValue FROM AttributeValue " +
                " WHERE attributeID = @attributeID";
            cmdContentRead.Parameters.AddWithValue("@attributeID", attributeID);
            cmdContentRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdContentRead.ExecuteReader();

            return tempReader;
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
        //function for expanding an entry
        public static string[] KnowledgeItemView(int KnowledgeID)
        {
            string Query = "SELECT * FROM KnowledgeItems WHERE knowledgeItemID = @KnowledgeID";
            List<string> field = new List<string>();
            SqlCommand cmdQuery = new SqlCommand();
            cmdQuery.Connection = MainDBconnection;
            cmdQuery.Connection.ConnectionString = MainDBconnString;
            cmdQuery.CommandText = Query;
            cmdQuery.Parameters.AddWithValue("@KnowledgeID", KnowledgeID);
            cmdQuery.Connection.Open();
            SqlDataReader result = cmdQuery.ExecuteReader();
            if (result.Read())
            {
                field.Add(result["title"].ToString());
                field.Add(result["KISubject"].ToString());
                field.Add(result["category"].ToString());
                field.Add(result["information"].ToString());
            }
            return field.ToArray();
        }
        //functions for adding values to databases

        public static void CreateKnowledgeItem(MadisonCountyCollaborationApplication.Pages.DataClasses.KnowledgeItems ki)
        {
            String sqlQuery = "INSERT INTO KnowledgeItems (title, KISubject, category, information, KMDate) VALUES('";
            sqlQuery += ki.title + "','";
            sqlQuery += ki.KISubject + "','";
            sqlQuery += ki.category + "','";
            sqlQuery += ki.information + "','";
            sqlQuery += ki.KMDate + "')";

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.CommandText = sqlQuery;
            cmdProductRead.Connection.Open();
            cmdProductRead.ExecuteNonQuery();
        }

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
        public static void CreateAttribute(String column, int datasetID)
        {
            String sqlQuery = "INSERT INTO Attribute (datasetID, attributeName) VALUES('";
            sqlQuery += datasetID + "','";
            sqlQuery += column + "')";

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.CommandText = sqlQuery;
            cmdProductRead.Connection.Open();
            cmdProductRead.ExecuteNonQuery();
        }

        public static void CreateAttributeValue(String dataValue, int attributeID)
        {
            String sqlQuery = "INSERT INTO AttributeValue (attributeID, dataValue) VALUES('";
            sqlQuery += attributeID + "','";
            sqlQuery += dataValue + "')";

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.CommandText = sqlQuery;
            cmdProductRead.Connection.Open();
            cmdProductRead.ExecuteNonQuery();
        }

        public static void CreateSWOTItem(MadisonCountyCollaborationApplication.Pages.DataClasses.SWOT SWOTItem)
        {

            String sqlQuery = "INSERT INTO SWOT (title, strengths, weaknesses, opportunities, threats, category, swotDate) VALUES ('";
            sqlQuery += SWOTItem.title + "','";
            sqlQuery += SWOTItem.strengths + "','";
            sqlQuery += SWOTItem.weaknesses + "','";
            sqlQuery += SWOTItem.opportunities + "','";
            sqlQuery += SWOTItem.threats + "','";
            sqlQuery += SWOTItem.category + "','";
            sqlQuery += SWOTItem.swotDate + "')";

            SqlCommand cmdSWOTInsert = new SqlCommand();
            cmdSWOTInsert.Connection = MainDBconnection; // Assuming Lab3DBConnection is a predefined SqlConnection object
                                                         // cmdSWOTInsert.Connection.ConnectionString = Lab3DBConnString; // This line might be redundant if the connection is already established with a connection string
            cmdSWOTInsert.CommandText = sqlQuery;
            cmdSWOTInsert.Connection.Open();
            cmdSWOTInsert.ExecuteNonQuery();
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
        public static void CreateMessage(MadisonCountyCollaborationApplication.Pages.DataClasses.CollabChat Chat)
        {
            String sqlQuery = "INSERT INTO CollabChat (messageInfo, messageTime, userID, collabID) VALUES('";
            sqlQuery += Chat.messageInfo + "','";
            sqlQuery += Chat.messageTime + "','";
            sqlQuery += Chat.userID + "','";
            sqlQuery += Chat.collabID + "')";

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = MainDBconnection;
            cmdProductRead.Connection.ConnectionString = MainDBconnString;
            cmdProductRead.CommandText = sqlQuery;
            cmdProductRead.Connection.Open();
            cmdProductRead.ExecuteNonQuery();
        }
        public static void CreateAssist(MadisonCountyCollaborationApplication.Pages.DataClasses.Assists Assist)
        {
            String sqlQuery = "INSERT INTO Assits (knowledgeItemID, collabID) VALUES('";
            sqlQuery += Assist.knowledgeID + "','";
            sqlQuery += Assist.collabID + "')";

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
        public static void CreateSWOTAssist(MadisonCountyCollaborationApplication.Pages.DataClasses.SWOT_Assists Assist)
        {
            String sqlQuery = "INSERT INTO SWOT_Assits (swotID, collabID) VALUES('";
            sqlQuery += Assist.swotID + "','";
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
        public static string[] CollabReportView(int collabID)
        {
            string Query = @"SELECT collabName, notesAndInfo, dateCreated
                             FROM Collaboration
                             WHERE collabID = @collabID";
            List<string> field = new List<string>();
            SqlCommand cmdQuery = new SqlCommand();
            cmdQuery.Connection = MainDBconnection;
            cmdQuery.Connection.ConnectionString = MainDBconnString;
            cmdQuery.CommandText = Query;
            cmdQuery.Parameters.AddWithValue("@collabID", collabID);
            cmdQuery.Connection.Open();
            SqlDataReader result = cmdQuery.ExecuteReader();
            if (result.Read())
            {
                field.Add(result["collabName"].ToString());
                field.Add(result["notesAndInfo"].ToString());
                field.Add(result["dateCreated"].ToString());
            }
            return field.ToArray();
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



        public static SqlDataReader GetKnowledgeItemsFromCollabReader(int collabID)
        {
            SqlCommand cmdQuery = new SqlCommand();
            cmdQuery.Connection = MainDBconnection;
            cmdQuery.Connection.ConnectionString = MainDBconnString;
            cmdQuery.CommandText = @"SELECT 
                                ki.knowledgeItemID, ki.title, ki.KISubject, ki.category, ki.information, ki.KMDate
                             FROM 
                                KnowledgeItems ki, Assits a, Collaboration c
                             WHERE 
                                ki.knowledgeItemID = a.knowledgeItemID AND c.collabID = a.collabID AND c.collabID = @collabID";
            cmdQuery.Parameters.AddWithValue("@collabID", collabID);

            cmdQuery.Connection.Open();

            SqlDataReader resultReader = cmdQuery.ExecuteReader();

            return resultReader;
        }

        public static string[] GetUserForReportGenerator(string userName)
        {
            string Query = @"SELECT 
                                firstName, lastName
                             FROM 
                                Users
                             WHERE
                                userName = @userName;";
            List<string> field = new List<string>();
            SqlCommand cmdQuery = new SqlCommand();
            cmdQuery.Connection = MainDBconnection;
            cmdQuery.Connection.ConnectionString = MainDBconnString;
            cmdQuery.CommandText = Query;
            cmdQuery.Parameters.AddWithValue("@userName", userName);
            cmdQuery.Connection.Open();
            SqlDataReader result = cmdQuery.ExecuteReader();
            if (result.Read())
            {
                field.Add(result["firstName"].ToString());
                field.Add(result["lastName"].ToString());
            }
            return field.ToArray();
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
