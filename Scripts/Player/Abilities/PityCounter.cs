using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PityCounter : HealthDrain
{
    // Start is called before the first frame update
    public override void Start()
    {
        triggered = false;
        attribute = 2;

        minMod = 1;
        maxMod = 5;

        debuff = (Debuff)ScriptableObject.CreateInstance("Debuff");
        debuff.SetBuff(modifier * -9, -1);
    }

    // Update is called once per frame
    public override void Update()
    {
        triggerCondition = true;
        if (!triggered) //only try to add buff if its not already active
        {
            if (triggerCondition)
            {
                debuff = (Debuff)ScriptableObject.CreateInstance("Debuff");
                debuff.SetBuff(-9, -1);
                user.AddDebuff(debuff, attribute);
                triggered = true;
            }
        }
        else //only try to remove buff if active
        {
            if (!triggerCondition)
            {
                user.RemoveDebuff(debuff, attribute);
                triggered = false;
            }
        }
    }
}
