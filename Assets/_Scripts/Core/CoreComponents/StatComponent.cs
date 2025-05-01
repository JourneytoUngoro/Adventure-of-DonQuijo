using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class StatComponent
{
    public string name { get; private set; }
    public Entity entity { get; private set; }

    public event Action OnCurrentValueMin;
    public event Action OnCurrentValueMax;
    public event Action OnCurrentValueChange;

    [SerializeField] private Slider slider;

    [field: SerializeField] public float maxValue { get; private set; }
    [field: SerializeField] public float minValue { get; private set; }
    [field: SerializeField] public float currentValue { get; private set; }
    [field: SerializeField] public bool enableRecovery { get; private set; }
    [field: SerializeField] public float recoveryStartTime { get; private set; }
    [field: SerializeField, Tooltip("Recovery duration of -1 means it does not recover automatically (ex. health). Recovery duration of 0 means it will recover every frame.")] public float recoveryDuration { get; private set; }
    [field: SerializeField] public float recoveryValue { get; private set; }
    /*[field: SerializeField] public AnimationCurve incrementPerLevel { get; private set; }
    [field: SerializeField] public AnimationCurve accumulationPerLevel { get; private set; }*/
    [field: SerializeField] public AnimationCurveSet graph { get; private set; }

    private Timer recoveryStartTimer;
    private Timer recoveryTimer;
    private bool onRecovery;
    private float epsilon = 0.001f;

    public void Init(Entity entity, string name)
    {
        this.entity = entity;
        this.name = name;
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
        if (currentValue < maxValue)
        {
            currentValue += amount;
            currentValue = allowMaxValue ? Mathf.Clamp(currentValue, minValue, maxValue) : Mathf.Clamp(currentValue, minValue, maxValue - epsilon);
            SetSliderValue();
            OnCurrentValueChange?.Invoke();

            if (currentValue == maxValue)
            {
                OnCurrentValueMax?.Invoke();
            }
        }
    }

    public void DecreaseCurrentValue(float amount, bool allowMinValue = true)
    {
        if (currentValue > minValue)
        {
            currentValue -= amount;
            currentValue = allowMinValue ? Mathf.Clamp(currentValue, minValue, maxValue) : Mathf.Clamp(currentValue, minValue + epsilon, maxValue);
            SetSliderValue();
            OnCurrentValueChange?.Invoke();
            onRecovery = false;
            recoveryStartTimer.StartSingleUseTimer();

            if (currentValue == minValue)
            {
                OnCurrentValueMin?.Invoke();
            }
        }
    }

    public void SetCurrentValue(float value)
    {
        currentValue = Mathf.Clamp(value, minValue, maxValue);
        SetSliderValue();
        OnCurrentValueChange?.Invoke();

        if (currentValue == maxValue)
        {
            OnCurrentValueMax?.Invoke();
        }

        if (currentValue == minValue)
        {
            OnCurrentValueMin?.Invoke();
        }
    }

    public void SetMaxValue(float value)
    {
        maxValue = value;
        SetSliderValue();
        OnCurrentValueChange?.Invoke();
    }

    public void IncreaseMaxValue(float amount)
    {
        maxValue += amount;
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
