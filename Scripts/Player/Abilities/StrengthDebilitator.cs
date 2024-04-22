using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthDebilitator : Ability
{
    // Start is called before the first frame update
    public override void Start()
    {
        triggered = false;
        attribute = 1;
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    public override void Initialize()
    {
        triggered = false;
        attribute = 1;
    }

    public override float GetMightMult()
    {
        return modifier * 0.3f;
    }

    public static void SetMinMaxMods()
    {
        minMod = 0.4f;
        maxMod = 1f;
    }
}
