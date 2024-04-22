using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRateUp : Ability
{
    // Start is called before the first frame update
    public override void Start()
    {
        minMod = 0.15f;
        maxMod = 0.45f;
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    public override void Initialize()
    {

    }

    public override float GetMightMult()
    {
        return modifier * 0.67f;
    }

    public static void SetMinMaxMods()
    {
        minMod = 0.15f;
        maxMod = 0.45f;
    }
}
