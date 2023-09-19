using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Game");
    }

    public void Settings()
    {
        SceneManager.LoadSceneAsync("Settings");
    }

    public void Quit()
    {
        Application.Quit();

        Debug.Log("Quit game");
    }


}
