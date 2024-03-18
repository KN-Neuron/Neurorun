using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableEnemy : MonoBehaviour, Interactable.InteractableObject
{
    [SerializeField] private float timeToUse = 2f;
    private float interactionProgress = 0f;
    private Animator animator;
    private EnemyFollowPlayer enemyFollowPlayer;

    private bool stopHurting = false;


    private float timeToUseLeft;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        enemyFollowPlayer = GetComponentInParent<EnemyFollowPlayer>();
    }

    private void Start()
    {
        timeToUseLeft = timeToUse;
    }
    
    public float getProgress()
    {
        return 1f - interactionProgress;
    }

    public void interact(float interactionProgress)
    {
        this.interactionProgress = interactionProgress;
        Debug.Log("Interacting with enemy" + interactionProgress);
        if (this.interactionProgress >= 1)
        {
            if (timeToUseLeft > 0)
            {
                if (!stopHurting)
                {
                    stopHurting = true;
                    StartCoroutine(waitForHurt());
                }
                //Check if player is facing this object
                if (enemyFollowPlayer.isFacingPlayer())
                {
                    animator.SetBool("IsHurt", true);
                    enemyFollowPlayer.setHurt(true);
                    timeToUseLeft -= Time.deltaTime;
                }

               
            }
            else
            {
                Destroy(GetComponentInParent<Interactable>().gameObject);
            }
        }
        else
        {
            Debug.Log("Reset");
            animator.SetBool("IsHurt", false);
            enemyFollowPlayer.setHurt(false);
            timeToUseLeft = timeToUse;
        }
    }

    private void Update()
    {
    
    }

    IEnumerator waitForHurt()
    {
        yield return new WaitForSeconds(0.7f);
        Debug.Log("Reset");
        animator.SetBool("IsHurt", false);
        enemyFollowPlayer.setHurt(false);
        stopHurting = false;
    }

    public void reset()
    {
        throw new System.NotImplementedException();
    }




}
