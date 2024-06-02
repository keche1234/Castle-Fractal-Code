using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPresetSettings : MonoBehaviour
{
    [Header("Other settings")]
    protected const int MAX_RANGE_ASSIST = 5;
    protected const float MIN_SCROLL_SENSITIVITY = 0f;
    protected const float MAX_SCROLL_SENSITIVITY = 2f;
    [SerializeField] protected SignatureActivation sigActivation; //Changing this changes the signature activation between Button <-> Button + Attack
    [SerializeField] protected MeleeAim meleeAim;
    [SerializeField] protected RangedAim rangedAim;

    [Range(1, MAX_RANGE_ASSIST)]
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
    public void SetSignatureActivationMode(SignatureActivation mode)
    {
        sigActivation = mode;
    }

    public SignatureActivation GetSignatureActivationMode()
    {
        return sigActivation;
    }

    public void SetMeleeAim(MeleeAim mode)
    {
        meleeAim = mode;
    }

    public MeleeAim GetMeleeAim()
    {
        return meleeAim;
    }

    public void SetRangedAim(RangedAim mode, int assist = 3)
    {
        rangedAim = mode;
        rangedAssist = assist;
    }

    public int GetRangedAssist()
    {
        if (rangedAim == RangedAim.Manual)
            return 0;

        if (rangedAim == RangedAim.Auto)
            return MAX_RANGE_ASSIST + 1;

        return rangedAssist;
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
