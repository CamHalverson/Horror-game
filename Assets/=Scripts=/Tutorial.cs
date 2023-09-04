using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    [SerializeField] private GameObject loadPanel;
    [SerializeField] private GameObject effect;
    [SerializeField] private AudioSource source;

    // Use this for initialization
    IEnumerator Start () {
        effect.SetActive(PlayerPrefs.GetInt("EffectsDisabled") != 1);
        source.enabled = PlayerPrefs.GetInt("MusicDisabled") != 1;
        yield return new WaitForSeconds(60);
        LoadMenu();
	}
	
	public void LoadMenu()
    {
        loadPanel.SetActive(true);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
