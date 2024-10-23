using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VioletKnight : Enemy
{
    public GameObject knight; //for pointing purposes
    public GameObject sword;
    public Vector3 initSwordPos;
    public Vector3 initSwordRot;
    public Vector3 initSwordScale;

    public Hitbox attack;

    private float startupTime = 1.25f;
    private float activeTime = 1.5f;
    private float cooldownTime = 0.75f;
    private float spinSpeed = 1620; //.222 seconds to make a full rotation = 4.5 rotations (360 degrees) a second
    private float chaseSpeed = 1.5f;

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

        //try
        //{
        //    spawner = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        //    xLimit = spawner.GetXBorder();
        //    zLimit = spawner.GetZBorder();
        //}
        //catch
        //{
        //    xLimit = 9f;
        //    zLimit = 4.5f;
        //}
    }

    // Update is called once per frame
    public override void Update()
    {
        hitByList.Clear();
        //if (state == ActionState.Waiting || state == ActionState.Startup || state == ActionState.Cooldown)
        //    charRb.velocity *= 0;

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
                    Vector3 distanceVector = new Vector3(player.transform.position.x - knight.transform.position.x, 0, player.transform.position.z - knight.transform.position.z);
                    LookTowardsPlayer();
                    if (distanceVector.magnitude >= 2)
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
        Vector3 lookVector = new Vector3(player.transform.position.x - knight.transform.position.x, 0, player.transform.position.z - knight.transform.position.z);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, lookVector, rotateSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    //protected IEnumerator Step(Vector3 stepVector)
    //{
    //    state = ActionState.Moving;
    //    while (!IsFacingPlayer())
    //    {
    //        LookTowardsPlayer();
    //        yield return null;
    //    }

    //    float moveTime = 0;
    //    float stepTime = 0.25f;
    //    charRb.AddForce(transform.forward.normalized * speed, ForceMode.VelocityChange);
    //    yield return new WaitForSeconds(stepTime);

    //    charRb.velocity *= 0;
    //    state = ActionState.Cooldown;

    //    yield return new WaitForSeconds(0.25f);
    //    state = ActionState.Waiting;
    //}

    protected override IEnumerator Attack()
    {
        Reset(true);
        //Startup
        state = ActionState.Startup;
        sword.transform.localScale *= 2;
        sword.transform.localPosition += new Vector3(0.5f, -0.25f, -0.15f);

        float i = 0;
        while (i < startupTime)
        {
            if (GetMyFreezeTime() <= 0) i += Time.deltaTime;
            yield return null;
        }

        //Spin
        float actTime = 0;
        state = ActionState.Attacking;
        attack.gameObject.SetActive(true);
        GetComponent<Collider>().isTrigger = true;
        Vector3 targetAim = new Vector3(player.transform.position.x - knight.transform.position.x, 0, player.transform.position.z - knight.transform.position.z);
        Vector3 currentAim = targetAim;

        while (actTime <= activeTime && state == ActionState.Attacking)
        {
            if (GetMyFreezeTime() <= 0)
            {
                transform.RotateAround(knight.transform.position, transform.up, spinSpeed * Time.deltaTime);

                //velocity is forward
                targetAim = new Vector3(player.transform.position.x - knight.transform.position.x, 0, player.transform.position.z - knight.transform.position.z);
                currentAim = Vector3.RotateTowards(currentAim, targetAim, rotateSpeed / 2 * Time.deltaTime, 0.0f);

                charRb.velocity = currentAim.normalized * chaseSpeed;
                actTime += Time.deltaTime;

                if (IsOOB(0.5f))
                    ReturnToInBounds(0.5f);
            }
            yield return null;
        }

        //End
        state = ActionState.Cooldown;
        charRb.velocity *= 0;
        GetComponent<Collider>().isTrigger = false;
        attack.ClearConnected();
        attack.gameObject.SetActive(false);

        i = 0;
        while (i < cooldownTime / 2)
        {
            if (GetMyFreezeTime() <= 0) i += Time.deltaTime;
            yield return null;
        }

        sword.transform.localScale /= 2;
        sword.transform.localPosition -= new Vector3(0.5f, -0.25f, -0.15f);

        i = 0;
        while (i < cooldownTime / 2)
        {
            if (GetMyFreezeTime() <= 0) i += Time.deltaTime;
            yield return null;
        }

        state = ActionState.Waiting;
    }

    public override void StunMe(float t)
    {
        base.StunMe(t);
        attack.gameObject.SetActive(false);
    }

    public override void Reset(bool zeroSpeed)
    {
        //StopAllCoroutines();
        sword.transform.localPosition = initSwordPos;
        sword.transform.localEulerAngles = initSwordRot;
        sword.transform.localScale = initSwordScale;
        attack.gameObject.SetActive(false);

        if (zeroSpeed) charRb.velocity *= 0;
    }
}
