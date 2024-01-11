using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSnake : Enemy
{
    public Projectile poisonPrefab; //projectile prefab
    private Projectile[] projectiles = new Projectile[3];

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        state = ActionState.Waiting;
        speed = 8f;
        rotateSpeed = 4f;
        power = 1.5f;
        currentHealth = 22;
        maxHealth = 22;
        miniHealthBar.SetMax(maxHealth);
        miniHealthBar.SetValue(currentHealth);

        player = GameObject.Find("Player");
        appearanceRate = 6;

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

    public override void Update()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (freezeTime > 0)
        {
            freezeTime -= Time.deltaTime;
            rotateSpeed = 0;
            charRb.velocity *= 0;
            foreach (Projectile p in projectiles)
            {
                if (p != null) p.gameObject.GetComponent<Projectile>().enabled = false;
            }
        }
        else if (frozen) //this is the specific act of unfreezing
        {
            charRb.velocity = preVel;
            rotateSpeed = 4f;
            frozen = false;
            foreach (Projectile p in projectiles)
            {
                if (p != null) p.gameObject.GetComponent<Projectile>().enabled = true;
            }
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
                if (state == ActionState.Waiting || state == ActionState.Cooldown)
                    LookTowardsPlayer();

                if (state == ActionState.Waiting && player.GetComponent<PlayerController>().GetCurrentHealth() > 0)
                {
                    charRb.velocity *= 0;
                    Vector3 distanceVector = player.transform.position - transform.position;
                    if (distanceVector.magnitude < 4)
                    {
                        StartCoroutine(Push(distanceVector));
                    }
                    else
                    {
                        charRb.velocity *= 0;
                        transform.rotation = Quaternion.LookRotation(distanceVector);
                        StartCoroutine(Attack());
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

        while (freezeTime > 0)
        {
            yield return null;
        }
        yield return null;

        Vector3 friction = pushVector.normalized * speed;
        while (charRb.velocity.normalized == -pushVector.normalized && moveTime < maxMoveTime)
        {
            if (freezeTime <= 0 && stunTime <= 0)
            {
                charRb.AddForce(friction * Time.deltaTime, ForceMode.VelocityChange);
                moveTime += Time.deltaTime;
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
            if (freezeTime <= 0) i += Time.deltaTime;
            yield return null;
        }

        state = ActionState.Attacking;
        projectiles[0] = Instantiate(poisonPrefab, (transform.position + transform.forward), Quaternion.LookRotation(transform.forward));
        projectiles[0].transform.parent = roomManager.GetCurrent().transform;

        projectiles[1] = Instantiate(poisonPrefab, (transform.position + transform.forward - (transform.right * 0.25f)), Quaternion.LookRotation(transform.forward));
        projectiles[1].transform.Rotate(new Vector3(0, -22.5f, 0));
        projectiles[1].transform.parent = roomManager.GetCurrent().transform;

        projectiles[2] = Instantiate(poisonPrefab, (transform.position + transform.forward + (transform.right * 0.25f)), Quaternion.LookRotation(transform.forward));
        projectiles[2].transform.Rotate(new Vector3(0, 22.5f, 0));
        projectiles[2].transform.parent = roomManager.GetCurrent().transform;

        foreach (Projectile shot in projectiles)
        {
            shot.Setup(3, gameObject.GetComponent<Character>());
        }

        state = ActionState.Cooldown;

        i = 0;
        delay = 3;
        while (i < delay)
        {
            if (freezeTime <= 0)
                i += Time.deltaTime;
            yield return null;
        }
        state = ActionState.Waiting;
        yield return null;
    }

    public override void Reset(bool zeroSpeed)
    {
        //StopAllCoroutines();
        if (zeroSpeed)
            charRb.velocity *= 0;
    }
}
