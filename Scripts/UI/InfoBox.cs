using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoBox : MonoBehaviour
{
    [SerializeField] protected PlayerController player; //to access weapon and potion info.

    [Header("The Box")]
    [SerializeField] protected Image box;
    [SerializeField] protected Image border;
    [SerializeField] protected Color filledColor;
    [SerializeField] protected Color emptyColor;

    [Header("Weapon Info")]
    [SerializeField] protected GameObject weaponGroup;
    [SerializeField] protected Text weaponName;
    [SerializeField] protected Text mightPoints;
    [SerializeField] protected Text power;
    [SerializeField] protected Text durabilityVal;
    [SerializeField] protected BarUI durabilityBar;
    [SerializeField] protected BarUI signatureBar;
    [SerializeField] protected List<Image> abilityIcons; //MY icons (only two)
    [SerializeField] protected List<Image> abilityFills; //MY fills (only two)
    [SerializeField] protected List<Image> abilityBorders; //MY borders (only two)
    [SerializeField] protected List<Text> abilityNames; //MY abilities (only two)
    [SerializeField] protected List<Color> colors; //blue and red

    [Header("Potion Info")]
    [SerializeField] protected GameObject potionGroup;
    [SerializeField] protected Text potionType;
    [SerializeField] protected Text potionDescription;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearDescription()
    {
        box.color = emptyColor;
        border.color = emptyColor;

        weaponGroup.SetActive(false);
        potionGroup.SetActive(false);
    }

    public void DrawDescription(WeaponButton wb)
    {
        ClearDescription();
        if (wb.GetComponent<Button>().enabled)
        {
            box.color = filledColor;
            border.color = filledColor;
            weaponGroup.SetActive(true);

            CustomWeapon current = player.GetCustomWeapon(wb.GetWeaponNumber());

            weaponName.text = current.GetWeaponName();
            mightPoints.text = "(" + current.GetMightPoints() + " MP)";
            power.text = "Power: " + current.GetPower();
            durabilityVal.text = "(" + current.DecrementDurability(0) + "/" + current.GetMaxDurability() + ")";
            durabilityBar.SetMax(current.GetMaxDurability());
            durabilityBar.SetValue(current.DecrementDurability(0));
            signatureBar.SetMax(current.GetSignatureCap());
            signatureBar.SetValue(current.GetSignatureGauge());

            List<Image> buttonIcons = wb.GetAbilityIcons();
            for (int i = 0; i < Mathf.Min(current.GetAbilities().Count, abilityIcons.Count); i++)
            {
                abilityIcons[i].gameObject.SetActive(true);
                abilityNames[i].gameObject.SetActive(true);
                abilityFills[i].gameObject.SetActive(true);
                abilityBorders[i].gameObject.SetActive(true);

                abilityIcons[i].sprite = buttonIcons[i].sprite;
                abilityNames[i].text = Ability.GetGenericNames()[current.GetAbilities()[i]];

                switch (current.GetAbilities()[i])
                {
                    case 2: //Strength Debilitator Chance
                        abilityFills[i].color = colors[1];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 3: //Defense Debilitator Chance
                        abilityFills[i].color = colors[1];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 4: //Attack Rate
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 5: //Dodge Recovery
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 8: //Healthy Speed
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 9: //Healthy Signature Gain
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 10: //Blade Dull
                        abilityFills[i].color = colors[1];
                        break;
                    case 11: //Armor Pierce
                        abilityFills[i].color = colors[1];
                        break;
                    case 12: //Attack Range Up
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 13: //Dodge Distance Up
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 16: //Burst Speed
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 17: //Burst Signature Gain
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 18: //Lucky Strike
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 19: //Quick Dodge
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 20: //Signature Damage
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 21: //Signature Duration
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 24: //Crisis Speed
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 25: //Crisis Signature
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + (current.GetMods()[i] * 100f) + "%";
                        break;
                    case 26: //Health Drain
                        abilityFills[i].color = colors[0];
                        float hdMod = current.GetMods()[i];
                        if (hdMod == 1)
                            abilityNames[i].text += " I";
                        else if (hdMod == 2)
                            abilityNames[i].text += " II";
                        else if (hdMod == 3)
                            abilityNames[i].text += " III";
                        else if (hdMod == 4)
                            abilityNames[i].text += " IV";
                        else if (hdMod == 5)
                            abilityNames[i].text += " V";
                        break;
                    case 27: //Signature Drain
                        abilityFills[i].color = colors[0];
                        float sdMod = current.GetMods()[i];
                        if (sdMod == 1)
                            abilityNames[i].text += " I";
                        else if (sdMod == 2)
                            abilityNames[i].text += " II";
                        else if (sdMod == 3)
                            abilityNames[i].text += " III";
                        else if (sdMod == 4)
                            abilityNames[i].text += " IV";
                        else if (sdMod == 5)
                            abilityNames[i].text += " V";
                        break;
                    case 28: //Pity Counter
                        abilityFills[i].color = colors[0];
                        float pcMod = current.GetMods()[i];
                        if (pcMod == 1)
                            abilityNames[i].text += " I";
                        else if (pcMod == 2)
                            abilityNames[i].text += " II";
                        else if (pcMod == 3)
                            abilityNames[i].text += " III";
                        else if (pcMod == 4)
                            abilityNames[i].text += " IV";
                        else if (pcMod == 5)
                            abilityNames[i].text += " V";
                        break;
                    case 29: //Pity Signature
                        abilityFills[i].color = colors[0];
                        float psMod = current.GetMods()[i];
                        if (psMod == 1)
                            abilityNames[i].text += " I";
                        else if (psMod == 2)
                            abilityNames[i].text += " II";
                        else if (psMod == 3)
                            abilityNames[i].text += " III";
                        else if (psMod == 4)
                            abilityNames[i].text += " IV";
                        else if (psMod == 5)
                            abilityNames[i].text += " V";
                        break;
                    case 32: //Healthy Wolfsoul
                        abilityFills[i].color = colors[0];
                        float hwMod = current.GetMods()[i];
                        if (hwMod == 1)
                            abilityNames[i].text += " I";
                        else if (hwMod == 2)
                            abilityNames[i].text += " II";
                        else if (hwMod == 3)
                            abilityNames[i].text += " III";
                        else if (hwMod == 4)
                            abilityNames[i].text += " IV";
                        else if (hwMod == 5)
                            abilityNames[i].text += " V";
                        else if (hwMod == 6)
                            abilityNames[i].text += " VI";
                        else if (hwMod == 7)
                            abilityNames[i].text += " VII";
                        else if (hwMod == 8)
                            abilityNames[i].text += " VIII";
                        else if (hwMod == 9)
                            abilityNames[i].text += " IX";
                        break;
                    case 33: //Crisis Wolfsoul
                        abilityFills[i].color = colors[0];
                        float cwMod = current.GetMods()[i];
                        if (cwMod == 1)
                            abilityNames[i].text += " I";
                        else if (cwMod == 2)
                            abilityNames[i].text += " II";
                        else if (cwMod == 3)
                            abilityNames[i].text += " III";
                        else if (cwMod == 4)
                            abilityNames[i].text += " IV";
                        else if (cwMod == 5)
                            abilityNames[i].text += " V";
                        else if (cwMod == 6)
                            abilityNames[i].text += " VI";
                        else if (cwMod == 7)
                            abilityNames[i].text += " VII";
                        else if (cwMod == 8)
                            abilityNames[i].text += " VIII";
                        else if (cwMod == 9)
                            abilityNames[i].text += " IX";
                        break;
                    case 34: //All Or Nothing D
                        abilityFills[i].color = colors[1];
                        break;
                    case 35: //All Or Nothing S
                        abilityFills[i].color = colors[1];
                        break;
                    default:
                        abilityFills[i].color = colors[0];
                        abilityNames[i].text += " +" + current.GetMods()[i];
                        break;
                }
            }

            for (int i = Mathf.Min(current.GetAbilities().Count, abilityIcons.Count); i < 2 - Mathf.Min(current.GetAbilities().Count, abilityIcons.Count); i++)
            {
                abilityIcons[i].gameObject.SetActive(false);
                abilityNames[i].gameObject.SetActive(false);
                abilityFills[i].gameObject.SetActive(false);
                abilityBorders[i].gameObject.SetActive(false);
            }
        }
    }

    public void DrawDescription(PotionButton pb)
    {
        ClearDescription();
        if (pb.GetComponent<Button>().enabled)
        {
            box.color = filledColor;
            border.color = filledColor;
            potionGroup.SetActive(true);

            switch (player.GetPotions()[pb.GetNumber()])
            {
                case 1:
                    potionType.text = "Strength Potion";
                    potionDescription.text = "Increases your Strength by 3 for 20 seconds!";
                    break;
                case 2:
                    potionType.text = "Shield Potion";
                    potionDescription.text = "Increases your Defense by 3 for 20 seconds!";
                    break;
                case 3:
                    potionType.text = "Speed Potion";
                    potionDescription.text = "Increases your Speed by 30% for 20 seconds!";
                    break;
                case 4:
                    potionType.text = "Signature Potion";
                    potionDescription.text = "Increases the rate at which the Signature Gauge fills up by 30% for 20 seconds!";
                    break;
                case 5:
                    potionType.text = "Stun Potion";
                    potionDescription.text = "Stuns all enemies for a short period!";
                    break;
                case 6:
                    potionType.text = "Saving Potion";
                    potionDescription.text = "Gradually restores up to 30% of your Health over 20 seconds!";
                    break;
                default:
                    break;
            }
        }
    }

    void OnAwake()
    {
        ClearDescription();
    }
}
