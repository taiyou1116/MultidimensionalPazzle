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

    [Header("BUTTON")]
    public Button[] editButton;

    [Header("TEXT")]
    public TextMeshProUGUI stageNumberText;
    public Text playerStageNameText;
    [SerializeField] Text stageName;
    public Text pickaxeText;
    public Text stoneText;


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
                .Append(blackImage.DOFade(0,0.5f).SetEase(Ease.Linear).SetDelay(2))
                .AppendCallback(() => blackImage.gameObject.SetActive(false));
    }

    public void EndStage()
    {
        updateText.SetActive(true);
        goalPanel.SetActive(false);
    }

    public void StageTextAnim(int num)
    {
        numberTextPanel.SetActive(true);
        numberText.text = "STAGE " + (num + 1);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(numberText.transform.DOMoveX(numberTextMovePos[0].position.x,0))
                .Append(numberText.transform.DOMoveX(numberTextMovePos[1].position.x,0.5f))
                .Append(numberText.transform.DOMoveX(numberTextMovePos[2].position.x,0.5f).SetDelay(2f))
                .AppendCallback(() => numberTextPanel.SetActive(false));
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
