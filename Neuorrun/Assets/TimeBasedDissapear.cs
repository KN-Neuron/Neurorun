using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBasedDissapear : MonoBehaviour
{
    [SerializeField] private float timeToDissapear = 5f;

    private void Start()
    {
        StartCoroutine(Dissapear());
    }

    private IEnumerator Dissapear()
    {
        GetComponentInChildren<SpriteRenderer>().DOFade(0, timeToDissapear);
        yield return new WaitForSeconds(timeToDissapear);
        Destroy(gameObject);
    }

}
