using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeruleanSatyr : Enemy
{
    private float runSpeed = 2f;
    private float runAcceleration = 0.1f;
    private float startupTime = 0.5f;
    private float cooldownTime = 1.5f;
    public List<Hitbox> attack;

    public GameObject satyr; //for pointing purposes
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
            frozen = false;

            rotateSpeed = 1.5f;
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
                        if (distanceVector.magnitude >= 4.5f)
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
        state = ActionState.Startup;
        int strikes = 2;
        yield return new WaitForSeconds(startupTime);
        Vector3 aim = new Vector3(0, 0, 0);

        while (strikes >= 0)
        {
            //Aim
            transform.LookAt(player.gameObject.transform);
            transform.rotation *= Quaternion.Euler(0, 15 * strikes, 0);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            aim = transform.forward;
            state = ActionState.Attacking;

            //Fire!
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
                    mov += transform.forward * runAcceleration * Time.timeScale;
                }
                else
                    foreach (Hitbox a in attack)
                        a.gameObject.SetActive(false);
                yield return null;
            }
            charRb.velocity *= 0;
            strikes--;
            yield return null;
        }

        //Bump Cooldown
        foreach (Hitbox a in attack)
        {
            a.ClearConnected();
            a.gameObject.SetActive(false);
        }
        yield return null;

        //Final strike --> Bump
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
        if ((collider.gameObject.CompareTag("Wall") || collider.gameObject.CompareTag("Door")) && state == ActionState.Attacking)
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
