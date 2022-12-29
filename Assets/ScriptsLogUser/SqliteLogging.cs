using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class SqliteLogging : MonoBehaviour
{
    // private IDbConnection CreateAndOpenDatabase()
    void Start()
    {
        // Open a connection to the database.
        string path_to_db = "URI=file:" + Application.dataPath + "/experiment_log.db";
        Debug.Log("Database: Creating tables at " + path_to_db);

        // Open db connection
        IDbConnection dbConnection = new SqliteConnection(path_to_db); 
        dbConnection.Open();

        // Create a table for the end user
        IDbCommand dbCommandCreateTableEndUser = dbConnection.CreateCommand();
        dbCommandCreateTableEndUser.CommandText = "CREATE TABLE IF NOT EXISTS EndUser (participant_id INTEGER PRIMARY KEY, experiment_id INTEGER )";
        IDataReader reader_enduser = dbCommandCreateTableEndUser.ExecuteReader();

        // Create a table for the positions
        IDbCommand dbCommandCreateTablePositions = dbConnection.CreateCommand();
        dbCommandCreateTablePositions.CommandText = "CREATE TABLE IF NOT EXISTS Positions (id INTEGER PRIMARY KEY, experiment_id INTEGER NOT NULL, position_x REAL NOT NULL, position_y REAL NOT NULL, position_z REAL NOT NULL, i INTEGER )";
        dbCommandCreateTablePositions.ExecuteReader();

        // Create a table for the rotations
        IDbCommand dbCommandCreateTableRotations = dbConnection.CreateCommand();
        dbCommandCreateTableRotations.CommandText = "CREATE TABLE IF NOT EXISTS Rotations (id INTEGER PRIMARY KEY, experiment_id INTEGER NOT NULL, w REAL NOT NULL, x REAL NOT NULL, y REAL NOT NULL, z REAL NOT NULL, i INTEGER )";
        dbCommandCreateTableRotations.ExecuteReader();

        // Create a table for the rotations
        IDbCommand dbCommandCreateTablePictures = dbConnection.CreateCommand();
        dbCommandCreateTablePictures.CommandText = "CREATE TABLE IF NOT EXISTS Pictures (id INTEGER PRIMARY KEY, participant_id INTEGER NOT NULL, environment_id INTEGER NOT NULL, filepath STRING NOT NULL)";
        dbCommandCreateTablePictures.ExecuteReader();

        reader_enduser.Close();
        reader_enduser = null;

        dbCommandCreateTableEndUser.Dispose();
        dbCommandCreateTableEndUser = null;

        dbCommandCreateTablePositions.Dispose();
        dbCommandCreateTablePositions = null;

        dbCommandCreateTableRotations.Dispose();
        dbCommandCreateTableRotations = null;
        
        dbConnection.Close();
        dbConnection = null;
    }

    public void createUserEnvironment(int participant_id, int experiment_id) {
        string path_to_db = "URI=file:" + Application.dataPath + "/experiment_log.db";
        Debug.Log("Database: Storing user id: " + participant_id + " and environment id: " + experiment_id);

        IDbConnection dbConnection = new SqliteConnection(path_to_db); 
        dbConnection.Open();

        IDbCommand dbCommandStoreUserAndEnvironment = dbConnection.CreateCommand();

        IDbDataParameter param1 = dbCommandStoreUserAndEnvironment.CreateParameter();
        IDbDataParameter param2 = dbCommandStoreUserAndEnvironment.CreateParameter();

        param1.ParameterName = "@v1";
        param1.Value = participant_id;

        param2.ParameterName = "@v2";
        param2.Value = experiment_id;

        dbCommandStoreUserAndEnvironment.Parameters.Add(param1);
        dbCommandStoreUserAndEnvironment.Parameters.Add(param2);

        dbCommandStoreUserAndEnvironment.CommandText = "INSERT INTO EndUser (participant_id ,experiment_id) VALUES(@v1, @v2)";
        IDataReader reader = dbCommandStoreUserAndEnvironment.ExecuteReader();

        reader.Close();
        reader = null;
        dbCommandStoreUserAndEnvironment.Dispose();
        dbCommandStoreUserAndEnvironment = null;
        dbConnection.Close();
        dbConnection = null;
    }

    public void storeUserPositionQueue(int user_id, int experiment_id, Queue<ReplayData> recordingQueue) {
        string path_to_db = "URI=file:" + Application.dataPath + "/experiment_log.db";
        // Debug.Log("Database: Storing user id: " + user_id + " and environment id: " + experiment_id);

        
        IDbConnection dbConnection = new SqliteConnection(path_to_db); 
        dbConnection.Open();
        int i = 0;
        foreach (ReplayData data in recordingQueue)
        {


            IDbCommand dbCommandStoreUserAndEnvironment = dbConnection.CreateCommand();


            IDbDataParameter param1 = dbCommandStoreUserAndEnvironment.CreateParameter();
            IDbDataParameter param2 = dbCommandStoreUserAndEnvironment.CreateParameter();
            IDbDataParameter param3 = dbCommandStoreUserAndEnvironment.CreateParameter();
            IDbDataParameter param4 = dbCommandStoreUserAndEnvironment.CreateParameter();
            IDbDataParameter param5 = dbCommandStoreUserAndEnvironment.CreateParameter();

            param1.ParameterName = "@v1";
            param1.Value = experiment_id;

            param2.ParameterName = "@v2";
            param2.Value = data.position.x;

            param3.ParameterName = "@v3";
            param3.Value = data.position.y;

            param4.ParameterName = "@v4";
            param4.Value = data.position.z;

            param5.ParameterName = "@v5";
            param5.Value = i;

            dbCommandStoreUserAndEnvironment.Parameters.Add(param1);
            dbCommandStoreUserAndEnvironment.Parameters.Add(param2);
            dbCommandStoreUserAndEnvironment.Parameters.Add(param3);
            dbCommandStoreUserAndEnvironment.Parameters.Add(param4);
            dbCommandStoreUserAndEnvironment.Parameters.Add(param5);

            dbCommandStoreUserAndEnvironment.CommandText = "INSERT INTO Positions (experiment_id, position_x, position_y, position_z, i) VALUES(@v1, @v2, @v3, @v4, @v5)";
            // " + experiment_id + ", " + data.position.x + ", " + data.position.y + ", " + data.position.z + ", " + i + "

            IDataReader reader = dbCommandStoreUserAndEnvironment.ExecuteReader();

            reader.Close();
            reader = null;
            dbCommandStoreUserAndEnvironment.Dispose();
            dbCommandStoreUserAndEnvironment = null;
            
            i++;
        }
        dbConnection.Close();
        dbConnection = null;
        // dbCommandStoreUserAndEnvironment.Dispose();
        // dbCommandStoreUserAndEnvironment = null;
        
    }

    public void storeUserPosition(int user_id, int experiment_id, ReplayData data, int i) {
        string path_to_db = "URI=file:" + Application.dataPath + "/experiment_log.db";
        // Debug.Log("Database: Storing position of user : " + user_id + " and environment id: " + experiment_id);

        
        IDbConnection dbConnection = new SqliteConnection(path_to_db); 
        dbConnection.Open();

        IDbCommand dbCommandStoreUserAndEnvironment = dbConnection.CreateCommand();

        IDbDataParameter param1 = dbCommandStoreUserAndEnvironment.CreateParameter();
        IDbDataParameter param2 = dbCommandStoreUserAndEnvironment.CreateParameter();
        IDbDataParameter param3 = dbCommandStoreUserAndEnvironment.CreateParameter();
        IDbDataParameter param4 = dbCommandStoreUserAndEnvironment.CreateParameter();
        IDbDataParameter param5 = dbCommandStoreUserAndEnvironment.CreateParameter();

        param1.ParameterName = "@v1";
        param1.Value = experiment_id;

        param2.ParameterName = "@v2";
        param2.Value = data.position.x;

        param3.ParameterName = "@v3";
        param3.Value = data.position.y;

        param4.ParameterName = "@v4";
        param4.Value = data.position.z;

        param5.ParameterName = "@v5";
        param5.Value = i;

        dbCommandStoreUserAndEnvironment.Parameters.Add(param1);
        dbCommandStoreUserAndEnvironment.Parameters.Add(param2);
        dbCommandStoreUserAndEnvironment.Parameters.Add(param3);
        dbCommandStoreUserAndEnvironment.Parameters.Add(param4);
        dbCommandStoreUserAndEnvironment.Parameters.Add(param5);

        // dbCommandStoreUserAndEnvironment.CommandText = "INSERT INTO Positions (experiment_id, position_x, position_y, position_z, i) VALUES(" + experiment_id + ", " + data.position.x + ", " + data.position.y + ", " + data.position.z + ", " + i + ")";
        dbCommandStoreUserAndEnvironment.CommandText = "INSERT INTO Positions (experiment_id, position_x, position_y, position_z, i) VALUES(@v1, @v2, @v3, @v4, @v5)";
        // " + experiment_id + ", " + data.position.x + ", " + data.position.y + ", " + data.position.z + ", " + i + "

        IDataReader reader = dbCommandStoreUserAndEnvironment.ExecuteReader();

        reader.Close();
        reader = null;
        dbCommandStoreUserAndEnvironment.Dispose();
        dbCommandStoreUserAndEnvironment = null;
            
        dbConnection.Close();
        dbConnection = null;
        // dbCommandStoreUserAndEnvironment.Dispose();
        // dbCommandStoreUserAndEnvironment = null;
        
    }

    public void storeUserRotation(int user_id, int experiment_id, ReplayData data, int i) {
        string path_to_db = "URI=file:" + Application.dataPath + "/experiment_log.db";
        // Debug.Log("Database: Storing user id: " + user_id + " and environment id: " + experiment_id);

        
        IDbConnection dbConnection = new SqliteConnection(path_to_db); 
        dbConnection.Open();


        IDbCommand dbCommandStoreUserAndEnvironment = dbConnection.CreateCommand();


        IDbDataParameter param1 = dbCommandStoreUserAndEnvironment.CreateParameter();
        IDbDataParameter param2 = dbCommandStoreUserAndEnvironment.CreateParameter();
        IDbDataParameter param3 = dbCommandStoreUserAndEnvironment.CreateParameter();
        IDbDataParameter param4 = dbCommandStoreUserAndEnvironment.CreateParameter();
        IDbDataParameter param5 = dbCommandStoreUserAndEnvironment.CreateParameter();
        IDbDataParameter param6 = dbCommandStoreUserAndEnvironment.CreateParameter();


        param1.ParameterName = "@v1";
        param1.Value = experiment_id;

        param2.ParameterName = "@v2";
        param2.Value = data.rotation.w;

        param3.ParameterName = "@v3";
        param3.Value = data.rotation.x;

        param4.ParameterName = "@v4";
        param4.Value = data.rotation.y;

        param5.ParameterName = "@v5";
        param5.Value = data.rotation.z;

        param6.ParameterName = "@v6";
        param6.Value = i;

        dbCommandStoreUserAndEnvironment.Parameters.Add(param1);
        dbCommandStoreUserAndEnvironment.Parameters.Add(param2);
        dbCommandStoreUserAndEnvironment.Parameters.Add(param3);
        dbCommandStoreUserAndEnvironment.Parameters.Add(param4);
        dbCommandStoreUserAndEnvironment.Parameters.Add(param5);
        dbCommandStoreUserAndEnvironment.Parameters.Add(param6);

        dbCommandStoreUserAndEnvironment.CommandText = "INSERT INTO Rotations (experiment_id, w, x, y, z, i) VALUES(@v1, @v2, @v3, @v4, @v5, @v6)";
        // " + experiment_id + ", " + data.rotation.w+ ", " + data.rotation.x + ", " + data.rotation.y + ", " + data.rotation.z + ", " + i + "
        IDataReader reader = dbCommandStoreUserAndEnvironment.ExecuteReader();

        reader.Close();
        reader = null;
        dbCommandStoreUserAndEnvironment.Dispose();
        dbCommandStoreUserAndEnvironment = null;
            
        dbConnection.Close();
        dbConnection = null;
        // dbCommandStoreUserAndEnvironment.Dispose();
        // dbCommandStoreUserAndEnvironment = null;
        
    }

    public void getUserPosition(int user_id, int experiment_id) {
        string path_to_db = "URI=file:" + Application.dataPath + "/experiment_log.db";
        // Debug.Log("Database: Storing position of user : " + user_id + " and environment id: " + experiment_id);

        
        IDbConnection dbConnection = new SqliteConnection(path_to_db); 
        dbConnection.Open();


        IDbCommand dbCommandStoreUserAndEnvironment = dbConnection.CreateCommand();
        dbCommandStoreUserAndEnvironment.CommandText = "SELECT * FROM Positions WHERE experiment_id='" + experiment_id + "'";
        IDataReader reader = dbCommandStoreUserAndEnvironment.ExecuteReader();

        object[] dataRow = new object[reader.FieldCount];
        while (reader.Read()) { 
            int cols = reader.GetValues(dataRow); 
            for (int i = 0; i < cols; i++) {
                Debug.Log(dataRow[i]);
            }
        }
        reader.Close();

        reader.Close();
        reader = null;
        dbCommandStoreUserAndEnvironment.Dispose();
        dbCommandStoreUserAndEnvironment = null;
            
        dbConnection.Close();
        dbConnection = null;
    }

    public void storePicture(int participant_id, int environment_id, string filepath) {
        string path_to_db = "URI=file:" + Application.dataPath + "/experiment_log.db";

        //  (id INTEGER PRIMARY KEY, participant_id INTEGER , environment_id INTEGER,  filepath TEXT NOT NULL        
        IDbConnection dbConnection = new SqliteConnection(path_to_db); 
        dbConnection.Open();

        IDbCommand dbCommandStoreUserAndEnvironment = dbConnection.CreateCommand();

        IDbDataParameter param1 = dbCommandStoreUserAndEnvironment.CreateParameter();
        IDbDataParameter param2 = dbCommandStoreUserAndEnvironment.CreateParameter();
        IDbDataParameter param3 = dbCommandStoreUserAndEnvironment.CreateParameter();

        param1.ParameterName = "@v1";
        param1.Value = participant_id;

        param2.ParameterName = "@v2";
        param2.Value = environment_id;

        param3.ParameterName = "@v3";
        param3.Value = filepath;

        dbCommandStoreUserAndEnvironment.Parameters.Add(param1);
        dbCommandStoreUserAndEnvironment.Parameters.Add(param2);
        dbCommandStoreUserAndEnvironment.Parameters.Add(param3);

        dbCommandStoreUserAndEnvironment.CommandText = "INSERT INTO Pictures (participant_id, environment_id, filepath) VALUES(@v1, @v2, @v3)";

        IDataReader reader = dbCommandStoreUserAndEnvironment.ExecuteReader();

        reader.Close();
        reader = null;
        dbCommandStoreUserAndEnvironment.Dispose();
        dbCommandStoreUserAndEnvironment = null;
            
        dbConnection.Close();
        dbConnection = null;   
    }

    public object getCountPictureByUserInEnvironment(int participant_id) { //, int environment_id
        string path_to_db = "URI=file:" + Application.dataPath + "/experiment_log.db";

        IDbConnection dbConnection = new SqliteConnection(path_to_db); 
        dbConnection.Open();


        IDbCommand dbCommandStoreUserAndEnvironment = dbConnection.CreateCommand();
        // Debug.Log("SELECT COUNT(*) FROM Pictures WHERE participant_id='" + participant_id + "' AND environment_id='" + environment_id + "'");
        dbCommandStoreUserAndEnvironment.CommandText = "SELECT COUNT(*) FROM Pictures WHERE participant_id='" + participant_id + "'"; //"' AND environment_id='" + environment_id +
        IDataReader reader = dbCommandStoreUserAndEnvironment.ExecuteReader();

        object[] dataRow = new object[reader.FieldCount];        
        object count = 0;
        while (reader.Read()) {


            int cols = reader.GetValues(dataRow);
            count = dataRow[0];
        }
        Debug.Log(count);
        reader.Close();

        reader.Close();

        dbConnection.Close();
        dbConnection = null;



        Debug.Log("User: " + participant_id + " has taken " + count + " pictures");       
        return count;
    }
}
