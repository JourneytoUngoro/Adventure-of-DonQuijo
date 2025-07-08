using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MemoryFragmentData 
{
    public int count = 0;
    public List<SerializableVector3> memoryFragmentPositions = new List<SerializableVector3>();

    public void AddMemoryFragmentPositions(Vector3 mfPosition)
    {
        memoryFragmentPositions.Add(new SerializableVector3(mfPosition));
    }
}
