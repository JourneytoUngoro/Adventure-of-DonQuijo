using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICutScene 
{
    public Action onFinish {  get; set; }
    public void InitializeScene();

    public void PlayScene();
}
