using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class EndMenu : PopupMenu
{
    public TextMeshProUGUI endScreenText;
    public string endText = @"Congratulations you finished this level in:

";


    public void SetTime()
    {
        endScreenText.text = FindObjectOfType<Timer>().GetTime();
    }
}
