using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void Settings()
    {
        SceneManager.LoadSceneAsync("Settings 1");
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Resume");
    }
}
