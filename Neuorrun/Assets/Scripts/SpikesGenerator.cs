using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesGenerator : MonoBehaviour
{
    public int numberOfSpikes = 10;
    public GameObject spikePrefab;

    private void Awake()
    {
        Init();
    }
    
    private void Init()
    {
        for (int i = 0; i < numberOfSpikes; i++)
        {
            GameObject spike = Instantiate(spikePrefab, transform);
            var spikeWidth = spike.GetComponent<SpriteRenderer>().bounds.size.x;
            spike.transform.localPosition = new Vector3(i * spikeWidth, 0,0 );
        }
    }
}
