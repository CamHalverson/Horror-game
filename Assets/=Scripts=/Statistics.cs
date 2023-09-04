using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour {

    [SerializeField] private UnityEngine.UI.Text stats;
    private string admiredTheView;
    private string backroomsResident;

    // Use this for initialization
    void Awake() {
        if (PlayerPrefs.GetInt("CheckingView") == 1) admiredTheView = "Yes";
        else admiredTheView = "No";

        if (PlayerPrefs.GetInt("NOOB") == 1) backroomsResident = "Yes";
        else backroomsResident = "No";

        stats.text =

            "Highscore: " + PlayerPrefs.GetInt("HS").ToString() + "\n" +
            "Highscore (Deaf mode or VR): " + PlayerPrefs.GetInt("HSDeafmode").ToString() + "\n" +
            "Times Played (Any Mode): " + PlayerPrefs.GetInt("TimesPlayed").ToString() + "\n" +
            "Seen Friends: " + PlayerPrefs.GetInt("FriendSeen").ToString() + "\n" +
            "Times Drowned in Nightmare: " + PlayerPrefs.GetInt("Lost").ToString() + "\n" +
            "Backrooms Enthusiast: " + admiredTheView + "\n" +
            "Backrooms Resident: " + backroomsResident;
    }

    public void CheckStats()
    {
        Debug.Log("Local Save Data is Deleted...");
    }
	
}
