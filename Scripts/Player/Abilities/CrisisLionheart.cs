using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrisisLionheart : Ability
{
    [SerializeField] protected List<float> modifiers;
    [SerializeField] protected List<int> attributes;
    [SerializeField] protected List<Buff> buffs;

    // Start is called before the first frame update
    public override void Start()
    {
        minMod = 5;
        maxMod = 9;

        //Debug.Log("Start c");
        //modifiers = new List<float>();
        //attributes = new List<int>();
        //buffs = new List<Buff>();

        //triggered = false;

        //for (int i = 0; i < 2; i++)
        //{
        //    attributes.Add(i + 1);
        //    buffs.Add((Buff)ScriptableObject.CreateInstance("Buff"));
        //    buffs[i].SetBuff(0, -1);
        //    modifiers.Add(0);
        //}

        //name = "CrisisLionheart";
    }

    // Update is called once per frame
    public override void Update()
    {
        triggerCondition = user.IsInCrisis();
        if (!triggered) //only try to add buffs if its not already active
        {
            if (triggerCondition)
            {
                for (int i = 0; i < buffs.Count; i++)
                {
                    buffs[i] = (Buff)ScriptableObject.CreateInstance("Buff");
                    buffs[i].SetBuff(modifiers[i], -1);
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
            attributes.Add(i + 1);
            buffs.Add((Buff)ScriptableObject.CreateInstance("Buff"));
            buffs[i].SetBuff(0, -1);
            modifiers.Add(0);
        }
    }

    public override void SetModifier(float mod)
    {
        modifier = mod;
        modifiers = new List<float>();
        for (int i = 0; i < 2; i++)
        {
            modifiers.Add(mod);
        }
    }

    //public override List<float> GetMods()
    //{
    //    return modifiers;
    //}

    public override void Deactivate()
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            user.RemoveBuff(buffs[i], attributes[i]);
            //Object.Destroy(buffs[i]);
        }
        triggered = false;
    }

    public override float GetMightMult()
    {
        return modifier * 0.1f;
    }

    public static void SetMinMaxMods()
    {
        minMod = 5;
        maxMod = 9;
    }

    public static float GetMeanMod()
    {
        return ((int)minMod + (int)maxMod) / 2;
    }

    public static float GetRandomMod()
    {
        return Random.Range((int)minMod, (int)maxMod + 1);
    }
}
