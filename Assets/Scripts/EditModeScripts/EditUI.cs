using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EditUI : MonoBehaviour
{
    public GameObject objErrorPanel;

    [SerializeField] Button closeErrorPanel;

    void Start()
    {
        closeErrorPanel.onClick.AddListener(() => {
            objErrorPanel.SetActive(false);
        });
    }
}
