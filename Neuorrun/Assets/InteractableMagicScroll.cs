using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableMagicScroll : MonoBehaviour, Interactable.InteractableObject
{
    [SerializeField] private float dissapearTime = 3f;

    private float interactionProgress = 0f;
    private float dissapearTimeLeft;

    private void Awake()
    {
        dissapearTimeLeft = dissapearTime;
    }

    private void Update()
    {
            dissapearTimeLeft -= Time.deltaTime;
            if (dissapearTimeLeft <= 0)
            {
                Destroy(gameObject);
            }
        
    }

    public float getProgress()
    {
        return 1-interactionProgress;
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
        interactionProgress = 0f;
    }
}
