using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Analytics;


public class OnboardManager : MonoBehaviour
{
    public GameObject SplashPanel, StartPanel;
    bool isLogin = false;
    int counter = 0;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => { FirebaseAnalytics.SetAnalyticsCollectionEnabled(true); });
        Debug.Log("Firebase init");
 
        StartPanel.SetActive(true);
    }


    public void ClickNext()
    {
        SceneManager.LoadScene("Home");
    }

    public void ClickLogin()
    {
        SceneManager.LoadScene("Register");
    }

}
