using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    protected float rotateSpeed;
    protected ActionState state;
    protected bool knocked;

    public GameObject player;
    [SerializeField] protected int appearanceRate;
    [SerializeField] protected int weaponDropType;
    [SerializeField] protected Potion potionDropPrefab;
    [SerializeField] protected int pointValue;

    protected const float POW_LOW_ROLL_BASE = 0.8f;
    protected const float POW_HIGH_ROLL_BASE = 1.2f;
    protected const float POW_LOW_ROLL_GROWTH = 0.2f;
    protected const float POW_HIGH_ROLL_GROWTH = 0.3f;
    protected int growthCap;
    protected const float DP_LOW_ROLL = 0.67f;
    protected const float DP_HIGH_ROLL = 1.5f;
    protected bool hasItem = false; // will only attempt an item drop if hasItem is true

    //[SerializeField] protected int order;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        player = GameObject.Find("Player");
        growthCap = player.GetComponent<PlayerController>().GetMaxRank() * 2;
    }

    // Update is called once per frame
    public override void Update()
    {

    }

    public override void DealDamage(float val, Character target, float p, Vector3 kbDir, bool triggerInvinc = true, float kbMod = 0, bool over = false, bool fixKB = false)
    {
        int damage;
        float mod, guardMod = 1;

        mod = ((strength + Mathf.Min(Mathf.Max(SummationBuffs(1) + SummationDebuffs(1), -9), 9)) - (target.GetDefense() + Mathf.Min(Mathf.Max(target.SummationBuffs(2) + target.SummationDebuffs(2), -9), 9))) / 10;

        PlayerController player = (PlayerController)target;
        CustomWeapon pWeapon = player.GetCustomWeapon();
        if (pWeapon != null)
        {
            int place = System.Array.IndexOf(Ability.GetNames(), "BladeDull");
            if (pWeapon.GetAbilities().Contains(place))
            {
                mod = ((strength + Mathf.Min(Mathf.Max(SummationBuffs(1) + SummationDebuffs(1), -9), 0)) - (Mathf.Min(Mathf.Max(target.GetDefense() + target.SummationBuffs(2) + target.SummationDebuffs(2), -9), 9))) / 10;
            }
        }

        damage = Mathf.Max(0, (int)Mathf.Floor((power * val * (1 + mod) * guardMod) - Random.Range(0.001f, 1.000f) + 1.0f));
        target.TakeDamage(damage, kbDir, triggerInvinc, kbMod, fixKB);
    }

    public override void TakeDamage(int damage, Vector3 kbDir, bool triggerInvinc = true, float kbMod = 0, bool fixKB = false)
    {
        if (!invincible || damage < 0) //if invicible => damage is negative (healing)
        {
            currentHealth -= damage;
        }
        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
        else if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        else if (!armored && kbMod != 0)
        {
            if (knocked) StopKnockback();

            if (fixKB)
                StartCoroutine(TakeKnockback(1, kbDir, kbMod));
            else
                StartCoroutine(TakeKnockback(damage / maxHealth, kbDir, kbMod));
        }

        miniHealthBar.SetMax(maxHealth);
        miniHealthBar.SetValue(currentHealth);
        miniHealthBar.UpdateAmountTxt(currentHealth.ToString("0"));
    }

    public override IEnumerator TakeKnockback(float knockback, Vector3 kbDir, float kbMod = 0)
    {
        knocked = true;
        yield return null;
        charRb.velocity = kbDir.normalized * knockback * kbMod * 20;
        preFreezeVelocity = charRb.velocity;
        yield return new WaitForSeconds(0.5f);
        knocked = false;
        charRb.velocity *= 0;
        preFreezeVelocity = charRb.velocity;
        if (stunTime > stunCooldown) stunTime = stunCooldown;
        yield return null;
    }

    public override void StopKnockback()
    {
        StopCoroutine("TakeKnockback");
        knocked = false;
    }

    public int SimulateDamage(float val, Character target)
    {
        float mod, guardMod = 1;

        mod = ((strength + Mathf.Min(Mathf.Max(SummationBuffs(1) + SummationDebuffs(1), -9), 9)) - (target.GetDefense() + Mathf.Min(Mathf.Max(target.SummationBuffs(2) + target.SummationDebuffs(2), -9), 9))) / 10;

        PlayerController player = (PlayerController)target;
        CustomWeapon pWeapon = player.GetCustomWeapon();
        if (pWeapon != null)
        {
            int place = System.Array.IndexOf(Ability.GetNames(), "BladeDull");
            if (pWeapon.GetAbilities().Contains(place))
            {
                mod = ((strength + Mathf.Min(Mathf.Max(SummationBuffs(1) + SummationDebuffs(1), -9), 0)) - (target.GetDefense() + Mathf.Min(Mathf.Max(target.SummationBuffs(2) + target.SummationDebuffs(2), -9), 9))) / 10;

            }
        }

        return (int)Mathf.Floor((power * val * (1 + mod) * guardMod) - Random.Range(0.001f, 1.000f) + 1.0f);
    }

    protected virtual void LookTowardsPlayer()
    {
        Vector3 lookVector = player.transform.position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, lookVector, rotateSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    protected virtual bool IsFacingPlayer()
    {
        LayerMask mask = LayerMask.GetMask("Player");
        Ray ray = new Ray();
        RaycastHit hit;
        ray.origin = transform.position;
        ray.direction = transform.TransformDirection(Vector3.forward);
        return Physics.Raycast(ray, out hit, 100f, mask);
    }

    //Define attack behavior
    protected abstract IEnumerator Attack();

    protected enum ActionState
    {
        Moving,
        Startup,
        Attacking,
        Cooldown,
        Waiting
    }

    public int GetAppearanceRate()
    {
        return appearanceRate;
    }

    public virtual void OnCollisionEnter(Collision targetCollider)
    {
        if (targetCollider.gameObject.CompareTag("Enemy"))
        {
            Vector3 dir = transform.position - targetCollider.gameObject.transform.position;
            dir = (new Vector3(dir.x, 0, dir.z)).normalized;
            transform.position += dir * charRb.velocity.magnitude * 2 * Time.deltaTime;
            charRb.velocity *= 0;
        }
    }

    public override void StunMe(float t)
    {
        StopAllCoroutines();
        Reset(true);
        state = ActionState.Waiting;
        stunTime += t;
        if (stunTime <= 0) stunTime += stunCooldown;
    }

    public abstract void Reset(bool zeroSpeed);

    public virtual void OnDestroy()
    {
        if (hasItem && currentHealth <= 0)
            DropItem();
    }

    public void SetHasItem(bool b)
    {
        hasItem = b;
    }

    protected void DropItem()
    {
        PlayerController playerInfo = player.GetComponent<PlayerController>();
        if (playerInfo != null)
        {
            float weaponChance = 1.5f / appearanceRate;
            float potionChance = 0.5f;

            // Roll for drop
            if (Random.Range(0, 0.99999f) < weaponChance)
            {
                float powLowRoll = POW_LOW_ROLL_BASE + (POW_LOW_ROLL_GROWTH * Mathf.Min(playerInfo.GetRank(1), growthCap));
                float powHighRoll = POW_HIGH_ROLL_BASE + (POW_HIGH_ROLL_GROWTH * Mathf.Min(spawnManager.GetBossesDefeated(), growthCap));

                PickupCW weaponDrop = roomManager.GenerateWeapon(weaponDropType, powLowRoll, powHighRoll, DP_LOW_ROLL, DP_HIGH_ROLL, true);
                weaponDrop.gameObject.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                weaponDrop.gameObject.transform.parent = roomManager.GetCurrent().transform;
            }
            else if (Random.Range(0, 0.99999f) < potionChance)
            {
                List<int> potionTypeChance = new List<int>();

                // Strike, Shield, Swift, Skill
                for (int i = 0; i < 4; i++)
                    potionTypeChance.Add(20);

                // Stun: 15 + 1 for every 10% of Health lost below 60%
                int stunChance = 15 + Mathf.Max(0, (int)(0.6f - playerInfo.GetHealthPercentage()));
                potionTypeChance.Add(stunChance);

                // Saving: 15, +5 (when not Healthy), +10 in Crisis
                int savingChance = 15;
                if (!playerInfo.IsHealthy())
                    savingChance += 5;
                if (!playerInfo.IsInCrisis())
                    savingChance += 10;

                potionTypeChance.Add(savingChance);

                int potionScore = 0;
                foreach (int score in potionTypeChance)
                    potionScore += score;

                float roll = Random.Range(0, potionScore);
                int ceil = potionTypeChance[0];
                for (int i = 0; i < potionTypeChance.Count; i++)
                {
                    if (roll < ceil)
                    {
                        Potion potionDrop = Instantiate(potionDropPrefab, new Vector3(transform.position.x, player.transform.position.y, transform.position.z), Quaternion.Euler(0, 0, 0));
                        potionDrop.SetPotionAttribute(i + 1);
                        potionDrop.gameObject.transform.parent = roomManager.GetCurrent().transform;
                        i = potionTypeChance.Count;
                    }
                    else
                        ceil += (i + 1 < potionTypeChance.Count) ? potionTypeChance[i + 1] : 0;
                }
            }
        }
    }

    public override void SetHealthPowerSpeed(float healthMod, float powerMod, float speedMult)
    {
        base.SetHealthPowerSpeed(healthMod, powerMod, speedMult);
        miniHealthBar.UpdateAmountTxt(currentHealth.ToString("0"));
    }

    public override void MultiplyHealthPowerSpeed(float healthMod, float powerMod, float speedMult)
    {
        base.MultiplyHealthPowerSpeed(healthMod, powerMod, speedMult);
        miniHealthBar.UpdateAmountTxt(currentHealth.ToString("0"));
    }

    public int GetPointValue()
    {
        return pointValue;
    }

    //public void AssignOrder(int i)
    //{
    //    order = i;
    //}
}
