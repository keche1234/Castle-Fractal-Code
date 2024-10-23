using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeridotPaladin : Enemy
{
    //objects
    public GameObject templar; //for pointing purposes
    public GameObject lance;
    public GameObject shield;

    //initial lance transform
    public Vector3 initLancePos;
    public Vector3 initLanceRot;
    public Vector3 initLanceScale;

    //initial shield transform
    public Vector3 initShieldPos;
    public Vector3 initShieldRot;
    public Vector3 initShieldScale;

    //attacks
    public Hitbox lanceSwing;
    public Projectile lanceBeamPrefab;
    private Projectile lanceBeam;

    private float counterWindow = 2f;
    private bool counterHit = false;
    private float counterMod = 1.0f;

    private float swingDelay = 0.8f;
    private float startupTime = 0.1f;
    private float swingTime = 0.1f;
    private float cooldownTime = 3f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        state = ActionState.Waiting;
        rotateSpeed = 2.5f;
        miniHealthBar.SetMax(maxHealth);
        miniHealthBar.SetValue(currentHealth);
        armored = true;

        player = GameObject.Find("Player");
        appearanceRate = 2;

        charRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public override void Update()
    {
        //if (state == ActionState.Waiting || state == ActionState.Startup || state == ActionState.Cooldown)
        //    charRb.velocity *= 0;

        hitByList.Clear();
        if (GetMyFreezeTime() > 0)
        {
            //freezeTime -= Time.deltaTime;
            charRb.velocity *= 0;
            rotateSpeed = 0;
            frozen = true;
        }
        else if (frozen) //this is the specific act of unfreezing
        {
            charRb.velocity = preFreezeVelocity;
            rotateSpeed = 2.5f;
            frozen = false;
        }
        else
        {
            if (stunTime > 0)
            {
                stunTime -= Time.deltaTime;
                if (stunTime > stunCooldown) transform.Rotate(new Vector3(0, stunRotateSpeed * Time.deltaTime, 0));
                else transform.Rotate(new Vector3(0, Mathf.Max(0, stunRotateSpeed * ((stunTime - (stunCooldown / 2)) / (stunCooldown / 2))) * Time.deltaTime, 0));
            }
            else
            {
                if ((state == ActionState.Waiting || state == ActionState.Moving) && player.GetComponent<PlayerController>().GetCurrentHealth() > 0)
                {
                    Vector3 distanceVector = new Vector3(player.transform.position.x - templar.transform.position.x, 0, player.transform.position.z - templar.transform.position.z);
                    LookTowardsPlayer();
                    if (distanceVector.magnitude >= 4)
                    {
                        charRb.velocity = transform.forward * speed;
                        state = ActionState.Moving;
                    }
                    else
                    {
                        StartCoroutine(Attack());
                    }
                }
            }
            ProgressBuffTime();
        }

        if (currentHealth <= 0)
        {
            spawnManager.RemoveMe(this);
        }

        if (IsOOB(2f))
            ReturnToInBounds(2f);

        UpdateAttributeUI();
    }

    protected override void LookTowardsPlayer()
    {
        Vector3 lookVector = new Vector3(player.transform.position.x - templar.transform.position.x, 0, player.transform.position.z - templar.transform.position.z);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, lookVector, rotateSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    protected void LookAwayFromPlayer()
    {
        Vector3 lookVector = new Vector3(templar.transform.position.x - player.transform.position.x, 0, templar.transform.position.z - player.transform.position.z);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, lookVector, rotateSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    public override void TakeDamage(int damage, Vector3 kbDir, bool triggerInvinc = true, float kbMod = 0, bool fixKB = false)
    {
        base.TakeDamage(damage, kbDir, triggerInvinc, kbMod, fixKB);

        if (state == ActionState.Startup && !counterHit && damage > 0)
        {
            counterMod = 1 + ((damage * 2f) / maxHealth);
            counterHit = true;
        }
    }

    protected override IEnumerator Attack()
    {
        Reset(true);
        //Startup
        state = ActionState.Startup;

        float i = 0;
        while (!counterHit)
        {
            if (GetMyFreezeTime() <= 0)
            {
                LookTowardsPlayer();
                i += Time.deltaTime;
                if (i >= counterWindow)
                    counterHit = true;
                charRb.velocity *= 0;
            }
            yield return null;
        }

        //Fire:
        //Rear up
        float actTime = 0;

        while (counterMod <= 1.0f && actTime < swingDelay)
        {
            if (GetMyFreezeTime() <= 0)
            {
                actTime += Time.deltaTime;
                charRb.velocity *= 0;
            }
            yield return null;
        }

        actTime = 0;
        while (actTime < startupTime)
        {
            if (GetMyFreezeTime() <= 0)
            {
                shield.transform.RotateAround(templar.transform.position, Vector3.up, (-90 / startupTime) * Time.deltaTime);
                lance.transform.RotateAround(templar.transform.position, Vector3.up, (-105 / startupTime) * Time.deltaTime);
                actTime += Time.deltaTime;
            }
            yield return null;
        }

        //Swing
        actTime = 0;
        lanceSwing.gameObject.SetActive(true);
        state = ActionState.Attacking;
        while (actTime < swingTime)
        {
            if (GetMyFreezeTime() <= 0)
            {
                shield.transform.RotateAround(templar.transform.position, Vector3.up, (-15 / startupTime) * Time.deltaTime);
                lance.transform.RotateAround(templar.transform.position, Vector3.up, (120 / startupTime) * Time.deltaTime);
                actTime += Time.deltaTime;
            }

            yield return null;
        }

        //Create Beam
        lanceBeam = Instantiate(lanceBeamPrefab, templar.transform.position + transform.forward, Quaternion.LookRotation(transform.forward));
        lanceBeam.transform.Translate(0, -0.5f, 0);
        lanceBeam.transform.parent = roomManager.GetCurrent().transform;
        lanceBeam.Setup(10, gameObject.GetComponent<PeridotPaladin>(), true, 0, 0.4f, 1, 0, true, false, 0);
        lanceBeam.SetDamageMod(counterMod);
        lanceSwing.SetHitboxLinks(lanceBeam);
        lanceBeam.gameObject.SetActive(true);
        yield return null;

        //End
        state = ActionState.Cooldown;
        charRb.velocity *= 0;
        GetComponent<Collider>().isTrigger = false;
        lanceSwing.ClearConnected();
        lanceSwing.gameObject.SetActive(false);

        //Jump back
        float moveTime = 0;
        float maxMoveTime = 1f;

        Vector3 targetAim = new Vector3(player.transform.position.x - templar.transform.position.x, 0, player.transform.position.z - templar.transform.position.z);
        charRb.AddForce((-targetAim).normalized * speed * 4, ForceMode.VelocityChange);
        Vector3 friction = targetAim.normalized * speed * 4;
        while (Vector3.Angle(charRb.velocity, -targetAim) < 15f && moveTime < maxMoveTime)
        {
            if (GetMyFreezeTime() <= 0)
            {
                charRb.AddForce(friction * Time.deltaTime, ForceMode.VelocityChange);
                moveTime += Time.deltaTime;
            }
            yield return null;
        }
        yield return null;

        charRb.velocity *= 0;

        i = 0;
        while (i < cooldownTime - maxMoveTime)
        {
            if (GetMyFreezeTime() <= 0)
                i += Time.deltaTime;
            yield return null;
        }

        state = ActionState.Waiting;
        Reset(true);
        yield return null;
    }

    public override void StunMe(float t)
    {
        base.StunMe(t);
    }

    public override void Reset(bool zeroSpeed)
    {
        lance.transform.localPosition = initLancePos;
        lance.transform.localEulerAngles = initLanceRot;
        lance.transform.localScale = initLanceScale;

        shield.transform.localPosition = initShieldPos;
        shield.transform.localEulerAngles = initShieldRot;
        shield.transform.localScale = initShieldScale;

        lanceSwing.gameObject.SetActive(false);
        counterMod = 1.0f;
        counterHit = false;

        if (zeroSpeed)
            charRb.velocity *= 0;
    }
}
