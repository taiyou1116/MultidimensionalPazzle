using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;
using System.Collections;
public class TitleUIManager : MonoBehaviour
{
    public AudioManager audiom;
    private int pageNumber;
    private bool onButton = false;

    [Header("UI")]
    [SerializeField] GameObject titlePanel;
    [SerializeField] GameObject titleImage;
    [SerializeField] GameObject selectPanel;
    [SerializeField] GameObject[] stagePanels;
    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject[] optionPanels;
    [SerializeField] GameObject onlinePanel;
    [SerializeField] GameObject dLPanel;
    [SerializeField] GameObject createPanel;
    [SerializeField] GameObject everyonePanel;
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject newUserPanel;
    [SerializeField] GameObject startText;

    // WEBCONNECT
    public GameObject errorPanel;
    public GameObject[] errorPanels;
    public GameObject connectWebPanel;
    public GameObject stageBG;
    public GameObject stageMyBG;
    public GameObject stageDLBG;

    // RESISTER
    public GameObject[] newUserErrorPanel;
    
    [Header("BUTTON")]
    [SerializeField] Button showStagePanelButton;
    [SerializeField] Button showOptionPanelButton;
    [SerializeField] Button closeOptionPanelButton;
    [SerializeField] Button showMainPanel;
    [SerializeField] Button showOnlinePanel;
    [SerializeField] Button showDLPanel;
    [SerializeField] Button showCreatePanel;
    [SerializeField] Button showEveryOnePanel;
    [SerializeField] Button showloginPanel;
    [SerializeField] Button backTitle;
    [SerializeField] Button[] backMain;
    [SerializeField] Button[] backOnline;
    [SerializeField] Button[] optionchange;
    [SerializeField] Button[] pageButton;

    [SerializeField] Button createStage;

    // RESISTER
    [SerializeField] Button[] newUser;

    void Start()
    {
        Application.targetFrameRate = 60;

        PanelsTrue();
        MainForOnline.Instance.Initialize();
        titlePanel.SetActive(true);
        titleImage.SetActive(true);

        // クリックしてスタートのアニメーション
        startText.transform.DOScale(1.3f, 1f)
                           .SetEase(Ease.OutBounce)
                           .SetLoops(-1, LoopType.Yoyo);

        showStagePanelButton.onClick.AddListener(() => {
            ShowStagePanel();
        });
        showOptionPanelButton.onClick.AddListener(() => {
            OpenOptionPanel();
        });
        closeOptionPanelButton.onClick.AddListener(() => {
            CloseOptionPanel();
        });
        showMainPanel.onClick.AddListener(() => {
            ShowPanel(mainPanel);
        });
        showOnlinePanel.onClick.AddListener(() => {
            ShowPanel(onlinePanel);
        });
        showDLPanel.onClick.AddListener(() => {
            ShowPanel(dLPanel);
            SQLiteDB dB = GameObject.Find("SQLite").GetComponent<SQLiteDB>();
            dB.GetSQLiteDBData(stageDLBG);
        });
        showCreatePanel.onClick.AddListener(() => {
            ShowPanel(createPanel);
            StartCoroutine(MainForOnline.Instance.web.ReadMyStage(stageMyBG));
        });
        showEveryOnePanel.onClick.AddListener(() => {
            ShowPanel(everyonePanel);
            // Panelを開くと同時にSTAGEを取得
            StartCoroutine(MainForOnline.Instance.web.ReadData(stageBG));
        });
        showloginPanel.onClick.AddListener(() => {
            ShowPanel(loginPanel);
            if (MainForOnline.Instance.playerName != "") {
                StartCoroutine(MainForOnline.Instance.web.Login(MainForOnline.Instance.playerName,MainForOnline.Instance.playerPass));
            }
        });
        pageButton[0].onClick.AddListener(() => {
            BackPage();
        });
        pageButton[1].onClick.AddListener(() => {
            NextPage();
        });
        optionchange[0].onClick.AddListener(() => {
            ShowOptionPanel(0);
        });
        optionchange[1].onClick.AddListener(() => {
            ShowOptionPanel(1);
        });
        optionchange[2].onClick.AddListener(() => {
            ShowOptionPanel(2);
        });
        backTitle.onClick.AddListener(() => {
            ShowPanel(titlePanel);
            titleImage.SetActive(true);
        });
        createStage.onClick.AddListener(() => {
            PlayerPrefs.SetString("MODE", "EDIT");
            StartCoroutine(WaitProcess());
        });
        BackMain();
        BackOnline();
        Resister();
    }
    private void BackOnline()
    {
        foreach (var value in backOnline) {
            value.onClick.AddListener(() => {
            ShowPanel(onlinePanel);
        });
        }
    }

