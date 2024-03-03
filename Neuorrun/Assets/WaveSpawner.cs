using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "WaveSettings", menuName = "Enemy Waves/New Wave", order = 1)]
public class WaveSettings : ScriptableObject
{
    public GameObject enemyPrefab;
    public int numberOfEnemiesLeft;
    public int numberOfEnemiesRight;
    public GameObject leftSpawnPoint;
    public GameObject rightSpawnPoint;
    public float timeIntervalBetweenSpawns = 1.0f;
    public bool spawnAllAtOnce = false;
}

public class WaveSpawner : MonoBehaviour
{
    public List<WaveSettings> waves = new List<WaveSettings>();

    private void Start()
    {
       
    }

    private IEnumerator SpawnWaves()
    {
        while (true)  // Keep spawning waves indefinitely
        {
            WaveSettings randomWave = GetRandomWave();
            SpawnWave(randomWave);
            yield return new WaitForSeconds(randomWave.timeIntervalBetweenSpawns);
        }
    }

    public void SpawnRandomWave()
    {
        WaveSettings randomWave = GetRandomWave();
        SpawnWave(randomWave);
    }

    private void SpawnWave(WaveSettings wave)
    {
        if (wave.spawnAllAtOnce)
        {
            SpawnAllAtOnce(wave);
        }
        else
        {
            SpawnWithInterval(wave);
        }
    }

    private void SpawnAllAtOnce(WaveSettings wave)
    {
        SpawnEnemies(wave.leftSpawnPoint.transform, wave.numberOfEnemiesLeft, wave.enemyPrefab);
        SpawnEnemies(wave.rightSpawnPoint.transform, wave.numberOfEnemiesRight, wave.enemyPrefab);
        Debug.Log("Spawned " + wave.numberOfEnemiesLeft + " enemies on the left and " + wave.numberOfEnemiesRight + " enemies on the right.");
    }

    private void SpawnWithInterval(WaveSettings wave)
    {
        StartCoroutine(SpawnEnemiesWithInterval(wave.leftSpawnPoint.transform, wave.numberOfEnemiesLeft, wave.enemyPrefab, wave.timeIntervalBetweenSpawns));
        StartCoroutine(SpawnEnemiesWithInterval(wave.rightSpawnPoint.transform, wave.numberOfEnemiesRight, wave.enemyPrefab, wave.timeIntervalBetweenSpawns));
    }

    private void SpawnEnemies(Transform spawnPoint, int numberOfEnemies, GameObject enemyPrefab)
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    private IEnumerator SpawnEnemiesWithInterval(Transform spawnPoint, int numberOfEnemies, GameObject enemyPrefab, float timeInterval)
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            yield return new WaitForSeconds(timeInterval);
        }
    }

    private WaveSettings GetRandomWave()
    {
        return waves[Random.Range(0, waves.Count)];
    }
}
