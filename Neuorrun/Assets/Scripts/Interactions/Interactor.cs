using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public bool isInteracting { get; private set; } = false;
    private List<Interactable> interactables = new List<Interactable>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            Interactable interactable = other.gameObject.GetComponent<Interactable>();
            interactables.Add(interactable);
            Debug.Log("Added interactable to list" + interactable);
            interactable.enterZone();
            isInteracting = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            Interactable interactable = other.gameObject.GetComponent<Interactable>();
            interactables.Remove(interactable);
            interactable.exitZone();
            isInteracting = (interactables.Count > 0);
        }
    }

    public void interact(float interactionProgress)
    {
        List<Interactable> tempList = new List<Interactable>(interactables);

        foreach (Interactable interactable in tempList)
        {
            Interactable.InteractableObject interactableObject = interactable.GetComponentInChildren<Interactable.InteractableObject>();
            if (interactableObject != null)
            {
                interactableObject.interact(interactionProgress);
                interactable.setProgressBar(interactionProgress);
            }
        }
    }

    public void setProgressBar(float progress)
    {
        foreach (Interactable interactable in interactables)
        {
            interactable.setProgressBar(progress);
        }
    }
}