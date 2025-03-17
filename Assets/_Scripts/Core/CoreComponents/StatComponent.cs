using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class StatComponent
{
    [HideInInspector] public string name { get; set; }
    [HideInInspector] public Entity entity { get; set; }
    public event Action OnCurrentValueMin;
    public event Action OnCurrentValueMax;

    [SerializeField] private Slider slider;

    [field: SerializeField] public float maxValue { get; private set; }
    [field: SerializeField] public float minValue { get; private set; }
    [field: SerializeField] public float currentValue { get; private set; }
    [field: SerializeField] public float recoveryStartTime { get; private set; }
    [field: SerializeField, Tooltip("Recovery delay time of -1 means it does not recover automatically (ex. health). Recovery delay time of 0 means it will recover every frame (ex. posture)")] public float recoveryDuration { get; private set; }
    [field: SerializeField] public float recoveryValue { get; private set; }

    private Timer recoveryStartTimer;

    private Timer recoveryTimer;

    private bool onRecovery;

    private float epsilon = 0.001f;

    public void Init()
    {
        recoveryStartTimer = new Timer(recoveryStartTime);
        recoveryStartTimer.timerAction += () => { onRecovery = true; recoveryTimer.ChangeStartTime(Time.time); };
        recoveryTimer = new Timer(recoveryDuration);
        recoveryTimer.timerAction += () => { IncreaseCurrentValue(recoveryValue); };
        recoveryTimer.StartMultiUseTimer();
    }

    public void Recovery()
    {
        recoveryStartTimer.Tick();

        if (recoveryDuration != -1 && onRecovery && currentValue < maxValue)
        {
            if (recoveryDuration == 0)
            {
                IncreaseCurrentValue(recoveryValue * Time.deltaTime);
            }
            else
            {
                recoveryTimer.Tick();
            }
        }
    }

    public void IncreaseCurrentValue(float amount, bool allowMaxValue = true)
    {
        currentValue += amount;
        currentValue = allowMaxValue ? Mathf.Clamp(currentValue, minValue, maxValue) : Mathf.Clamp(currentValue, minValue, maxValue - epsilon);
        SetSliderValue();

        if (currentValue == maxValue)
        {
            OnCurrentValueMax?.Invoke();
        }
    }

    public void DecreaseCurrentValue(float amount, bool allowMinValue = true)
    {
        currentValue -= amount;
        currentValue = allowMinValue ? Mathf.Clamp(currentValue, minValue, maxValue) : Mathf.Clamp(currentValue, minValue + epsilon, maxValue);
        SetSliderValue();
        onRecovery = false;
        recoveryStartTimer.StartSingleUseTimer();

        if (currentValue == 0.0f)
        {
            OnCurrentValueMin?.Invoke();
        }
    }

    public void IncreaseMaxValue(float amount)
    {
        maxValue += amount;
        IncreaseCurrentValue(amount);
        SetSliderValue();
    }

    public void DecreaseMaxValue(float amount)
    {
        maxValue -= amount;
        Mathf.Clamp(currentValue, 0.0f, maxValue);
        SetSliderValue();
    }

    private void SetSliderValue()
    {
        if (slider != null)
        {
            slider.value = currentValue / maxValue;
        }
    }
}
