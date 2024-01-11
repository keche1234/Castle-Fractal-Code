using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthyWolfsoul : HealthyLionheart
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        triggerCondition = user.GetCurrentHealth() / user.GetMaxHealth() >= .7f;
        if (!triggered) //only try to add buffs if its not already active
        {
            if (triggerCondition)
            {
                for (int i = 0; i < buffs.Count; i++)
                {
                    buffs[i] = (Buff)ScriptableObject.CreateInstance("Buff");
                    buffs[i].SetBuff(modifiers[i] * .1f * (i + 1), -1);
                    user.AddBuff(buffs[i], attributes[i]);
                }
                triggered = true;
            }
        }
        else //only try to remove buffs if active
        {
            if (!triggerCondition)
            {
                for (int i = 0; i < buffs.Count; i++)
                    user.RemoveBuff(buffs[i], attributes[i]);
                triggered = false;
            }
        }
    }

    public override void Initialize()
    {
        modifiers = new List<float>();
        attributes = new List<int>();
        buffs = new List<Buff>();

        triggered = false;

        for (int i = 0; i < 2; i++)
        {
            attributes.Add(i + 3);
            buffs.Add((Buff)ScriptableObject.CreateInstance("Buff"));
            buffs[i].SetBuff(0, -1);
            modifiers.Add(0);
        }
    }

    public override float GetMightMult()
    {
        return modifier * 0.15f;
    }
}
