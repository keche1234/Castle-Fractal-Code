using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sword : Weapon
{
    private float startupTime = 8f / 60f; //8 start up frames
    private float attackTime = 3f / 60f; //3 active frames
    private float chainWindow = 24f / 60f; //24 frames for the chain window
    private float cooldownTime = 16f / 60f; //16 endlag frames
    private bool chain = true; //can chain up to three attacks
    private readonly float chainMax = 3;
    private List<Character> chainHitList; // set to true on hit, reset to false at the end of attack

    private float sigStartup = 0.5f; //slow down time by 50% for this (realtime) duration
    private float sigActiveTime = 3;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        chainHitList = new List<Character>();

        heavy = false;
        melee = true;
    }

    // Update is called once per frame
    protected override void Update()
    {

    }

    public override IEnumerator Attack()
    {
        chainHitList = new List<Character>();
        state = ActionState.Startup;
        owner.SetAttackState(1);

        // Attack Rate, Range Up
        float rate = CalculateRate();
        float range = CalculateRange();

        //Startup
        float actionTime = 0;
        float degreesRotated = 0;
        while (actionTime <= startupTime / rate)
        {
            transform.RotateAround(owner.transform.position, owner.transform.up, (180 / (startupTime / rate)) * Time.deltaTime * -1);
            actionTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale *= 2 * range;

        float finalDist = 1.5f * range;
        transform.localPosition -= new Vector3(finalDist - 0.5f, 0, 0);

        int chainNum = 1;
        while (chain)
        {
            state = ActionState.Active;
            owner.SetAttackState(2);
            mainAttack[chainNum - 1].gameObject.SetActive(true);
            mainAttack[chainNum - 1].ClearConnected();

            actionTime = 0;

            //Swipe
            degreesRotated = 0;
            while (actionTime <= attackTime)
            {
                float angSpeed = (180 / attackTime) * (1 + (((2 * chainNum) - 1) / 20)) * -1;
                for (int i = 0; i < chainNum; i++)
                {
                    angSpeed *= -1;
                }

                transform.RotateAround(owner.transform.position, owner.transform.up, angSpeed * Time.deltaTime);

                degreesRotated += angSpeed * Time.deltaTime;
                actionTime += Time.deltaTime;

                yield return null;
            }
            yield return new WaitForSeconds(1f / 60f);

            chain = false;
            //Cooldown/Chain Window: Wait 12 frames for an input
            state = ActionState.Cooldown;
            owner.SetAttackState(3);
            mainAttack[chainNum - 1].gameObject.SetActive(false);

            actionTime = 0;
            while (chainNum < chainMax && actionTime <= chainWindow)
            {
                //if you haven't made a new chain AND the total number of chains is less than 3 && the player attempts to chain
                if (!chain && Input.GetMouseButton(owner.GetAtkBtn()))
                {
                    chain = true;
                    chainNum++;
                    actionTime = chainWindow;
                    yield return new WaitForSeconds(startupTime * 0.5f / rate);
                }
                actionTime += Time.deltaTime;
                yield return null;
            }
        }
        if (chainNum % 2 == 1) transform.localPosition -= new Vector3(finalDist - 0.5f, 0, 0);
        else transform.localPosition += new Vector3(finalDist - 0.5f, 0, 0);
        yield return new WaitForSeconds(cooldownTime * 0.5f / rate);
        transform.localScale /= 2 * range;

        //Full Cooldown
        chain = true;
        actionTime = 0;
        while (actionTime <= (cooldownTime * 0.4f / rate))
        {
            float angSpeed;
            if (chainNum % 2 == 0) angSpeed = -(degreesRotated + 27) / (cooldownTime * 0.8f / rate);
            else angSpeed = -(degreesRotated - 180) / (cooldownTime * 0.8f / rate);

            transform.RotateAround(owner.transform.position, owner.transform.up, angSpeed * Time.deltaTime);
            actionTime += Time.deltaTime;

            yield return null;
        }
        InitializeTransform();
        yield return new WaitForSeconds(cooldownTime * 0.1f / rate);

        chainHitList = new List<Character>();
        owner.SetAttackState(0);
        state = ActionState.Inactive;
    }

    public override IEnumerator Signature()
    {
        state = ActionState.Startup;
        owner.SetAttackState(1);

        // Signature Damage, Duration Up
        float damage = CalculateSignatureDamage();
        float duration = CalculateSignatureDuration();

        owner.SetMobile(false);
        //owner.StopInvincibility();
        StartCoroutine(owner.GrantInvincibility(sigStartup + (sigActiveTime * duration) + (cooldownTime * 2)));
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

        //Let 'er rip! Grant the user a 100% speed buff, enable and disable the signature hitbox for 3 seconds, then enable the final hitbox!
        Time.timeScale = tempTime;
        owner.SetMobile(true);
        owner.AddDirectMult(2f, 3);
        //Buff b = (Buff)ScriptableObject.CreateInstance("Buff");
        //b.SetBuff(0.5f + (1.5f * owner.SummationBuffs(3)), sigActiveTime);
        //owner.AddBuff(b, 3); //speed

        state = ActionState.Active;
        owner.SetAttackState(2);

        // Set new damage mod
        for (int i = 0; i < sigAttack.Count; i++)
            sigAttack[i].SetDamageMod(sigMods[i] * damage);

        //enable the inner hitbox "frame" 1-3, 6-8, 11-13, ... , 231-233.
        for (float i = 0; i < (sigActiveTime * duration) - (1 / 15f); i += Time.deltaTime)
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

        //Big finisher
        owner.SetMobile(false);
        owner.AddDirectMult(1, 3);
        yield return new WaitForSeconds(6f / 60f);
        sigAttack[1].gameObject.SetActive(true);
        yield return new WaitForSeconds(3f / 60f);

        //Cooldown
        owner.gameObject.GetComponent<Collider>().isTrigger = false;
        state = ActionState.Cooldown;
        owner.SetAttackState(3);
        owner.SetSigning(false);
        sigAttack[1].gameObject.SetActive(false);
        yield return new WaitForSeconds(cooldownTime * 2);
        owner.SetMobile(true);
        state = ActionState.Inactive;
        owner.SetAttackState(0);

        // Restore old damage mod
        for (int i = 0; i < sigAttack.Count; i++)
            sigAttack[i].SetDamageMod(sigMods[i]);

        yield return null;
    }

    public List<Character> ChainHitList()
    {
        return new List<Character>(chainHitList);
    }

    public void ChainHit(Character c)
    {
        chainHitList.Add(c);
    }

    public new static float GetBasePower()
    {
        return 9;
    }

    public new static float GetBaseDurability()
    {
        return 24;
    }
}
