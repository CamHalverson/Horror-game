using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    [SerializeField]
    private Transform pos1;
    [SerializeField]
    private Transform pos2;    
    [SerializeField]
    private Sprite warning;    
    [SerializeField]
    private Toggle effects;
    [SerializeField]
    private GameObject effectObject;
    [SerializeField]
    private Slider sensitivity;
    [SerializeField]
    private Toggle music;
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private GameObject exitButton;
    [SerializeField]
    private Button VRButton;
    [SerializeField]
    private GameObject directVR;

    private bool isitFirstAnimation = true;
    private bool posOK;
    private bool effectsOn;
    private bool musicOn;
    private bool done;
    private int theGameMode;

    void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;       
        XRSettings.enabled = false;        
        if (!PlayerPrefs.HasKey("EffectsDisabled") && Application.platform == RuntimePlatform.Android)
        {
            effectsOn = false;
            PlayerPrefs.SetInt("EffectsDisabled", 1);
        }
        else effectsOn = PlayerPrefs.GetInt("EffectsDisabled") != 1;
        effects.isOn = effectsOn;
        effectObject.SetActive(effectsOn);
        musicOn = PlayerPrefs.GetInt("MusicDisabled") != 1;
        music.isOn = musicOn;
        musicSource.enabled = musicOn;
        sensitivity.value = PlayerPrefs.GetFloat("MouseSensitivity");
        if (Application.platform == RuntimePlatform.WebGLPlayer) exitButton.SetActive(false);
        if (Application.platform == RuntimePlatform.Android)
        {
            if (SystemInfo.supportsGyroscope && SystemInfo.supportsAccelerometer)
            {
                VRButton.interactable = true;
                directVR.SetActive(true);
            }
            else
            {
                VRButton.transform.Find("Text").GetComponent<Text>().text = "VR (Not Compatible)";
                VRButton.transform.Find("Text (1)").GetComponent<Text>().text = "VR (Not Compatible)";
            }
        }
        done = true;
    }
    
    // Update is called once per frame
    private void LateUpdate()
    {
        effectObject.SetActive(effectsOn);
        if (isitFirstAnimation)
        {
            if (!posOK)
            {
                transform.SetPositionAndRotation(pos1.position, pos1.rotation);
                posOK = true;
            }
            if (transform.position.x > -72)
            {
                transform.Translate(new Vector3(-0.5f, 0, 0) * Time.deltaTime, Space.World);
            }
            else
            {
                isitFirstAnimation = false;
                posOK = false;
            }
        }

        else
        {
            if (!posOK)
            {
                transform.SetPositionAndRotation(pos2.position, pos2.rotation);
                posOK = true;
            }
            if (transform.position.z < -35)
            {
                transform.Translate(new Vector3(0, 0, 0.5f) * Time.deltaTime, Space.World);
            }
            else
            {
                isitFirstAnimation = true;
                posOK = false;
            }
        }
    }

    public void LoadURL(string Address)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            IndiexpoAPI_WebGL.OpenLinkInNewTab(Address);
            return;
        }
        else
            Application.OpenURL(Address);
    }

    public void Exit()
    {
        PlayerPrefs.SetString("UserName", null);
        Application.Quit();
    }
    
    public void SetDiffcultyAndGO(int difficulty)
    {
        PlayerPrefs.SetInt("Difficulty", difficulty);
        PlayerPrefs.SetInt("Intro", 0);
        if (difficulty == 5) UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        else
        switch (theGameMode)
        {
            case 0:
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
                break;
            case 1:
                UnityEngine.SceneManagement.SceneManager.LoadScene("Deaf");
                break;
            case 2:
                UnityEngine.SceneManagement.SceneManager.LoadScene("VR Setup");
                break;
            default:
                break;
        }        
    }    

    public void SetGameMode(int gamemode) //0: Classic 1: Deaf 2: VR
    {
        theGameMode = gamemode;
        PlayerPrefs.SetInt("Gamemode",gamemode);
    }

    public void LoadTut()
    {
        PlayerPrefs.SetInt("Intro", 0);
        switch (theGameMode)
        {
            case 0: UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
                break;
            case 1:
                UnityEngine.SceneManagement.SceneManager.LoadScene("DeafTutorial");
                break;
            case 2:
                UnityEngine.SceneManagement.SceneManager.LoadScene("VRTutorial");
                break;
            default:
                break;
        }        
    }

    public void EffectsSwap()
    {
        if (done)
        {
            effectsOn = !effectsOn;
            effects.isOn = effectsOn;
            effectObject.SetActive(effectsOn);
            PlayerPrefs.SetInt("EffectsDisabled", effectsOn ? 0 : 1);
        }
    }

    public void ChangeSensitivity()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity.value);
    }

    public void ClearData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void MusicSwap()
    {
        if (done)
        {
            musicOn = !musicOn;
            music.isOn = musicOn;
            musicSource.enabled = musicOn;
            PlayerPrefs.SetInt("MusicDisabled", musicOn ? 0 : 1);
        }
    }
}
