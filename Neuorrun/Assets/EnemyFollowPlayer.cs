using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowPlayer : MonoBehaviour
{
    private Character player;
    private float walkingSpeed = 2f;
    private void Awake()
    {
        player = FindObjectOfType<Character>();
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.transform.position.x,transform.position.y), walkingSpeed * Time.deltaTime);
    }
}
