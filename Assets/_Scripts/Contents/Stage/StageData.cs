using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public string id;
    public int collectedFragmentCount;
    public int killedEnemtyCount;

    public StageData ()
    {
        id = string.Empty;
        collectedFragmentCount = 0;
        killedEnemtyCount = 0;
    }

    public StageData(string id, int collectedFragmentCount, int killedEnemtyCount)
    {
        this.id = id;
        this.collectedFragmentCount = collectedFragmentCount;
        this.killedEnemtyCount = killedEnemtyCount;
    }
}
