using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedMage : Enemy
{
    private float sizeTime = 0.35f;
    private float stepTime = 0.05f;
    
    public GameObject blastAoe;
    public GameObject maxAoe;

    private float attackCharge = 2.5f;
    private float cooldownTime = 1.5f;

    [SerializeField] protected Hitbox attack;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        state = ActionState.Waiting;
        speed = BASE_SPEED / stepTime;
        rotateSpeed = 100f;
        miniHealthBar.SetMax(maxHealth);
        miniHealthBar.SetValue(currentHealth);

        player = GameObject.Find("Player");
        appearanceRate = 4;

        charRb = GetComponent<Rigidbody>();
        blastAoe.transform.parent = roomManager.GetCurrent().transform;
        maxAoe.transform.parent = roomManager.GetCurrent().transform;

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
            frozen = true;
        }
        else if (frozen) //this is the specific act of unfreezing
        {
            charRb.velocity = preFreezeVelocity;
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
                if (!knocked && state != ActionState.Moving)
                    charRb.velocity *= 0;

                if (state == ActionState.Waiting && player.GetComponent<PlayerController>().GetCurrentHealth() > 0)
                {
                    Vector3 distanceVector = player.transform.position - transform.position;
                    if (distanceVector.magnitude < 4)
                    {
                        StartCoroutine(Teleport(-distanceVector.normalized));
                    }
                    else if (player.GetComponent<PlayerController>().GetCurrentHealth() > 0)
                    {
                        StartCoroutine(Attack());
                    }
                }
            }
            ProgressBuffTime();
        }

        if (currentHealth <= 0)
        {
            Destroy(blastAoe);
            Destroy(maxAoe);
            spawnManager.RemoveMe(this);
        }

        if (IsOOB(2f))
            ReturnToInBounds(2f);

        UpdateAttributeUI();
    }

    protected IEnumerator Teleport(Vector3 aim)
    {
        Reset(true);
        aim = aim.normalized;
        transform.rotation = Quaternion.LookRotation(-aim);

        if ((transform.position.x >= 0 && xLimit - transform.position.x <= 2) ||
            (transform.position.x < 0 && transform.position.x + xLimit <= 2))
            aim = new Vector3(-aim.x, aim.y, aim.z);
        if ((transform.position.z >= 0 && zLimit - transform.position.z <= 2) ||
            (transform.position.z < 0 && transform.position.z + zLimit <= 2))
            aim = new Vector3(aim.x, aim.y, -aim.z);

        state = ActionState.Moving;
        float delayTime = 0;
        while (delayTime <= 0.2f) //delay before teleporting
        {
            if (GetMyFreezeTime() <= 0) delayTime += Time.deltaTime;
            yield return null;
        }

        //Shrink for 0.45 seconds
        GetComponent<Collider>().isTrigger = true;
        Vector3 target = new Vector3(0.01f, 0.01f, 0.01f);

        float xVelocity = 0;
        float yVelocity = 0;
        float zVelocity = 0;

        float newX;
        float newY;
        float newZ;

        float actionTime = 0;
        while (transform.localScale != target)
        {
            if (GetMyFreezeTime() <= 0)
            {
                gameObject.tag = "Untagged"; //immune to freezing
                newX = Mathf.SmoothDamp(transform.localScale.x, target.x, ref xVelocity, sizeTime - actionTime);
                newY = Mathf.SmoothDamp(transform.localScale.y, target.y, ref yVelocity, sizeTime - actionTime);
                newZ = Mathf.SmoothDamp(transform.localScale.z, target.z, ref zVelocity, sizeTime - actionTime);

                actionTime += Time.deltaTime;
                transform.localScale = new Vector3(newX, newY, newZ);
            }
            yield return null;
        }

        //move for 0.1 second
        float t = 0;
        while (t < stepTime)
        {
            if (GetMyFreezeTime() <= 0)
            {
                charRb.velocity = aim * speed;
                t += Time.deltaTime;
            }
            yield return null;
        }

        charRb.velocity *= 0;

        if (IsOOB(0.5f))
            ReturnToInBounds(0.5f);

        //Grow for 0.45s
        target = new Vector3(1f, 1f, 1f);

        xVelocity = 0;
        yVelocity = 0;
        zVelocity = 0;

        actionTime = 0;
        while (transform.localScale != target)
        {
            if (GetMyFreezeTime() <= 0)
            {
                newX = Mathf.SmoothDamp(transform.localScale.x, target.x, ref xVelocity, sizeTime - actionTime);
                newY = Mathf.SmoothDamp(transform.localScale.y, target.y, ref yVelocity, sizeTime - actionTime);
                newZ = Mathf.SmoothDamp(transform.localScale.z, target.z, ref zVelocity, sizeTime - actionTime);

                actionTime += Time.deltaTime;
                transform.localScale = new Vector3(newX, newY, newZ);
            }
            yield return null;
        }

        //Cooldown
        state = ActionState.Cooldown;
        GetComponent<Collider>().isTrigger = false;
        gameObject.tag = "Enemy";
        float i = 0;
        while (i < cooldownTime)
        {
            if (GetMyFreezeTime() <= 0) i += Time.deltaTime;
            yield return null;
        }
        
        state = ActionState.Waiting;
    }

    protected override IEnumerator Attack()
    {
        Reset(true);
        //Startup
        state = ActionState.Startup;
        blastAoe.SetActive(true);
        blastAoe.transform.position = new Vector3(player.transform.position.x, 0.05f, player.transform.position.z);
        blastAoe.transform.localScale *= 0;
        maxAoe.SetActive(true);
        maxAoe.transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z);

        float attackChargeVar = Random.Range(0.8f, 1.25f);

        float t = 0;
        while (t < attackCharge * attackChargeVar)
        {
            if (GetMyFreezeTime() <= 0)
            {
                blastAoe.transform.localScale = new Vector3 ((t / (attackCharge * attackChargeVar)) * maxAoe.transform.localScale.x,
                                                                maxAoe.transform.localScale.y,
                                                                (t / (attackCharge * attackChargeVar)) * maxAoe.transform.localScale.z);
                t += Time.deltaTime;
            }
            yield return null;
        }

        //Blast
        state = ActionState.Attacking;
        attack.gameObject.SetActive(true);
        attack.transform.position = blastAoe.transform.position;
        blastAoe.SetActive(false);
        maxAoe.SetActive(false);
        t = 0;
        while (t < 0.1f)
        {
            if (GetMyFreezeTime() <= 0) t += Time.deltaTime;
            yield return null;
        }

        //Cooldown
        attack.ClearConnected();
        attack.gameObject.SetActive(false);
        state = ActionState.Cooldown;
        t = 0;
        while (t < cooldownTime)
        {
            if (GetMyFreezeTime() <= 0) t += Time.deltaTime;
            yield return null;
        }
        state = ActionState.Waiting;
        yield return null;
    }

    public override void StunMe(float t)
    {
        base.StunMe(t);
        attack.gameObject.SetActive(false);
        blastAoe.SetActive(false);
        maxAoe.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Wall") && state == ActionState.Moving)
        {
            GetComponent<Collider>().isTrigger = false;
            charRb.velocity *= 0;
        }
    }

    public override void Reset(bool zeroSpeed)
    {
        //StopAllCoroutines();
        transform.localScale = new Vector3(1f, 1f, 1f);
        state = ActionState.Waiting;
        blastAoe.SetActive(false);
        maxAoe.SetActive(false);

        if (zeroSpeed) charRb.velocity *= 0;
    }

    public override void OnDestroy()
    {
        Destroy(blastAoe.gameObject);
        Destroy(maxAoe.gameObject);
        base.OnDestroy();
    }
}
