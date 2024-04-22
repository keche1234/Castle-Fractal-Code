using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthUp : Ability
{
    // Start is called before the first frame update
    public override void Start()
    {
        triggered = false;
        attribute = 1;

        buff = (Buff)ScriptableObject.CreateInstance("Buff");
        buff.SetBuff(modifier, -1);
    }

    // Update is called once per frame
    public override void Update()
    {
        triggerCondition = true;
        if (!triggered) //only try to add buff if its not already active
        {
            if (triggerCondition)
            {
                buff = (Buff)ScriptableObject.CreateInstance("Buff");
                buff.SetBuff(modifier, -1);
                user.AddBuff(buff, attribute);
                triggered = true;
            }
        }
        else //only try to remove buff if active
        {
            if (!triggerCondition)
            {
                user.RemoveBuff(buff, attribute);
                triggered = false;
            }
        }
    }

    public override void Initialize()
    {
        triggered = false;
        attribute = 1;

        buff = (Buff)ScriptableObject.CreateInstance("Buff");
        buff.SetBuff(modifier, -1);
    }

    public override float GetMightMult()
    {
        return modifier * 0.1f;
    }

    public static void SetMinMaxMods()
    {
        minMod = 1;
        maxMod = 3;
    }
}
