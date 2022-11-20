using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadStageData : MonoBehaviour
{
    public Button readButton;
    void Start()
    {   
        readButton.onClick.AddListener(() => {
            StartCoroutine(MainForOnline.Instance.web.ReadData(this.gameObject));
        });
    }
}
