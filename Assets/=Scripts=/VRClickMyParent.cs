using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRClickMyParent : MonoBehaviour {

    private Image myFill;
    private Button myParent;    
    private bool fillIt;

    void Start()
    {
        myFill = GetComponent<Image>();
        if (gameObject.name != "PauseVR")
        {
            transform.GetChild(0).GetComponent<Text>().text = transform.parent.GetComponent<Text>().text;            
            myParent = transform.parent.parent.GetComponent<Button>();
        }        
        else myParent = transform.parent.GetComponent<Button>();
    }
	
	void OnEnable()
    {
        StartCoroutine(Enabled());
	}

    IEnumerator Enabled()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        myFill.fillAmount = 0;
        fillIt = true;
        yield return new WaitForSecondsRealtime(2);
        fillIt = false;
        myParent.onClick.Invoke();        
    }
	
    void OnDisable()
    {
        myFill.fillAmount = 0;
        fillIt = false;
    }

	// Update is called once per frame
	void Update ()
    {
        if (fillIt) myFill.fillAmount += Time.unscaledDeltaTime/2;
	}
}
