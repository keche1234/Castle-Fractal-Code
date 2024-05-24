using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class Character : MonoBehaviour
{
    protected Rigidbody charRb;
    protected int level;

    [Header("Basic Attributes")]
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float power;
    [SerializeField] protected int strength;
    [SerializeField] protected int defense;
    [SerializeField] protected float speed;
    protected bool armored;

    [Header("Buffs and Debuffs")]
    /*0) Special
      1) Strength
      2) Defense
      3) Speed
      4) Signature Gauge Fill Rate (player only)
      5) Regen*/
    public static readonly string[] buffTypes = { "Special", "Strength", "Defense", "Speed", "Signature Gain", "Regen" };
    public static readonly string[] debuffTypes = { "Special", "Strength", "Defense", "Speed", "Signature Gain", "Stun" };
    public static readonly string[] directMultTypes = { "Special", "Strength", "Defense", "Speed", "Signature Gain" };
    [SerializeField] protected List<List<Buff>> buffs;
    [SerializeField] protected List<List<Debuff>> debuffs;
    [SerializeField] protected List<float> directMults;

    [Header("Game Management")]
    [SerializeField] protected SpawnManager spawner;
    [SerializeField] protected RoomManager roomManager;
    [SerializeField] protected GameManager gameManager;

    [Header("General States")]
    [SerializeField] protected bool invincible;
    [SerializeField] protected GameObject invincibilityCover;

    [Header("Status Bars UI")]
    /* 1) Health
       2) Durability*/
    [SerializeField] protected BarUI miniHealthBar;
    [SerializeField] protected Canvas attributesUI;
    [SerializeField] protected Text strengthUI;
    [SerializeField] protected Text defenseUI;

    //[Header("Time Freeze Handling")]
    protected FreezeManager freezeManager;
    //protected static List<string> tFreezeTargets;
    //protected static List<float> tFreezeDurs;
    //protected static List<(string, float)> freezeTags;
    protected bool frozen;
    //protected float freezeTime;
    protected Vector3 preVel; //velocity before timeFreeze

    //[Header("Stun Handling")]
    protected float initialStun = 0;
    protected float stunTime;
    protected float stunCooldown = 1.5f;
    protected float stunRotateSpeed = Mathf.PI * 80;

    protected List<Character> hitByList;

    protected float xLimit;
    protected float zLimit;

    // Start is called before the first frame update
    public virtual void Start()
    {
        buffs = new List<List<Buff>>();
        for (int i = 0; i < buffTypes.Length; i++)
            buffs.Add(new List<Buff>());

        debuffs = new List<List<Debuff>>();
        for (int i = 0; i < debuffTypes.Length; i++)
            debuffs.Add(new List<Debuff>());

        directMults = new List<float>();
        for (int i = 0; i < directMultTypes.Length; i++)
            directMults.Add(1);

        hitByList = new List<Character>();

        //tFreezeTargets = new List<string>();
        //tFreezeDurs = new List<float>();
        //EventManager.OnFreeze += FreezeSelf;

        //if (freezeTags == null)
        //    freezeTags = new List<(string, float)>();

        charRb = GetComponent<Rigidbody>();
        armored = false;

        miniHealthBar.gameObject.transform.parent.GetComponent<Billboard>().SetCamera(GameObject.Find("UI Camera").GetComponent<Camera>());
        miniHealthBar.gameObject.transform.parent.GetComponent<UIAttach>().Setup(gameObject, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(0, -50));

        attributesUI.GetComponent<Billboard>().SetCamera(GameObject.Find("UI Camera").GetComponent<Camera>());
        attributesUI.GetComponent<UIAttach>().Setup(gameObject, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(5, -90));

        spawner = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        freezeManager = FindObjectOfType<FreezeManager>();
    }

    //void OnEnable()
    //{
    //    EventManager.OnFreeze += FreezeSelf;
    //}

    //void OnDisable()
    //{
    //    EventManager.OnFreeze -= FreezeSelf;
    //}

    //void OnDestroy()
    //{
    //    EventManager.OnFreeze -= FreezeSelf;
    //}

    // Update is called once per frame
    public virtual void Update()
    {

    }

    //Uses Power, currentHealth, Strength, and Defense to determine damage.
    //this.dealDamage() is called first, and inside of it,
    //the target's TakeDamage() method is called.
    //overrideDMG determines if power should be overriden with value p
    public virtual void DealDamage(float val, Character target, float p, Vector3 kbDir, bool triggerInvinc = true, float kbMod = 0, bool overrideDMG = false, bool fixKB = false)
    {
        int damage;
        float mod, guardMod = 1;

        mod = ((strength + Mathf.Min(Mathf.Max(SummationBuffs(1) + SummationDebuffs(1), -9), 9)) - (target.GetDefense() + Mathf.Min(Mathf.Max(target.SummationBuffs(2) + target.SummationDebuffs(2), -9), 9))) / 10;

        if (Random.Range(0, 0.99f) < target.SummationBuffs(6))
            guardMod = 0.4f;

        if (overrideDMG)
            damage = Mathf.Max(0, (int)(Mathf.Floor((p * val * (1 + mod) * guardMod) - Random.Range(0.001f, 1.000f) + 1.0f) * (directMults[1] / target.GetDirectMult(2))));
        else
            damage = Mathf.Max(0, (int)(Mathf.Floor((power * val * (1 + mod) * guardMod) - Random.Range(0.001f, 1.000f) + 1.0f) * (directMults[1] / target.GetDirectMult(2))));

        //knockback calc, likely with a Vector3 parameter calculated by the hitbox that causes the character to call this method.

        target.TakeDamage(damage, kbDir, triggerInvinc, kbMod, fixKB);
    }

    public virtual void TakeDamage(int damage, Vector3 kbDir, bool triggerInvinc = true, float kbMod = 0, bool fixKB = false)
    {
        if (!invincible || damage < 0) //if invicible => damage is negative (healing)
        {
            currentHealth -= damage;
        }
        if (currentHealth <= 0)
            currentHealth = 0;
        else if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        else if (!armored && kbMod != 0)
        {
            StopKnockback();
            if (fixKB) StartCoroutine(TakeKnockback(1, kbDir, kbMod));
            else StartCoroutine(TakeKnockback(damage / maxHealth, kbDir, kbMod));
        }

        miniHealthBar.SetMax(maxHealth);
        miniHealthBar.SetValue(currentHealth);
    }

    public virtual IEnumerator TakeKnockback(float knockback, Vector3 kbDir, float kbMod = 0)
    {
        charRb.velocity *= 0;
        charRb.AddForce(kbDir.normalized * knockback * kbMod * 20, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);
        charRb.velocity *= 0;
        yield return null;
    }

    public virtual void StopKnockback()
    {
        StopCoroutine("TakeKnockback");
        charRb.velocity *= 0;
    }

    public IEnumerator GrantInvincibility(float duration)
    {
        invincible = true;
        invincibilityCover.SetActive(true);
        float t = 0;
        while (t < duration)
        {
            //if (invincible == false)
            //{
            //    Debug.Log("breaki?!");
            //    yield break;
            //}
            invincible = true;
            t += Time.deltaTime;
            //Debug.Log(invincible + ", " + t + "/" + duration);
            yield return null;
        }

        invincibilityCover.SetActive(false);
        t = 0;
        while (t < 0.25f)
        {
            //if (invincible == false)
            //{
            //    Debug.Log("breaki?!");
            //    yield break;
            //}
            invincible = true;
            t += Time.deltaTime;
            //Debug.Log(invincible + ", " + (duration + t) + "/" + duration);
            yield return null;
        }

        invincible = false;
        yield return null;
    }

    public void SetInvincible(bool i)
    {
        invincible = i;
        invincibilityCover.gameObject.SetActive(i);
    }

    public List<Character> GetHitByList()
    {
        return new List<Character>(hitByList);
    }

    public void AddToHitByList(Character c)
    {
        hitByList.Add(c);
    }

    //public void StopInvincibility()
    //{
    //    invincible = false;
    //    invincibilityCover.SetActive(false);
    //}

    /*********************************************************
     * The following functions deal with character parameters.
     *********************************************************/
    public int GetLevel()
    {
        return level;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    //@requires h > 0
    public void SetMaxHealth(float h)
    {
        maxHealth = h;
        currentHealth = h;
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    public float GetPower()
    {
        return power;
    }

    public void SetPower(float p)
    {
        power = p;
    }

    public float GetStrength()
    {
        return strength;
    }

    public float GetDefense()
    {
        return defense;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetSpeed(float s)
    {
        speed = s;
    }

    public bool IsHealthy()
    {
        return currentHealth / maxHealth >= 0.75f;
    }
    public bool IsInCrisis()
    {
        return currentHealth / maxHealth <= 0.25f;
    }

    public List<List<Buff>> GetBuffs()
    {
        return buffs;
    }

    public List<List<Debuff>> GetDebuffs()
    {
        return debuffs;
    }

    public void AddBuff(Buff b, int attribute)
    {
        buffs[attribute].Add(b);
        b.StartPause();
        b.SetOwner(GetComponent<Character>());
    }

    public void RemoveBuff(Buff b, int attribute)
    {
        buffs[attribute].Remove(b);
        b.SetOwner(null);
    }

    public void AddDebuff(Debuff d, int attribute)
    {
        debuffs[attribute].Add(d);
        d.StartPause();
        d.SetOwner(GetComponent<Character>());
    }

    public void RemoveDebuff(Debuff d, int attribute)
    {
        debuffs[attribute].Remove(d);
        //d.SetOwner(null);
    }

    /*******************************************************
     * Determine the total value of all buffs of type `type`
     *******************************************************/
    public float SummationBuffs(int type)
    {
        RemoveNull();
        float sum = 0;
        for (int i = 0; i < buffs[type].Count; i++)
            sum += buffs[type][i].GetMod();
        return sum;
    }

    /*******************************************************
     * Determine the shortest running buff of type `type`
     * and its percentage of the total buff.
     * If there are no running timed buffs, return 1
     *******************************************************/
    public float ShortestBuffPercent(int type)
    {
        RemoveNull();
        if (buffs[type].Count == 0) return 1;
        float shortest = buffs[type][0].GetTimeRemaining();
        float shortPercent = buffs[type][0].GetTimeRemaining() / buffs[type][0].GetDuration();

        for (int i = 1; i < buffs[type].Count; i++)
        {
            float temp = buffs[type][i].GetTimeRemaining();
            if (shortest < 0 || (temp < shortest && temp > 0))
            {
                shortest = temp;
                shortPercent = buffs[type][i].GetTimeRemaining() / buffs[type][i].GetDuration();
            }
        }

        return shortPercent;
    }

    /*********************************************************
     * Determine the total value of all debuffs of type `type`
     *********************************************************/
    public float SummationDebuffs(int type)
    {
        RemoveNull();
        if (type != System.Array.IndexOf(debuffTypes, "Stun"))
        {
            float sum = 0;
            for (int i = 0; i < debuffs[type].Count; i++)
                sum += debuffs[type][i].GetMod();
            return sum;
        }
        else return (stunTime > (stunCooldown / 2)) ? 1 : 0;
    }

    /*******************************************************
     * Determine the shortest running debuff of type `type`
     * and its percentage of the total buff.
     * If there are no running timed buffs, return 1
     *******************************************************/
    public float ShortestDebuffPercent(int type)
    {
        RemoveNull();
        if (type != System.Array.IndexOf(debuffTypes, "Stun"))
        {
            if (debuffs[type].Count == 0) return 1;
            float shortest = debuffs[type][0].GetTimeRemaining();
            float shortPercent = shortest / debuffs[type][0].GetDuration();

            for (int i = 1; i < debuffs[type].Count; i++)
            {
                float temp = debuffs[type][i].GetTimeRemaining();
                if (temp < shortest && temp > 0)
                {
                    shortest = temp;
                    shortPercent = shortest / debuffs[type][i].GetDuration();
                }
            }

            return shortPercent;
        }
        else return (stunTime - (stunCooldown / 2)) / (initialStun - (stunCooldown / 2));
    }

    public void RemoveNull()
    {
        if (buffs == null)
            Debug.Log("what");
        for (int a = 0; a < buffs.Count; a++)
        {
            for (int b = 0; b < buffs[a].Count; b++)
            {
                if (buffs[a][b] == null)
                    buffs[a].RemoveAt(b--);
            }
        }

        for (int a = 0; a < debuffs.Count; a++)
        {
            for (int b = 0; b < debuffs[a].Count; b++)
            {
                if (debuffs[a][b] == null)
                    debuffs[a].RemoveAt(b--);
            }
        }
    }

    protected void ProgressBuffTime()
    {
        RemoveNull();
        for (int a = 0; a < buffs.Count; a++)
        {
            for (int b = 0; b < buffs[a].Count; b++)
                buffs[a][b].Update();
        }

        for (int a = 0; a < debuffs.Count; a++)
        {
            for (int b = 0; b < debuffs[a].Count; b++)
                debuffs[a][b].Update();
        }
    }

    public void AddDirectMult(float val, int type)
    {
        directMults[type] = val;
    }

    public float GetDirectMult(int type)
    {
        return directMults[type];
    }

    public void ChangeStrength(int change)
    {
        strength += change;
        if (strength > 9)
            strength = 9;
        else if (strength < -9)
            strength = -9;

        return;
    }

    public void ChangeDefense(int change)
    {
        defense += change;
        if (defense > 9)
            defense = 9;
        else if (defense < -9)
            defense = -9;

        return;
    }

    public void SetSpawnManager(SpawnManager sm)
    {
        spawner = sm;
    }

    public void SetRoomManager(RoomManager rm)
    {
        roomManager = rm;
    }

    public SpawnManager GetSpawnManager()
    {
        return spawner;
    }

    public RoomManager GetRoomManager()
    {
        return roomManager;
    }

    public virtual void UpdateAttributeUI()
    {
        float s = strength + SummationBuffs(1) + SummationDebuffs(1);
        float d = defense + SummationBuffs(2) + SummationDebuffs(2);
        strengthUI.text = "" + (int)Mathf.Min(Mathf.Max(s, -9), 9);
        defenseUI.text = "" + (int)Mathf.Min(Mathf.Max(d, -9), 9);

        if (s < 0)
            strengthUI.color = Color.red;
        else
            strengthUI.color = Color.white;

        if (d < 0)
            defenseUI.color = Color.red;
        else
            defenseUI.color = Color.white;
    }

    //Detects if the object is out of bounds
    protected bool IsOOB()
    {
        if (Mathf.Abs(transform.position.x) > spawner.GetXBorder() || Mathf.Abs(transform.position.z) > spawner.GetZBorder())
            return true;
        return false;
    }

    protected void ReturnToInBounds()
    {
        if (transform.position.x > spawner.GetXBorder())
            transform.position = new Vector3(spawner.GetXBorder() - 0.5f, transform.position.y, transform.position.z);
        else if (transform.position.x < -spawner.GetXBorder())
            transform.position = new Vector3(-spawner.GetXBorder() + 0.5f, transform.position.y, transform.position.z);

        if (transform.position.z > spawner.GetZBorder())
            transform.position = new Vector3(transform.position.x, transform.position.y, spawner.GetZBorder() - 0.5f);
        else if (transform.position.z < -spawner.GetZBorder())
            transform.position = new Vector3(transform.position.x, transform.position.y, -spawner.GetZBorder() + 0.5f);
    }

    /***********************************************
     * Time Freeze Functions
     ***********************************************/
    //public static List<string> GetFreezeTargets()
    //{
    //    return tFreezeTargets;
    //}

    //public static List<float> GetFreezeDurs()
    //{
    //    return tFreezeDurs;
    //}

    public FreezeManager GetFreezeManager()
    {
        return freezeManager;
    }

    public float GetMyFreezeTime()
    {
        if (freezeManager != null)
        {
            string myTag = gameObject.tag;
            return freezeManager.GetFreezeTime(myTag);
        }
        return 0;
    }

    public void FreezeTargetAdd(string tag, float time)
    {
        freezeManager.FreezeTagAdd(tag, time);
    }

    public void FreezeTargetReplace(string tag, float time)
    {
        freezeManager.FreezeTagReplace(tag, time);
    }

    //protected virtual void FreezeSelf()
    //{
    //    if (tFreezeTargets.Contains(gameObject.tag))
    //    {
    //        int i = tFreezeTargets.IndexOf(gameObject.tag);
    //        preVel = charRb.velocity;
    //        charRb.velocity *= 0;
    //        frozen = true;
    //        freezeTime = tFreezeDurs[i];
    //    }
    //}

    public virtual void StunMe(float t)
    {
        StopAllCoroutines();
        stunTime += t;
        if (stunTime <= 0) stunTime += stunCooldown;
    }
}
