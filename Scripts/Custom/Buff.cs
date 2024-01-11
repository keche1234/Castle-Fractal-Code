using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : ScriptableObject
{
    [SerializeField] protected float mod, duration, timeRemaining; //duration < 0, means conditional buff. Otherwise, timed.
    [SerializeField] protected bool running; //is the time running on the buff? (timed buffs only)
    [SerializeField] protected Character owner;

    // Start is called before the first frame update
    public virtual void Start()
    {
        running = false;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (duration > 0 && running) //timed buff
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0)
                Destroy(this);
        }
    }

    public virtual void SetBuff(float val, float time)
    {
        mod = val;
        duration = time;
        timeRemaining = duration;
    }

    public float GetMod()
    {
        return mod;
    }

    public float GetDuration()
    {
        return duration;
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public void StartPause()
    {
        running = !running;
    }

    public void SetOwner(Character c)
    {
        owner = c;
    }
}
