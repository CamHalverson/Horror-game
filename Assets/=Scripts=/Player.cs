using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

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
    [SerializeField] private AudioClip[] exitSounds;
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
    [SerializeField] private GameObject hearWarning;

    private GameObject temp;
    private GameObject exit;
    private Transform exitSpawn;
    private Renderer tempRenderer;
    private Renderer exitRenderer;
    private AudioSource enemyAudioSource;
    private AudioSource exitAudioSource;
    private float timerTime = 300;
    private int timerTimeInt;
    private bool ended = false;
    private Coroutine enemyBackstabWait;
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

    void Start()
    {
        switch (PlayerPrefs.GetInt("Difficulty"))
        {
            case 0:                
                difficulty = 2; break;
            case 1:
                difficulty = 1; break;
            case 2: difficulty = 2; break;
            case 3: difficulty = 3; break;
            case 4: difficulty = 4; break;
            case 5:
                timerText.text = "FREE MODE";
                timerText.fontSize = 35;
                Destroy(detectionEye.gameObject);
                difficulty = 5; break;
            default:                
                difficulty = 2; break;
        }
        if (Application.platform != RuntimePlatform.Android)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (difficulty != 5)
        {
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

        if (!ended && difficulty != 5)
        {
            timerTime -= Time.deltaTime;
            timerTimeInt = Mathf.FloorToInt(timerTime);
            timerText.text = (timerTimeInt / 60).ToString() + ":" + (timerTimeInt % 60).ToString("00");
        }

        if (timerTime < 30) timerText.color = Color.red;

        if((timerTimeInt == 0 || timerTimeInt < 0) && !ended && difficulty != 5)
        {
            timerTime = 0;
            thisController.enabled = false;
            deathNote.text = "You have been drowned in your nightmares";
            PlayerPrefs.SetInt("Lost", PlayerPrefs.GetInt("Lost") + 1);

            if (PlayerPrefs.GetInt("Mazed") != 1)
            {
                PlayerPrefs.SetInt("Mazed", 1);
            }
            if (PlayerPrefs.GetInt("NOOB") != 1 && PlayerPrefs.GetInt("Lost") >= 100)
            {
                PlayerPrefs.SetInt("NOOB", 1);                
            }

            failPanel.SetActive(true);
            if (Application.platform != RuntimePlatform.Android)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            Destroy(temp);
            thisAudioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)]);
            ended = true;
        }

        if(temp && tempRenderer && tempRenderer.isVisible && !enemySeen && !ended)
        {
            enemySeenWait = StartCoroutine(EnemySeenWait());
            if (enemyBackstabWait != null) StopCoroutine(enemyBackstabWait);

            switch (difficulty)
            {
                case 1: detectionEyeBar.maxValue = 2; break;
                case 2: detectionEyeBar.maxValue = 1;break;
                case 3: detectionEyeBar.maxValue = 0.75f; break; 
                case 4: detectionEyeBar.maxValue = 0.5f; break;
                default:
                    break;
            }            

            enemySeen = true;
        }

        if (temp && tempRenderer && tempRenderer.isVisible && enemySeen && !ended)
        {
            detectionEyeBar.value += Time.deltaTime;
            temp.transform.rotation = Quaternion.Slerp(temp.transform.rotation, transform.rotation, 1);
            temp.transform.Translate(enemyMovement.Evaluate(Time.time) * Time.deltaTime, 0,-2*Time.deltaTime,Space.Self);            
        }
       
        if (temp && tempRenderer && !tempRenderer.isVisible && enemySeen && !ended)
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
        if (exit && Vector3.Distance(transform.position, exitSpawn.position) < 20)
            if (exitRenderer && Physics.Raycast(transform.position, transform.forward * 10, 100,1<<9) && !exitSeen)
        {
            detectionEye.sprite = seen;
            endEye.SetActive(true);
            exitSeen = true;
        }

        if (exitRenderer && !Physics.Raycast(transform.position, transform.forward * 10, 100,1<<9) && exitSeen)
        {
            detectionEye.sprite = unseenLol;            
            endEye.SetActive(false);
            exitSeen = false;
        }

        if (exit && Vector3.Distance(transform.position, exitSpawn.position) < 5 && Vector3.Distance(transform.position, exitSpawn.position) > 2 && !checkingView) checkingViewIE = StartCoroutine(CheckingView());

        if (exit && (Vector3.Distance(transform.position, exitSpawn.position) > 5 || Vector3.Distance(transform.position, exitSpawn.position) < 2) &&  checkingView)
        {
            StopCoroutine(checkingViewIE);
            checkingView = false;
        }

        if(exitSpawn && Vector3.Distance(transform.position,exitSpawn.position) < 1.25f && !ended)
        {
            SaveAndSendScore();
        }
    }

    private void SaveAndSendScore()
    {
        thisController.enabled = false;
        ended = true;
        endAnimation.SetTrigger("End");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        exitAudioSource.Stop();

        switch (difficulty)
        {
            case 1: scoreButPleaseDontHack = Mathf.CeilToInt(timerTime * 700); break;
            case 2: scoreButPleaseDontHack = Mathf.CeilToInt(timerTime * 1400); break;
            case 3: scoreButPleaseDontHack = Mathf.CeilToInt(timerTime * 1700); break;
            case 4: scoreButPleaseDontHack = Mathf.CeilToInt(timerTime * 2000); break;
            default:
                break;
        }
                
        scoreText.text = "Score: " + scoreButPleaseDontHack.ToString();
        if (scoreButPleaseDontHack > PlayerPrefs.GetInt("HS"))
        {
            PlayerPrefs.SetInt("HS", scoreButPleaseDontHack);            
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
            switch (Random.Range(0, 10))
            {
                case 1:
                case 2:// Friend appears behind you
                case 3:
                    SpawnEnemy(transform);
                    enemyBackstabWait = StartCoroutine(EnemyBackstabWait()); break;
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
        yield return new WaitForSeconds(7);
        exit = Instantiate(exits[Random.Range(0, exits.Length)]);
        exitSpawn = exitSpawnPoints[Random.Range(0, exitSpawnPoints.Length)];
        exit.transform.SetPositionAndRotation(exitSpawn.position, exitSpawn.rotation);
        exitRenderer = exit.GetComponent<Renderer>();
        exitAudioSource = exit.GetComponent<AudioSource>();
        tempCurrentClip = exitSounds[Random.Range(0, exitSounds.Length)];
        exitAudioSource.clip = tempCurrentClip;
        exitAudioSource.Play();
        StartCoroutine(SoundFade());
    }

    IEnumerator SoundFade()
    {
        yield return new WaitForSeconds(11);
        switch (difficulty)
        {
            case 1: yield return new WaitForSeconds(10); break;
            case 2: yield return new WaitForSeconds(20); break;
            case 3: yield return new WaitForSeconds(30); break;
            case 4: yield return new WaitForSeconds(60); break;
            default:
                break;
        }
        do tempCurrentClip2 = exitSounds[Random.Range(0, exitSounds.Length)];        
        while (tempCurrentClip == tempCurrentClip2);
        exitAudioSource.clip = tempCurrentClip2;
        exitAudioSource.Play();
        StartCoroutine(SoundFade());
    }

    IEnumerator EnemyBackstabWait()
    {
        hearWarning.SetActive(true);
        switch (difficulty)
        {
            case 1: yield return new WaitForSeconds(3.5f); break;
            case 2: yield return new WaitForSeconds(2.5f); break;
            case 3: yield return new WaitForSeconds(1.5f); break;
            case 4: yield return new WaitForSeconds(0.75f); break;
            default:
                break;
        }
        hearWarning.SetActive(false);
        if (!ended)
        {
            thisController.enabled = false;
            failPanel.SetActive(true);            
            Destroy(temp);
            deathNote.text = "Always watch your back!";
            if (Application.platform != RuntimePlatform.Android)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            thisAudioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)]);
            ended = true;
        }
    }

    IEnumerator EnemySeenWait()
    {
        detectionEye.sprite = seen;
        if (enemyBackstabWait != null)
        {
            StopCoroutine(enemyBackstabWait);
            hearWarning.SetActive(false);
        }
        switch (difficulty)
        {
            case 1: yield return new WaitForSeconds(2f); break;
            case 2: yield return new WaitForSeconds(1f); break;
            case 3: yield return new WaitForSeconds(0.75f); break;
            case 4: yield return new WaitForSeconds(0.5f); break;
            default:
                break;
        }
        if (!ended)
        {            
            thisController.enabled = false;
            failPanel.SetActive(true);
            if (Application.platform != RuntimePlatform.Android)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            Destroy(temp);
            thisAudioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)]);
            ended = true;
        }
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
