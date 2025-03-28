using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Trigger
{
    private bool initialFlag;

    public static implicit operator Trigger(bool initialFlag)
    {
        Trigger trigger = new Trigger();
        trigger.initialFlag = initialFlag;
        return trigger;
    }

    public bool Value
    {
        get
        {
            if (initialFlag)
            {
                initialFlag = false;
                return true;
            }
            return false;
        }
    }

    public bool Peek
    {
        get
        {
            return initialFlag;
        }
    }

    public void Reset()
    {
        initialFlag = true;
    }
}
