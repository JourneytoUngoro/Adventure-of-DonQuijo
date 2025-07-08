using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageInfo", menuName = "Scriptable Object / StagetInfoSO")]
public class StageInfoSO : ScriptableObject
{
    public string id;
    public string stageName;

    public int coinValuePer;
    public int memoryFragmentCount;
    public int totalEnemyCount;
    public int probabilityCheckPoint;

    // TODO : should be modified 
    [SerializeField] public List<StageEnemyType> stageEnemyTypes;

}

[System.Serializable]
public class StageEnemyType
{
    [SerializeField] public string id { get; private set; }
    public string enemyName;
    public EnemyType enemyType;
    public int rewardMinValue;
    public int rewardMaxValue;
    public int rewardCoinCount; // when enemy dies, this number of Coins will be dropped
}
