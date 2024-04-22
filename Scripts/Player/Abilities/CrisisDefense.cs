using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrisisDefense : CrisisStrength
{
    // Start is called before the first frame update
    public override void Start()
    {
        triggered = false;
        attribute = 2;

        minMod = 4;
        maxMod = 7;

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

    public static void SetMinMaxMods()
    {
        minMod = 4;
        maxMod = 7;
    }
}
