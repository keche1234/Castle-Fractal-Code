using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignatureDrain : HealthDrain
{
    // Start is called before the first frame update
    public override void Start()
    {
        triggered = false;
        attribute = 1;
        minMod = 1;
        maxMod = 5;

        debuff = (Debuff)ScriptableObject.CreateInstance("Debuff");
        debuff.SetBuff(modifier * -5, -1);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    new public static void SetMinMaxMods()
    {
        minMod = 1;
        maxMod = 5;
    }

    new public static float GetMeanMod()
    {
        return ((int)minMod + (int)maxMod) / 2;
    }

    new public static float GetRandomMod()
    {
        return Random.Range((int)minMod, (int)maxMod + 1);
    }
}
