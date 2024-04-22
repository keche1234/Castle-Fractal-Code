using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseUp : StrengthUp
{
    // Start is called before the first frame update
    public override void Start()
    {
        triggered = false;
        attribute = 2;

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
        minMod = 1;
        maxMod = 3;
    }
}
