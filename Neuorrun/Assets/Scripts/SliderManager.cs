using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SliderManager : MonoBehaviour, ISelectHandler
{
    public TextMeshProUGUI deviceData;
    private Slider slider;
    public float sliderSpeed = 0.1f;
    private bool isSelected;
    private float _horizontalInput=0f;

    private const string SensitivityKey = "deviceSensitivity";

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.value = PlayerPrefs.GetInt(SensitivityKey, 40);
        OnSliderValueChanged();
    }

    public void OnSliderValueChanged()
    {
        float sliderValue = slider.value;
        deviceData.text = sliderValue.ToString();
        PlayerPrefs.SetInt(SensitivityKey, (int)sliderValue);
    }

    public void Move(InputAction.CallbackContext context)
    {

        _horizontalInput = context.ReadValue<Vector2>().x;
    
    }
    
    void Update()
    {
        if (isSelected)
        {
            /*float horizontalInput = Input.GetAxis("Horizontal");*/
            slider.value += _horizontalInput * sliderSpeed;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
    }
}
