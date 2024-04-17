using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckyStrike : Ability
{
    // Start is called before the first frame update
    public override void Start()
    {
        minMod = 0.22f;
        maxMod = 0.48f;
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
        return modifier * 2;
    }
}
