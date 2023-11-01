using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerEntity : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<Animator>().GetBool("isRespawned") != false)
                collision.gameObject.GetComponent<Animator>().SetTrigger("Death");
        }
    }
}
