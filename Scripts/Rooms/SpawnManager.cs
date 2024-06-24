using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] protected List<Enemy> enemyPrefabs;
    [SerializeField] protected List<Boss> bossPrefabs;
    [SerializeField] protected GameObject spawnCover;

    protected List<Enemy> currentWaveList;
    protected List<GameObject> covers;
    protected List<Vector3> spawnPosList;

    [Header("Spawn Time")]
    [SerializeField] protected float spawnDelay;
    [SerializeField] protected float waveDelay;
    protected Coroutine spawnCoroutine;

    //[Header("Spawn Count")]
    //[SerializeField] int lowRoll;
    //[SerializeField] int highRoll;

    //[Header("Game Handling")]
    //[SerializeField] protected int currentWaveNum = 0;
    //protected int totalWaves;

    // Boss Info
    protected bool bossWave = false;
    protected Boss boss;
    protected int bossesDefeated;

    //Enemy parameters
    //Growth inidcates a linear increase
    protected const int SPAWN_COUNT_FLOOR = 2;
    protected const float SPAWN_FLOOR_GROWTH = 0.5f;
    protected const int SPAWN_FLOOR_CAP = 10;

    protected const int SPAWN_COUNT_CEIL = 4;
    protected const float SPAWN_CEIL_GROWTH = 1;
    protected const int SPAWN_CEIL_CAP = 20;

    protected const float ENEMY_ATTRIBUTE_CEIL = 1.5f;
    protected const float ENEMY_ATTRIBUTE_GROWTH = 0.5f;
    protected const float ENEMY_ATTRIBUTE_CAP = 9;

    protected const float ENEMY_HEALTHPOW_GROWTH = 0.3f;

    protected List<int> waveCounts;
    protected int[] waveBreaks = { 0, 7, 13, 19 }; // Tells you the floor at which to raise the number of waves

    [Header("Managers")]
    [SerializeField] protected RoomManager roomManager;
    [SerializeField] protected UpgradeManager upgradeManager;
    protected PlayerController player;

    protected bool spawned = false;
    protected bool allDefeated = true;

    // Start is called before the first frame update
    void Start()
    {
        currentWaveList = new List<Enemy>();
        waveCounts = new List<int>();

        spawnPosList = new List<Vector3>();
        player = FindObjectOfType<PlayerController>();
        bossesDefeated = 0;
    }

    // Update is called once per frame
    protected void Update()
    {
        Room currentRoom = roomManager.GetCurrent();

        if (spawned)
        {
            spawnCoroutine = null;
            spawned = false;
        }

        // Regular room, not everyone is defeated, not currently spawning anything
        if (!currentRoom.IsBossRoom() && !currentRoom.IsBreakRoom() && !allDefeated && spawnCoroutine == null)
        {
            if (currentWaveList.Count == 0) // all enemies in wave defeated
            {
                if (waveCounts.Count == 0) //no more waves to spawn
                {
                    allDefeated = true;
                }
                else
                {
                    if (spawnCoroutine == null) //not already in the process of spawning
                    {
                        spawnCoroutine = StartCoroutine(SpawnWave(waveCounts[waveCounts.Count - 1]));
                        waveCounts.RemoveAt(waveCounts.Count - 1);
                    }
                }
            }
        }
    }

    public void SetWaveCounts()
    {
        // Calculate number using floor, ceil, and bosses defeated
        int lowRoll = (int)Mathf.Min(SPAWN_COUNT_FLOOR + (SPAWN_FLOOR_GROWTH * bossesDefeated), SPAWN_FLOOR_CAP);
        int highRoll = (int)Mathf.Min(SPAWN_COUNT_CEIL + (SPAWN_CEIL_GROWTH * bossesDefeated), SPAWN_CEIL_CAP);
        int totalSpawnCount = Random.Range(lowRoll, highRoll + 1);

        // Distribute waves based on count
        int numWaves = 0;
        for (int i = 0; i < waveBreaks.Length; i++)
            if (totalSpawnCount >= waveBreaks[i])
                numWaves = i + 1;
            else
                i = waveBreaks.Length;

        waveCounts = new List<int>();
        for (int i = 0; i < numWaves; i++)
            waveCounts.Add(totalSpawnCount / numWaves);
        for (int i = 0; i < totalSpawnCount % numWaves; i++)
            waveCounts[i]++;

        allDefeated = false;
    }

    public virtual IEnumerator SpawnWave(int spawnCount)
    {
        //currentWaveNum++;
        //allDefeated = false;
        spawnPosList = new List<Vector3>();
        covers = new List<GameObject>();
        List<int> spawnNum = new List<int>();

        float t = 0;
        while (t < waveDelay)
        {
            t += Time.deltaTime;
            yield return null;
        }

        List<Vector3> positions = roomManager.GetCurrent().OpenWorldPositions();
        Debug.Log(positions.Count);
        int ogPositionCount = positions.Count;
        for (int i = 0; i < Mathf.Min(spawnCount, ogPositionCount); i++)
        {
            spawnNum.Add(SelectEnemy());
            int spawnIndex = Random.Range(0, positions.Count);
            spawnPosList.Add(positions[spawnIndex] + (Vector3.up * (-positions[spawnIndex].y + enemyPrefabs[spawnNum[i]].transform.position.y)));
            positions.RemoveAt(spawnIndex);

            covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
            covers[i].transform.parent = roomManager.GetCurrent().transform;
        }

        t = 0;
        while (t < spawnDelay)
        {
            t += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < spawnPosList.Count; i++)
        {
            Destroy(covers[i]);
            currentWaveList.Add(Instantiate(enemyPrefabs[spawnNum[i]], spawnPosList[i], gameObject.transform.rotation));

            currentWaveList[i].SetSpawnManager(this);
            currentWaveList[i].SetRoomManager(roomManager);
            currentWaveList[i].transform.parent = roomManager.GetCurrent().transform;

            SetEnemyMods(currentWaveList[i]);

            int attributeCeil = (int)Mathf.Min(ENEMY_ATTRIBUTE_CEIL + (ENEMY_ATTRIBUTE_GROWTH * bossesDefeated), ENEMY_ATTRIBUTE_CAP);
            currentWaveList[i].SetStrength(Random.Range(0, attributeCeil + 1));
            currentWaveList[i].ChangeDefense(Random.Range(0, attributeCeil + 1));
        }
        spawned = true;
        yield return null;
    }

    //private void SpawnPickups()

    //protected Vector3 GenerateSpawnPos(Enemy type)
    //{
    //    return new Vector3(Random.Range(-spawnRangeX, spawnRangeX + 1), type.transform.position.y, Random.Range(-spawnRangeZ, spawnRangeZ + 1));
    //}

    protected int SelectEnemy()
    {
        int floor = 0;
        int num;
        int ceil = 0;
        foreach (Enemy enemy in enemyPrefabs)
            ceil += enemy.GetAppearanceRate();
        num = Random.Range(0, ceil); //this number lies somewhere in the summation of the appearance rates (think like a spectrum)

        ceil = enemyPrefabs[0].GetAppearanceRate(); //bring ceil back down to the first enemy's appearance rate
        for (int i = 0; i < enemyPrefabs.Count - 1; i++)
        {
            if (floor <= num && num < ceil)
                return i; //num landed in enemyPrefab[i]'s range
            else
            {
                //bump up the range in the spectrum
                floor = ceil;
                ceil += enemyPrefabs[i + 1].GetAppearanceRate();
            }
        }

        return enemyPrefabs.Count - 1;
    }

    public virtual void RemoveMe(Enemy me)
    {
        currentWaveList.Remove(me);
        Destroy(me.gameObject);

        if (bossWave && boss == me)
        {
            allDefeated = true;
            if (!player.IsPlayerMaxRank())
                upgradeManager.StartCoroutine("StartUpgradeSequence");
            bossesDefeated++;
        }
    }

    public void SetBossInfo(bool b)
    {
        bossWave = b;
    }

    public bool AllDefeated()
    {
        return allDefeated;
    }

    public void SetAllDefeated(bool b)
    {
        allDefeated = b;
    }

    public void SetSpawned(bool b)
    {
        spawned = b;
    }

    /*
     * Sets Health, Power, and Speed mods based on the number of bosses defeated
     */
    public void SetEnemyMods(Enemy e)
    {
        float healthPowMod = 1 + (ENEMY_HEALTHPOW_GROWTH * bossesDefeated);
        e.SetHealthPowerSpeed(healthPowMod, healthPowMod, 1);
    }

    public void SetBoss(Boss b)
    {
        Debug.Log(bossesDefeated);
        boss = b;
        bossWave = boss != null;
    }

    public void SetBossMods()
    {
        float healthPowMod = 1 + (ENEMY_HEALTHPOW_GROWTH * bossesDefeated);
        boss.SetHealthPowerSpeed(healthPowMod, healthPowMod, 1);
        Debug.Log("Set Power to " + boss.GetPower());

        int attributeCeil = (int)Mathf.Min(ENEMY_ATTRIBUTE_CEIL + (ENEMY_ATTRIBUTE_GROWTH * bossesDefeated), ENEMY_ATTRIBUTE_CAP);
        int strengthBase = Random.Range(0, 3);
        boss.SetStrength(strengthBase + Random.Range(0, attributeCeil + 1));
        boss.SetDefense(Random.Range(0, 3) + attributeCeil - (int)(boss.GetStrength() - strengthBase));
    }

    public Boss GetBoss()
    {
        return boss;
    }

    public void DestroyAllEnemies()
    {
        for (int i = currentWaveList.Count - 1; i >= 0; i--)
        {
            RemoveMe(currentWaveList[i]);
        }
    }
}
