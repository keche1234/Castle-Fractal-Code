using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffGroupUI : MonoBehaviour
{
    [SerializeField] protected Character owner;
    [SerializeField] protected List<Sprite> allBuffs;
    [SerializeField] protected List<Sprite> allDebuffs;
    [SerializeField] protected List<BuffUI> myIcons;

    protected Color32 buffFill = new Color32(0, 192, 255, 255);
    protected Color32 debuffFillColor = new Color32(255, 96, 96, 255);

    protected Color32 buffTextColor = Color.white;
    protected Color32 debuffTextColor = new Color32(255, 240, 240, 255);

    protected float fade = 0; // bounces between minFade and maxFade transparency (draws the clamped value)
    protected float minFade = -0.3f;
    protected float maxFade = 1.5f;
    protected float fadeSpeed = 1f; //Multiply by -1 to switch direction


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        fade += fadeSpeed * Time.unscaledDeltaTime;

        if (fade <= minFade) fadeSpeed = 1;
        else if (fade >= maxFade) fadeSpeed = -1;

        DrawBuffs();
    }

    /*
     * Each type of buff and debuff is represented by an icon with its symbol,
     * with a sum that flashes
     */
    public void DrawBuffs()
    {
        List<float> buffVals = new List<float>();
        List<float> debuffVals = new List<float>();

        int buffTypeCount = Character.buffTypes.Length;
        for (int i = 0; i < buffTypeCount; i++)
            buffVals.Add(owner.SummationBuffs(i));

        int debuffTypeCount = Character.buffTypes.Length;
        for (int i = 0; i < debuffTypeCount; i++)
            debuffVals.Add(owner.SummationDebuffs(i));

        bool anyDebuffs = false;
        int iconIndex = myIcons.Count / 2;

        // Start drawing the debuffs
        for (int i = 1; i < debuffTypeCount; i++)
        {
            if (debuffVals[i] != 0)
            {
                float shortestDebuff = owner.ShortestDebuffPercent(i);
                myIcons[iconIndex].gameObject.SetActive(true);
                myIcons[iconIndex].SetFill(shortestDebuff, debuffFillColor);
                myIcons[iconIndex].SetIcon(allDebuffs[i - 1]);

                if (i == System.Array.IndexOf(Character.debuffTypes, "Strength") || i == System.Array.IndexOf(Character.debuffTypes, "Defense"))
                    myIcons[iconIndex].SetValue(debuffVals[i]);
                else if (i == System.Array.IndexOf(Character.debuffTypes, "Speed") || i == System.Array.IndexOf(Character.debuffTypes, "Signature Gain"))
                    myIcons[iconIndex].SetValue(PercentToRomanNumeral(debuffVals[i]));
                else
                    myIcons[iconIndex].SetValue("");

                myIcons[iconIndex].SetValueColor(new Color32(debuffTextColor.r, debuffTextColor.g, debuffTextColor.b, (byte)(255 * Mathf.Clamp01(fade))));

                iconIndex++;
                anyDebuffs = true;
            }
        }

        if (anyDebuffs)
        {
            // Hide from the bottom row
            for (int i = iconIndex; i < myIcons.Count; i++)
                myIcons[i].gameObject.SetActive(false);
            iconIndex = 0;
        }
        else // Hide from the top row
            for (int i = 0; i < myIcons.Count; i++)
                myIcons[i].gameObject.SetActive(false);

        // Now draw buffs
        for (int i = 1; i < buffTypeCount; i++)
        {
            if (buffVals[i] != 0)
            {
                float shortestBuff = owner.ShortestBuffPercent(i);
                myIcons[iconIndex].gameObject.SetActive(true);
                myIcons[iconIndex].SetFill(shortestBuff, buffFill);
                myIcons[iconIndex].SetIcon(allBuffs[i - 1]);

                if (i == System.Array.IndexOf(Character.buffTypes, "Strength") || i == System.Array.IndexOf(Character.buffTypes, "Defense"))
                    myIcons[iconIndex].SetValue(buffVals[i]);
                else if (i == System.Array.IndexOf(Character.buffTypes, "Speed") || i == System.Array.IndexOf(Character.buffTypes, "Signature Gain"))
                    myIcons[iconIndex].SetValue(PercentToRomanNumeral(buffVals[i]));
                else
                    myIcons[iconIndex].SetValue("");

                myIcons[iconIndex].SetValueColor(new Color32(buffTextColor.r, buffTextColor.g, buffTextColor.b, (byte)(255 * Mathf.Clamp01(fade))));
                iconIndex++;
            }
        }

        if (anyDebuffs)
        {
            // Hide from the top row
            for (int i = iconIndex; i < myIcons.Count / 2; i++)
                myIcons[i].gameObject.SetActive(false);
        }
        else // Hide ALL from the top row
            for (int i = 0; i < myIcons.Count / 2; i++)
                myIcons[i].gameObject.SetActive(false);
    }

    /*
     * Converts a percentage x% to the roman numeral (x * 10)
     */
    public string PercentToRomanNumeral(float mod)
    {
        int num = (int)(mod * 10);
        switch (num)
        {
            case 1:
                return "I";
            case 2:
                return "II";
            case 3:
                return "III";
            case 4:
                return "IV";
            case 5:
                return "V";
            case 6:
                return "VI";
            case 7:
                return "VII";
            case 8:
                return "VIII";
            case 9:
                return "IX";
            case 10:
                return "X";
            case 11:
                return "XI";
            case 12:
                return "XII";
            case 13:
                return "XIII";
            case 14:
                return "XIV";
            case 15:
                return "XV";
            case 16:
                return "XVI";
            case 17:
                return "XVII";
            case 18:
                return "XVIII";
            case 19:
                return "XIX";
            case 20:
                return "XX";
            case 21:
                return "XXI";
            case 22:
                return "XXII";
            case 23:
                return "XXIII";
            case 24:
                return "XXIV";
            case 25:
                return "XXV";
            case 26:
                return "XXVI";
            case 27:
                return "XXVII";
            case 28:
                return "XXVIII";
            case 29:
                return "XXIX";
            default:
                return "";
        }
    }
}
