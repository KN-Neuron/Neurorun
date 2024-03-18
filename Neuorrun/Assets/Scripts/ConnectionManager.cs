using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    public Color noConnection;
    public Color weakConnection;
    public Color midConnection;
    public Color bestConnection;
    public Controller controller;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        controller = FindObjectOfType<Controller>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void SetConnectionState(int poorSignalLevel, bool isConnected)
    {
        if (!isConnected)
        {
            gameObject.GetComponent<Image>().color = noConnection;
        }
        else if (poorSignalLevel >= 100)
        {
            gameObject.GetComponent<Image>().color = weakConnection;
        }
        else if (poorSignalLevel >= 25)
        {
            gameObject.GetComponent<Image>().color = midConnection;
        }
        else
        {
            gameObject.GetComponent<Image>().color = bestConnection;
        }

    }
}
