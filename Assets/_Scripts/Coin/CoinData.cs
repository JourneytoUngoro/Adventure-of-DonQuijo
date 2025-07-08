using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CoinData 
{
    public int count = 0;
    public List<SerializableVector3> coinPositions = new List<SerializableVector3>();

    public void AddCoinPositions(Vector3 coinPosition)
    {
        coinPositions.Add(new SerializableVector3(coinPosition));
    }
}
