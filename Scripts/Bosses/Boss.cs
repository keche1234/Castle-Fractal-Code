using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : Enemy
{
    //[Header("Attack Patterns")]
    protected int numAttacks; //number of attacks
    protected int currentAttack; //0 is for summon, attacks start at 1
    protected Room room;

    [Header("Summons")]
    [SerializeField] protected GameObject spawnCover;
    [SerializeField] protected List<Enemy> summonPrefabs;
    protected List<Enemy> summons;
    protected List<Vector3> summonLastPositions; // Used to keep summons in specific places (x,z) places without being too jarring
    protected float summonStartup = 1;
    protected float summonCooldown = 1;
    protected int summonCount;

    /************************************************************
     * Each attack needs its own framedata and set of hitboxes
     * (attack1, attack2, etc.)
     ************************************************************/

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        state = ActionState.Waiting;
        armored = true;
        room = roomManager.GetCurrent();
        summons = new List<Enemy>();
        summonLastPositions = new List<Vector3>();
    }

    // Update is called once per frame
    public override void Update()
    {
        hitByList.Clear();
        if (GetMyFreezeTime() > 0)
        {
            //GetMyFreezeTime() -= Time.deltaTime;
            if (charRb != null)
                charRb.velocity *= 0;
            rotateSpeed = 0;
            frozen = true;
        }
        else if (frozen) //this is the specific act of unfreezing
        {
            charRb.velocity = preVel;
            rotateSpeed = 2.5f;
            frozen = false;
        }
        else
        {
            for (int i = 0; i < summons.Count; i++)
            {
                if (summons[i] == null)
                {
                    summonLastPositions.RemoveAt(i);
                    summons.RemoveAt(i--);
                }
                else
                    summonLastPositions[i] = summons[i].transform.position;
            }
            
            if (state == ActionState.Waiting && player.GetComponent<PlayerController>().GetCurrentHealth() > 0)
                StartCoroutine(Attack());

            ProgressBuffTime();

            if (currentHealth <= 0)
            {
                for (int i = 0; i < summons.Count; i++)
                {
                    Destroy(summons[i].gameObject);
                }
                spawner.SetAllDefeated(true);
                Destroy(this.gameObject);
            }
        }

        UpdateAttributeUI();
    }

    public void SetRoom(Room r)
    {
        room = r;
    }

    /*
     * Summons `count` enemies randomly selected from `summonPrefabs`,
     * with Random[1, `strength`] strength and Random[1, `defense`] defense;
     * `healthMult` times as much base health, `powMult` times as much base power, and `speedMult` times as much base speed
     * then increments the current move to one of the `nextAttacks` amount of next attacks.
     */
    public IEnumerator Summon(int count, int strength, int defense, float healthMult, float powMult, float speedMult, int nextAttacks)
    {
        //Spawn covers
        state = ActionState.Attacking;
        List<GameObject> covers = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            covers.Add(Instantiate(spawnCover, GenerateSpawnPos(summonPrefabs[0]), Quaternion.Euler(0, 0, 0)));
            covers[i].transform.parent = roomManager.GetCurrent().transform;
        }

        //Hold the covers for the duration of the spawn
        float t = 0;
        while (t < summonStartup)
        {
            if (GetMyFreezeTime() <= 0)
                t += Time.deltaTime;
            yield return null;
        }

        List<Enemy> newSummons = new List<Enemy>();
        //Remove the covers and create the enemies
        for (int i = 0; i < count; i++)
        {
            newSummons.Add((Enemy)Instantiate(summonPrefabs[Random.Range(0, summonPrefabs.Count)], covers[0].transform.position, Quaternion.Euler(0, 0, 0)));
            newSummons[i].transform.parent = roomManager.GetCurrent().transform;
            summonLastPositions.Add(newSummons[i].transform.position);
            Destroy(covers[0]);
            covers.RemoveAt(0);
        }
        yield return null;

        for (int i = 0; i < count; i++)
        {
            newSummons[i].SetHealthPowerSpeed(healthMult, powMult, speedMult);
            newSummons[i].SetStrength(Random.Range(1, strength+1));
            newSummons[i].SetDefense(Random.Range(1, defense+1));
        }

        for (int i = 0; i < newSummons.Count; i++)
            summons.Add(newSummons[i]);

        t = 0;
        state = ActionState.Cooldown;
        while (t < summonStartup)
        {
            if (GetMyFreezeTime() <= 0)
                t += Time.deltaTime;
            yield return null;
        }

        currentAttack += 1 + Random.Range(0, nextAttacks);
        state = ActionState.Waiting;
        yield return null;
    }

    public int GetCurrentAttack()
    {
        return currentAttack;
    }

    public int GetState()
    {
        if (state == ActionState.Startup)
            return 1;
        if (state == ActionState.Attacking)
            return 2;
        if (state == ActionState.Cooldown)
            return 3;

        return 0;
    }

    public void SetState(int i)
    {
        if (i == 1)
            state = ActionState.Startup;
        else if (i == 2)
            state = ActionState.Attacking;
        else if (i == 3)
            state = ActionState.Cooldown;
        else
            state = ActionState.Waiting;
    }

    //TODO: Push Summon Away From Spot

    //public override void TakeDamage(int damage, Vector3 kbDir, bool triggerInvinc = true, float kbMod = 0, bool fixKB = false)
    //{
    //    base.TakeDamage(damage, kbDir, triggerInvinc, kbMod, fixKB);
    //}

    protected Vector3 GenerateSpawnPos(Enemy type)
    {
        return new Vector3(Random.Range(-(room.GetXDimension() / 2f) + 0.5f, (room.GetXDimension() / 2f) - 0.5f),
                            type.transform.position.y,
                            Random.Range(-(room.GetZDimension() / 2f) + 0.5f, (room.GetZDimension() / 2f) - 0.5f));
    }

    public override void OnDestroy()
    {
        GetSpawnManager().RemoveMe(this);
    }
}
