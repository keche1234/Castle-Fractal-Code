using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseDebilitator : StrengthDebilitator
{
    // Start is called before the first frame update
    public override void Start()
    {
        triggered = false;
        attribute = 2;
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    public override void Initialize()
    {
        triggered = false;
        attribute = 2;
    }

    new public static void SetMinMaxMods()
    {
        minMod = 0.4f;
        maxMod = 1f;
    }
}
