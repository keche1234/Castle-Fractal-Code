using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomWeapon : ScriptableObject
{
    [SerializeField] protected int weapon; //type: 0 --> 4 are Sword, Axe, Spear, Crossbow, Tome
    [Header("Basic Attributes")]
    [SerializeField] protected float power;
    [SerializeField] protected float currentDurability;
    [SerializeField] protected float maxDurability; //Non-positive => Infinite Durability
    [SerializeField] protected int signatureGauge;
    [SerializeField] protected int signatureCap; //weapon specific
    [SerializeField] protected int mightPoints;

    //[Header("Abilities (type and potency)")]
    [SerializeField] protected List<int> abilities;
    [SerializeField] protected List<float> mods; //parallel array


    // Start is called before the first frame update
    void Start()
    {
        if (abilities != null) abilities = new List<int>();
        if (mods != null) mods = new List<float>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(int w, float pow, float cDur, float mDur, int sg, List<int> a, List<float> m)
    {
        weapon = w;
        switch (weapon)
        {
            case 0: //Sword
                signatureCap = 225;
                break;
            case 1: //Axe
                signatureCap = 375;
                break;
            case 2: //Spear
                signatureCap = 175;
                break;
            case 3: //Crossbow
                signatureCap = 300;
                break;
            case 4: //Tome
                signatureCap = 425;
                break;
            default:
                break;
        }

        power = pow;
        currentDurability = cDur;
        maxDurability = mDur;
        signatureGauge = sg;
        AddSignature(0); //correct overflow or underflow
        abilities = new List<int>();
        mods = new List<float>();

        if (a != null)
            for (int i = 0; i < a.Count; i++)
                abilities.Add(a[i]);

        if (m != null)
            for (int i = 0; i < m.Count; i++)
                mods.Add(m[i]);

        mightPoints = CalculateMightPoints();
    }

    public void Setup(int w, float pow, float cDur, float mDur, int sg, List<int> a, List<float> m, int mp)
    {
        Setup(w, pow, cDur, mDur, sg, a, m);
        mightPoints = mp;
    }

    public int CalculateMightPoints()
    {
        float mightMod = 1;

        if (abilities != null)
            for (int i = 0; i < abilities.Count; i++)
            {
                GameObject empty = new GameObject();
                empty.SetActive(false);
                empty.AddComponent(System.Type.GetType(Ability.GetNames()[abilities[i]]));
                Ability ab = empty.GetComponent<Ability>();
                ab.SetModifier(mods[i]);
                mightMod += ab.GetMightMult();
                Destroy(empty);
            }

        switch (weapon)
        {
            case 0: //Sword
                mightMod *= power / Sword.GetBasePower();
                mightMod *= maxDurability / Sword.GetBaseDurability();
                break;
            case 1: //Axe
                mightMod *= power / Axe.GetBasePower();
                mightMod *= maxDurability / Axe.GetBaseDurability();
                break;
            case 2: //Spear
                mightMod *= power / Spear.GetBasePower();
                mightMod *= maxDurability / Spear.GetBaseDurability();
                break;
            case 3: //Crossbow
                mightMod *= power / Crossbow.GetBasePower();
                mightMod *= maxDurability / Crossbow.GetBaseDurability();
                break;
            case 4: //Tome
                mightMod *= power / Tome.GetBasePower();
                mightMod *= maxDurability / Tome.GetBaseDurability();
                break;
            default:
                break;
        }

        return (int) (100 * mightMod);
    }

    public int GetWeaponType()
    {
        return weapon;
    }

    public string GetWeaponName()
    {
        switch (weapon)
        {
            case 0:
                return "Sword";
            case 1:
                return "Axe";
            case 2:
                return "Spear";
            case 3:
                return "Crossbow";
            case 4:
                return "Tome";
            default:
                return "N/A";
        }
    }

    public float GetPower()
    {
        return power;
    }

    public List<int> GetAbilities()
    {
        List<int> a = new List<int>();
        for (int i = 0; i < abilities.Count; i++) a.Add(abilities[i]);
        return a;
    }

    public List<float> GetMods()
    {
        List<float> m = new List<float>();
        for (int i = 0; i < mods.Count; i++) m.Add(mods[i]);
        return m;
    }

    public int GetMightPoints()
    {
        return mightPoints;
    }

    public float DecrementDurability(float dur)
    {
        currentDurability -= dur;
        return currentDurability;
    }

    public float GetMaxDurability()
    {
        return maxDurability;
    }

    public void AddSignature(int i)
    {
        signatureGauge += i;
        if (signatureGauge < 0) signatureGauge = 0;
        else if (signatureGauge > signatureCap) signatureGauge = signatureCap;
    }

    public void ResetSignature()
    {
        signatureGauge = 0;
    }

    public float SignaturePercentage()
    {
        return ((float)signatureGauge) / signatureCap;
    }

    public int GetSignatureGauge()
    {
        return signatureGauge;
    }

    public int GetSignatureCap()
    {
        return signatureCap;
    }

    //public static string[] GetNames()
    //{
    //    return names;
    //}

    //public static int GetNumAbilities()
    //{
    //    return names.Length;
    //}
}
