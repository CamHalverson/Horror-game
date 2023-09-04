using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deaf : MonoBehaviour {

    [SerializeField] GameObject[] exit1Guides;
    [SerializeField] GameObject[] exit2Guides;
    [SerializeField] GameObject[] exit3Guides;
    [SerializeField] GameObject[] exit4Guides;
    [SerializeField] GameObject[] exit5Guides;
    [SerializeField] GameObject[] exit6Guides;
    [SerializeField] GameObject[] exit7Guides;
    [SerializeField] GameObject[] exit8Guides;
    [SerializeField] GameObject[] exit9Guides;
    [SerializeField] GameObject[] exit10Guides;
    [SerializeField] private MonoBehaviour thisController;
    [SerializeField] private AudioSource thisAudioSource;
    [SerializeField] private UnityEngine.UI.Text timerText;
    [SerializeField] private AnimationCurve enemyMovement;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject[] exits;
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private Transform[] exitSpawnPoints;
    [SerializeField] private AudioClip[] scarySounds;
    [SerializeField] private AudioClip[] deathSounds;
    [SerializeField] private AudioClip[] scaredSounds;   
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject endEye;
    [SerializeField] private Animator endAnimation;
    [SerializeField] private UnityEngine.UI.Slider detectionEyeBar;
    [SerializeField] private UnityEngine.UI.Image detectionEye;
    [SerializeField] private UnityEngine.UI.Text deathNote;
    [SerializeField] private UnityEngine.UI.Text scoreText;
    [SerializeField] private Sprite seen;
    [SerializeField] private Sprite unseenLol;  
    [SerializeField] private Sprite warning;  
    [SerializeField] private GameObject pauseButton;
    [SerializeField] Transform cameraPos;

    private GameObject temp;
    private GameObject exit;
    private Transform exitSpawn;
    private Renderer tempRenderer;
    private Renderer exitRenderer;
    private AudioSource enemyAudioSource;
    private float timerTime;
    private int timerTimeInt;
    private bool ended = false;    
    private Coroutine enemySeenWait;
    private Coroutine checkingViewIE;
    private bool enemySeen = false;
    private bool exitSeen = false;
    private bool paused = false;
    private int scoreButPleaseDontHack;
    private bool connected = false;
    private bool done = false;
    private bool checkingView = false;
    private int difficulty;
    private AudioClip tempCurrentClip;
    private AudioClip tempCurrentClip2;
    private int exitSpawnpointIndex;
    private bool isVRSetup;

    void Start()
    {
        isVRSetup = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "VR Setup";
        if (!isVRSetup)
        {
            switch (PlayerPrefs.GetInt("Difficulty"))
            {
                case 1:
                    difficulty = 1;
                    timerTime = 600;
                    break;
                case 2:
                    difficulty = 2;
                    timerTime = 480;
                    break;
                case 3:
                    difficulty = 3;
                    timerTime = 390;
                    break;
                case 4:
                    difficulty = 4;
                    timerTime = 300;
                    break;
                default:                    
                    difficulty = 2; break;
            }
            if (Application.platform != RuntimePlatform.Android)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            StartCoroutine(EnemySpawnWait());
            StartCoroutine(ExitSpawnWait());
            PlayerPrefs.SetInt("TimesPlayed", PlayerPrefs.GetInt("TimesPlayed") + 1);
        }
    }
	
	void Update ()
    {
        if (Input.GetButtonUp("Pause") && !paused && !ended)
        {
            ResumeOrPause();           
            PausePanel.SetActive(true);
        }

        if (!ended && !isVRSetup)
        {
            timerTime -= Time.deltaTime;
            timerTimeInt = Mathf.FloorToInt(timerTime);
            timerText.text = (timerTimeInt / 60).ToString() + ":" + (timerTimeInt % 60).ToString("00");
        }

        if (timerTime < 30 && !isVRSetup) timerText.color = Color.red;

        if((timerTimeInt == 0 || timerTimeInt < 0) && !ended && !isVRSetup)
        {
            timerTime = 0;
            thisController.enabled = false;
            deathNote.text = "You have been drowned in your nightmares";
            PlayerPrefs.SetInt("Lost", PlayerPrefs.GetInt("Lost") + 1);

            if (PlayerPrefs.GetInt("Mazed") != 1)
            {
                PlayerPrefs.SetInt("Mazed", 1);
            }

            failPanel.SetActive(true);
            pauseButton.SetActive(false);
            if (Application.platform != RuntimePlatform.Android)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            Destroy(temp);
            thisAudioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)]);
            ended = true;
        }

        if(!isVRSetup && temp && tempRenderer && tempRenderer.isVisible && !enemySeen && !ended)
        {
            enemySeenWait = StartCoroutine(EnemySeenWait());                       
            enemySeen = true;
        }

        if (!isVRSetup && temp && tempRenderer && tempRenderer.isVisible && enemySeen && !ended)
        {
            detectionEyeBar.value += Time.deltaTime;
            temp.transform.rotation = Quaternion.Slerp(temp.transform.rotation, transform.rotation, 1);
            temp.transform.Translate(enemyMovement.Evaluate(Time.time) * Time.deltaTime, 0,-2*Time.deltaTime,Space.Self);            
        }
       
        if (!isVRSetup && temp && tempRenderer && !tempRenderer.isVisible && enemySeen && !ended)
        {
            if (enemySeenWait != null) StopCoroutine(enemySeenWait);
            thisController.SendMessage("RUN");
            thisAudioSource.PlayOneShot(scaredSounds[Random.Range(0, 3)]);
            thisAudioSource.PlayOneShot(scaredSounds[4]);
            detectionEye.sprite = unseenLol;
            Destroy(tempRenderer);
            detectionEyeBar.value = 0;
            PlayerPrefs.SetInt("FriendSeen", PlayerPrefs.GetInt("FriendSeen") + 1);

            if(PlayerPrefs.GetInt("FriendSeen") >= 10 && PlayerPrefs.GetInt("NotJustNoises") != 1)
            {
                PlayerPrefs.SetInt("NotJustNoises", 1);                                                
            }

            if (PlayerPrefs.GetInt("FriendSeen") >= 50 && PlayerPrefs.GetInt("ItsMeAgain") != 1)
            {
                PlayerPrefs.SetInt("ItsMeAgain", 1);                
            }

            if (PlayerPrefs.GetInt("FriendSeen") >= 100 && PlayerPrefs.GetInt("LeaveMeALONE") != 1)
            {
                PlayerPrefs.SetInt("LeaveMeALONE", 1);                
            }

            if (PlayerPrefs.GetInt("FriendSeen") >= 1000 && PlayerPrefs.GetInt("AddictedToFriendship") != 1)
            {
                PlayerPrefs.SetInt("AddictedToFriendship", 1);                
            }

            StartCoroutine(EnemySpawnWait());
            enemySeen = false;            
        }
        if (!isVRSetup && exit && Vector3.Distance(transform.position, exitSpawn.position) < 20)
            if (exitRenderer && Physics.Raycast(cameraPos.position, cameraPos.forward * 10, 100, 1 << 9) && !exitSeen)
        {
            detectionEye.sprite = seen;
            endEye.SetActive(true);
            exitSeen = true;
        }

        if (!isVRSetup && exitRenderer && !Physics.Raycast(cameraPos.position, cameraPos.forward * 10, 100, 1 << 9) && exitSeen)
        {
            detectionEye.sprite = unseenLol;            
            endEye.SetActive(false);
            exitSeen = false;
        }

        if (!isVRSetup && exit && Vector3.Distance(transform.position, exitSpawn.position) < 5 && 
            Vector3.Distance(transform.position, exitSpawn.position) > 2 && !checkingView)
            checkingViewIE = StartCoroutine(CheckingView());

        if (!isVRSetup && exit && (Vector3.Distance(transform.position, exitSpawn.position) > 5 || 
            Vector3.Distance(transform.position, exitSpawn.position) < 2) &&  checkingView)
        {
            StopCoroutine(checkingViewIE);
            checkingView = false;
        }

        if(!isVRSetup && exitSpawn && Vector3.Distance(transform.position,exitSpawn.position) < 1.25f && !ended)
        {
            SaveAndSendScore();
        }
    }

    private void SaveAndSendScore()
    {
        pauseButton.SetActive(false);
        thisController.enabled = false;
        ended = true;
        endAnimation.SetTrigger("End");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        switch (difficulty)
        {
            case 1: scoreButPleaseDontHack = Mathf.CeilToInt(timerTime * 600); break;
            case 2: scoreButPleaseDontHack = Mathf.CeilToInt(timerTime * 1400); break;
            case 3: scoreButPleaseDontHack = Mathf.CeilToInt(timerTime * 1700); break;
            case 4: scoreButPleaseDontHack = Mathf.CeilToInt(timerTime * 2000); break;
            default:
                break;
        }
                
        scoreText.text = "Score: " + scoreButPleaseDontHack.ToString();
        if (scoreButPleaseDontHack > PlayerPrefs.GetInt("HSDeafmode"))
        {
            PlayerPrefs.SetInt("HSDeafmode", scoreButPleaseDontHack);            
        }                             
    }

    private IEnumerator CheckingView()
    {
        checkingView = true;
        yield return new WaitForSeconds(30);

        if (PlayerPrefs.GetInt("CheckingView") != 1)
        {
            PlayerPrefs.SetInt("CheckingView", 1);            
        }
    }
    
    IEnumerator EnemySpawnWait()
    {
        yield return new WaitForSeconds(Random.Range(15, 45));

        if (!ended)
        {                       
            switch (Random.Range(3, 10))
            {               
                case 4:
                case 5:// Friend appears behind you, but you can't see them
                case 6:
                    SpawnEnemy(transform);
                    Destroy(tempRenderer);
                    StartCoroutine(EnemySpawnWait()); break;
                case 7:
                case 8:
                case 9:// Friend appears in front of you
                case 10:
                    SpawnEnemy(GetClosestEnemy(enemySpawnPoints)); break;

                default:
                    SpawnEnemy(GetClosestEnemy(enemySpawnPoints)); break;
            }
        }

    }

    IEnumerator ExitSpawnWait()
    {
        yield return new WaitForSeconds(2);
        exit = Instantiate(exits[Random.Range(0, exits.Length)]);
        exitSpawnpointIndex = Random.Range(0, exitSpawnPoints.Length);        
        switch (exitSpawnpointIndex)
        {
            case 0:
                foreach (GameObject guide in exit1Guides)
                {
                    Destroy(guide);
                }
                break;
            case 1:
                foreach (GameObject guide in exit2Guides)
                {
                    Destroy(guide);
                }
                break;
            case 2:
                foreach (GameObject guide in exit3Guides)
                {
                    Destroy(guide);
                }
                break;
            case 3:
                foreach (GameObject guide in exit4Guides)
                {
                    Destroy(guide);
                }
                break;
            case 4:
                foreach (GameObject guide in exit5Guides)
                {
                    Destroy(guide);
                }
                break;
            case 5:
                foreach (GameObject guide in exit6Guides)
                {
                    Destroy(guide);
                }
                break;
            case 6:
                foreach (GameObject guide in exit7Guides)
                {
                    Destroy(guide);
                }
                break;
            case 7:
                foreach (GameObject guide in exit8Guides)
                {
                    Destroy(guide);
                }
                break;
            case 8:
                foreach (GameObject guide in exit9Guides)
                {
                    Destroy(guide);
                }
                break;
            case 9:
                foreach (GameObject guide in exit10Guides)
                {
                    Destroy(guide);
                }
                break;
            default:                
                break;
        }
        exitSpawn = exitSpawnPoints[exitSpawnpointIndex];
        exit.transform.SetPositionAndRotation(exitSpawn.position, exitSpawn.rotation);
        exitRenderer = exit.GetComponent<Renderer>();
        Destroy(exit.GetComponent<AudioSource>());        
    }    
    
    IEnumerator EnemySeenWait()
    {
        detectionEye.sprite = seen;
        yield return new WaitForSeconds(2f);
        if (!ended)
        {
            Die();
        }
    }   

    public void Die(string deathMessage= "You are dead, Not a big surprise...")
    {
        thisController.enabled = false;
        deathNote.text = deathMessage;
        failPanel.SetActive(true);
        pauseButton.SetActive(false);
        if (Application.platform != RuntimePlatform.Android)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        Destroy(temp);
        thisAudioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)]);
        ended = true;
    }

    private void SpawnEnemy(Transform SpawnPoint)
    {
        temp = Instantiate(enemies[Random.Range(0,enemies.Length)]);
        if (SpawnPoint == transform)
        {
            temp.transform.SetPositionAndRotation(SpawnPoint.position, SpawnPoint.rotation);
            temp.transform.Translate(Random.Range(-1, 1), 0.6f, -0.35f, Space.Self);
        }
        else
        {
            temp.transform.SetPositionAndRotation(SpawnPoint.position, SpawnPoint.rotation);
            temp.transform.Translate(0, 0.6f, 0, Space.Self);
        }
        tempRenderer = temp.GetComponent<Renderer>();
        enemyAudioSource = temp.GetComponent<AudioSource>();
        enemyAudioSource.PlayOneShot(scarySounds[Random.Range(0, scarySounds.Length)]);
    }

    public void LoadScene(string scene)
    {
        Time.timeScale = 1;
        UnityEngine.XR.XRSettings.enabled = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    public void ResumeOrPause()
    {
        bool done = false;

        if (!paused)
        {
            thisController.enabled = false;
            Time.timeScale = 0;
            if (Application.platform != RuntimePlatform.Android)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            paused = true;
            done = true;
        }

        if (paused && !done)
        {
            thisController.enabled = true;
            if (Application.platform != RuntimePlatform.Android)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            Time.timeScale = 1;            
            paused = false;
        }        
    }

    Transform GetClosestEnemy(Transform[] enemies)
    {        
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        Transform bestTarget = null;
        foreach (Transform potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget <= closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }
}
