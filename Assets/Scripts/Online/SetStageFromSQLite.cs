using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SetStageFromSQLite : MonoBehaviour
{
    public int originID{get; set;}
    public string stageData{private get; set;}
    public string stageName{get; set;}
    private Button startButton;
    private Button deleteButton;

    void Start()
    {
        startButton = GetComponent<Button>();
        deleteButton = transform.GetChild(5).GetComponent<Button>();
        
        startButton.onClick.AddListener(() => {
            PlayerPrefs.SetString("MODE", "ONLINE");
            MainForOnline.Instance.web.stageName = stageName;
            ReadInStageData();
        });
        deleteButton.onClick.AddListener(() => {
            SQLiteDB sQLiteDB = GameObject.Find("SQLite").GetComponent<SQLiteDB>();
            sQLiteDB.DeleteData(originID);
        });
    }

    private void ReadInStageData()
    {
        string rows = "";
        string path = Application.dataPath + "/test.txt";
        StreamWriter sw = new StreamWriter(path,false);
        
        int count = 0;
        int layerCount = 0;
        int fiishCount = 0;

        for(int i = 0; i < stageData.Length; i++) {
            rows += stageData[i] + ",";
            count++;
            if (count == 9 && layerCount == 8 && fiishCount == 5) {
                string result = rows.Substring(0, rows.Length - 1);
                sw.Write(result + "\n");
            }
            else if (layerCount == 8 && count == 9) {
                //i % 8 == 0 && i != 0
                string result = rows.Substring(0, rows.Length - 1) + ".";
                sw.Write(result + "\n");// ファイルに書き出したあと改行
                rows = "";

                count = 0;
                layerCount = 0;
                fiishCount++;
            }
            else if (count == 9) {
                string result = rows.Substring(0, rows.Length - 1);
                sw.WriteLine(result);// ファイルに書き出したあと改行
                rows = "";

                count = 0;
                layerCount++;
            }
        }
        sw.Close();
        FadeManager.Instance.LoadScene("MainScene",1);
    }
}
