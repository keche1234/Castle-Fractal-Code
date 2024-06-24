using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueOgre : Enemy
{
    private float liftSpeed = 90;
    private float attackSpeed = 180;
    private float cooldownTime = 1.5f;
    private float returnSpeed = 180;
    protected float stepTime = 0.35f;

    public GameObject ogre; //for pointing purposes
    public GameObject club;
    public Vector3 initClubPos;
    public Vector3 initClubRot;
    public Vector3 initClubScale;

    public List<Hitbox> attack;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        miniHealthBar.gameObject.transform.parent.GetComponent<UIAttach>().Setup(gameObject, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(0, -75));
        attributesUI.GetComponent<UIAttach>().Setup(gameObject, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(5, -125));
        state = ActionState.Waiting;
        rotateSpeed = 2f;
        power = 6f;
        currentHealth = 46;
        maxHealth = 46;
        miniHealthBar.SetMax(maxHealth);
        miniHealthBar.SetValue(currentHealth);

        player = GameObject.Find("Player");
        appearanceRate = 3;

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
            //freezeTime -= Time.deltaTime;

            charRb.velocity *= 0;
            rotateSpeed = 0;
            liftSpeed = 0;
            attackSpeed = 0;
            returnSpeed = 0;
            frozen = true;
        }
        else if (frozen) //this is the specific act of unfreezing
        {
            charRb.velocity = preFreezeVelocity;
            frozen = false;

            rotateSpeed = 2f;
            liftSpeed = 90f;
            attackSpeed = 180;
            returnSpeed = 180;
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
                stunTime = 0;
                if (state == ActionState.Waiting && player.GetComponent<PlayerController>().GetCurrentHealth() > 0)
                {
                    if (!knocked) charRb.velocity *= 0;
                    LookTowardsPlayer();
                    Vector3 distanceVector = new Vector3(player.transform.position.x - ogre.transform.position.x, 0, player.transform.position.z - ogre.transform.position.z);
                    if (distanceVector.magnitude >= 4 && IsFacingPlayer())
                        StartCoroutine(Step(distanceVector));
                    else
                    {
                        if (IsFacingPlayer())
                        {
                            StartCoroutine(Attack());
                        }
                    }
                }
            }
            ProgressBuffTime();
        }

        if (transform.position.y < 1)
            transform.Translate(0, 1 - transform.position.y, 0);

        if (currentHealth <= 0)
        {
            spawnManager.RemoveMe(this);
        }

        if (IsOOB())
            ReturnToInBounds();

        UpdateAttributeUI();
    }

    protected override void LookTowardsPlayer()
    {
        Vector3 lookVector = new Vector3(player.transform.position.x - ogre.transform.position.x, 0, player.transform.position.z - ogre.transform.position.z);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, lookVector, rotateSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    protected override bool IsFacingPlayer()
    {
        LayerMask mask = LayerMask.GetMask("Player");
        Ray ray = new Ray();
        RaycastHit hit;
        ray.origin = ogre.transform.position;
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
        state = ActionState.Startup;
        float maxTime = ((float)90 / liftSpeed) + Random.Range(-0.1f, 0.1f);
        if (!knocked) charRb.velocity *= 0;

        //Lift
        float actionTime = 0;
        float degreesRotated = 0;

        club.transform.localScale *= 3.333f;
        club.transform.localPosition -= new Vector3(1.5f, 0, 0);
        while (actionTime < maxTime)
        {
            club.transform.RotateAround(ogre.transform.position, ogre.transform.forward, -liftSpeed * Time.deltaTime);

            degreesRotated += liftSpeed * Time.deltaTime;
            if (GetMyFreezeTime() <= 0) actionTime += Time.deltaTime;

            yield return null;
        }
        club.transform.RotateAround(ogre.transform.position, ogre.transform.forward, degreesRotated - 90);
        yield return new WaitForSeconds(0.5f);

        //Strike
        state = ActionState.Attacking;
        actionTime = 0;
        degreesRotated = 0;

        maxTime = 90 / attackSpeed;

        //activate first hitbox
        attack[0].gameObject.SetActive(true);
        while (actionTime < maxTime)
        {
            club.transform.RotateAround(ogre.transform.position, ogre.transform.right, attackSpeed * Time.deltaTime);

            degreesRotated += attackSpeed * Time.deltaTime;
            if (GetMyFreezeTime() <= 0)
            {
                actionTime += Time.deltaTime;
                attack[0].gameObject.SetActive(true);
            }
            else
                attack[0].gameObject.SetActive(false);

            yield return null;
        }
        yield return new WaitForSeconds(1 / 60f);
        //activate ground hitboxes
        for (int i = 1; i < 3; i++)
        {
            attack[i].gameObject.SetActive(true);
        }

        club.transform.RotateAround(ogre.transform.position, ogre.transform.right, degreesRotated - 90);

        //float rTime = 0.5f;
        float r = 0;
        while (r <= cooldownTime - 0.5f)
        {
            if (GetMyFreezeTime() <= 0)
            {
                r += Time.deltaTime;
                for (int i = 0; i < 3; i++)
                {
                    attack[i].gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    attack[i].gameObject.SetActive(false);
                }
            }
            yield return null;
        }

        //Return
        //deactivate all hitboxes
        for (int i = 0; i < 3; i++)
        {
            attack[i].ClearConnected();
            attack[i].gameObject.SetActive(false);
        }

        //returnSpeed = (float)90 / rTime;
        club.transform.localScale /= 3.333f;
        club.transform.localPosition -= new Vector3(0, 0, 1.5f);

        actionTime = 0;
        degreesRotated = 0;
        while (actionTime < 0.5f)
        {
            club.transform.RotateAround(ogre.transform.position, ogre.transform.up, -returnSpeed * Time.deltaTime);

            degreesRotated += -returnSpeed * Time.deltaTime;
            if (GetMyFreezeTime() <= 0) actionTime += Time.deltaTime;

            yield return null;
        }
        club.transform.RotateAround(ogre.transform.position, ogre.transform.up, -(degreesRotated + 90));
        club.transform.RotateAround(club.transform.position, club.transform.up, 90);

        r = 0;
        while (r <= 0.1f)
        {
            if (GetMyFreezeTime() <= 0) r += Time.deltaTime;
            yield return null;
        }
        state = ActionState.Waiting;
    }

    public override void Reset(bool zeroSpeed)
    {
        //StopAllCoroutines();
        club.transform.localPosition = initClubPos;
        club.transform.localEulerAngles = initClubRot;
        club.transform.localScale = initClubScale;

        for (int i = 0; i < 3; i++)
            attack[i].gameObject.SetActive(false);

        if (zeroSpeed) charRb.velocity *= 0;
    }
}
