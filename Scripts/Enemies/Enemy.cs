using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    protected float rotateSpeed;
    protected ActionState state;
    protected bool knocked;

    public GameObject player;
    public int appearanceRate;
    //[SerializeField] protected int order;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        player = GameObject.Find("Player");
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
            currentHealth = 0;
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
    }

    public override IEnumerator TakeKnockback(float knockback, Vector3 kbDir, float kbMod = 0)
    {
        knocked = true;
        yield return null;
        charRb.velocity = kbDir.normalized * knockback * kbMod * 20;
        preVel = charRb.velocity;
        yield return new WaitForSeconds(0.5f);
        knocked = false;
        charRb.velocity *= 0;
        preVel = charRb.velocity;
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

    //public void AssignOrder(int i)
    //{
    //    order = i;
    //}
}
