using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public EndMenu _endMenu;
    bool _isPlayerInZone = false;
    public GameObject _player;


    void EndGame()
    {
        _endMenu.SetTime();
        _endMenu.showMenu();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Rigidbody2D>().simulated = false;
            _isPlayerInZone = true;
            _player = other.gameObject;
            other.GetComponent<Animator>().SetBool("Exit",true);
            GetComponent<Animator>().SetTrigger("Close");
        
        }
    }

    void Update()
    {
        if (_isPlayerInZone)
        {
            _player.transform.position = Vector3.MoveTowards(_player.transform.position, transform.position, 0.01f);
        }
    }

    public void Vanish()
    {
        
    }
}
