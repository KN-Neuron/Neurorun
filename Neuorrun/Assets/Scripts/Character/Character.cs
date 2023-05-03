using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : MonoBehaviour
{

    //Implement death from getting squished by door

    public float _interactionProgress = 0f;
    public float _interactionMaxDuration = 2f;
    public float _interactionStartTime = -1f;

    Controller controller;
    Interactor interactor;
    
    private void Awake()
    {
        controller = GetComponent<Controller>();
        interactor = GetComponent<Interactor>();

    }

    public void Update()
    {
        if (interactor.isInteracting)
        {
            controller._canInteract = true;
        }
        else
        {
            controller._canInteract = false;
        }

        if(controller._isInteracting)
        {
            if (_interactionStartTime == -1f)
            {
                _interactionStartTime = Time.time;
            }
  
            if (_interactionProgress < 1f)
            {
                _interactionProgress = (Time.time - _interactionStartTime)/ _interactionMaxDuration;
            }
            else
            {
                _interactionProgress = 1f;
            }
        }
        else if(_interactionStartTime !=-1f)
        {
            _interactionStartTime = -1f;
            _interactionProgress = 0f;
        }

/*        Debug.Log(_interactionProgress);*/


        if (controller._isInteracting && interactor.isInteracting)
        {
            interactor.interact(_interactionProgress);
        }


        
      

    }
}
