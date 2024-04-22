using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstDefense : BurstStrength
{
    // Start is called before the first frame update
    public override void Start()
    {
        attribute = 2;

        minMod = 1;
        maxMod = 2;

        //buff = (Buff)ScriptableObject.CreateInstance("Buff");
        //buff.SetBuff(modifier, 5);
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    public override void Initialize()
    {
        attribute = 2;
        duration = 10;

        //buff = (Buff)ScriptableObject.CreateInstance("Buff");
        //buff.SetBuff(modifier, 5);
    }

    public override void StartBuff()
    {
        base.StartBuff();
    }

    public static void SetMinMaxMods()
    {
        minMod = 1;
        maxMod = 2;
    }
}
