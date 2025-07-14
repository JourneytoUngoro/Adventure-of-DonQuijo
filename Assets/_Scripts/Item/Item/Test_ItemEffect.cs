using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ItemEffect : MonoBehaviour
{

    public SpeedEffect speedEffect;
    public Player player;
    
    public void SpeedEffectItem()
    {
        speedEffect.ApplyEffect(player);
    }


}
