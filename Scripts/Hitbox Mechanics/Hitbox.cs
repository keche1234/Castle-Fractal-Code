using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [Header("Properties")]
    public Character source;
    [SerializeField] protected float damageMod;
    [SerializeField] protected Vector3 direction; //relative to hitbox, 0 vector means "out"
    [SerializeField] protected float knockbackMod;
    [SerializeField] protected bool fixedKB;
    [SerializeField] protected bool triggerInvincibility = true; //default

    [Header("Connections")]
    [SerializeField] protected List<Character> connected;
    [SerializeField] protected List<float> connectedTimer;
    [SerializeField] protected float clearTime;

    [SerializeField] protected string targetTag;
    [SerializeField] protected float myPow;
    [SerializeField] protected bool preserved; //preserve your own damage

    // Start is called before the first frame update
    void Start()
    {
        direction = direction.normalized;
        //gameObject.layer = LayerMask.NameToLayer("Hitbox");
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < connectedTimer.Count(); i++)
            connectedTimer[i] = connectedTimer[i] - Time.deltaTime;

        for (int i = 0; i < connected.Count(); i++)
        {
            if (connectedTimer[i] < 0)
            {
                connectedTimer.RemoveAt(i);
                connected.RemoveAt(i--);
            }
        }
    }

    public virtual void SetSource(Character sObject)
    {
        source = sObject;
    }

    public void SetMyPow(float m)
    {
        myPow = m;
    }

    public Character GetSource()
    {
        return source;
    }

    public void SetDamageMod(float mod)
    {
        damageMod = mod;
    }

    public virtual float GetDamageMod()
    {
        return damageMod;
    }

    public virtual void SetTargetTag(string s)
    {
        targetTag = s;
    }

    public virtual bool GetInvincTrigger()
    {
        return triggerInvincibility;
    }

    public virtual void SetInvincTrigger(bool b)
    {
        triggerInvincibility = b;
    }

    public void SetKB(float x, float y, float z, float mod = 0, bool f = false)
    {
        direction = new Vector3(x, y, z);
        knockbackMod = mod;
        fixedKB = f;
    }

    public virtual void OnTriggerStay(Collider targetCollider)
    {
        Character c = targetCollider.gameObject.GetComponent<Character>();
        if (targetCollider.gameObject.CompareTag(targetTag) && !AlreadyConnected(c))
        {
            AddConnected(targetCollider.gameObject.GetComponent<Character>());
            if (direction.magnitude != 0)
            {
                Vector3 d = transform.TransformDirection(direction);
                d = (new Vector3(d.x, 0, d.z)).normalized;
                if (targetCollider.gameObject.GetComponent<Character>() != null)
                    source.DealDamage(damageMod, targetCollider.gameObject.GetComponent<Character>(), myPow, d, triggerInvincibility, knockbackMod, preserved, fixedKB);
                else //Twinotaurs
                    source.DealDamage(damageMod, targetCollider.gameObject.transform.parent.GetComponent<Character>(), myPow, d, triggerInvincibility, knockbackMod, preserved, fixedKB);
            }
            else
            {
                Vector3 d = (targetCollider.gameObject.transform.position - gameObject.transform.position);
                if (d.magnitude < 0.01f) d = transform.forward;
                d = (new Vector3(d.x, 0, d.z)).normalized;
                if (targetCollider.gameObject.GetComponent<Character>() != null)
                    source.DealDamage(damageMod, targetCollider.gameObject.GetComponent<Character>(), myPow, d, triggerInvincibility, knockbackMod, preserved, fixedKB);
                else //Twinotaurs
                    source.DealDamage(damageMod, targetCollider.gameObject.transform.parent.GetComponent<Character>(), myPow, d, triggerInvincibility, knockbackMod, preserved, fixedKB);
            }
        }
    }

    //checks if the object has already connected with the target
    public bool AlreadyConnected(Character target)
    {
        for (int i = 0; i < connected.Count; i++)
        {
            if (target == connected.ElementAt(i))
                return true;
        }

        return false;
    }

    public void ClearConnected()
    {
        connected = new List<Character>();
        connectedTimer = new List<float>();
    }

    public void AddConnected(Character c)
    {
        connected.Add(c);
        connectedTimer.Add(clearTime);
    }

    public List<Character> GetConnected()
    {
        List<Character> res = new List<Character>();
        foreach (Character c in connected)
            res.Add(c);
        return res;
    }
}
