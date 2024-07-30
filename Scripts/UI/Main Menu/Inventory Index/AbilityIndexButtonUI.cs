using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityIndexButtonUI : InventoryIndexButtonUI
{
    [SerializeField] protected int abilityType;
    protected bool numericAbility;
    protected string minValue;
    protected string maxValue;
    // Start is called before the first frame update
    void Start()
    {
        numericAbility = true;
        switch (abilityType)
        {
            case 0:
                StrengthUp.SetMinMaxMods();
                minValue = StrengthUp.GetMinMod().ToString("0");
                maxValue = StrengthUp.GetMaxMod().ToString("0");
                break;
            case 1:
                DefenseUp.SetMinMaxMods();
                minValue = DefenseUp.GetMinMod().ToString("0");
                maxValue = DefenseUp.GetMaxMod().ToString("0");
                break;
            case 2:
                StrengthDebilitator.SetMinMaxMods();
                minValue = (StrengthDebilitator.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (StrengthDebilitator.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 3:
                DefenseDebilitator.SetMinMaxMods();
                minValue = (DefenseDebilitator.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (DefenseDebilitator.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 4:
                AttackRateUp.SetMinMaxMods();
                minValue = (AttackRateUp.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (AttackRateUp.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 5:
                RollRecoveryUp.SetMinMaxMods();
                minValue = (RollRecoveryUp.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (RollRecoveryUp.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 6:
                HealthyStrength.SetMinMaxMods();
                minValue = HealthyStrength.GetMinMod().ToString("0");
                maxValue = HealthyStrength.GetMaxMod().ToString("0");
                break;
            case 7:
                HealthyDefense.SetMinMaxMods();
                minValue = HealthyDefense.GetMinMod().ToString("0");
                maxValue = HealthyDefense.GetMaxMod().ToString("0");
                break;
            case 8:
                HealthySpeed.SetMinMaxMods();
                minValue = (HealthySpeed.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (HealthySpeed.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 9:
                HealthySignatureGain.SetMinMaxMods();
                minValue = (HealthySignatureGain.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (HealthySignatureGain.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 10: //Blade Dull
            case 11: //Armor Pierce
                numericAbility = false;
                break;
            case 12:
                AttackRangeUp.SetMinMaxMods();
                minValue = (AttackRangeUp.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (AttackRangeUp.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 13:
                RollDistanceUp.SetMinMaxMods();
                minValue = (RollDistanceUp.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (RollDistanceUp.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 14:
                BurstStrength.SetMinMaxMods();
                minValue = BurstStrength.GetMinMod().ToString("0");
                maxValue = BurstStrength.GetMaxMod().ToString("0");
                break;
            case 15:
                BurstDefense.SetMinMaxMods();
                minValue = BurstDefense.GetMinMod().ToString("0");
                maxValue = BurstDefense.GetMaxMod().ToString("0");
                break;
            case 16:
                BurstSpeed.SetMinMaxMods();
                minValue = (BurstSpeed.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (BurstSpeed.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 17:
                BurstSignatureGain.SetMinMaxMods();
                minValue = (BurstSignatureGain.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (BurstSignatureGain.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 18:
                LuckyStrike.SetMinMaxMods();
                minValue = (LuckyStrike.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (LuckyStrike.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 19:
                QuickDodge.SetMinMaxMods();
                minValue = (QuickDodge.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (QuickDodge.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 20:
                SignatureDamageUp.SetMinMaxMods();
                minValue = (SignatureDamageUp.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (SignatureDamageUp.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 21:
                SignatureDurationUp.SetMinMaxMods();
                minValue = (SignatureDurationUp.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (SignatureDurationUp.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 22:
                CrisisStrength.SetMinMaxMods();
                minValue = CrisisStrength.GetMinMod().ToString("0");
                maxValue = CrisisStrength.GetMaxMod().ToString("0");
                break;
            case 23:
                CrisisDefense.SetMinMaxMods();
                minValue = CrisisDefense.GetMinMod().ToString("0");
                maxValue = CrisisDefense.GetMaxMod().ToString("0");
                break;
            case 24:
                CrisisSpeed.SetMinMaxMods();
                minValue = (CrisisSpeed.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (CrisisSpeed.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 25:
                CrisisSignatureGain.SetMinMaxMods();
                minValue = (CrisisSignatureGain.GetMinMod() * 100).ToString("0") + "%";
                maxValue = (CrisisSignatureGain.GetMaxMod() * 100).ToString("0") + "%";
                break;
            case 26: //Health Drain
            case 27: //Signature Drain
            case 28: //Pity Counter
            case 29: //Pity Signature
                minValue = "I";
                maxValue = "V";
                break;
            case 30:
                HealthyLionheart.SetMinMaxMods();
                minValue = HealthyLionheart.GetMinMod().ToString("0");
                maxValue = HealthyLionheart.GetMaxMod().ToString("0");
                break;
            case 31:
                CrisisLionheart.SetMinMaxMods();
                minValue = CrisisLionheart.GetMinMod().ToString("0");
                maxValue = CrisisLionheart.GetMaxMod().ToString("0");
                break;
            case 32: //Healthy Wolfsoul
            case 33: //Crisis Wolfsoul
                minValue = "V";
                maxValue = "IX";
                break;
            case 34: //All or Nothing D
            case 35: //All or Nothing S
                numericAbility = false;
                break;
            default:
                minValue = "";
                maxValue = "";
                Debug.LogError("Please only input ability numbers between 0 and " + (Ability.GetNames().Length - 1) + ".");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsNumeric()
    {
        return numericAbility;
    }

    public string GetMinValue()
    {
        return minValue;
    }

    public string GetMaxValue()
    {
        return maxValue;
    }
}
