using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TimerControl { Stop, Start }

[Serializable]
public class StatComponent
{
    public string name { get; private set; }
    public Entity entity { get; private set; }

    public event Action OnCurrentValueMin;
    public event Action OnCurrentValueMax;
    public event Action OnCurrentValueChange;
    public event Action OnMaxValueChange;

    [SerializeField] private bool reverseSlider;
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
        if (enableRecovery)
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
    }

    public void ControlRecoveryTimer(TimerControl timerControl)
    {
        switch (timerControl)
        {
            case TimerControl.Stop:
                recoveryStartTimer.StopTimer(); break;
            case TimerControl.Start:
                recoveryStartTimer.StartSingleUseTimer(); break;
            default:
                Debug.LogWarning($"Unknown type of TimerControl variable found in in {entity.name}.");
                break;
        }
    }

    public void IncreaseCurrentValue(float amount, bool allowMaxValue = true, bool resetRecovery = false, bool invoke = true)
    {
        if (currentValue < maxValue)
        {
            currentValue += amount;
            currentValue = allowMaxValue ? Mathf.Clamp(currentValue, minValue, maxValue) : Mathf.Clamp(currentValue, minValue, maxValue - epsilon);
            SetSliderValue();

            if (invoke)
            {
                OnCurrentValueChange?.Invoke();
            }

            if (resetRecovery)
            {
                onRecovery = false;
                recoveryStartTimer.StartSingleUseTimer();
            }

            if (currentValue == maxValue)
            {
                OnCurrentValueMax?.Invoke();
            }
        }
    }

    public void DecreaseCurrentValue(float amount, bool allowMinValue = true, bool resetRecovery = true, bool invoke = true)
    {
        if (currentValue > minValue)
        {
            currentValue -= Mathf.Abs(amount);
            currentValue = allowMinValue ? Mathf.Clamp(currentValue, minValue, maxValue) : Mathf.Clamp(currentValue, minValue + epsilon, maxValue);
            SetSliderValue();
            
            if (invoke)
            {
                OnCurrentValueChange?.Invoke();
            }

            if (resetRecovery)
            {
                onRecovery = false;
                recoveryStartTimer.StartSingleUseTimer();
            }

            if (currentValue == minValue)
            {
                OnCurrentValueMin?.Invoke();
            }
        }
    }

    public void SetCurrentValue(float value, bool allowExtremeValues = true, bool? resetRecovery = null, bool invoke = true)
    {
        currentValue = allowExtremeValues ? Mathf.Clamp(value, minValue, maxValue) : Mathf.Clamp(value, minValue + epsilon, maxValue - epsilon);
        SetSliderValue();

        if (invoke)
        {
            OnCurrentValueChange?.Invoke();
        }

        if (resetRecovery.HasValue)
        {
            onRecovery = !resetRecovery.Value;

            if (resetRecovery.Value)
            {
                recoveryStartTimer.StartSingleUseTimer();
            }
        }
        else
        {
            onRecovery = value < currentValue;

            if (!onRecovery)
            {
                recoveryStartTimer.StartSingleUseTimer();
            }
        }

        if (currentValue == maxValue)
        {
            OnCurrentValueMax?.Invoke();
        }

        if (currentValue == minValue)
        {
            OnCurrentValueMin?.Invoke();
        }
    }

    public void SetMaxValue(float value, bool maxValueChangeInvoke = true, bool influenceCurrentValue = false, bool allowExtremeValues = true, bool? resetRecovery = null, bool currentValueInvoke = true)
    {
        if (influenceCurrentValue)
        {
            float diffValue = value - currentValue;

            if (diffValue < 0)
            {
                if (resetRecovery.HasValue)
                {
                    DecreaseCurrentValue(diffValue, allowExtremeValues, resetRecovery.Value, currentValueInvoke);
                }
                else
                {
                    DecreaseCurrentValue(diffValue, allowExtremeValues, true, currentValueInvoke);
                }
            }
            else if (diffValue > 0)
            {
                if (resetRecovery.HasValue)
                {
                    IncreaseCurrentValue(diffValue, allowExtremeValues, resetRecovery.Value, currentValueInvoke);
                }
                else
                {
                    IncreaseCurrentValue(diffValue, allowExtremeValues, false, currentValueInvoke);
                }
            }
        }

        maxValue = value;
        SetSliderValue();

        if (maxValueChangeInvoke)
        {
            OnMaxValueChange?.Invoke();
        }
    }

    public void IncreaseMaxValue(float amount, bool maxValueChangeInvoke = true, bool influenceCurrentValue = false, bool allowExtremeValues = true, bool resetRecovery = false, bool currentValueInvoke = true)
    {
        maxValue += amount;

        if (influenceCurrentValue)
        {
            IncreaseCurrentValue(amount, allowExtremeValues, resetRecovery, currentValueInvoke);
        }

        SetSliderValue();

        if (maxValueChangeInvoke)
        {
            OnMaxValueChange?.Invoke();
        }
    }

    public void DecreaseMaxValue(float amount, bool maxValueChangeInvoke = true, bool influenceCurrentValue = false, bool allowExtremeValues = true, bool resetRecovery = true, bool currentValueInvoke = true)
    {
        maxValue -= amount;

        if (influenceCurrentValue)
        {
            DecreaseCurrentValue(amount, allowExtremeValues, resetRecovery, currentValueInvoke);
        }

        Mathf.Clamp(currentValue, 0.0f, maxValue);

        SetSliderValue();

        if (maxValueChangeInvoke)
        {
            OnMaxValueChange?.Invoke();
        }

        if (currentValueInvoke && currentValue == maxValue)
        {
            OnCurrentValueMax?.Invoke();
        }
    }

    private void SetSliderValue()
    {
        if (slider != null)
        {
            float targetValue = reverseSlider ? 1.0f - currentValue / maxValue : currentValue / maxValue;
            slider.DOValue(targetValue, 0.2f).SetEase(Ease.OutCubic);
        }
    }
}
