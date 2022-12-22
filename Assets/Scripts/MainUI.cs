using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject updateText;
    public GameObject numberTextPanel;
    public TextMeshProUGUI numberText;
    public Text playerNameText;
    public Transform[] numberTextMovePos;
    public UnityEngine.UI.Image blackImage;
    public GameObject goalPanel;
    public GameObject onlinegoalPanel;
    public GameObject operationPanel;
    public GameObject[] pages;
    public GameObject[] pageArrow;
    public GameObject editClearPanel;
    public GameObject uploadPanel;
    public GameObject errorPanel;
    public GameObject loadPanel;
    public GameObject uploadedPanel;
    public GameObject updatePanel;
    public GameObject leftUIs;

    [Header("BUTTON")]
    public Button[] editButton;

    [Header("TEXT")]
    public TextMeshProUGUI stageNumberText;
    public Text playerStageNameText;
    [SerializeField] Text stageName;
    public Text pickaxeText;
    public Text stoneText;
    // 変数
    public bool operation{get; set;}
    private int pageNum = 0;

    void Start()
    {
        goalPanel.SetActive(false);
        editButton[0].onClick.AddListener(() => {
            BackEditScene();
        });
        editButton[1].onClick.AddListener(() => {
            NextEditPanel();
        });
        editButton[2].onClick.AddListener(() => {
            UploadStage();
        });
        editButton[3].onClick.AddListener(() => {
            FadeManager.Instance.LoadScene("TitleScene", 1);
        });
        editButton[4].onClick.AddListener(() => {
            UpdateStage();
        });
    }

    public void ShowStageNumber(int num)
    {
        stageNumberText.text = "STAGE " + (num + 1);
    }

    public void ShowBlackPanel()
    {
        goalPanel.SetActive(false);
        blackImage.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackImage.DOFade(1,0.5f).SetEase(Ease.Linear))
                .Append(blackImage.DOFade(0,0.5f).SetEase(Ease.Linear).SetDelay(1.5f))
                .AppendCallback(() => blackImage.gameObject.SetActive(false));
    }

    public void ShowOperationPanel()
    {
        operationPanel.SetActive(true);
        Sounds.instance.se[11].Play();
    }
    public void CloseOperationPanel()
    {
        operationPanel.SetActive(false);
        Sounds.instance.se[11].Play();
    }
    public void PageUpdate()
    {
        if (!operation) return;
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (pageNum == 2) return;
            pageNum++;
            ShowPage();
            Sounds.instance.se[4].Play();
            if (pageNum == 2) pageArrow[0].SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (pageNum == 0) return;
            pageNum--;
            ShowPage();
            Sounds.instance.se[4].Play();
            if (pageNum == 0) pageArrow[1].SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            operation = false;
            CloseOperationPanel();
        }
        
    }
    private void ShowPage()
    {
        foreach (var value in pages) {
            value.SetActive(false);
        }
        foreach (var value in pageArrow) {
            value.SetActive(true);
        }
        pages[pageNum].SetActive(true);
    }

    public void EndStage()
    {
        updateText.SetActive(true);
        goalPanel.SetActive(false);
    }

    public void StageTextAnim(int num)
    {
        if (PlayerPrefs.GetString("MODE") == "DEFAULT") {
            playerNameText.gameObject.SetActive(false);
            numberTextPanel.SetActive(true);
            numberText.text = "STAGE " + (num + 1);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(numberText.transform.DOMoveX(numberTextMovePos[0].position.x,0))
                    .Append(numberText.transform.DOMoveX(numberTextMovePos[1].position.x,0.5f))
                    .Append(numberText.transform.DOMoveX(numberTextMovePos[2].position.x,0.5f).SetDelay(2f))
                    .AppendCallback(() => numberTextPanel.SetActive(false));
        }
        if (PlayerPrefs.GetString("MODE") == "ONLINE") {
            numberText.gameObject.SetActive(false);
            numberTextPanel.SetActive(true);
            playerNameText.text = MainForOnline.Instance.web.stageName;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(playerNameText.transform.DOMoveX(numberTextMovePos[0].position.x,0))
                    .Append(playerNameText.transform.DOMoveX(numberTextMovePos[1].position.x,0.5f))
                    .Append(playerNameText.transform.DOMoveX(numberTextMovePos[2].position.x,0.5f).SetDelay(2f))
                    .AppendCallback(() => numberTextPanel.SetActive(false));
        }
    }

    // BUTTON 0
    private void BackEditScene()
    {
        FadeManager.Instance.LoadScene("EditScene",1);
    }

    private void NextEditPanel()
    {
        editClearPanel.SetActive(false);
        uploadPanel.SetActive(true);
    }

    // BUTTON 2
    private void UploadStage()
    {
        string name = stageName.text;
        StartCoroutine(MainForOnline.Instance.web.SendData(name, MainForOnline.Instance.stageData));
    }

    // 再編集
    private void UpdateStage()
    {
        StartCoroutine(MainForOnline.Instance.web.ReSendData(MainForOnline.Instance.stageData));
    }
}
