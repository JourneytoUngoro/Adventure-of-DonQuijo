using UnityEngine;

public abstract class Reward : MonoBehaviour
{
    public bool canGiveReward { get; set; } = true;
    public abstract void GiveRewardTo();
}
