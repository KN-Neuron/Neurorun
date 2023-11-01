using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePlatform : MonoBehaviour, Interactable.InteractableObject
{

    [SerializeField] public GameObject finalDestination;
    [SerializeField] public GameObject startingPoint;
    [SerializeField] public BoxCollider2D platformCollider;

    [SerializeField] public GameObject player;
    [SerializeField] float risingSpeed = 4f;
    [SerializeField] float loweringSpeed = 1.5f;
    public bool isNegative = false;
    public Vector3 finalPosition;
    public Vector3 startingPosition;
    private bool isInteracting = false;
    private Interactor interactor;
    private Controller controller;

    private void Awake()
    {
        player = FindObjectOfType<Character>().gameObject;
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        startingPosition = startingPoint.transform.position;
        finalPosition = finalDestination.transform.position;
        transform.position = startingPosition;
        interactor = player.GetComponent<Interactor>();
        controller = player.GetComponent<Controller>();
    }

    public void interact(float interactionProgress)
    {
     Vector3 targetPosition = Vector3.Lerp(startingPosition, finalPosition, interactionProgress);
    float currentProgress = (transform.position - startingPosition).magnitude / (finalPosition - startingPosition).magnitude;

    if (interactionProgress >= currentProgress)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * risingSpeed * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, Mathf.Min(startingPosition.x, finalPosition.x), Mathf.Max(startingPosition.x, finalPosition.x));
        newPosition.y = Mathf.Clamp(newPosition.y, Mathf.Min(startingPosition.y, finalPosition.y), Mathf.Max(startingPosition.y, finalPosition.y));
        newPosition.z = Mathf.Clamp(newPosition.z, Mathf.Min(startingPosition.z, finalPosition.z), Mathf.Max(startingPosition.z, finalPosition.z));
        
        transform.position = newPosition;
    }
        
    }

    public void reset()
    {
        Vector3 newPosition = Vector3.MoveTowards(transform.position, startingPosition, loweringSpeed * Time.deltaTime);
        transform.position = newPosition;
    }

    private void Update()
    {
        isInteracting = controller._isInteracting && interactor.isInteracting;

        if ((transform.position.y != startingPosition.y || transform.position.x != startingPosition.x) && !isInteracting)
        {
            reset();
        }

    }

    public float getProgress()
    {
        float progress = Vector3.Distance(transform.position, finalPosition) / Vector3.Distance(startingPosition, finalPosition);
        return progress;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startingPosition, finalPosition);
    }

    Vector3 basePlayerScale;
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
