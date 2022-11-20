using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainForOnline : MonoBehaviour
{
    public static MainForOnline Instance;
    public TitleUIManager title;
    public MainUI mainUI;
    public WebConnect web;
    public Login login;
    public Resister resister;
    public GameObject onlinePanel;

    // SCENEを超えても消えないSTAGEDATA
    public string stageData{get; set;}

    // シーン遷移後最初に
    void Start()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    public void Initialize()
    {
        GameObject loginObj = GameObject.Find("loginPanel");
        GameObject registerObj = GameObject.Find("newuserPanel");
        GameObject titleObj = GameObject.Find("Canvas (TitleUIManager)");
        GameObject mainUIObj = GameObject.Find("Canvas(MainUI)");
        if (titleObj != null) {
            title = titleObj.GetComponent<TitleUIManager>();
        }
        if (mainUIObj != null) {
            mainUI = mainUIObj.GetComponent<MainUI>();
        }
        if (loginObj != null) {
            login = loginObj.GetComponent<Login>();
            onlinePanel = GameObject.Find("onlinePanel");
        }
        if (registerObj != null) {
            resister = registerObj.GetComponent<Resister>();
            title.PanelsFalse();
        }
        web = GetComponent<WebConnect>();
        web.Initialize();
    }
}
