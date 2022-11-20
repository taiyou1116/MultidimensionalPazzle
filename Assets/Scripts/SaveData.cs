using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public bool[] stageClears;
    public GameObject[] stars;
    /*void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        stageClears = new bool[25];
        //PlayerPrefs.SetString("Save","first");
        ShowStar();
    }*/
    public void ClearStage(int num)
    {
        stageClears[num] = true;
        Save();
    }
    public void ShowStar()
    {
        Load();
        for(int i = 0; i < stageClears.Length; i++)
        {
            if(stageClears[i]) stars[i].SetActive(true);
        }
    }
    private void Save()
    {
        SaveInfo saveInfo = new SaveInfo();
        saveInfo.stageClears = stageClears;
        string json = JsonUtility.ToJson(saveInfo);
        PlayerPrefs.SetString("Save",json);
        PlayerPrefs.Save();
    }
    private void Load()
    {
        if(PlayerPrefs.GetString("Save","first") == "first") return;
        SaveInfo savedata = SaveInfo.CreateFromJson(PlayerPrefs.GetString("Save","first"));
        stageClears = savedata.stageClears;
    }
}
[System.Serializable]
public class SaveInfo
{
    public bool[] stageClears = new bool[25];
    public static SaveInfo CreateFromJson(string jsonString)
    {
        return JsonUtility.FromJson<SaveInfo>(jsonString);
    }
}
