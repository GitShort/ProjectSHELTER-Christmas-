using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] BoxCollider[] _SpawnPoints;
    Vector3[] _SpawnPointsSize;
    Vector3[] _SpawnPointsCenter;

    static readonly System.Random rnd = new System.Random();

    [SerializeField] GameObject[] Enemies;
    int _chosenSpawnPoint;
    [SerializeField] float _timeBetweenSpawns = 4f;
    float _timer = 0f;
    bool _isTimerStarted = false;
    bool _isTimerFinished = false;

    int _spawnCount = 0;

    private void Awake()
    {
        _SpawnPointsSize = new Vector3[_SpawnPoints.Length];
        _SpawnPointsCenter = new Vector3[_SpawnPoints.Length];
        CheckSpawnPositions();
    }

    void Update()
    {
        EnemySpawnTime();

        if (_isTimerFinished)
        {
            _isTimerStarted = false;
            _isTimerFinished = false;
        }

    }

    void EnemySpawnTime()
    {
        if (!_isTimerStarted && !_isTimerFinished)
        {
            _timer = 0f;
            _isTimerStarted = true;
        }
        if (_isTimerStarted && !_isTimerFinished)
        {
            _timer += Time.deltaTime;
            //Debug.Log(_timer);
            if (_timeBetweenSpawns <= _timer)
            {
                SpawnEnemies();
                _isTimerFinished = true;
            }
        }
    }

    void SpawnEnemies()
    {
        _spawnCount = rnd.Next(Enemies.Length) + 1;
        for (int i = 0; i < _spawnCount; i++)
        {
            _chosenSpawnPoint = rnd.Next(_SpawnPoints.Length);
            Instantiate(Enemies[i], GetRandomPosition(_chosenSpawnPoint), transform.rotation);
        }
    }

    void CheckSpawnPositions()
    {
        for (int i = 0; i < _SpawnPoints.Length; i++)
        {
            _SpawnPointsCenter[i] = _SpawnPoints[i].transform.position;
            _SpawnPointsSize[i].x = _SpawnPoints[i].transform.localScale.x * _SpawnPoints[i].size.x;
            _SpawnPointsSize[i].z = _SpawnPoints[i].transform.localScale.z * _SpawnPoints[i].size.z;
        }
    }

    Vector3 GetRandomPosition(int index)
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-_SpawnPointsSize[index].x / 2, _SpawnPointsSize[index].x / 2),
            _SpawnPointsCenter[index].y,
            Random.Range(-_SpawnPointsSize[index].z / 2, _SpawnPointsSize[index].z / 2));

        return new Vector3(_SpawnPointsCenter[index].x + randomPos.x, randomPos.y, _SpawnPointsCenter[index].z + randomPos.z);
    }
}
