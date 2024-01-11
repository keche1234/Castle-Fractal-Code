using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : Projectile
{
    [SerializeField] protected Hitbox[] explosions; //array of each hitbox
    [SerializeField] protected float[] explosionTimes; //parallel array for time
    [SerializeField] protected bool triggered;
    // Start is called before the first frame update
    void Start()
    {
        if (wallBehavior < 1 || wallBehavior > 2)
            wallBehavior = 1;

        piercing = false;
        multihit = false;
        triggered = false;

        projectileRb = GetComponent<Rigidbody>();

        foreach (Hitbox e in explosions)
            e.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!triggered)
        {
            //Look Towards with a certain speed
            projectileRb.velocity = transform.forward * speed;

            if (lifeTime > 0)
                currentTime += Time.deltaTime;

            if (IsOOB() || (lifeTime > 0 && currentTime >= lifeTime))
                StartCoroutine("Explode");
        }
    }

    public override void SetTargetTag(string s)
    {
        targetTag = s;
        foreach (Hitbox e in explosions)
            e.SetTargetTag(s);
    }

    public override void SetSource(Character sObject)
    {
        source = sObject;
        foreach (Hitbox e in explosions)
            e.SetSource(sObject);
    }

    public override float GetDamageMod()
    {
        return explosions[0].GetDamageMod();
    }

    public void SetExplosionMod(float m, int i)
    {
        explosions[i].SetDamageMod(m);
    }

    public void SetExplosionMod(float m)
    {
        foreach (Hitbox e in explosions)
            e.SetDamageMod(m);
    }

    public override void OnTriggerEnter(Collider targetCollider)
    {
        if (!triggered)
        {
            if (targetCollider.gameObject.CompareTag(targetTag) && !AlreadyConnected(targetCollider.gameObject.GetComponent<Character>()))
            {
                if (!piercing)
                    StartCoroutine("Explode");
            }

            if (wallBehavior == 1 && targetCollider.gameObject.CompareTag("Wall"))
                StartCoroutine("Explode");
        }
    }

    public IEnumerator Explode()
    {
        triggered = true;
        projectileRb.velocity *= 0;
        for (int i = 0; i < explosions.Length; i++)
        {
            explosions[i].gameObject.SetActive(true);
            explosions[i].SetSource(GetSource());
            explosions[i].SetMyPow(myPow);
            yield return new WaitForSeconds(explosionTimes[i]);
            explosions[i].gameObject.SetActive(false);
        }

        Destroy(gameObject);
        yield return null;
    }
}
