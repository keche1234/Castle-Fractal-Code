using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : Weapon
{
    private float startupTime = 9f / 60; //9 frames of startup
    private float chainWindow = 15f / 60; //15 frames for the chain window
    private float cooldownTime = 27f / 60; //27 frames of endlag after chain window
    private bool chain = true; //can chain up to three attacks
    private readonly float chainMax = 5;
    private float spread = 10f; //spread per arrow

    private float sigStartup = 0.5f; //slow down time by 50% for this (realtime) duration
    private float sigActiveTime = 5;

    public Projectile arrowPrefab;
    private Projectile[] arrows = new Projectile[5];

    [SerializeField] protected Canvas reticle;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        heavy = false;
        melee = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        float depth = new Vector3(0, cam.transform.position.y, cam.transform.position.z).magnitude;
        reticle.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth));
        reticle.transform.rotation = Quaternion.Euler(70, 0, 0);
    }

    public override IEnumerator Attack()
    {
        //Vector3 startVector = new Vector3(0, 0.8f, 0.6f);
        //Startup: Charge up
        state = ActionState.Startup;
        owner.SetAttackState(1);

        Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.y));
        Vector3 depthVec = (worldPoint - cam.transform.position).normalized;
        float distance = (cam.transform.position.y - 1f) / (depthVec.y != 0 ? Mathf.Abs(depthVec.y) : 1);
        worldPoint = cam.transform.position + (depthVec * distance);
        Vector3 dir = new Vector3(worldPoint.x - owner.gameObject.transform.position.x, 0, worldPoint.z - owner.gameObject.transform.position.z).normalized;

        // Attack Rate, Range Up
        float rate = CalculateRate();
        float range = CalculateRange();
        yield return new WaitForSeconds(startupTime / rate);

        CustomWeapon current = owner.GetCustomWeapon();
        //Chain stuff starts here
        int chainNum = 1;
        while (chain)
        {
            //Fire!
            state = ActionState.Active;
            owner.SetAttackState(2);
            transform.localPosition -= new Vector3(0, 0, 0.1f);

            //Create projectiles based on number of chains
            float width = 0.4f + (0.2f * chainNum);
            float initAngle = -(spread * (1 + chainNum)) / 2; //set up initial angle

            if (dir.magnitude == 0) dir = transform.forward;

            for (int i = 0; i < (2 + chainNum); i++)
            {
                arrows[i] = Instantiate(arrowPrefab, owner.gameObject.transform.position + dir + (Vector3.up * 0.3f), Quaternion.LookRotation(dir));
                arrows[i].transform.position += ((Quaternion.AngleAxis(90, transform.up) * dir) * (-width / 2)) + (Quaternion.AngleAxis(90, transform.up) * dir * i * 0.2f);
                arrows[i].transform.Rotate(new Vector3(0, initAngle + (i * spread), 0));
                arrows[i].Setup(12 * range, owner, true, owner.GetCustomWeapon().GetPower(), 0.5f, (3 + chainNum) / 9f);
                arrows[i].transform.parent = roomManager.GetCurrent().transform;
            }

            if (current.GetMaxDurability() > 0f && current.DecrementDurability(1) <= 0)
            {
                owner.BreakCustomWeapon(current);

                //Restore the properties of the user
                chain = true;
                yield break;
            }
            owner.UpdateDPUI(current.DecrementDurability(0));

            yield return new WaitForSeconds(1f / 60f);
            transform.localPosition += new Vector3(0, 0, 0.1f);

            chain = false;
            //Cooldown/Chain Window: Wait 15 frames for an input
            state = ActionState.Cooldown;
            owner.SetAttackState(3);
            float actionTime = 0;
            while (chainNum < chainMax && actionTime < chainWindow)
            {
                //if you haven't made a new chain AND the total number of chains is less than 3 && the player attempts to chain
                if (!chain && Input.GetMouseButton(owner.GetAtkBtn()))
                {
                    chain = true;
                    chainNum++;
                    actionTime = chainWindow;

                    worldPoint = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.y));
                    depthVec = (worldPoint - cam.transform.position).normalized;
                    distance = (cam.transform.position.y - 1f) / (depthVec.y != 0 ? Mathf.Abs(depthVec.y) : 1);
                    worldPoint = cam.transform.position + (depthVec * distance);
                    dir = new Vector3(worldPoint.x - owner.gameObject.transform.position.x, 0, worldPoint.z - owner.gameObject.transform.position.z).normalized;
                    yield return new WaitForSeconds(startupTime);
                }
                actionTime += Time.deltaTime;
                yield return null;
            }
        }

        //Full Cooldown
        chain = true;
        yield return new WaitForSeconds(cooldownTime / rate);

        //Inactive
        state = ActionState.Inactive;
        owner.SetAttackState(0);
        yield return null;
    }

    public override IEnumerator Signature()
    {
        state = ActionState.Startup;
        owner.SetAttackState(1);

        // Signature Damage, Duration Up
        float damage = CalculateSignatureDamage();
        float duration = CalculateSignatureDuration();

        //owner.StopInvincibility();
        StartCoroutine(owner.GrantInvincibility(sigStartup + (sigActiveTime * duration) + (cooldownTime * 2) + 1));
        owner.gameObject.GetComponent<Collider>().isTrigger = true;
        owner.SetMobile(false);
        owner.SetControllable(false);
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

        //And FIRE!
        Time.timeScale = tempTime;
        state = ActionState.Active;
        owner.SetAttackState(2);
        owner.gameObject.GetComponent<Collider>().isTrigger = true;

        //Every 6 frames shoot an arrow
        float delay = 0;
        for (float i = 0; i < (sigActiveTime * duration); i += Time.deltaTime)
        {
            if (delay >= (6f / 60))
            {
                Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.y));
                Vector3 depthVec = (worldPoint - cam.transform.position).normalized;
                float distance = (cam.transform.position.y - 1f) / (depthVec.y != 0 ? Mathf.Abs(depthVec.y) : 1);
                worldPoint = cam.transform.position + (depthVec * distance);
                Vector3 dir = new Vector3(worldPoint.x - owner.gameObject.transform.position.x, 0, worldPoint.z - owner.gameObject.transform.position.z).normalized;

                Projectile arrow = Instantiate(arrowPrefab, owner.gameObject.transform.position + dir + (Vector3.up * 0.3f), Quaternion.LookRotation(dir));
                arrow.transform.position += ((Quaternion.AngleAxis(90, transform.up) * dir) * -0.6f) + ((Quaternion.AngleAxis(90, transform.up) * dir) * 0.2f);
                arrow.Setup(18, owner, true, owner.GetCustomWeapon().GetPower() * damage, -1, 1.25f, 1, true);
                arrow.SetKB(0, 0, 1, 3, false);
                arrow.transform.parent = roomManager.GetCurrent().transform;
                delay = 0;
            }

            delay += Time.deltaTime;
            yield return null;
        }

        //Endlag
        owner.gameObject.GetComponent<Collider>().isTrigger = false;
        state = ActionState.Cooldown;
        owner.SetAttackState(3);
        yield return new WaitForSeconds(cooldownTime * 2);

        state = ActionState.Inactive;
        owner.SetAttackState(0);
        owner.SetMobile(true);
        owner.SetControllable(true);
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
        return 3;
    }

    public new static float GetBaseDurability()
    {
        return 60;
    }
}
