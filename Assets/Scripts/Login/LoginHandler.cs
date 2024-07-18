using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginHandler : MonoBehaviour
{
    public string Username { get; set; }
    public string Password { get; set; }

    public string coachPanel;
    public string userPanel;

    APICall myApi;
    void Start()
    {
        myApi = new APICall("");
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Login()
    {
        StartCoroutine(myApi.Get("localhost:5000/api/User", loginCallback));

        if (Username == "coach")
        {
            SceneManager.LoadScene(coachPanel);
            return;
        }

        if (Username == "user")
        {
            SceneManager.LoadScene(userPanel);
            return;
        }
    }

    void loginCallback(UnityWebRequest.Result result, string message)
    {
        print(message);
    }
}
