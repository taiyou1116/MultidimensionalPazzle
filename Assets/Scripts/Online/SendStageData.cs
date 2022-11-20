using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendStageData : MonoBehaviour
{
    public PlayerEdit playerEdit;
    public Button sendButton;

    public string name = "teststage2";

    void Start()
    {
        sendButton.onClick.AddListener(() => {
            StartCoroutine(MainForOnline.Instance.web.SendData(name,playerEdit.WriteInStageData()));
        });
    }
}
