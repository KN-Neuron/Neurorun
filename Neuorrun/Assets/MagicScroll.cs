using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicScroll : MonoBehaviour, Interactable.InteractableObject
{
    [SerializeField] private float dissapearTime = 3f;

    private bool isInteracting = false;
    private float interactionProgress = 0f;
    private float interactionSpeed = 0.5f;
    private float interactionTime = 0f;
    private float interactionMaxTime = 2f;
    private bool isInteracted = false;
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
        throw new System.NotImplementedException();
    }

    public void interact(float interactionProgress)
    {
        throw new System.NotImplementedException();
    }

    public void reset()
    {
        throw new System.NotImplementedException();
    }
}
