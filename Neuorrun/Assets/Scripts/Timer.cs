using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float elapsedTime = 0f;
    public TextMeshProUGUI timerText;

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);
        int milliseconds = (int)((elapsedTime * 1000) % 1000);

        UpdateTimerText(minutes, seconds, milliseconds);
    }

    private void UpdateTimerText(int minutes, int seconds, int milliseconds)
    {
        timerText.SetText($"{minutes:00}:{seconds:00}:{milliseconds:000}");
    }

    public string GetTime()
    {
        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);
        int milliseconds = (int)((elapsedTime * 1000) % 1000);
        string time = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
        return time;
    }


}
