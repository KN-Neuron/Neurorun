using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PopupMenu : MonoBehaviour
{
    public bool isPressActivated = false;


    public void showMenu()
    {
      
        gameObject.SetActive(true);
        Button firstButton = GetComponentInChildren<Button>();
      
        if (firstButton != null)
        {
            firstButton.Select();
        }
    }

    public void hideMenu()
    {
        gameObject.SetActive(false);
    }

    public void PauseMenu(InputAction.CallbackContext context)
    {
        if(isPressActivated)
        {
            if (context.started)
            {
             
                if (Time.timeScale == 1)
                {
                    showMenu();
                   
                    Time.timeScale = 0;
                   

                }
                else
                {
                    Time.timeScale = 1;
                    hideMenu();
                }
            }
        }
    
    }

    public void PauseMenu()
    {
        if (Time.timeScale == 1)
        {
            showMenu();
            

           
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            hideMenu();
        }
    }



}
