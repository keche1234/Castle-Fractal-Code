using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Hitbox
{
    [SerializeField] protected float speed;
    [SerializeField] protected float acceleration;
    [SerializeField] protected float lifeTime; //-1 means infinite distance
    protected float currentTime;
    [SerializeField] protected float lateralHomingStrength; //Determines the rate of rotation, in degrees per second (velocity is always forward for a basic projecitle)
    [SerializeField] protected float homingDuration; //-1 means always homing
    [SerializeField] protected bool piercing; //goes through multiple targets
    [SerializeField] protected bool multihit; //hits multiple times.
    [SerializeField] protected int wallBehavior; //0- phases through, 1- destroyed, 2- bounces
    protected Rigidbody projectileRb;
    //[SerializeField] public GameObject target; //Primarily used for homing (if the projectile has homing)

    // Start is called before the first frame update
    void Start()
    {
        projectileRb = GetComponent<Rigidbody>();
        //gameObject.layer = LayerMask.NameToLayer("Hitbox");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Look Towards with a certain speed
        speed += acceleration * Time.deltaTime;

        //Homing
        if (lateralHomingStrength > 0 && (currentTime < homingDuration || homingDuration < 0))
        {
            //Find objects with target tag
            GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);

            if (targets.Length > 0)
            {
                int closest = 0;
                for (int i = 1; i < targets.Length; i++)
                {
                    if ((targets[i].transform.position - transform.position).magnitude < (targets[closest].transform.position - transform.position).magnitude)
                        closest = i;
                }

                //Calculate direciton
                Vector3 newDirection = Vector3.RotateTowards(new Vector3(transform.forward.x, 0, transform.forward.z),
                                                            new Vector3(targets[closest].transform.position.x - transform.position.x, 0, targets[closest].transform.position.z - transform.position.z),
                                                            (lateralHomingStrength * Mathf.PI / 180) * Time.deltaTime, 0);
                transform.rotation = Quaternion.LookRotation(newDirection);
            }
        }

        projectileRb.velocity = transform.forward * speed;

        if (lifeTime > 0 || homingDuration > 0)
            currentTime += Time.deltaTime;

        if (IsOOB() || (lifeTime > 0 && currentTime >= lifeTime))
            Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider targetCollider)
    {
        Character c = targetCollider.gameObject.GetComponent<Character>();
        if (targetCollider.gameObject.CompareTag(targetTag) && !AlreadyConnected(c) && c.enabled)
        {
            AddConnected(targetCollider.gameObject.GetComponent<Character>());
            Vector3 d;
            if (direction.magnitude != 0)
            {
                d = transform.TransformDirection(direction);
                d = (new Vector3(d.x, 0, d.z)).normalized;
            }
            else
            {
                d = (targetCollider.gameObject.transform.position - gameObject.transform.position);
                if (d.magnitude < 0.01f) d = transform.forward;
                d = (new Vector3(d.x, 0, d.z)).normalized;
            }

            if (c != null)
            {
                if (!c.GetHitByList().Contains(source))
                {
                    c.AddToHitByList(source);
                    source.DealDamage(damageMod, targetCollider.gameObject.GetComponent<Character>(), myPow, d, triggerInvincibility, knockbackMod, preserved, fixedKB);
                }
                AddConnected(c);
            }
            else //Twinotaurs
            {
                Character p = targetCollider.gameObject.transform.parent.GetComponent<Character>();
                if (p != null)
                {
                    if (!p.GetHitByList().Contains(source))
                    {
                        p.AddToHitByList(source);
                        source.DealDamage(damageMod, targetCollider.gameObject.transform.parent.GetComponent<Character>(), myPow, d, triggerInvincibility, knockbackMod, preserved, fixedKB);
                    }
                    AddConnected(p);
                }
            }

            if (!piercing && !multihit)
                Destroy(gameObject);
        }

        //Character c = targetCollider.gameObject.GetComponent<Character>();
        //if (c != null && c.enabled)
        //{
        //    if (!c.GetHitByList().Contains(source))
        //    {
        //        if (targetCollider.gameObject.CompareTag(targetTag) && !AlreadyConnected(c) && c.enabled)
        //        {
        //            connected.Add(targetCollider.gameObject.GetComponent<Character>());
        //            connectedTimer.Add(clearTime);
        //            if (direction.magnitude != 0)
        //                source.DealDamage(damageMod, targetCollider.gameObject.GetComponent<Character>(), myPow,
        //                                    transform.TransformDirection(direction), triggerInvincibility, knockbackMod, preserved);
        //            else
        //            {
        //                Vector3 d = (targetCollider.gameObject.transform.position - gameObject.transform.position);
        //                d = (new Vector3(d.x, 0, d.z)).normalized;
        //                if (targetCollider.gameObject.GetComponent<Character>() != null)
        //                    source.DealDamage(damageMod, targetCollider.gameObject.GetComponent<Character>(), myPow, d, triggerInvincibility, knockbackMod, preserved, fixedKB);
        //                else //Twinotaurs
        //                    source.DealDamage(damageMod, targetCollider.gameObject.transform.parent.GetComponent<Character>(), myPow, d, triggerInvincibility, knockbackMod, preserved, fixedKB);
        //            }

        //            if (!piercing && !multihit)
        //                Destroy(gameObject);
        //        }
        //    }
        //    else
        //        connected.Add(targetCollider.gameObject.GetComponent<Character>());
        //}

        if (targetCollider.gameObject.CompareTag("Wall") && wallBehavior == 1)
            Destroy(gameObject);
    }

    public override void OnTriggerStay(Collider targetCollider)
    {
        if (multihit)
            OnTriggerEnter(targetCollider);
    }

    public void Setup(float fast, Character owningChar, bool p = false, float m = 0, float time = -1, float mod = 1, float latHomFast = 0, bool doesPierce = false, bool hitsMultiple = false, int wallInteract = 1)
    {
        damageMod = mod;
        speed = fast;
        lifeTime = time;
        lateralHomingStrength = latHomFast;
        piercing = doesPierce;
        multihit = hitsMultiple;
        wallBehavior = wallInteract;
        source = owningChar;
        preserved = p;
        myPow = m;

        if (wallBehavior < 0 || wallBehavior > 2)
            wallBehavior = 1;
    }

    public void SetSpeed(float s)
    {
        speed = s;
    }

    public void SetHomingStrength(float h)
    {
        lateralHomingStrength = h;
    }

    protected bool IsOOB()
    {
        if (Mathf.Abs(transform.position.x) > 11 || Mathf.Abs(transform.position.z) > 5.5f)
            return true;
        return false;
    }
}
