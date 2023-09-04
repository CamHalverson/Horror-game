using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour {

    [SerializeField] private Animator thisAnimator;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject intro;
    [SerializeField] private AudioListener thisListener;
    [SerializeField] private Camera thisCamera;
    [SerializeField] private MonoBehaviour thisRP;
    [SerializeField] private UnityEngine.UI.Text tutText1;
    [SerializeField] private UnityEngine.UI.Text tutText2;
    [SerializeField] private GameObject effect;
    [SerializeField] private Canvas worldSpaceCanvas;
    private Coroutine introWait;

    // Use this for initialization
    void Start () {

        effect.SetActive(PlayerPrefs.GetInt("EffectsDisabled") != 1);        
        if (PlayerPrefs.GetInt("Intro") == 1 || SceneManager.GetActiveScene().name.Contains("VR"))
        {
            thisAnimator.SetTrigger("Skip");
            player.SetActive(true);
            if (thisRP) Destroy(thisRP);
            if (thisCamera) Destroy(thisCamera);            
            if (intro) Destroy(intro);
            if (thisListener) Destroy(thisListener);
        }
        else
        {
            if (PlayerPrefs.GetInt("Difficulty") == 5)
            {
                tutText1.text = "Objective: Explore";
                tutText2.text = "Walk with AD/WS or arrow keys & Look with mouse\n Don't worry; No-one else is here;\n Explore the Backrooms freely!";
            }
            introWait = StartCoroutine(IntroWait());
            PlayerPrefs.SetInt("Intro",1);
        }
	}
    
	public void Skip()
    {
        StopCoroutine(introWait);        
        Destroy(thisRP);
        Destroy(thisCamera);
        Destroy(intro);
        Destroy(thisListener);
        player.SetActive(true);
    }

    IEnumerator IntroWait()
    {       
        yield return new WaitForSeconds(35);
        Skip();
    }
}
