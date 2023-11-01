using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour, Interactable.InteractableObject
{
   

    [SerializeField] public GameObject finalDestination;
    [SerializeField] public GameObject startingPoint;
    [SerializeField] public BoxCollider2D platformCollider;
    [SerializeField] public BoxCollider2D interactionSpace;
    [SerializeField] public GameObject player;
    [SerializeField] float risingSpeed = 4f;
    [SerializeField] float loweringSpeed = 1.5f;


    public Vector3 finalPosition;
    public Vector3 startingPosition;
    public Vector3 progressFinalPos;
    public Vector3 progressStartPos;
    private float topEdgeFinalPos, bottomEdgeFinalPos;
    private bool isInteracting = false;
    private Interactor interactor;
    private Controller controller;





    private void Awake()
    {
        player = FindObjectOfType<Character>().gameObject;
        GetComponent<SpriteRenderer>().enabled = true;
        startingPosition = startingPoint.transform.position;
        finalPosition = finalDestination.transform.position;
        topEdgeFinalPos = finalPosition.y - platformCollider.bounds.extents.y;
        bottomEdgeFinalPos = startingPosition.y + platformCollider.bounds.extents.y;
        transform.position = new Vector2(startingPosition.x,bottomEdgeFinalPos);
        progressFinalPos = new Vector3(finalPosition.x, topEdgeFinalPos, finalPosition.z);
        progressStartPos = new Vector3(startingPosition.x, bottomEdgeFinalPos, startingPosition.z);
        interactor = player.GetComponent<Interactor>();
        controller = player.GetComponent<Controller>();


    }
    public void interact(float interactionProgress)
    {
        
        if (transform.position.y != finalPosition.y *interactionProgress)
        {
            Vector2 newPositon = Vector2.MoveTowards(new Vector2(startingPosition.x, transform.position.y), new Vector2(startingPosition.x, finalPosition.y * interactionProgress), risingSpeed* Time.deltaTime);
            transform.position = new Vector2(newPositon.x, Mathf.Min(newPositon.y, topEdgeFinalPos));
        }
          


    }

    public void reset()
    {
        Vector2 newPositon = Vector2.MoveTowards(new Vector2(startingPosition.x, transform.position.y), startingPosition, loweringSpeed * Time.deltaTime);
        transform.position = new Vector2(newPositon.x, Mathf.Max(newPositon.y, bottomEdgeFinalPos));
    }

    private void Update()
    {

        isInteracting = controller._isInteracting && interactor.isInteracting;

        if ( transform.position.y != startingPosition.y && !isInteracting)
        {
            reset();
        }



  

    }

    public float getProgress()
    {
        float progress = Vector2.Distance(transform.position, progressFinalPos) / Vector2.Distance(progressFinalPos, progressStartPos);
        return progress;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startingPosition, finalPosition);
    }

    GameObject _player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {





            collision.gameObject.GetComponent<Controller>().isOnMoving = true;
        }
    }

    Vector3 offset;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _player = collision.gameObject;
            offset = collision.transform.position - transform.position;
        }
    }



    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _player = null;

            collision.gameObject.GetComponent<Controller>().isOnMoving = false;
        }
    }

    private void LateUpdate()
    {
        if (_player != null)
        {
            _player.transform.position = transform.position + offset;
        }
    }


}
