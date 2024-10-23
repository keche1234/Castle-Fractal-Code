using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TangerineTroll : Enemy
{
    public GameObject troll; //for pointing purposes

    protected float stepTime = 0.35f;
    protected int stepsRemaining = 3;

    protected float jumpTime = 15f / 60;
    protected float jumpHeight = 3f;
    protected float jumpDistance = 2f;

    protected float attackDelay = 30f / 60;
    protected float attackTime = 15f / 60;
    protected float shockwaveTime = 6f / 60;

    protected float cooldownTime = 1.5f;

    public List<Hitbox> attack;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        miniHealthBar.gameObject.transform.parent.GetComponent<UIAttach>().Setup(gameObject, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(0, -75));
        attributesUI.GetComponent<UIAttach>().Setup(gameObject, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(5, -125));
        state = ActionState.Waiting;
        rotateSpeed = 2f;
        miniHealthBar.SetMax(maxHealth);
        miniHealthBar.SetValue(currentHealth);

        player = GameObject.Find("Player");
        appearanceRate = 3;

        charRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public override void Update()
    {
        //if (state != ActionState.Moving && !inKB)
        //    charRb.velocity *= 0;
        hitByList.Clear();
        if (GetMyFreezeTime() > 0)
        {
            //GetMyFreezeTime() -= Time.deltaTime;
            charRb.velocity *= 0;
            rotateSpeed = 0;
            frozen = true;
        }
        else if (frozen) //this is the specific act of unfreezing
        {
            charRb.velocity = preFreezeVelocity;
            rotateSpeed = 2f;
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
                if (state == ActionState.Waiting && player.GetComponent<PlayerController>().GetCurrentHealth() > 0)
                {
                    transform.position = new Vector3(transform.position.x, player.transform.position.y + 0.5f, transform.position.z);
                    if (!knocked) charRb.velocity *= 0;
                    LookTowardsPlayer();
                    Vector3 distanceVector = new Vector3(player.transform.position.x - troll.transform.position.x, 0, player.transform.position.z - troll.transform.position.z);
                    if (distanceVector.magnitude >= jumpDistance && IsFacingPlayer() && stepsRemaining > 0)
                    {
                        StartCoroutine(Step(distanceVector));
                        stepsRemaining--;
                    }
                    else
                    {
                        if (IsFacingPlayer())
                        {
                            StartCoroutine(Attack());
                            stepsRemaining = 3;
                        }
                    }
                }
            }
            ProgressBuffTime();
        }

        //if (transform.position.y < 1.3f)
        //    transform.Translate(0, 1.3f - transform.position.y, 0);

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
        Vector3 lookVector = new Vector3(player.transform.position.x - troll.transform.position.x, 0, player.transform.position.z - troll.transform.position.z);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, lookVector, rotateSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    protected override bool IsFacingPlayer()
    {
        LayerMask mask = LayerMask.GetMask("Player");
        Ray ray = new Ray();
        RaycastHit hit;
        ray.origin = troll.transform.position;
        ray.direction = transform.TransformDirection(Vector3.forward);
        return Physics.Raycast(ray, out hit, 100f, mask);
    }

    protected IEnumerator Step(Vector3 stepVector)
    {
        state = ActionState.Moving;

        float speedChange = (1 + SummationBuffs(3)) * (1 + (Mathf.Max(SummationDebuffs(3), -0.5f)));
        charRb.AddForce(transform.forward.normalized * speed * speedChange * directMults[3], ForceMode.VelocityChange);
        preFreezeVelocity = charRb.velocity;

        float i = 0;
        while (i < (stepTime / speedChange))
        {
            if (GetMyFreezeTime() <= 0) i += Time.deltaTime;
            yield return null;
        }

        charRb.velocity *= 0;
        preFreezeVelocity = charRb.velocity;
        state = ActionState.Cooldown;

        i = 0;
        while (i < (stepTime / speedChange))
        {
            if (GetMyFreezeTime() <= 0) i += Time.deltaTime;
            yield return null;
        }

        state = ActionState.Waiting;
    }

    protected override IEnumerator Attack()
    {
        Reset(false);
        armored = true;

        //Jump!
        state = ActionState.Startup;
        charRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        float actionTime = 0;
        Vector3 jumpVel = (new Vector3(player.transform.position.x - troll.transform.position.x, 0, player.transform.position.z - troll.transform.position.z)).normalized;
        jumpVel *= jumpDistance / jumpTime;
        jumpVel += new Vector3(0, jumpHeight / jumpTime, 0);

        while (actionTime < jumpTime || transform.position.y - 1 < jumpHeight)
        {
            if (GetMyFreezeTime() <= 0)
            {
                if (!knocked)
                {
                    charRb.velocity = jumpVel;
                }
                actionTime += Time.deltaTime;
            }
            yield return null;
        }

        //Delay in the air for the ground pound
        charRb.velocity *= 0;
        actionTime = 0;
        while (actionTime < attackDelay)
        {
            if (GetMyFreezeTime() <= 0)
            {
                actionTime += Time.deltaTime;
            }
            yield return null;
        }

        //Ground pound!
        charRb.velocity *= 0;
        state = ActionState.Attacking;
        attack[0].gameObject.SetActive(true);
        GetComponent<Collider>().isTrigger = true;
        actionTime = 0;
        while (state == ActionState.Attacking)
        {
            if (GetMyFreezeTime() <= 0)
            {
                charRb.velocity = new Vector3(0, -jumpHeight / attackTime, 0);
                actionTime += Time.deltaTime;
            }
            yield return null;
        }

        state = ActionState.Attacking;
        actionTime = 0;
        while (actionTime < shockwaveTime)
        {
            if (GetMyFreezeTime() <= 0)
            {
                charRb.velocity *= 0;
                actionTime += Time.deltaTime;
            }
            yield return null;
        }

        //Standup (cooldown)
        state = ActionState.Cooldown;
        charRb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        foreach (Hitbox h in attack)
            h.gameObject.SetActive(false);

        actionTime = 0;
        while (actionTime < cooldownTime)
        {
            if (GetMyFreezeTime() <= 0)
            {
                charRb.velocity *= 0;
                actionTime += Time.deltaTime;
            }
            yield return null;
        }

        state = ActionState.Waiting;
        armored = false;
        yield return null;
    }

    public override void StunMe(float t)
    {
        base.StunMe(t);
        transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Floor") && state == ActionState.Attacking)
        {
            attack[1].gameObject.SetActive(true); //shockwave
            GetComponent<Collider>().isTrigger = false;

            state = ActionState.Cooldown;
            charRb.velocity *= 0;
        }
    }

    public override void Reset(bool zeroSpeed)
    {
        //StopAllCoroutines();
        //for (int i = 0; i < 3; i++)
        //    attack[i].gameObject.SetActive(false);

        if (zeroSpeed) charRb.velocity *= 0;
    }
}
