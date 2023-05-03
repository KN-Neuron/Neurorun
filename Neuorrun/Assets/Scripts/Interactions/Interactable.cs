using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    /* Implement reacting for the player triggering the zone collider and then progress the interaction progress bar on parent object*/

    [SerializeField] protected GameObject progressBar;




    public interface InteractableObject
    {

        void interact(float interactionProgress);

        void reset();

        float getProgress();
    }

    public void enterZone()
    {
        progressBar.SetActive(true);
        Debug.Log("Player entered zone");
    }

    public void exitZone()
    {
        progressBar.SetActive(false);
        Debug.Log("Player exited zone");
    }

    public void setProgressBar(float progress)
    {
        progressBar.GetComponentInChildren<ProgressBar>().setProgress(progress);
    }




}
