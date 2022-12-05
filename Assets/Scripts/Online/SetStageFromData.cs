using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetStageFromData : MonoBehaviour
{
    Button button;
    public string stageID{get; set;}

    void Start()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(() => {
            MainForOnline main = GameObject.Find("MainANDweb").GetComponent<MainForOnline>();
            // PlayerPrefs.SetString("MODE", "ONLINE");
            StartCoroutine(main.web.SetStage(stageID));
        });
    }
}
