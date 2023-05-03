using UnityEngine;

public class Interactor : MonoBehaviour
{
    public bool isInteracting = false;
    public Interactable _interectable { get; private set; }
    public Interactable.InteractableObject _interactableObject { get; private set; }



    public void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.CompareTag("Interactable"))
        {
            
            isInteracting = true;
            other.gameObject.GetComponent<Interactable>().enterZone();
            _interectable = other.gameObject.GetComponent<Interactable>();
            _interactableObject = other.gameObject.GetComponentInChildren<Interactable.InteractableObject>();
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            isInteracting = false;
            other.gameObject.GetComponent<Interactable>().exitZone();
            _interectable = null;
            _interactableObject = null;
        }
    }

    public void interact(float interactionProgress)
    {
        if (_interactableObject != null)
        {
            _interactableObject.interact(interactionProgress);
            setProgressBar(interactionProgress);
        }
    }

    public void setProgressBar(float progress)
    {
        if (_interectable != null)
        {
            _interectable.setProgressBar(progress);
        }
    }
}
