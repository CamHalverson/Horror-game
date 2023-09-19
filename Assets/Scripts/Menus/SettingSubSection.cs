using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingSubSection : MonoBehaviour
{
    public void Back()
    {
        SceneManager.LoadSceneAsync("Pause");
    }

}
