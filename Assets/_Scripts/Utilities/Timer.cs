using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Timer class is for replacing the coroutines when a class is not inheriting from Monobehavior
/// Timer must be used in Update function
/// Using Timer in FixedUpdate might cause serious problems
/// </summary>
public class Timer
{
    public event Action timerAction;

    public float duration { get; private set; }
    private float startTime;
    private float timeOffset;

    private bool timerActive;
    private bool isSingleUse;
    // private bool resetStartTime;
    private bool isAdjustTimeSingleUse;

    private int maxMultiUseAmount;
    private int currentMultiUseAmount;

    public Timer(float duration)
    {
        this.duration = duration;
        timerActive = false;
        // resetStartTime = true;
    }

    /// <summary>
    /// A Default Timer that is affected by Time.timeScale. Only active when condition is fit. If resetTime parameter is true, it will continuously reset startTime until the condition is fit.
    /// </summary>
    /// <param name="condition"></param>
    public void Tick(bool condition = true, bool resetTime = false)
    {
        startTime += Time.deltaTime * (1.0f - Time.timeScale);

        if (condition)
        {
            if (timerActive)
            {
                if (Time.time + timeOffset > startTime + duration)
                {
                    timerAction?.Invoke();

                    if (isSingleUse)
                    {
                        StopTimer();

                        if (isAdjustTimeSingleUse)
                        {
                            timeOffset = 0.0f;
                        }
                    }
                    else
                    {
                        startTime = Time.time;

                        if (maxMultiUseAmount != 0)
                        {
                            if (currentMultiUseAmount < maxMultiUseAmount)
                            {
                                currentMultiUseAmount += 1;
                            }
                            else
                            {
                                StopTimer();
                            }
                        }
                    }
                }
            }
        }
        else if (resetTime)
        {
            startTime = Time.time;
        }
    }

    public void TickUnscaled(bool condition, bool resetTime)
    {
        if (condition)
        {
            if (timerActive)
            {
                if (Time.time + timeOffset > startTime + duration)
                {
                    timerAction?.Invoke();

                    if (isSingleUse)
                    {
                        StopTimer();

                        if (isAdjustTimeSingleUse)
                        {
                            timeOffset = 0.0f;
                        }
                    }
                    else
                    {
                        startTime = Time.time;

                        if (maxMultiUseAmount != 0)
                        {
                            if (currentMultiUseAmount < maxMultiUseAmount)
                            {
                                currentMultiUseAmount += 1;
                            }
                            else
                            {
                                StopTimer();
                            }
                        }
                    }
                }
            }
        }
        else if (resetTime)
        {
            startTime = Time.time;
        }
    }

    public void StartSingleUseTimer()
    {
        timerActive = true;
        isSingleUse = true;
        startTime = Time.time;
    }

    public void StartMultiUseTimer(int maxMultiUseAmount = 0)
    {
        timerActive = true;
        isSingleUse = false;
        startTime = Time.time;
        currentMultiUseAmount = 0;
        this.maxMultiUseAmount = maxMultiUseAmount;
    }

    public void ChangeDuration(float duration)
    {
        this.duration = duration;
    }

    public void ChangeStartTime(float startTime)
    {
        this.startTime = startTime;
    }

    public void StopTimer()
    {
        timerActive = false;
    }

    public void AdjustTimeFlow(float adjustTimeAmount, bool isAdjustTimeSingleUse = true)
    {
        timeOffset = adjustTimeAmount;
        this.isAdjustTimeSingleUse = isAdjustTimeSingleUse;
    }
}
