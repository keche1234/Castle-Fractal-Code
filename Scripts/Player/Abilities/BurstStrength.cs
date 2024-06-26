using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstStrength : Ability
{
    protected float duration;
    // Start is called before the first frame update
    public override void Start()
    {
        attribute = 1;
        duration = 10;

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
        attribute = 1;
        duration = 5;

        //buff = (Buff)ScriptableObject.CreateInstance("Buff");
        //buff.SetBuff(modifier, 5);
    }

    public override void StartBuff()
    {
        //if (user.GetBuffs()[attribute].Contains(buff))
        //    user.RemoveBuff(buff, attribute);

        buff = (Buff)ScriptableObject.CreateInstance("Buff");
        buff.SetBuff(modifier, duration);

        //buff.StartPause();
        user.AddBuff(buff, attribute);
    }

    public override float GetMightMult()
    {
        return modifier * 0.075f;
    }

    new public static void SetMinMaxMods()
    {
        minMod = 1;
        maxMod = 2;
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
