using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] protected List<Enemy> enemyPrefabs;
    [SerializeField] protected List<Enemy> spawnList;
    [SerializeField] protected GameObject spawnCover;
    protected List<Vector3> spawnPos;
    
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

    [Header("Game Handling")]
    [SerializeField] protected int currentWave = 0;
    [SerializeField] protected int totalWaves;
    [SerializeField] protected bool bossWave = false;
    [SerializeField] protected RoomManager roomManager;

    protected bool spawned = false;
    protected bool allDefeated = false;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        spawnList = new List<Enemy>();
        spawnPos = new List<Vector3>();
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!bossWave && !spawned && spawnList.Count == 0 && currentWave < totalWaves)
        {
            StartCoroutine("SpawnWave");
        }
    }

    public virtual IEnumerator SpawnWave()
    {
        spawned = true;
        currentWave++;
        allDefeated = false;
        int spawnCount = Random.Range(lowRoll, highRoll);
        spawnPos = new List<Vector3>();
        List<GameObject> covers = new List<GameObject>();
        List<int> spawnNum = new List<int>();

        float t = 0;
        while (t < waveDelay)
        {
            t += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            spawnNum.Add(SelectEnemy());
            spawnPos.Add(GenerateSpawnPos(enemyPrefabs[spawnNum[i]]));
            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
            covers[i].transform.parent = roomManager.GetCurrent().transform;
        }

        t = 0;
        while (t < spawnDelay)
        {
            t += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Destroy(covers[i]);
            spawnList.Add((Enemy)Instantiate(enemyPrefabs[spawnNum[i]], spawnPos[i], gameObject.transform.rotation));
            spawnList[i].SetSpawnManager(this);
            spawnList[i].SetRoomManager(roomManager);
            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
            spawnList[i].ChangeStrength(Random.Range(0, 3));
            spawnList[i].ChangeDefense(Random.Range(0, 3));
        }
        yield return null;
    }

    //private void SpawnPickups()

    protected Vector3 GenerateSpawnPos(Enemy type)
    {
        return new Vector3(Random.Range(-spawnRangeX, spawnRangeX + 1), type.transform.position.y, Random.Range(-spawnRangeZ, spawnRangeZ + 1));
    }

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
        spawnList.Remove(me);
        Destroy(me.gameObject);
        if (spawnList.Count == 0)
        {
            spawned = false;
            if (currentWave >= totalWaves) allDefeated = true;
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

    public void SetWaveInfo(int c, int t)
    {
        currentWave = c;
        totalWaves = t;

        if (currentWave < totalWaves) allDefeated = false;
    }

    public bool AllDefeated()
    {
        return allDefeated;
    }

    public void SetAllDefeated(bool b)
    {
        allDefeated = b;
    }
}
