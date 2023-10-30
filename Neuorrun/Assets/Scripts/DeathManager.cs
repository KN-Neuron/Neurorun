using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public LayerMask _groundLayer;
    public Vector3 _lastGroundPosition;
    public float _groundCheckLength=1f;
   

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _groundCheckLength, _groundLayer);
        if (hit.collider != null)
        {
            Debug.Log("Hit" + transform.position);
            _lastGroundPosition = transform.position;
            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, Vector2.down * _groundCheckLength);
    }

    public void Kill()
    {
        GetComponent<Animator>().SetBool("isRespawned", false);
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        GameObject.FindGameObjectsWithTag("Aura")[0].GetComponent<SpriteRenderer>().enabled = false;



    }

    public void StartRespawn()
    {
        transform.position = _lastGroundPosition;
        GetComponent<Animator>().SetTrigger("Respawn");
        
    }
    
    public void Respawn()
    {

        GetComponent<Animator>().SetBool("isRespawned", true);
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        GameObject.FindGameObjectsWithTag("Aura")[0].GetComponent<SpriteRenderer>().enabled = true;

    }
}
