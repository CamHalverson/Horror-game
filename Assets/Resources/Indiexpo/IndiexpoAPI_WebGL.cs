using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class IndiexpoAPI_WebGL {


    [DllImport("__Internal")]
    private static extern void ShowMessage(string str);

    [DllImport("__Internal")]
    private static extern void UploadScore(int s);

    [DllImport("__Internal")]
    private static extern bool CheckIfLogged();

    [DllImport("__Internal")]
    private static extern void openPage(string url);

    public static int iESavedScore = 0;    

    //Constructor for the class, it checks if there's already a saved record to retrieve
    static IndiexpoAPI_WebGL()
    {
        RetrieveSavedScore();
    }

    //Use this method to send score to the Indiexpo server
    public static void SaveScoreAndSend(int s)
    {
        if (s > iESavedScore)
        {
            PlayerPrefs.SetInt("indieExpoSavedScore", s);
            iESavedScore = s;
            UploadScore(s);
        }
    }

    //Old method to send score to the Indiexpo server use it if you want to implement your own logic to send scores
    public static void SendScore(int s)
    {
        UploadScore(s);
    }

    //Use this method if you want to show a messagge to the user in the browser window
    public static void SendMessage(string str)
    {
        ShowMessage(str);
    }

    public static bool LoginCheck()
    {
        bool check =   CheckIfLogged();
        return check;
    }

    //Use this method to retrieve the saved score, remember that the score is saved locally
    public static void RetrieveSavedScore()
    {
        if (PlayerPrefs.GetInt("indieExpoSavedScore", 0) > -0)
        {
            iESavedScore = PlayerPrefs.GetInt("indieExpoSavedScore");
        }
    }

    //Use this method if you want to save manually the score, remember that the score is saved locally
    public static void SaveScore(int s)
    {
           PlayerPrefs.SetInt("savedScore", s);
    }

    public static void OpenLinkInNewTab(string link)
    {
        openPage(link);
    }
}
