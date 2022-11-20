using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Languages : MonoBehaviour
{
    [SerializeField] Button jaButton;
    [SerializeField] Button enButton;
    [Header("TEXT")]
    [SerializeField] Text[] languageText;
    [SerializeField] TextAsset languageAsset;

    /*void Start()
    {
        LoadLanguageData();

        jaButton.onClick.AddListener(() => {
            SetLanguage(0);
        });
        enButton.onClick.AddListener(() => {
            SetLanguage(1);
        });
    }*/

    // 言語をロード
    private void LoadLanguageData()
    {
        string[] high = languageAsset.text.Split(new[] {',','\n','\r'},System.StringSplitOptions.RemoveEmptyEntries);
        int count = 0;
        int textNum = 0;
        for (int i = 0; i < high.Length; i++)
        {
            if (PlayerPrefs.GetInt("jap",0) == 0 && count == 0) 
            {
                languageText[textNum].text = high[i];
                textNum++;
            }
            if (PlayerPrefs.GetInt("jap",0) == 1 && count == 1)
            {
                languageText[textNum].text = high[i];
                textNum++;
            }
            count++;
            if (count == 2)
            {
                count = 0;
            } 
        }
    }

    // 言語選択
    public void SetLanguage(int num)
    {
        PlayerPrefs.SetInt("jap",num);
        LoadLanguageData();
    }
}
