using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Stage
{
    public StageInfoSO stageInfo;

    // static infos
    public string stageName;
    public int coinValuePer;
    public int memoryFramentCount;
    public int totalEnemyCount;

    // dynamic infos
    public int collectedMemoryFragmentCount;
    public int killedEnemyCount;

    // check spawn probability of memory fragment
    public float baseMemoryFragmentProbability;
    public float currentMemoryFramgnetProbability;
    private int probabilityCheckPoint;

    private bool forceSpawn;
    private bool checkAgain;

    // current stage's enemy lists
    private Dictionary<EnemyType, StageEnemyType> enemiesDict;

    public Stage(StageInfoSO stageInfo)
    {
        this.stageInfo = stageInfo;

        this.coinValuePer = stageInfo.coinValuePer;
        this.stageName = stageInfo.stageName;
        this.memoryFramentCount = stageInfo.memoryFragmentCount;
        this.probabilityCheckPoint = stageInfo.probabilityCheckPoint;
        this.totalEnemyCount = stageInfo.totalEnemyCount;

        collectedMemoryFragmentCount = 0;
        killedEnemyCount = 0;

        baseMemoryFragmentProbability = memoryFramentCount / totalEnemyCount;
        currentMemoryFramgnetProbability = (float)(memoryFramentCount - collectedMemoryFragmentCount) / (float)(totalEnemyCount - killedEnemyCount);

        forceSpawn = false;
        checkAgain = false;

        LoadCurrentStageEnemies(stageInfo.stageEnemyTypes);
    }

    public Stage(StageInfoSO stageInfo, int collectedMemoryFragmentCount, int killedEnemyCount)
    {
        this.stageInfo = stageInfo;

        this.stageName = stageInfo.stageName;
        this.coinValuePer = stageInfo.coinValuePer;
        this.memoryFramentCount = stageInfo.memoryFragmentCount;
        this.probabilityCheckPoint = stageInfo.probabilityCheckPoint;
        this.totalEnemyCount = stageInfo.totalEnemyCount;

        this.collectedMemoryFragmentCount = collectedMemoryFragmentCount;
        this.killedEnemyCount = killedEnemyCount;

        baseMemoryFragmentProbability = memoryFramentCount / totalEnemyCount;
        currentMemoryFramgnetProbability = (float) (memoryFramentCount - collectedMemoryFragmentCount) / (float) (totalEnemyCount - killedEnemyCount);

        forceSpawn = false;
        checkAgain = false;

        LoadCurrentStageEnemies(stageInfo.stageEnemyTypes);
    }

    public void Initialize()
    {
        UpdateCurrentProbability();
    }

    private void LoadCurrentStageEnemies(List<StageEnemyType> enemies)
    {
        enemiesDict = new Dictionary<EnemyType, StageEnemyType>();

        foreach (StageEnemyType enemy in enemies)
        {
            if (!enemiesDict.ContainsKey(enemy.enemyType))
            {
                enemiesDict.Add(enemy.enemyType, enemy);
            }
        }

        Debug.Log($"loaded current stage's enemies : {enemiesDict.Count}");
    }

    public void UpdateCollectedMemoryFragment(int amount = 1)
    {
        collectedMemoryFragmentCount += amount;
        // Debug.Log($"player collected mf : {collectedMemoryFragmentCount} ");

        UpdateCurrentProbability();
    }

    private void UpdateCurrentProbability()
    {
        int remainingEnemies = Mathf.Max(1, totalEnemyCount - killedEnemyCount); // prevent division by 0 
        int remainingMemoryFragments = Mathf.Max(0, memoryFramentCount - collectedMemoryFragmentCount);

        currentMemoryFramgnetProbability = (float)remainingMemoryFragments / (float)remainingEnemies;
    }

    private void TrySpawnMemoryFragment(Transform enemy)
    {
        float random = Random.Range(0, 100) * 0.01f;

        // Debug.Log($"try spawn mf : random({random}), probability({currentMemoryFramgnetProbability})");
        if (random <= currentMemoryFramgnetProbability || forceSpawn)
        {
            Debug.Log("spawn mf now");

            Manager.Instance.objectPoolingManager.pooledMemoryFragmentController.DropMemoryFragmentFromEnemy(enemy);
        }

    }

    private void RewardCoinsByEnemy(EnemyType type, Transform enemy)
    {
        int coinMin = enemiesDict[type].rewardMinValue;
        int coinMax = enemiesDict[type].rewardMaxValue;
        int coinRandomReward = Random.Range(coinMin, coinMax + 1);
        int coinSpawnCount = coinRandomReward / coinValuePer;

        Manager.Instance.stageManager.StartCoroutine(SpawnCoinsCoroutine(0.05f, coinSpawnCount, enemy));
    }

    private IEnumerator SpawnCoinsCoroutine(float duration, int spawnCount, Transform enemy)
    {
        while (spawnCount -- > 0)
        {
            Manager.Instance.objectPoolingManager.pooledCoinController.DropCoinsFromEnemy(enemy);
            yield return new WaitForSeconds(duration);
        }
    }

    public void OnEnemyDeath(Transform enemy)
    {
        // coin spawn logic

        // test ======================================================
        // RewardCoinsByEnemy(EnemyType.PlagueDoctor, enemy);
        // RewardCoinsByEnemy(EnemyType.VampireBat, enemy);
        RewardCoinsByEnemy(EnemyType.PrisonGuard, enemy);
        // =========================================================
        killedEnemyCount++;

        // memory fragment spawn logic
        if (killedEnemyCount % probabilityCheckPoint == 0 || checkAgain)
        {
            int goalCount = (int) (killedEnemyCount * baseMemoryFragmentProbability);

            // force spawn to meet requried spawn count
            if (collectedMemoryFragmentCount < goalCount)
            {
                forceSpawn = true;
                checkAgain = true;
                Debug.Log("mf needs to be spawned force");
            }
            else
            {
                forceSpawn=false;
                checkAgain = false;
            }
        }

        TrySpawnMemoryFragment(enemy);
    }


    public StageData GetStageData()
    {
        return new StageData(stageInfo.id, collectedMemoryFragmentCount, killedEnemyCount);
    }

    public void SetData(StageData data)
    {
        collectedMemoryFragmentCount = data.collectedFragmentCount;
        killedEnemyCount = data.killedEnemtyCount;
    }
}
