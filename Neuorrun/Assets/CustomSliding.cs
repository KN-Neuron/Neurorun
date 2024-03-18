using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class CustomSliding : MonoBehaviour
{
    [SerializeField] private RectMask2D mask;
    [SerializeField] private float fillSpeed = 0.5f;

    private Coroutine fillingCoroutine;

    public void StartFilling(float progress)
    {


        // Normalize progress to range [0, 1]
        progress = Mathf.Clamp01(progress);

        // Calculate the target right padding based on progress
        float targetRightPadding = (1f - progress) * mask.rectTransform.rect.width;

        // If there is already a coroutine running, stop it
        if (fillingCoroutine != null)
        {
            StopCoroutine(fillingCoroutine);
        }

        // Start a new coroutine to update filling to the new progress value
        fillingCoroutine = StartCoroutine(UpdateFillOverTime(targetRightPadding));
    }

    private IEnumerator UpdateFillOverTime(float targetRightPadding)
    {
        while (!Mathf.Approximately(mask.padding.z, targetRightPadding))
        {
            // Lerp the right padding of the RectMask2D towards the target value
            Vector4 padding = mask.padding;
            padding.z = Mathf.Lerp(padding.z, targetRightPadding, fillSpeed * Time.deltaTime);
            mask.padding = padding;

            // Wait until the next frame
            yield return null;
        }
    }





}
