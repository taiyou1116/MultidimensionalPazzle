using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SQLiteUnity;

public class SQLiteDB : MonoBehaviour
{
    private string path = "";
    private string databaseFileName = "instanceDB.db";
    private SQLite sqlite;
    private GameObject objParent;
    private List<GameObject> dlStageList = new List<GameObject>();

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
        // id, name, data, image, originID
        sqlite.ExecuteQuery(string.Format("INSERT INTO `stages` VALUES (null, '{0}', '{1}', '{2}', '{3}', '{4}')", sname, sdata, simage, originID, 0));
    }

    private void UpdateData()
    {
        // UPDATE書き方
        sqlite.ExecuteQuery(string.Format("UPDATE stages SET data = 'updated' WHERE name = 'sayoko_takayama'"));
    }

    public void DeleteData(int id)
    {
        // DELETE書き方
        sqlite.ExecuteQuery(string.Format("DELETE FROM stages WHERE originID = '{0}'", id));

        GetSQLiteDBData(objParent);
    }

    public void GetSQLiteDBData(GameObject parent)
    {
        if (!objParent) objParent = parent;
        
        foreach (var value in dlStageList) {
            Destroy(value);
        }
        dlStageList = new List<GameObject>();
        string selectQuery = "SELECT name, data, image, originID FROM `stages`";
        SQLiteTable result = sqlite.ExecuteQuery(selectQuery);

        foreach (SQLiteRow dr in result.Rows) {
            GameObject stageG = Instantiate(Resources.Load("Prefabs/DLstages") as GameObject);
            stageG.transform.SetParent(parent.transform);
            stageG.transform.localScale = Vector3.one;
            stageG.transform.localPosition = Vector3.zero;

            stageG.transform.Find("Name").GetComponent<Text>().text = (string)dr["name"];
            stageG.GetComponent<SetStageFromSQLite>().stageName = (string)dr["name"];
            stageG.GetComponent<SetStageFromSQLite>().stageData = (string)dr["data"];
            stageG.GetComponent<SetStageFromSQLite>().originID = (int)dr["originID"];

            byte[] bytes = System.Convert.FromBase64String((string)dr["image"]);
            Texture2D texture = new Texture2D(2,2);
            texture.LoadImage(bytes);
            Image image = stageG.transform.Find("Image").GetComponent<Image>();
            image.sprite = Sprite.Create(texture,new Rect(0,0,texture.width, texture.height), Vector2.zero);

            dlStageList.Add(stageG);
        }
    }
}
