using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowPlayer : MonoBehaviour
{
    private Character player;
    [SerializeField] private float walkingSpeed = 2f;
    private void Awake()
    {
        player = FindObjectOfType<Character>();
    }

    private bool isFlipped = false;

    private void Update()
    {
        // Flip the enemy sprite based on the player's position
        if (player.transform.position.x > transform.position.x)
        {
            // Player is to the right, flip the sprite
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            // Player is to the left, keep the default orientation
            transform.localScale = new Vector3(1, 1, 1);
        }


        transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.transform.position.x,transform.position.y), walkingSpeed * Time.deltaTime);
    }
}
