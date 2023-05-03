using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour, Interactable.InteractableObject
{
   

    [SerializeField] public GameObject finalDestination;
    public Vector3 finalPosition;
    public Vector3 startingPosition;
    private bool _isResetting;


    


    private void Awake()
    {
        startingPosition = transform.position;
        finalPosition = finalDestination.transform.position;

    }
    public void interact(float interactionProgress)
    {

        if (transform.position.y < finalPosition.y *interactionProgress)
            /* transform.position = Vector2.MoveTowards(transform.position, new Vector2(finalPosition.x, finalPosition.y * interactionProgress), 0.1f);*/
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(finalPosition.x, finalPosition.y * interactionProgress), 4f * Time.deltaTime);
        /* transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, finalPosition.y * interactionProgress / finalPosition.y), 5f);*/
        /*        Debug.Log("Player interacted with door" + interactionProgress);*/


    }

    public void reset()
    {

        /*transform.position = Vector2.Lerp(transform.position, startingPosition, 1f);*/
        /*        transform.position = Vector2.Lerp(transform.position,startingPosition, 12f);
/*       
        /*        Debug.Log("Player reset door");*/
        transform.position = Vector2.MoveTowards(transform.position, startingPosition, 1.5f * Time.deltaTime);
    }

    private void Update()
    {

       
        if( transform.position.y > startingPosition.y)
        {
            reset();
        }



       

    }

    public float getProgress()
    {
        return Vector2.Distance(transform.position, finalPosition) / Vector2.Distance(startingPosition, finalPosition);
    }
}
