using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkPython : Enemy
{
    [SerializeField] protected Hitbox poisonGas;
    protected float attackTime = 3f;
    protected bool defend = false; //what should the Pink Python do next? False means move, true means attack

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        state = ActionState.Waiting;
        speed = 4f;
        rotateSpeed = 4f;
        power = 1.5f;
        currentHealth = 22;
        maxHealth = 22;
        miniHealthBar.SetMax(maxHealth);
        miniHealthBar.SetValue(currentHealth);

        player = GameObject.Find("Player");
        appearanceRate = 6;

        charRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public override void Update()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        hitByList.Clear();
        if (freezeTime > 0)
        {
            freezeTime -= Time.deltaTime;
            rotateSpeed = 0;
            charRb.velocity *= 0;
            poisonGas.gameObject.GetComponent<Hitbox>().enabled = false;
        }
        else if (frozen) //this is the specific act of unfreezing
        {
            charRb.velocity = preVel;
            rotateSpeed = 4f;
            frozen = false;
            poisonGas.gameObject.GetComponent<Hitbox>().enabled = true;
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
                if (state == ActionState.Waiting || state == ActionState.Attacking)
                    LookTowardsPlayer();

                if (state == ActionState.Waiting && player.GetComponent<PlayerController>().GetCurrentHealth() > 0)
                {
                    charRb.velocity *= 0;
                    Vector3 distanceVector = player.transform.position - transform.position;

                    if (defend)
                    {
                        charRb.velocity *= 0;
                        transform.rotation = Quaternion.LookRotation(distanceVector);
                        StartCoroutine(Attack());
                        defend = !defend;
                    }
                    else
                    {
                        if (distanceVector.magnitude < 4)
                        {
                            StartCoroutine(Push(distanceVector));
                        }
                        else
                        {
                            StartCoroutine(Push(-distanceVector));
                        }
                        defend = !defend;
                    }
                }
            }
            ProgressBuffTime();
        }
        if (currentHealth <= 0)
        {
            spawner.RemoveMe(this);
        }

        if (IsOOB())
            ReturnToInBounds();

        UpdateAttributeUI();
    }

    protected IEnumerator Push(Vector3 pushVector)
    {
        Reset(true);
        state = ActionState.Moving;
        transform.rotation = Quaternion.LookRotation(-pushVector);

        float moveTime = 0;
        float maxMoveTime = 1;

        charRb.AddForce((-pushVector).normalized * speed, ForceMode.VelocityChange);
        yield return null;

        while (freezeTime > 0)
        {
            yield return null;
        }

        Vector3 friction = pushVector.normalized * speed;
        while (charRb.velocity.normalized == -pushVector.normalized && moveTime <= maxMoveTime)
        {
            if (freezeTime <= 0)
            {
                moveTime += Time.deltaTime;
                charRb.AddForce(friction * Time.deltaTime, ForceMode.VelocityChange);
            }

            yield return null;
        }

        yield return new WaitUntil(() => !knocked);
        charRb.velocity *= 0;
        state = ActionState.Cooldown;
        yield return new WaitForSeconds(1);
        state = ActionState.Waiting;
    }

    protected override IEnumerator Attack()
    {
        Reset(true);
        state = ActionState.Startup;
        float i = 0;
        float delay = Random.Range(1.5f, 2);
        while (i < delay)
        {
            if (freezeTime <= 0)
                i += Time.deltaTime;
            yield return null;
        }

        state = ActionState.Attacking;
        i = 0;
        while (i < attackTime)
        {
            int frame = (int)Mathf.Floor(i * 60);
            if (frame % 6 < 1 || frame % 6 > 3)
            {
                poisonGas.ClearConnected();
                poisonGas.gameObject.SetActive(false);
            }
            else
                if (!frozen) poisonGas.gameObject.SetActive(true);

            if (freezeTime <= 0)
                i += Time.deltaTime;
            yield return null;
        }
        poisonGas.gameObject.SetActive(false);
        state = ActionState.Cooldown;

        i = 0;
        delay = 3;
        while (i < delay)
        {
            if (freezeTime <= 0) i += Time.deltaTime;
            yield return null;
        }
        state = ActionState.Waiting;
        yield return null;
    }

    public override void Reset(bool zeroSpeed)
    {
        //StopAllCoroutines();
        if (zeroSpeed) charRb.velocity *= 0;
    }
}
