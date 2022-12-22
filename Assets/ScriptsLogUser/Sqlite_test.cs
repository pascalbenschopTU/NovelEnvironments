using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class Sqlite_test : MonoBehaviour
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
        // IDbCommand dbCommandCreateTable = dbConnection.CreateCommand();
        // dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS Positions (id INTEGER PRIMARY KEY, experiment_id INTEGER NOT NULL, position_x REAL NOT NULL, position_y REAL NOT NULL, position_z REAL NOT NULL, index INTEGER )";
        // dbCommandCreateTable.ExecuteReader();


        reader_enduser.Close();
        reader_enduser = null;
        dbCommandCreateTableEndUser.Dispose();
        dbCommandCreateTableEndUser = null;
        dbConnection.Close();
        dbConnection = null;

        createUserEnvironment(11, 11);
        // return dbConnection;
    }

    public void createUserEnvironment(int user_id, int experiment_id) {
        string path_to_db = "URI=file:" + Application.dataPath + "/experiment_log.db";
        Debug.Log("Database: Storing user id: " + user_id + " and environment id: " + experiment_id);

        IDbConnection dbConnection = new SqliteConnection(path_to_db); 
        dbConnection.Open();

        IDbCommand dbCommandStoreUserAndEnvironment = dbConnection.CreateCommand();
        dbCommandStoreUserAndEnvironment.CommandText = "INSERT INTO EndUser (participant_id ,experiment_id) VALUES(" + user_id + ", " + experiment_id + ")";
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
        Debug.Log("Database: Storing user id: " + user_id + " and environment id: " + experiment_id);

        
        IDbConnection dbConnection = new SqliteConnection(path_to_db); 
        dbConnection.Open();
        int i = 0;
        foreach (ReplayData data in recordingQueue)
        {


            IDbCommand dbCommandStoreUserAndEnvironment = dbConnection.CreateCommand();
            dbCommandStoreUserAndEnvironment.CommandText = "INSERT INTO Positions (experiment_id, position_x, position_y, position_z, i) VALUES(" + experiment_id + ", " + data.position.x + ", " + data.position.y + ", " + data.position.z + ", " + i + ")";
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

    public void storeUserPosition(int user_id, int experiment_id, ReplayData data, int index) {
        string path_to_db = "URI=file:" + Application.dataPath + "/experiment_log.db";
        Debug.Log("Database: Storing position of user : " + user_id + " and environment id: " + experiment_id);

        
        IDbConnection dbConnection = new SqliteConnection(path_to_db); 
        dbConnection.Open();


        IDbCommand dbCommandStoreUserAndEnvironment = dbConnection.CreateCommand();
        dbCommandStoreUserAndEnvironment.CommandText = "INSERT INTO Positions (experiment_id, position_x, position_y, position_z, i) VALUES(" + experiment_id + ", " + data.position.x + ", " + data.position.y + ", " + data.position.z + ", " + index + ")";
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
        Debug.Log("Database: Storing user id: " + user_id + " and environment id: " + experiment_id);

        
        IDbConnection dbConnection = new SqliteConnection(path_to_db); 
        dbConnection.Open();


        IDbCommand dbCommandStoreUserAndEnvironment = dbConnection.CreateCommand();
        dbCommandStoreUserAndEnvironment.CommandText = "INSERT INTO Rotations (experiment_id, w, x, y, z, i) VALUES(" + experiment_id + ", " + data.rotation.w+ ", " + data.rotation.x + ", " + data.rotation.y + ", " + data.rotation.z + ", " + i + ")";
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
}
