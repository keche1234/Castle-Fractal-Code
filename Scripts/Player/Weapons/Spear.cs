using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spear : Weapon
{
    private float startupTime = 18f / 60; //18 frames of startup
    private float activeTime = 6f / 60f; // 6 active frames
    private float cooldownTime = 6f / 60; //6 frames of endlag

    private float sigStartup = 0.5f; //slow down time by 50% for this (realtime) duration
    private float sigActiveTimeA = 1f;
    private float sigActiveTimeB = 6 / 60f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        heavy = true;
        melee = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        RenderReticles(true);
        FindAutoTarget();

        //if (IsInactive() && owner.GetMeleeAuto())
        //{
        //    Enemy target = FindClosestTarget();
        //    RenderSubReticle(target ? target.gameObject : null);
        //}
        //else
        //    subReticle.gameObject.SetActive(false);
    }

    public override IEnumerator Attack(InputDevice device)
    {
        //Auto Aiming
        GameObject target = autoTarget ? autoTarget.gameObject : null;
        owner.transform.rotation = Quaternion.LookRotation(DetermineAttackDirection(device));
        RenderReticles(true);

        //Pull the spear in
        owner.SetMobile(false);
        state = ActionState.Startup;
        owner.SetAttackState(1);

        // Attack Rate, Range Up
        float rate = CalculateRate();
        float range = CalculateRange();
        transform.localPosition = new Vector3(0.25f, 0.2f, 0) * range;

        for (float t = 0; t < startupTime / rate; t += Time.deltaTime)
        {
            RenderSubReticle(target);
            yield return null;
        }

        //Thrust!
        state = ActionState.Active;
        owner.SetAttackState(2);
        transform.localPosition = new Vector3(0f, 0.2f, 1.6f) * range;
        transform.localScale *= 2 * range;

        mainAttack[0].gameObject.SetActive(true);
        mainAttack[0].ClearConnected();
        List<Character> con = mainAttack[1].GetConnected();
        foreach (Character c in con)
            mainAttack[1].AddConnected(c);
        mainAttack[1].gameObject.SetActive(true);

        for (float i = 0; i < activeTime; i += Time.deltaTime)
        {
            con = mainAttack[0].GetConnected();
            foreach (Character c in con)
                mainAttack[1].AddConnected(c);
            RenderSubReticle(target);
            yield return null;
        }

        //Wait, then pull it back in again
        state = ActionState.Cooldown;
        owner.SetAttackState(3);
        foreach (Hitbox item in mainAttack)
        {
            item.ClearConnected();
            item.gameObject.SetActive(false);
        }

        for (float t = 0; t < (cooldownTime / rate) / 2; t += Time.deltaTime)
        {
            RenderSubReticle(target);
            yield return null;
        }
        transform.localPosition = new Vector3(0.25f, 0.2f, 0) * range;
        transform.localScale /= 2 * range;
        for (float t = 0; t < (cooldownTime / rate) / 2; t += Time.deltaTime)
        {
            RenderSubReticle(target);
            yield return null;
        }

        //Set it to normal
        state = ActionState.Inactive;
        owner.SetAttackState(0);
        transform.localPosition = new Vector3(0.25f, 0.2f, 0.5f);
        owner.SetMobile(true);
        yield return null;
    }

    public override IEnumerator Signature(InputDevice device)
    {
        state = ActionState.Startup;
        owner.SetAttackState(1);

        // Signature Damage, Duration Up
        float damage = CalculateSignatureDamage();
        float duration = CalculateSignatureDuration();

        //owner.StopInvincibility();
        owner.OverrideInvincibility(sigStartup + (sigActiveTimeA * duration) + sigActiveTimeB + (cooldownTime / 3) + 1);
        owner.gameObject.GetComponent<Collider>().isTrigger = true;
        owner.SetSigning(true);
        owner.SetMobile(false);
        owner.SetControllable(false);

        // Auto Aim
        Vector3 dir = DetermineAttackDirection(device);
        owner.gameObject.transform.rotation = Quaternion.LookRotation(dir);

        //Slowdown time, for dramatic effect
        float tempTime = Time.timeScale;
        Time.timeScale = 1 / sigSlowdown;
        float t = 0;
        while (t < sigStartup)
        {
            t += Time.deltaTime;
            yield return null;
        }

        //Let 'er rip! Enable the driving hitbox, shoot forward
        Time.timeScale = tempTime;
        state = ActionState.Active;
        owner.SetAttackState(2);
        owner.SetMobile(true);

        // Set new damage mod
        for (int i = 0; i < sigAttack.Count; i++)
            sigAttack[i].SetDamageMod(sigMods[i] * damage);

        owner.gameObject.GetComponent<Rigidbody>().velocity = owner.gameObject.transform.rotation * Vector3.forward * owner.GetSpeed() * 3.5f / sigActiveTimeA;

        //enable the inner hitbox "frame" 1-3, 6-8, 11-13, ... , 56-58.
        float trueDuration = sigActiveTimeA * duration;
        for (float i = 0; i < trueDuration; i += Time.deltaTime)
        {
            int frame = (int)Mathf.Floor(i * 60);
            if (frame % 5 < 1 || frame % 5 > 3)
            {
                sigAttack[0].ClearConnected();
                sigAttack[0].gameObject.SetActive(false);
            }
            else
                sigAttack[0].gameObject.SetActive(true);
            yield return null;
        }
        sigAttack[0].gameObject.SetActive(false);
        owner.gameObject.GetComponent<Rigidbody>().velocity *= 0;

        //Then, the spin
        sigAttack[1].gameObject.SetActive(true);
        yield return new WaitForSeconds(sigActiveTimeB);

        //Cooldown
        owner.gameObject.GetComponent<Collider>().isTrigger = false;
        state = ActionState.Cooldown;
        owner.SetAttackState(3);
        sigAttack[1].gameObject.SetActive(false);
        yield return new WaitForSeconds(cooldownTime / 3);

        state = ActionState.Inactive;
        owner.SetAttackState(0);
        owner.SetControllable(true);
        owner.SetSigning(false);

        // Restore old damage mod
        for (int i = 0; i < sigAttack.Count; i++)
            sigAttack[i].SetDamageMod(sigMods[i]);

        yield return null;
    }

    public new static float GetBasePower()
    {
        return 6;
    }

    public new static float GetBaseDurability()
    {
        return 18;
    }

    public new static int GetSignatureCapacity()
    {
        return 100;
    }

    public void OnEnable()
    {
        for (int i = 0; i < mainAttack.Count; i++)
        {
            mainAttack[i].ClearConnected();
            mainAttack[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < sigAttack.Count; i++)
        {
            sigAttack[i].ClearConnected();
            sigAttack[i].gameObject.SetActive(false);
        }
    }
}
