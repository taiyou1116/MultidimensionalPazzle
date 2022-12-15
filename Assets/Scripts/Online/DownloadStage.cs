using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownloadStage : MonoBehaviour
{
    Button button;
    private string stageID;

    void Start()
    {
        button = GetComponent<Button>();
        SetStageFromData data = GetComponentInParent<SetStageFromData>();
        stageID = data.stageID;

        button.onClick.AddListener(() => {
            MainForOnline main = GameObject.Find("MainANDweb").GetComponent<MainForOnline>();
            StartCoroutine(main.web.DownloadStage(stageID));
        });
    }
}
