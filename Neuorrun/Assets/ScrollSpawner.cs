using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollSpawner : MonoBehaviour
{
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private GameObject scrollPrefab;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float spawnIntervalRandomness = 0.5f;

    private BoxCollider2D _spawnArea;

    void Awake()
    {
        _spawnArea = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        StartCoroutine(SpawnScrolls());
    }
  

    private IEnumerator SpawnScrolls()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval + Random.Range(-spawnIntervalRandomness, spawnIntervalRandomness));
            var portalSpawnPosition = GetRandomSpawnPosition();

            var scrollSpawnPosition = portalSpawnPosition + new Vector2(0, 0.3f);

            Instantiate(portalPrefab, portalSpawnPosition, portalPrefab.transform.rotation);
            Instantiate(scrollPrefab, scrollSpawnPosition, Quaternion.identity);
        }
    }

    private Vector2 GetRandomSpawnPosition()
    {
        var spawnAreaSize = _spawnArea.size;
        var spawnAreaCenter = _spawnArea.transform.position;

        var spawnAreaLeft = spawnAreaCenter.x - spawnAreaSize.x / 2;
        var spawnAreaRight = spawnAreaCenter.x + spawnAreaSize.x / 2;
        var spawnAreaBottom = spawnAreaCenter.y - spawnAreaSize.y / 2;
        var spawnAreaTop = spawnAreaCenter.y + spawnAreaSize.y / 2;

        var spawnPosition = new Vector2(Random.Range(spawnAreaLeft, spawnAreaRight), Random.Range(spawnAreaBottom, spawnAreaTop));

        return spawnPosition;
    }
}