    private void BackMain()
    {
        foreach (var value in backMain) {
            value.onClick.AddListener(() => {
            ShowPanel(mainPanel);
        });
        }
    }
    private void Resister()
    {
        foreach (var value in newUser) {
            value.onClick.AddListener(() => {
                errorPanel.SetActive(false);
                foreach(var value in errorPanels) {
                    value.SetActive(false);
                }
            });
        }
    }
    private IEnumerator WaitProcess()
    {
        string data = WriteInStageData();
        yield return new WaitUntil(() => ReadInStageData(data));
        audiom.bgm.DOFade(0,1);
        FadeManager.Instance.LoadScene("EditScene",1);
    }

    // 全てのパネルをオフ
    public void PanelsFalse()
    {
        titlePanel.SetActive(false);
        titleImage.SetActive(false);
        optionPanel.SetActive(false);
        onlinePanel.SetActive(false);
        createPanel.SetActive(false);
        everyonePanel.SetActive(false);
        mainPanel.SetActive(false);
        selectPanel.SetActive(false);
        loginPanel.SetActive(false);
        newUserPanel.SetActive(false);
        errorPanel.SetActive(false);
        connectWebPanel.SetActive(false);
        dLPanel.SetActive(false);
    }
    public void PanelsTrue()
    {
        onlinePanel.SetActive(true);
        loginPanel.SetActive(true);
        newUserPanel.SetActive(true);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            // ゲーム終了
            Application.Quit();
        }
    }
    
    private void NextPage()
    {
        if (pageNumber == stagePanels.Length-1) {
            return;
        }
        foreach (var value in stagePanels) {
            value.SetActive(false);
        }
        pageNumber++;
        stagePanels[pageNumber].SetActive(true);
        audiom.sounds[1].Play();
    }

    private void BackPage()
    {
        if (pageNumber == 0) {
            return;
        }
        foreach (var value in stagePanels) {
            value.SetActive(false);
        }
        pageNumber--;
        stagePanels[pageNumber].SetActive(true);
        audiom.sounds[1].Play();
    }

    // MainSceneへ遷移 ボタン
    public void Select(int number)
    {
        if (onButton) return;
        onButton = true;
        PlayerPrefs.SetString("MODE", "DEFAULT");
        PlayerPrefs.SetInt("STAGENUMBER",number);
        PlayerPrefs.Save();
        FadeManager.Instance.LoadScene("MainScene",1f);
        audiom.sounds[3].Play();
        audiom.bgm.DOFade(0,1);
    }

    private void ShowPanel(GameObject panel)
    {
        PanelsFalse();
        panel.SetActive(true);
        audiom.sounds[1].Play();
    }

    // ステージ選択パネルへ移動
    private void ShowStagePanel()
    {
        PanelsFalse();
        selectPanel.SetActive(true);
        audiom.sounds[1].Play();
    }

    //オプション
    private void ShowOptionPanel(int num)
    {
        foreach(var value in optionPanels)
        {
            value.SetActive(false);
        }
        optionPanels[num].SetActive(true);
        audiom.sounds[1].Play();
    }
    private void OpenOptionPanel()
    {
        PanelsFalse();
        optionPanel.SetActive(true);
        ShowOptionPanel(0);
    }
    private void CloseOptionPanel()
    {
        mainPanel.SetActive(true);
        optionPanel.SetActive(false);
        audiom.sounds[1].Play();
    }

    private string WriteInStageData()
    {
        string rows = "";
        for (int high = 0; high < 6; high++)
        {
            for (int culm = 0; culm < 9; culm++)
            {
                for (int row = 0; row < 9; row++)
                {
                    rows += "2";
                }
            }
        }
        return rows;
    }
    private bool ReadInStageData(string stageData)
    {
        string path = Application.dataPath + "/test.txt";
        StreamWriter sw = new StreamWriter(path,false);

        string rows = "";
        int count = 0;
        int layerCount = 0;
        int fiishCount = 0;

        for(int i = 0; i < stageData.Length; i++) {
            rows += stageData[i] + ",";
            count++;
            if (count == 9 && layerCount == 8 && fiishCount == 5)
            {
                string result = rows.Substring(0, rows.Length - 1);
                sw.Write(result + "\n");
            }
            else if (layerCount == 8 && count == 9) {
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
        return true;
    }
}
