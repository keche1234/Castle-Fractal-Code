using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] protected List<Enemy> enemyPrefabs;
    [SerializeField] protected List<Boss> bossPrefabs;
    [SerializeField] protected List<Enemy> spawnList;
    [SerializeField] protected List<GameObject> covers;
    [SerializeField] protected GameObject spawnCover;
    protected List<Vector3> spawnPosList;

    [Header("Spawn Time")]
    [SerializeField] protected float spawnDelay;
    [SerializeField] protected float waveDelay;

    [Header("Spawn Dimensions")]
    [SerializeField] protected int spawnRangeX;
    [SerializeField] protected int spawnRangeZ;
    [SerializeField] protected float xBorder;
    [SerializeField] protected float zBorder;

    [Header("Spawn Count")]
    [SerializeField] int lowRoll;
    [SerializeField] int highRoll;
    protected Coroutine spawnCoroutine;

    [Header("Game Handling")]
    [SerializeField] protected int currentWave = 0;
    [SerializeField] protected int totalWaves;
    [SerializeField] protected bool bossWave = false;
    protected Boss boss;
    [SerializeField] protected RoomManager roomManager;
    [SerializeField] protected UpgradeManager upgradeManager;
    protected PlayerController player;

    protected bool spawned = false;
    protected bool allDefeated = false;

    // Start is called before the first frame update
    void Start()
    {
        spawnList = new List<Enemy>();
        spawnPosList = new List<Vector3>();
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    protected void Update()
    {
        Room currentRoom = roomManager.GetCurrent();
        if (!bossWave && !currentRoom.IsBreakRoom() && !spawned && spawnList.Count == 0 && currentWave < totalWaves)
        {
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);
            for (int i = covers.Count - 1; i >= 0; i--)
            {
                Destroy(covers[i]);
                covers.Remove(covers[i]);
            }
            spawnCoroutine = StartCoroutine("SpawnWave");
        }

        if ((totalWaves == 0 && !currentRoom.IsBossRoom()) || currentRoom.IsBreakRoom())
            allDefeated = true;
    }

    public virtual IEnumerator SpawnWave()
    {
        spawned = true;
        currentWave++;
        allDefeated = false;
        int spawnCount = Random.Range(lowRoll, highRoll + 1);
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
            spawnList.Add((Enemy)Instantiate(enemyPrefabs[spawnNum[i]], spawnPosList[i], gameObject.transform.rotation));
            spawnList[i].SetSpawnManager(this);
            spawnList[i].SetRoomManager(roomManager);
            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
            spawnList[i].ChangeStrength(Random.Range(0, 3));
            spawnList[i].ChangeDefense(Random.Range(0, 3));
        }
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
        bool realEnemy = spawnList.Contains(me);
        spawnList.Remove(me);
        Destroy(me.gameObject);
        if (!bossWave && spawnList.Count == 0 && realEnemy)
        {
            spawned = false;
            if (currentWave >= totalWaves) allDefeated = true;
        }
        else if (bossWave && boss == me)
        {
            spawned = false;
            allDefeated = true;
            if (!player.IsPlayerMaxRank())
                upgradeManager.StartCoroutine("StartUpgradeSequence");
        }
    }

    public float GetXBorder()
    {
        return xBorder;
    }

    public float GetZBorder()
    {
        return zBorder;
    }

    public void SetWaveNumber(int w)
    {
        if (w >= 0)
            currentWave = w;
    }

    public void SetWaveInfo(int c, int t)
    {
        currentWave = c;
        totalWaves = t;

        if (currentWave < totalWaves) allDefeated = false;
    }

    public void SetToWaveZero()
    {
        currentWave = 0;
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

    public void SetBoss(Boss b)
    {
        boss = b;
        bossWave = boss != null;
    }

    public void DestroyAllEnemies()
    {
        for (int i = spawnList.Count - 1; i >= 0; i--)
        {
            RemoveMe(spawnList[i]);
        }
    }
}
