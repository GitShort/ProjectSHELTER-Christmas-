using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] int targetEnemyCount = 100;
    int spawnedEnemyCount;
    int currentEnemyCount;

    [SerializeField] float timeBetweenWaves = 5f;
    int currentWave;

    bool isWaveStarted = true;

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
                isWaveStarted = false;
                StartCoroutine(StartWave());
            }
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


}
