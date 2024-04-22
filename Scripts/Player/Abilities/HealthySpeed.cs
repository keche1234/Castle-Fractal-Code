using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthySpeed : HealthyStrength
{
    // Start is called before the first frame update
    public override void Start()
    {
        triggered = false;
        attribute = 3;

        minMod = 0.2f;
        maxMod = 0.5f;

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
        attribute = 3;

        buff = (Buff)ScriptableObject.CreateInstance("Buff");
        buff.SetBuff(modifier, -1);
    }

    public override float GetMightMult()
    {
        return modifier * 1.1f;
    }

    public static void SetMinMaxMods()
    {
        minMod = 0.2f;
        maxMod = 0.5f;
    }
}
