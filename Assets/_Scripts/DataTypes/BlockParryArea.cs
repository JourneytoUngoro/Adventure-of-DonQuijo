using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockParryType { Block, Parry };

[System.Serializable]
public class BlockParryArea
{
    [field: SerializeField] public BlockParryType blockParryType { get; private set; }
    [field: SerializeField] public bool changeToBlock { get; private set; } = true;
    [field: SerializeField] public float[] parryTime { get; private set; }
    [field: SerializeField] public float[] parryDurationTime { get; private set; }
    [field: SerializeField] public float parryTimeDecrementReset { get; private set; }

    public float lastCalledTime;

    public int currentIndex;
}
