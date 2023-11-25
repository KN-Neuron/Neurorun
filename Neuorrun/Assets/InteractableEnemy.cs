using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableEnemy : MonoBehaviour, Interactable.InteractableObject
{
    private float interactionProgress = 0f;

    public float getProgress()
    {
        return 1 - interactionProgress;
    }

    public void interact(float interactionProgress)
    {
        this.interactionProgress = interactionProgress;
        if (interactionProgress >= 1)
        {
            Destroy(GetComponentInParent<Interactable>().gameObject);
        }
    }

    public void reset()
    {
        throw new System.NotImplementedException();
    }




}
