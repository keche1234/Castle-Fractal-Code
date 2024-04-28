using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowMinotaur : Enemy
{
    private float runSpeed = 6f;
    private float startupTime = 1f;
    private float cooldownTime = 2f;
    public List<Hitbox> attack;

    public GameObject minotaur; //for pointing purposes

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        state = ActionState.Waiting;
        speed = 1.5f;
        rotateSpeed = 1.5f;
        power = 3f;
        currentHealth = 30;
        maxHealth = 30;
        miniHealthBar.SetMax(maxHealth);
        miniHealthBar.SetValue(currentHealth);

        appearanceRate = 5;

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
        if (GetMyFreezeTime() > 0)
        {
            //GetMyFreezeTime() -= Time.deltaTime;
            charRb.velocity *= 0;
            rotateSpeed = 0;
            frozen = true;
        }
        else if (frozen) //this is the specific act of unfreezing
        {
            charRb.velocity = preVel;
            rotateSpeed = 1.5f;
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
                if (state == ActionState.Waiting || state == ActionState.Moving)
                {
                    //charRb.velocity *= 0;
                    if (player.GetComponent<PlayerController>().GetCurrentHealth() > 0)
                    {
                        Vector3 distanceVector = new Vector3(player.transform.position.x - transform.position.x, 0, player.transform.position.z - transform.position.z);
                        transform.rotation = Quaternion.LookRotation(distanceVector);
                        if (distanceVector.magnitude >= 3)
                        {
                            charRb.velocity = transform.forward * speed;
                            state = ActionState.Moving;
                        }
                        else
                        {
                            charRb.velocity *= 0;
                            StartCoroutine(Attack());
                        }
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

    protected override IEnumerator Attack()
    {
        Reset(true);
        //Aim
        state = ActionState.Startup;
        Vector3 aim = transform.forward;

        yield return new WaitForSeconds(startupTime);

        //Fire!
        state = ActionState.Attacking;
        GetComponent<Collider>().isTrigger = true;
        foreach (Hitbox a in attack)
            a.gameObject.SetActive(true);
        charRb.velocity = transform.forward * runSpeed;
        Vector3 mov = charRb.velocity;

        while (state == ActionState.Attacking)
        {
            if (!knocked && !frozen)
            {
                charRb.velocity = mov; //if being knocked back, don't try to apply current velocity
                foreach (Hitbox a in attack)
                    a.gameObject.SetActive(true);
            }
            else
                foreach (Hitbox a in attack)
                    a.gameObject.SetActive(false);
            yield return null;
        }
        charRb.velocity *= 0;

        //Bump Cooldown
        foreach (Hitbox a in attack)
        {
            a.ClearConnected();
            a.gameObject.SetActive(false);
        }
        yield return null;
        charRb.velocity *= 0;

        float initBump = 2f;
        Vector3 decay = aim.normalized * initBump;
        charRb.AddForce(-decay, ForceMode.VelocityChange);
        yield return null;

        float actionTime = 0;
        float maxBumpTime = 1;

        while (actionTime <= maxBumpTime)
        {
            if (GetMyFreezeTime() <= 0)
            {
                charRb.AddForce(decay * Time.deltaTime, ForceMode.VelocityChange);
                actionTime += Time.deltaTime;
            }

            yield return null;
        }

        yield return new WaitUntil(() => !knocked);
        charRb.velocity *= 0;
        yield return new WaitForSeconds(cooldownTime);
        state = ActionState.Waiting;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if ((collider.gameObject.CompareTag("Wall") || collider.gameObject.CompareTag("Door") || collider.gameObject.CompareTag("Enemy")) && state == ActionState.Attacking)
        {
            GetComponent<Collider>().isTrigger = false;
            state = ActionState.Cooldown;
            charRb.velocity *= 0;
        }
    }

    public override void Reset(bool zeroSpeed)
    {
        //StopAllCoroutines();
        state = ActionState.Waiting;
        if (zeroSpeed) charRb.velocity *= 0;
    }
}
