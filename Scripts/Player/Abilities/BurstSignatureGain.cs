using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstSignatureGain : BurstStrength
{
    // Start is called before the first frame update
    public override void Start()
    {
        attribute = 4;

        minMod = 0.2f;
        maxMod = 0.4f;

        //buff = (Buff)ScriptableObject.CreateInstance("Buff");
        //buff.SetBuff(modifier, 5);
    }

    // Update is called once per frame
    public override void Update()
    {

    }

    public override void Initialize()
    {
        attribute = 4;
        duration = 10;

        //buff = (Buff)ScriptableObject.CreateInstance("Buff");
        //buff.SetBuff(modifier, 5);
    }

    public override void StartBuff()
    {
        base.StartBuff();
    }

    public override float GetMightMult()
    {
        return modifier * 0.6f;
    }

    new public static void SetMinMaxMods()
    {
        minMod = 0.2f;
        maxMod = 0.4f;
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
