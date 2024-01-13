using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tome : Weapon
{
    private float startupTime = 18f / 60f; //18 frames of startup
    private float cooldownTime = 18f / 60f; //18 frames of endlag

    public Explosive fireball;
    private float crisisMod = 1; //increases to 1.1 in Crisis (health is 25% or less)

    private float sigStartup = 1f; //slow down time by 50% for this (realtime) duration
    private float sigActiveTime = 7f;

    [SerializeField] protected Canvas reticle;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        heavy = true;
        melee = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (owner.InCrisis())
            crisisMod = 1.1f;
        else
            crisisMod = 1;

        reticle.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.y - owner.transform.position.y));
        reticle.transform.rotation = Quaternion.Euler(70, 0, 0);
    }

    public override IEnumerator Attack()
    {
        owner.SetMobile(false);
        state = ActionState.Startup;
        owner.SetAttackState(1);
        Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.y - owner.transform.position.y));
        Vector3 dir = (new Vector3(mousePos.x - owner.gameObject.transform.position.x, 0, mousePos.z - owner.gameObject.transform.position.z)).normalized;

        // Attack Rate, Range Up
        float rate = CalculateRate();
        float range = CalculateRange();
        if (dir.magnitude == 0) dir = transform.forward;
        yield return new WaitForSeconds(startupTime / rate);

        //Fire!
        state = ActionState.Active;
        owner.SetAttackState(2);
        //Create fireball
        Explosive attack = Instantiate(fireball, owner.gameObject.transform.position + dir + (Vector3.up * 0.6f), Quaternion.LookRotation(dir));
        attack.gameObject.transform.localScale *= range;
        attack.Setup(4f, owner, true, owner.GetCustomWeapon().GetPower(), -1, crisisMod);
        attack.transform.parent = roomManager.GetCurrent().transform;
        yield return new WaitForSeconds(1f / 60f);

        CustomWeapon current = owner.GetCustomWeapon();
        if (current.GetMaxDurability() > 0f && current.DecrementDurability(1) <= 0)
        {
            state = ActionState.Inactive;
            owner.SetAttackState(0);
            owner.SetMobile(true);
            owner.BreakCustomWeapon(current);
            yield break;
        }

        owner.UpdateDPUI(current.DecrementDurability(0));

        //Cooldown
        state = ActionState.Cooldown;
        owner.SetAttackState(3);
        yield return new WaitForSeconds(cooldownTime / rate);

        //Inactive
        state = ActionState.Inactive;
        owner.SetAttackState(0);
        owner.SetMobile(true);
        yield return null;
    }

    public override IEnumerator Signature()
    {
        //Startup
        state = ActionState.Startup;
        owner.SetAttackState(1);

        // Signature Damage, Duration Up
        float damage = CalculateSignatureDamage();
        float duration = CalculateSignatureDuration();

        owner.SetMobile(false);
        //owner.StopInvincibility();
        StartCoroutine(owner.GrantInvincibility((sigStartup * sigSlowdown * duration * damage)+ sigActiveTime + 1));
        owner.gameObject.GetComponent<Collider>().isTrigger = true;
        owner.SetSigning(true);

        //Slowdown time, for dramatic effect
        float tempTime = Time.timeScale;
        Time.timeScale = 1 / sigSlowdown;
        float t = 0;
        while (t < sigStartup)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        //Freeze Time!
        Time.timeScale = tempTime;
        state = ActionState.Active;
        owner.SetAttackState(2);
        owner.FreezeTime("Enemy", (sigActiveTime * duration * damage) + 2);
        yield return new WaitForSeconds(2);

        //And... go
        owner.SetMobile(true);
        owner.SetControllable(true);
        owner.gameObject.GetComponent<Collider>().isTrigger = false;
        state = ActionState.Inactive;
        owner.SetAttackState(0);
        owner.SetSigning(false);
        yield return null;
    }

    void OnEnable()
    {
        reticle.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        reticle.gameObject.SetActive(false);
    }

    public new static float GetBasePower()
    {
        return 15;
    }

    public new static float GetBaseDurability()
    {
        return 20;
    }
}
