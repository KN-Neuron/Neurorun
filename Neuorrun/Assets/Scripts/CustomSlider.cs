using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : Slider
{
    // Override the Set method to update the value without changing the handle and fill positions
    protected override void Set(float input, bool sendCallback)
    {
        // Update the value without updating the handle and fill positions
        m_Value = input;
    }

    // Method to update the handle position independently
    public void UpdateHandlePosition(float input)
    {
        // Update the handle position
        if (handleRect != null)
        {
            Vector2 anchoredPosition = handleRect.anchoredPosition;
            if (direction == Direction.LeftToRight || direction == Direction.RightToLeft)
            {
                anchoredPosition.x = input;
            }
            else
            {
                anchoredPosition.y = input;
            }
            handleRect.anchoredPosition = anchoredPosition;
        }
    }

    // Method to update the fill position independently
    public void UpdateFillPosition(float input)
    {
        // Update the fill position
        if (fillRect != null)
        {
            fillRect.anchorMax = (direction == Direction.LeftToRight || direction == Direction.RightToLeft) ? new Vector2(input, 1) : new Vector2(1, input);
        }
    }
}