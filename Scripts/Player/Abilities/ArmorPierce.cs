using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPierce : Ability
{
    // Start is called before the first frame update
    public override void Start()
    {
        triggered = false;
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    public override float GetMightMult()
    {
        return 0.3f;
    }
}
