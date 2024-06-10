using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrisisSignatureGain : CrisisStrength
{
    // Start is called before the first frame update
    public override void Start()
    {
        triggered = false;
        attribute = 4;

        minMod = 0.8f;
        maxMod = 1.4f;

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
        attribute = 4;

        buff = (Buff)ScriptableObject.CreateInstance("Buff");
        buff.SetBuff(modifier, -1);
    }

    public override float GetMightMult()
    {
        return modifier * 0.65f;
    }

    new public static void SetMinMaxMods()
    {
        minMod = 0.8f;
        maxMod = 1.4f;
    }

    new public static float GetMeanMod()
    {
        return (minMod + maxMod) / 2;
    }

    new public static float GetRandomMod()
    {
        return Random.Range(minMod, maxMod);
    }
}
