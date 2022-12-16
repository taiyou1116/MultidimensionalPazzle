using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SQLiteUnity;
using SimpleJSON;

public class SQLiteDB : MonoBehaviour
{
    private string path = "";
    private string databaseFileName = "instanceDB.db";
    private SQLite sqlite;

    // originIDを見比べて同じだった場合はSQLiteDBから 他の場合はOLINEから取得
    public void Start()
    {
        CreateDB();
    }

    private void CreateDB()
    {
        var createTableQuery = $@"
        CREATE TABLE stages
        (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name STRING,
            data TEXT,
            image TEXT,
            originID INTEGER,
            version INTEGER
        );
        ";

        #if UNITY_EDITOR
            path = Application.streamingAssetsPath;
        #else
            path = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
        #endif
            sqlite = new SQLite(databaseFileName, createTableQuery, path);
    }

    public void InsertData(string sname, string sdata, string simage, int originID)
    {
        // id
        // name
        // data
        // image
        // originID
        for (int i = 0; i < sname.Length; i++) {
            sqlite.ExecuteQuery(string.Format("INSERT INTO `stages` VALUES (null, '{0}', '{1}', '{2}', '{3}', '{4}')", sname, sdata, simage, originID, 0));
        }
    }

    private void UpdateData()
    {
        // UPDATE書き方
        sqlite.ExecuteQuery(string.Format("UPDATE stages SET data = 'updated' WHERE name = 'sayoko_takayama'"));
    }

    private void DeleteData()
    {
        // DELETE書き方
        sqlite.ExecuteQuery(string.Format("DELETE FROM stages WHERE name = 'kaede_kujo'"));
    }

    public void GetSQLiteDBData(GameObject parent)
    {
        string selectQuery = "SELECT name, data FROM `stages`";
        SQLiteTable result = sqlite.ExecuteQuery(selectQuery);
        foreach (var value in result) {
            GameObject stageG = Instantiate(Resources.Load("Prefabs/stages") as GameObject);
            stageG.transform.SetParent(parent.transform);
            stageG.transform.localScale = Vector3.one;
            stageG.transform.localPosition = Vector3.zero;
        }
        
    }
}
