using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthyDefense : HealthyStrength
{
    // Start is called before the first frame update
    public override void Start()
    {
        triggered = false;
        attribute = 2;

        minMod = 2;
        maxMod = 5;

        buff = (Buff)ScriptableObject.CreateInstance("Buff");
        buff.SetBuff(modifier, -1);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void Initialize()
    {
        triggered = false;
        attribute = 2;

        buff = (Buff)ScriptableObject.CreateInstance("Buff");
        buff.SetBuff(modifier, -1);
    }

    new public static void SetMinMaxMods()
    {
        minMod = 2;
        maxMod = 5;
    }

    new public static float GetMeanMod()
    {
        return ((int)minMod + (int)maxMod + Random.Range(0, 2)) / 2;
    }

    new public static float GetRandomMod()
    {
        return Random.Range((int)minMod, (int)maxMod + 1);
    }
}
