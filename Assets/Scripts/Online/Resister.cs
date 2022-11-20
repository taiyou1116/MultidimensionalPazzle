using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Resister : MonoBehaviour
{
    [SerializeField] TitleUIManager titleUI;
    public InputField usernameInput;
    public InputField passwordInput;
    public InputField confirmInput;
    public Button resisterButton;
    public Button loginButton;

    void Start()
    {
        resisterButton.onClick.AddListener(() => {
            if (passwordInput.text.Length == 0 || usernameInput.text.Length == 0) {
                titleUI.errorPanel.SetActive(true);
                titleUI.newUserErrorPanel[0].SetActive(true);
                return;
            }
            if (passwordInput.text != confirmInput.text) {
                titleUI.errorPanel.SetActive(true);
                titleUI.newUserErrorPanel[1].SetActive(true);
                return;
            }
            StartCoroutine(MainForOnline.Instance.web.ResisterUser(usernameInput.text, passwordInput.text));
        });

        loginButton.onClick.AddListener(() => {
            MainForOnline.Instance.login.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        });
    }
}
