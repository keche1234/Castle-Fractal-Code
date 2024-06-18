using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Axe : Weapon
{
    private float startupTime = 15f / 60; //15 frames of startup
    private float activeTime = 0.05f; // 3 active frames
    private float cooldownTime = 18f / 60; //18 frames of endlag

    private float sigStartup = 0.5f; //slow down time by 50% for this (realtime) duration
    private float sigActiveTime = 0.5f;

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
        if (IsInactive() && owner.GetMeleeAuto())
        {
            Enemy target = FindClosestTarget();
            RenderSubReticle(target ? target.gameObject : null);
        }
        else
            subReticle.gameObject.SetActive(false);
    }

    public override IEnumerator Attack(InputDevice device)
    {
        //Auto Aiming
        GameObject target = MeleeAutoAim();

        //Push the axe out
        owner.SetMobile(false);
        state = ActionState.Startup;
        owner.SetAttackState(1);

        // Attack Rate, Range Up
        float rate = CalculateRate();
        float range = CalculateRange();
        transform.localPosition = new Vector3(0, 0.2f, 1.25f) * range;

        //Lift
        float actionTime = 0;
        float degreesRotated = 0;

        while (actionTime < startupTime / rate)
        {
            transform.RotateAround(owner.transform.position, owner.transform.right, (-195 / (startupTime / rate)) * Time.deltaTime);
            degreesRotated += (-195 / (startupTime / rate)) * Time.deltaTime;

            RenderSubReticle(target);
            actionTime += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = Quaternion.Euler(-90, 0, 0); //correction

        //Active frames
        state = ActionState.Active;
        transform.localScale *= 2 * range;
        mainAttack[0].gameObject.SetActive(true);
        foreach (Hitbox h in mainAttack)
            h.ClearConnected();
        owner.SetAttackState(2);

        actionTime = 0;
        degreesRotated = 0;
        //bool groundCollision = false;

        while (actionTime < (startupTime / rate))
        {
            transform.RotateAround(owner.transform.position, owner.transform.right, (195 / (startupTime / rate)) * Time.deltaTime);
            degreesRotated += (195 / startupTime) * Time.deltaTime;

            RenderSubReticle(target);
            actionTime += Time.deltaTime;

            //if (degreesRotated >= 180 && !groundCollision)
            //{
            //    groundCollision = true;
            //    mainAttack[1].gameObject.SetActive(true);
            //    mainAttack[2].gameObject.SetActive(true);
            //    transform.localPosition += new Vector3(0, -0.5f, -0.5f);
            //}

            yield return null;
        }
        transform.localRotation = Quaternion.Euler(105, 0, 0); //correction
        List<Character> con = mainAttack[0].GetConnected();

        for (int i = 1; i < mainAttack.Count; i++)
        {
            foreach (Character c in con)
                mainAttack[i].AddConnected(c);
            mainAttack[i].gameObject.SetActive(true);
        }

        actionTime = 0;
        while (actionTime < activeTime)
        {
            RenderSubReticle(target);
            actionTime += Time.deltaTime;
            yield return null;
        }

        //Cooldown (shift to the side)
        state = ActionState.Cooldown;
        owner.SetAttackState(3);

        foreach (Hitbox item in mainAttack)
        {
            item.ClearConnected();
            item.gameObject.SetActive(false);
        }

        actionTime = 0;
        while (actionTime < cooldownTime / rate)
        {
            RenderSubReticle(target);
            actionTime += Time.deltaTime;
            yield return null;
        }

        state = ActionState.Inactive;
        transform.localPosition = new Vector3(-.25f, 0.2f, 0.2f) * range;
        transform.localRotation = Quaternion.Euler(90, 0, 0);
        transform.localScale /= 2 * range;
        owner.SetAttackState(0);
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

        owner.SetControllable(false);
        //owner.StopInvincibility();
        owner.OverrideInvincibility(sigStartup + (sigActiveTime * duration) + (cooldownTime * 2) + 1);
        owner.gameObject.GetComponent<Collider>().isTrigger = true;
        owner.SetSigning(true);

        //Slowdown time, for dramatic effect
        float tempTime = Time.timeScale;
        Time.timeScale = 1 / sigSlowdown;
        float t = 0;
        while (t < sigStartup)
        {
            t += Time.deltaTime;
            yield return null;
        }

        //Let 'er rip! Enable the signature hitbox, shoot forward
        Time.timeScale = tempTime;
        state = ActionState.Active;
        owner.SetAttackState(2);
        owner.SetMobile(true);

        // Set new damage mod
        for (int i = 0; i < sigAttack.Count; i++)
        {
            sigAttack[i].SetDamageMod(sigMods[i] * damage);
            sigAttack[i].ClearConnected();
        }

        owner.gameObject.GetComponent<Rigidbody>().velocity = (owner.gameObject.transform.rotation * Vector3.forward * owner.GetSpeed()) / sigActiveTime;
        yield return new WaitForSeconds(3 / 60f);
        sigAttack[0].gameObject.SetActive(true);
        yield return new WaitForSeconds((sigActiveTime * duration) - (3 / 60f));

        //Cooldown
        owner.gameObject.GetComponent<Rigidbody>().velocity *= 0;
        owner.gameObject.GetComponent<Collider>().isTrigger = false;
        state = ActionState.Cooldown;
        owner.SetAttackState(3);
        sigAttack[0].gameObject.SetActive(false);
        owner.SetSigning(false);
        yield return new WaitForSeconds(cooldownTime * 2);

        state = ActionState.Inactive;
        owner.SetAttackState(0);
        owner.SetControllable(true);

        // Restore old damage mod
        for (int i = 0; i < sigAttack.Count; i++)
            sigAttack[i].SetDamageMod(sigMods[i]);

        yield return null;
    }

    public new static float GetBasePower()
    {
        return 12;
    }

    public new static float GetBaseDurability()
    {
        return 30;
    }
}
