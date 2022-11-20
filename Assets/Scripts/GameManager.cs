using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] int targetEnemyCount = 100;
    int spawnedEnemyCount;
    int currentEnemyCount;

    [SerializeField] float timeBetweenWaves = 5f;
    int currentWave;

    bool isWaveStarted = true;

    [SerializeField] TextMeshProUGUI playerHealth;
    [SerializeField] TextMeshProUGUI score;

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

    }

    // Update is called once per frame
    void Update()
    {
        if (spawnedEnemyCount >= targetEnemyCount && isWaveStarted)
        {
            if (currentEnemyCount <= 0)
            {
                Debug.Log("Wave cleared");
                PlayerManager.Instance.AdjustPlayerHealth(+20, null);
                isWaveStarted = false;
                StartCoroutine(StartWave());
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(0);
            Time.timeScale = 1;
        }
    }

    IEnumerator StartWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        ResetWaveValues();
        Debug.Log("New wave began");
    }

    void ResetWaveValues()
    {
        isWaveStarted = true;
        spawnedEnemyCount = 0;
        currentEnemyCount = 0;
        targetEnemyCount = 150;
    }

    public void UpdateUI()
    {
        playerHealth.text = PlayerManager.Instance.CurrentPlayerHealth.ToString();
        score.text = killedEnemyCount.ToString();
    }

}
