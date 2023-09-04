using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGuide : MonoBehaviour {
    [SerializeField] private Transform player;
    private Transform playerCamera;
    private Camera cameraComp;
    private Deaf playerCode;
    private AudioSource playerSource;
    private float distance;
    private int changingMode; //0: Zooming 1: Shortening 2: Widing
    private bool paused;
    private bool VR;
    private bool done;

	// Use this for initialization
	void Start () {
        VR = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "VR";
        playerCode = player.gameObject.GetComponent<Deaf>();
        try
        {
            playerCamera = player.Find("FirstPersonCharacter").transform;
            cameraComp = playerCamera.gameObject.GetComponent<Camera>();
        }
        catch (System.NullReferenceException)
        {
            playerCamera = player.Find("CameraRig").transform;
            cameraComp = player.Find("CameraRig/MainCamera").GetComponent<Camera>();           
        } 
        playerSource = player.Find("Changeling").GetComponent<AudioSource>();
        changingMode = Random.Range(0,3);
	}
	
	// Update is called once per frame
	void Update () {
        distance = Vector3.Distance(player.position, transform.position);
        if (distance < 2 && !done)
        {
            if (paused)
            {
                playerSource.Play();
                paused = false;
            }
            switch (changingMode)
            {
                case 0:
                    if (!VR) cameraComp.fieldOfView = (distance / 2) * 60;
                    else changingMode = 1;
                    break;
                case 1:
                    playerCamera.localPosition = new Vector3(0, distance * 0.4f, 0);
                    break;
                case 2:
                    if (!VR) cameraComp.fieldOfView = 120 - (distance * 30);
                    else changingMode = 1;
                    break;
                default:
                    break;
            }
        }
        else if (!paused)
        {
            playerSource.Pause();
            paused = true;
        }
    }
    
    void OnTriggerEnter()
    {
        done = true;
        playerCamera.localPosition = new Vector3(0, 0.8f, 0);
        playerCode.Die("You have became one of them");
    }
}
