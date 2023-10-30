using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Character : MonoBehaviour
{


    //Implement death from getting squished by door
    public GameObject endScreen;

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
        doChecks();

        isInteracting();

        if (controller._isInteracting && interactor.isInteracting)
        {
            interactor.interact(_interactionProgress);
        }






    }

    private void isInteracting()
    {
        if (controller._isInteracting)
        {
            if (_interactionStartTime == -1f)
            {
                _interactionStartTime = Time.time;
            }

            if (_interactionProgress < 1f)
            {
                _interactionProgress = (Time.time - _interactionStartTime) / _interactionMaxDuration;
            }
            else
            {
                _interactionProgress = 1f;
            }
        }
        else if (_interactionStartTime != -1f)
        {
            _interactionStartTime = -1f;
            _interactionProgress = 0f;
        }
    }

    public void doChecks()
    {
        canInteract();
    }
    private void canInteract()
    {
        if (interactor.isInteracting)
        {
            controller._canInteract = true;
        }
        else
        {
            controller._canInteract = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Portal")
        {
            endScreen.GetComponent<EndMenu>().SetTime();
            endScreen.GetComponent<EndMenu>().PauseMenu();
           
           
            
        }
    }


}
