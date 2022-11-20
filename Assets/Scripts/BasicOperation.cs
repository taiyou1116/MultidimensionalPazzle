using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicOperation : MonoBehaviour
{
    [SerializeField] MainUI mainUI;
    private bool operation = false;
    private int pageNum = 0;
    public bool Operation
    {
        get{return operation;}
        set{operation = value;}
    }
    public void ShowOperationPanel()
    {
        mainUI.operationPanel.SetActive(true);
        Sounds.instance.se[11].Play();
    }
    public void CloseOperationPanel()
    {
        mainUI.operationPanel.SetActive(false);
        Sounds.instance.se[11].Play();
    }
    public void PageUpdate()
    {
        if(!operation) return;
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(pageNum == 2) return;
            pageNum++;
            ShowPage();
            Sounds.instance.se[4].Play();
            if(pageNum == 2) mainUI.pageArrow[0].SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(pageNum == 0) return;
            pageNum--;
            ShowPage();
            Sounds.instance.se[4].Play();
            if(pageNum == 0) mainUI.pageArrow[1].SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            operation = false;
            CloseOperationPanel();
        }
    }
    private void ShowPage()
    {
        foreach(var value in mainUI.pages)
        {
            value.SetActive(false);
        }
        foreach(var value in mainUI.pageArrow)
        {
            value.SetActive(true);
        }
        mainUI.pages[pageNum].SetActive(true);
    }
}
