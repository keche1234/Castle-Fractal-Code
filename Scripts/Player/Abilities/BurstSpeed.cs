using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstSpeed : BurstStrength
{
    // Start is called before the first frame update
    public override void Start()
    {
        attribute = 3;

        //buff = (Buff)ScriptableObject.CreateInstance("Buff");
        //buff.SetBuff(modifier, 5);
    }

    // Update is called once per frame
    public override void Update()
    {

    }

    public override void Initialize()
    {
        attribute = 3;
        duration = 10;

        //buff = (Buff)ScriptableObject.CreateInstance("Buff");
        //buff.SetBuff(modifier, 5);
    }

    public override void StartBuff()
    {
        base.StartBuff();
    }

    public override float GetMightMult()
    {
        return modifier * 1.2f;
    }
}
