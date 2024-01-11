using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] protected float modifier;
    [SerializeField] protected bool triggered; //primarily for conditional buffs
    [SerializeField] protected bool triggerCondition;
    [SerializeField] protected int attribute; //0 to indicate this is not an attribute change. See SetAttribute for different buff categories
    [SerializeField] protected Buff buff;
    [SerializeField] protected Character user;

    protected static readonly string[] names = new string[] { "StrengthUp", "DefenseUp", "StrengthDebilitator", "DefenseDebilitator",
                  "AttackRateUp", "DodgeRecoveryUp", "HealthyStrength", "HealthyDefense", "HealthySpeed", "HealthySignatureCharge", "BladeDull", "ArmorPierce",
                  "AttackRangeUp", "DodgeDistanceUp", "BurstStrength", "BurstDefense", "BurstSpeed", "BurstSignatureCharge", "LuckyStrike", "QuickDodge",
                  "SignatureDamageUp", "SignatureDurationUp", "CrisisStrength", "CrisisDefense", "CrisisSpeed", "CrisisSignatureCharge", "HealthDrain", "SignatureDrain",
                  "PityCounter", "PitySignature", "HealthyLionheart", "CrisisLionheart", "HealthyWolfsoul", "CrisisWolfsoul", "AllOrNothingD", "AllOrNothingS" };

    protected static readonly string[] genericNames = new string[] { "Strength Up", "Defense Up", "Strength Debilitator", "Defense Debilitator",
                  "Attack Rate Up", "Dodge Recovery Up", "Healthy Strength", "Healthy Defense", "Healthy Speed", "Healthy Signature Charge", "Blade Dull", "Armor Pierce",
                  "Attack Range Up", "Dodge Distance Up", "Burst Strength", "Burst Defense", "Burst Speed", "Burst Signature Charge", "Lucky Strike", "Quick Dodge",
                  "Signature Damage Up", "Signature Duration Up", "Crisis Strength", "Crisis Defense", "Crisis Speed", "Crisis Signature Charge", "Health Drain", "Signature Drain",
                  "Pity Counter", "Pity Signature", "Healthy Lionheart", "Crisis Lionheart", "Healthy Wolfsoul", "Crisis Wolfsoul", "All or Nothing D", "All or Nothing S" };

    public virtual void Start()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Initialize()
    {

    }

    public void Setup(Character c, float mod)
    {
        user = c;
        modifier = mod;
    }

    public virtual void SetModifier(float mod)
    {
        modifier = mod;
    }

    public float GetModifier()
    {
        return modifier;
    }

    public abstract float GetMightMult();

    public void SetUser(Character c)
    {
        user = c;
    }

    public Character GetUser()
    {
        return user;
    }

    public virtual void StartBuff()
    {

    }

    public virtual void Deactivate()
    {
        if (triggered && attribute > 0)
            user.RemoveBuff(buff, attribute);
        triggered = false;
        enabled = false;
    }

    public void SetAttribute(int type)
    {
        attribute = type;
    }

    public void SetTriggered(bool b)
    {
        triggered = b;
    }

    public static string[] GetNames()
    {
        string[] n = new string[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            n[i] = names[i];
        }

        return n;
    }

    public static string[] GetGenericNames()
    {
        string[] gn = new string[genericNames.Length];
        for (int i = 0; i < genericNames.Length; i++)
        {
            gn[i] = genericNames[i];
        }

        return gn;
    }
}
