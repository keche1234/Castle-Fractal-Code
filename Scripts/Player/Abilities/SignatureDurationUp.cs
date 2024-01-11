using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignatureDurationUp : Ability
{
    // Start is called before the first frame update
    public override void Start()
    {

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
