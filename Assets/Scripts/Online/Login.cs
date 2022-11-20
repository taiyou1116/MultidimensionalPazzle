using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Login : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button resisterButton;

    void Start()
    {

        loginButton.onClick.AddListener(() => {
            StartCoroutine(MainForOnline.Instance.web.Login(usernameInput.text,passwordInput.text));
        });

        resisterButton.onClick.AddListener(() => {
            MainForOnline.Instance.resister.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        });
    }
}
