using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] int targetEnemyCount = 100;
    int spawnedEnemyCount;
    int currentEnemyCount;

    [SerializeField] float timeBetweenWavesSet = 5f;
    [SerializeField] float startingTime = 15f;
    float timeBetweenWavesUse = 0f;
    float timeBeforeWave = 0f;
    bool timerOn = false;
    int currentWave;

    bool isWaveStarted = true;

    [SerializeField] TextMeshProUGUI playerHealth;
    [SerializeField] TextMeshProUGUI score;
    [SerializeField] TextMeshProUGUI enemiesLeft;
    [SerializeField] TextMeshProUGUI timeBeforeWaveUI;
    [SerializeField] TextMeshProUGUI nextWaveInTEXT;
    [SerializeField] Image deadImage;
    AudioSource deathSound;
    [SerializeField] AudioSource playerDeathSplatSound;
    [SerializeField] GameObject playerDeathSplat;

    public int CurrentEnemyCount
    {
        get { return currentEnemyCount; }
        set { currentEnemyCount = value; }
    }

    public int SpawnedEnemyCount
    {
        get { return spawnedEnemyCount; }
        set { spawnedEnemyCount = value; }
    }

    public int TargetEnemyCount
    {
        get { return targetEnemyCount; }
        set { targetEnemyCount = value; }
    }

    int killedEnemyCount = 0;

    public int KilledEnemyCount
    {
        get { return killedEnemyCount; }
        set { killedEnemyCount = value; }
    }

    public int GetCurrentWave
    {
        get { return currentWave; }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        timeBeforeWave = startingTime;
        timeBetweenWavesUse = startingTime;
        deathSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnedEnemyCount >= targetEnemyCount && isWaveStarted)
        {
            if (currentEnemyCount <= 0)
            {
                Debug.Log("Wave cleared");
                isWaveStarted = false;
                timerOn = true;
                StartCoroutine(StartWave());
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);

            deadImage.gameObject.SetActive(false);
        }
        DisplayTimeBeforeWave();
    }

    IEnumerator StartWave()
    {
        
        yield return new WaitForSeconds(timeBetweenWavesUse);
        ResetWaveValues();
        Debug.Log("New wave began");
    }
    
    void DisplayTimeBeforeWave()
    {
        if (timerOn)
        {
            if (timeBeforeWave > 1)
            {
                if (!nextWaveInTEXT.gameObject.activeInHierarchy)
                    nextWaveInTEXT.gameObject.SetActive(true);
                timeBeforeWave -= Time.deltaTime;
                timeBeforeWaveUI.text = Mathf.RoundToInt(timeBeforeWave).ToString();
            }
            else
            {
                timeBeforeWaveUI.text = "";
                nextWaveInTEXT.gameObject.SetActive(false);
                if (timeBetweenWavesUse != timeBetweenWavesSet)
                    timeBetweenWavesUse = timeBetweenWavesSet;
                timeBeforeWave = timeBetweenWavesUse;
                timerOn = false;
            }
        }
    }

    void ResetWaveValues()
    {
        isWaveStarted = true;
        currentWave++;
        Debug.Log("CURRENT WAVE: " + currentWave);
        spawnedEnemyCount = 0;
        currentEnemyCount = 0;
        if (currentWave == 4)
            targetEnemyCount -= 15;
        else if (currentWave == 8)
            targetEnemyCount -= 30;
        else if (currentWave == 12)
            targetEnemyCount -= 30;
        TargetEnemyCount += 8;
    }

    public void UpdateUI(bool isScore)
    {
        playerHealth.text = PlayerManager.Instance.CurrentPlayerHealth.ToString();
        if (PlayerManager.Instance.CurrentPlayerHealth <= 0)
        {
            StartCoroutine(DeathScreen());
            PlayerManager.Instance.gameObject.SetActive(false);
            GameObject playerSplat = Instantiate(playerDeathSplat, PlayerManager.Instance.transform.position, transform.rotation);
            playerDeathSplat.GetComponentInChildren<VisualEffect>().Play();
            playerDeathSplatSound.Play();
        }

        if (isScore)
        {
            score.gameObject.GetComponent<Animator>().Play("UIUpdate");
            score.text = killedEnemyCount.ToString();
        }
        //enemiesLeft.text = currentEnemyCount.ToString();
    }

    IEnumerator DeathScreen()
    {
        yield return new WaitForSeconds(2f);
        deadImage.gameObject.SetActive(true);
        deathSound.Play();
    }

}
