using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPresetSettings : MonoBehaviour
{
    protected const float RANGE_ASSIST_BASE = 10f; // Range assist is (base * level) degrees
    protected const int MAX_RANGE_ASSIST_LEVEL = 9;
    protected const float MIN_SCROLL_SENSITIVITY = 0f;
    protected const float MAX_SCROLL_SENSITIVITY = 2f;

    [Header("Settings")]
    [SerializeField] protected MeleeAim meleeAim;
    [SerializeField] protected RangedAim rangedAim;
    [SerializeField] protected SignatureActivation sigActivation; //Changing this changes the signature activation between Button <-> Button + Attack

    [Range(1, MAX_RANGE_ASSIST_LEVEL)]
    [SerializeField] protected int rangedAssist;

    [Range(MIN_SCROLL_SENSITIVITY, MAX_SCROLL_SENSITIVITY)]
    [SerializeField] protected float scrollSensitivty = 1f; //inventory scroll sensitivity, 0.1x-2x

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /**********
     * Settings
     **********/

    public void SetMeleeAim(MeleeAim mode)
    {
        meleeAim = mode;
    }

    public MeleeAim GetMeleeAim()
    {
        return meleeAim;
    }

    public void IncremenetMeleeAim()
    {
        if (meleeAim == MeleeAim.Manual)
            meleeAim = MeleeAim.Auto;
    }

    public void DecrementMeleeAim()
    {
        if (meleeAim == MeleeAim.Auto)
            meleeAim = MeleeAim.Manual;
    }

    public string GetMeleeAimString()
    {
        if (meleeAim == MeleeAim.Manual)
            return "MANUAL";
        return "AUTO";
    }

    public void SetRangedAim(RangedAim mode, int assist = 5)
    {
        rangedAim = mode;

        if (rangedAim == RangedAim.Assisted)
            rangedAssist = assist;
        else if (rangedAim == RangedAim.Manual)
            rangedAssist = 1;
        else
            rangedAssist = MAX_RANGE_ASSIST_LEVEL;
    }

    public RangedAim GetRangedAim()
    {
        return rangedAim;
    }

    public int GetRangedAssist()
    {
        if (rangedAim == RangedAim.Manual)
            return 0;

        if (rangedAim == RangedAim.Auto)
            return 999;

        return rangedAssist;
    }

    public void IncrementRangedAim()
    {
        if (rangedAim != RangedAim.Auto)
        {
            if (rangedAssist >= MAX_RANGE_ASSIST_LEVEL)
            {
                rangedAim = RangedAim.Auto;
                return;
            }

            if (rangedAim == RangedAim.Manual)
            {
                rangedAim = RangedAim.Assisted;
                return;
            }

            rangedAssist++;
            
        }
    }

    public void DecrementRangedAim()
    {
        if (rangedAim != RangedAim.Manual)
        {
            if (rangedAssist <= 1)
            {
                rangedAim = RangedAim.Manual;
                return;
            }

            if (rangedAim == RangedAim.Auto)
            {
                rangedAim = RangedAim.Assisted;
                return;
            }

            rangedAssist--;
        }
    }

    public string GetRangedAimString()
    {
        if (rangedAim == RangedAim.Manual)
            return "MANUAL";

        if (rangedAim == RangedAim.Auto)
            return "AUTO";

        return rangedAssist.ToString();
    }

    public void SetSignatureActivationMode(SignatureActivation mode)
    {
        sigActivation = mode;
    }

    public SignatureActivation GetSignatureActivationMode()
    {
        return sigActivation;
    }

    public void IncrementSignatureActivation()
    {
        if (sigActivation == SignatureActivation.Simple)
            sigActivation = SignatureActivation.Combo;
    }

    public void DecrementSignatureActivation()
    {
        if (sigActivation == SignatureActivation.Combo)
            sigActivation = SignatureActivation.Simple;
    }

    public string GetSigActivationString()
    {
        if (sigActivation == SignatureActivation.Simple)
            return "SIMPLE";
        return "COMBO";
    }

    public bool SetScrollSensititvity(float sensitivity)
    {
        if (sensitivity < MIN_SCROLL_SENSITIVITY || sensitivity > MAX_SCROLL_SENSITIVITY)
        {
            Debug.LogError("Scroll sensitivity must be within range (" + MIN_SCROLL_SENSITIVITY + ", " + MAX_SCROLL_SENSITIVITY + ")!");
            return false;
        }
        scrollSensitivty = sensitivity;
        return true;
    }

    public float GetScrollSensitivity()
    {
        return scrollSensitivty;
    }

    public static float GetRangeAssistBase()
    {
        return RANGE_ASSIST_BASE;
    }

    public static int GetMaxRangedAssist()
    {
        return MAX_RANGE_ASSIST_LEVEL;
    }

    //TODO: Save To and Load From Json

    /***************************
     * Enumerated Types
     ***************************/

    public enum MeleeAim
    {
        Manual,
        Auto
    }

    public enum RangedAim
    {
        Manual,
        Assisted,
        Auto
    }

    public enum SignatureActivation
    {
        Simple,
        Combo
    }
}
