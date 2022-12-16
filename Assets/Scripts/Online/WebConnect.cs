using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using System.IO;

public class WebConnect : MonoBehaviour
{
    private TitleUIManager titleUI;
    private AudioManager audiom;
    private MainUI mainUI;
    private string playerID;
    private int playStageID = 0;
    private string stageData;
    public string stageName;
    private List<GameObject> bgList = new List<GameObject>();
    private Dictionary<int, string> stageNameDic = new Dictionary<int, string>();

    // Sceneをまなぐため
    public void Initialize()
    {
        titleUI = MainForOnline.Instance.title;
        if (titleUI != null) {
            audiom = titleUI.audiom;
        }
        
        mainUI = MainForOnline.Instance.mainUI;
        
        Invoke("CaptureScreenshot",1f);
        
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainScene") {
            if (playStageID != 0) {
                StartCoroutine(UpdatePlayCount());
            }
        } else playStageID = 0;
    }
    private void CaptureScreenshot()
    {
        ScreenCapture.CaptureScreenshot(Application.dataPath + "/savedata.PNG",1);
    }

    public IEnumerator Login(string username, string password)
    {
        titleUI.connectWebPanel.SetActive(true);
        audiom.sounds[1].Play();

        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);

        using (UnityWebRequest www = UnityWebRequest.Post("http://taiyouserver.php.xdomain.jp/Login.php", form))
        {
            yield return www.SendWebRequest();

            titleUI.connectWebPanel.SetActive(false);

            if (www.result != UnityWebRequest.Result.Success) {
                titleUI.errorPanel.SetActive(true);
                titleUI.errorPanels[0].SetActive(true);
            }
            else {
                if(www.downloadHandler.text.Contains("Wrong Credentionals") || www.downloadHandler.text.Contains("Username does not exists")) {
                    titleUI.errorPanel.SetActive(true);
                    titleUI.errorPanels[1].SetActive(true);
                    Debug.Log("Try Again.");
                }
                else {
                    //if we logged in correctly
                    MainForOnline.Instance.login.gameObject.SetActive(false);
                    MainForOnline.Instance.onlinePanel.SetActive(true);

                    playerID = www.downloadHandler.text;
                }
            }
        }
    }

    public IEnumerator ResisterUser(string username, string password)
    {
        titleUI.connectWebPanel.SetActive(true);
        audiom.sounds[1].Play();

        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);

        using (UnityWebRequest www = UnityWebRequest.Post("http://taiyouserver.php.xdomain.jp/ResisterUser.php", form))
        {
            yield return www.SendWebRequest();

            titleUI.connectWebPanel.SetActive(false);

            if (www.result != UnityWebRequest.Result.Success) {
                titleUI.errorPanel.SetActive(true);
                titleUI.errorPanels[0].SetActive(true);
            }
            else {
                string result = www.downloadHandler.text;

                if (result == "Username is already taken") {
                    titleUI.errorPanel.SetActive(true);
                    titleUI.newUserErrorPanel[2].SetActive(true);

                } else {
                    MainForOnline.Instance.resister.gameObject.SetActive(false);
                    MainForOnline.Instance.onlinePanel.SetActive(true);
                    playerID = username;
                }
            }
        }
    }
    
    public IEnumerator SendData(string stageName, string data)
    {
        mainUI.loadPanel.SetActive(true);
        Sounds.instance.se[0].Play();

        WWWForm form = new WWWForm();
        
        form.AddField("stageName", stageName);
        form.AddField("data", data);
        form.AddField("playerID", playerID);

        string path = Application.dataPath + "/savedata.png";
        
        byte[] bytes = System.IO.File.ReadAllBytes(path);
        string encode = System.Convert.ToBase64String(bytes);
        
        form.AddField("image", encode);
        
        using (UnityWebRequest www = UnityWebRequest.Post("http://taiyouserver.php.xdomain.jp/SendData.php", form))
        {
            yield return www.SendWebRequest();

            mainUI.loadPanel.SetActive(false);

            if (www.result != UnityWebRequest.Result.Success) {
                mainUI.errorPanel.SetActive(true);
            }
            else {
                mainUI.uploadedPanel.SetActive(true);
                string result = www.downloadHandler.text;
            }
        }
    }

    public IEnumerator ReSendData(string data)
    {
        mainUI.loadPanel.SetActive(true);
        Sounds.instance.se[0].Play();

        WWWForm form = new WWWForm();
        
        form.AddField("data", data);
        form.AddField("stageID", playStageID);

        string path = Application.dataPath + "/savedata.png";
        
        byte[] bytes = System.IO.File.ReadAllBytes(path);
        string encode = System.Convert.ToBase64String(bytes);
        
        form.AddField("image", encode);
        
        using (UnityWebRequest www = UnityWebRequest.Post("http://taiyouserver.php.xdomain.jp/ReSendData.php", form))
        {
            yield return www.SendWebRequest();

            mainUI.loadPanel.SetActive(false);

            if (www.result != UnityWebRequest.Result.Success) {
                mainUI.errorPanel.SetActive(true);
            }
            else {
                mainUI.uploadedPanel.SetActive(true);
            }
        }
    }

    public IEnumerator ReadData(GameObject parent)
    {
        titleUI.connectWebPanel.SetActive(true);
        audiom.sounds[1].Play();

        WWWForm form = new WWWForm();
        
        using (UnityWebRequest www = UnityWebRequest.Post("http://taiyouserver.php.xdomain.jp/ReadData.php", form))
        {
            yield return www.SendWebRequest();

            titleUI.connectWebPanel.SetActive(false);

            if (www.result != UnityWebRequest.Result.Success) {
                titleUI.errorPanel.SetActive(true);
                titleUI.errorPanels[0].SetActive(true);
            }
            else {
                string json = www.downloadHandler.text;

                JSONArray jsonArray = JSON.Parse(json) as JSONArray;

                // 初期化
                foreach (var value in bgList) {
                    Destroy(value);
                }
                bgList = new List<GameObject>();
                stageNameDic = new Dictionary<int, string>();
                if (jsonArray != null) {
                    for(int i = 0; i < jsonArray.Count; i++) {
                        GameObject stageG = Instantiate(Resources.Load("Prefabs/stages") as GameObject);
                        bgList.Add(stageG);
                        stageG.transform.SetParent(parent.transform);
                        stageG.transform.localScale = Vector3.one;
                        stageG.transform.localPosition = Vector3.zero;
                        stageG.transform.Find("Name").GetComponent<Text>().text = jsonArray[i].AsObject["stagename"];
                        stageG.transform.Find("player").GetComponent<Text>().text = jsonArray[i].AsObject["name"];
                        stageG.transform.Find("count").GetComponent<Text>().text = "遊ばれた回数 : " + jsonArray[i].AsObject["nice"];
                        stageG.GetComponent<SetStageFromData>().stageID = jsonArray[i].AsObject["stageID"];
                        
                        byte[] bytes = System.Convert.FromBase64String(jsonArray[i].AsObject["image"]);
                    
                        Texture2D texture = new Texture2D(2,2);
                        texture.LoadImage(bytes);
                        Image image = stageG.transform.Find("Image").GetComponent<Image>();
                        image.sprite = Sprite.Create(texture,new Rect(0,0,texture.width, texture.height), Vector2.zero);
                        PlayerPrefs.SetString("MODE", "ONLINE");

                        stageNameDic.Add(jsonArray[i].AsObject["stageID"], jsonArray[i].AsObject["stagename"]);
                    }
                }
            }
        }
    }

    public IEnumerator ReadMyStage(GameObject parent)
    {
        titleUI.connectWebPanel.SetActive(true);
        audiom.sounds[1].Play();

        WWWForm form = new WWWForm();
        form.AddField("playerID", playerID);
        
        using (UnityWebRequest www = UnityWebRequest.Post("http://taiyouserver.php.xdomain.jp/ReadMyStage.php", form))
        {
            yield return www.SendWebRequest();

            titleUI.connectWebPanel.SetActive(false);

            if (www.result != UnityWebRequest.Result.Success) {
                titleUI.errorPanel.SetActive(true);
                titleUI.errorPanels[0].SetActive(true);
            }
            else {
                string json = www.downloadHandler.text;

                JSONArray jsonArray = JSON.Parse(json) as JSONArray;

                // 初期化
                foreach (var value in bgList) {
                    Destroy(value);
                }
                bgList = new List<GameObject>();
                stageNameDic = new Dictionary<int, string>();
                if (jsonArray != null) {
                    for(int i = 0; i < jsonArray.Count; i++) {
                        GameObject stageG = Instantiate(Resources.Load("Prefabs/stages") as GameObject);
                        bgList.Add(stageG);
                        stageG.transform.SetParent(parent.transform);
                        stageG.transform.localScale = Vector3.one;
                        stageG.transform.localPosition = Vector3.zero;
                        stageG.transform.Find("Name").GetComponent<Text>().text = jsonArray[i].AsObject["stagename"];
                        stageG.transform.Find("player").GetComponent<Text>().text = jsonArray[i].AsObject["name"];
                        stageG.transform.Find("count").GetComponent<Text>().text = "遊ばれた回数 : " + jsonArray[i].AsObject["nice"];
                        stageG.GetComponent<SetStageFromData>().stageID = jsonArray[i].AsObject["stageID"];
                        
                        byte[] bytes = System.Convert.FromBase64String(jsonArray[i].AsObject["image"]);
                    
                        Texture2D texture = new Texture2D(2,2);
                        texture.LoadImage(bytes);
                        Image image = stageG.transform.Find("Image").GetComponent<Image>();
                        image.sprite = Sprite.Create(texture,new Rect(0,0,texture.width, texture.height), Vector2.zero);
                        PlayerPrefs.SetString("MODE", "REEDIT");
                    }
                }
            }
        }
    }

    public IEnumerator UpdatePlayCount()
    {
        WWWForm form = new WWWForm();
        form.AddField("stageID", playStageID);
        using (UnityWebRequest www = UnityWebRequest.Post("http://taiyouserver.php.xdomain.jp/UpdatePlayCount.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                titleUI.errorPanel.SetActive(true);
                titleUI.errorPanels[0].SetActive(true);
            }
        }
    }

    public IEnumerator DownloadStage(string id)
    {
        WWWForm form = new WWWForm();
        form.AddField("stageID", id);
        using (UnityWebRequest www = UnityWebRequest.Post("http://taiyouserver.php.xdomain.jp/DownloadStage.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                titleUI.errorPanel.SetActive(true);
                titleUI.errorPanels[0].SetActive(true);
            }
            else {
                SQLiteDB sQLiteDB = GameObject.Find("SQLite").GetComponent<SQLiteDB>();

                string json = www.downloadHandler.text;
                JSONArray jsonArray = JSON.Parse(json) as JSONArray;
                // 元はfor
                string stageName = jsonArray[0].AsObject["stagename"];
                string stageData = jsonArray[0].AsObject["data"];
                string stageID = jsonArray[0].AsObject["stageID"];
                string stageImage = jsonArray[0].AsObject["image"];
                string userName = jsonArray[0].AsObject["name"];
                sQLiteDB.InsertData(stageName, stageData, stageImage, int.Parse(stageID));
            }
        }
    }

    public IEnumerator SetStage(string stageID)
    {
        titleUI.connectWebPanel.SetActive(true);
        audiom.sounds[1].Play();

        WWWForm form = new WWWForm();
        playStageID = int.Parse(stageID);
        form.AddField("stageID", stageID);
        
        using (UnityWebRequest www = UnityWebRequest.Post("http://taiyouserver.php.xdomain.jp/SetStage.php", form))
        {
            yield return www.SendWebRequest();

            titleUI.connectWebPanel.SetActive(false);

            if (www.result != UnityWebRequest.Result.Success) {
                titleUI.errorPanel.SetActive(true);
                titleUI.errorPanels[0].SetActive(true);
            }
            else {
                stageData = www.downloadHandler.text;
                ReadInStageData();
            }
        }
    }

    string rows = "";
    private void ReadInStageData()
    {
        rows = "";
        string path = Application.dataPath + "/test.txt";
        StreamWriter sw = new StreamWriter(path,false);
        
        int count = 0;
        int layerCount = 0;
        int fiishCount = 0;

        for(int i = 0; i < stageData.Length; i++)
        {
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

        if (PlayerPrefs.GetString("MODE") == "REEDIT") {
            FadeManager.Instance.LoadScene("EditScene",1);
        } else {
            foreach (var value in stageNameDic) {
                if (value.Key == playStageID) {
                    stageName = value.Value;
                    FadeManager.Instance.LoadScene("MainScene",1);
                    return;
                }
            }
        }
    }
}
