using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IClickable
{
    public bool canClick {  get;  set; }

    public UnityAction onClick { get; set; }    

}
