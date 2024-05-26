using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPreset : MonoBehaviour
{
    public string presetName;

    [Tooltip("Map an action to a keyboard button in parallel (buttons can have at most one action)")]
    [Header("Keyboard Controls Initializer")]
    [SerializeField] protected KeyCode[] buttons;
    [SerializeField] protected string[] actions;
    protected Dictionary<KeyCode, string> controls;

    ////0 = Left Click
    ////1 = Right Click
    ////2 = Middle Click
    //[Tooltip("Map an action to a mouse button in parallel (buttons can have at most one action)")]
    //[Header("Mouse Controls Initializer")]
    //[SerializeField] protected int[] mouseButtons;
    //[SerializeField] protected string[] mouseActions;
    //protected Dictionary<int, string> mouseControls;

    [Header("Other settings")]
    protected const int MAX_RANGE_ASSIST = 5;
    protected const float MIN_SCROLL_SENSITIVITY = 0f;
    protected const float MAX_SCROLL_SENSITIVITY = 2f;
    [SerializeField] protected SignatureActivation sigActivation;
    [SerializeField] protected MeleeAim meleeAim;
    [SerializeField] protected RangedAim rangedAim;

    [Range(1, MAX_RANGE_ASSIST)]
    [SerializeField] protected int rangedAssist;

    [Range(MIN_SCROLL_SENSITIVITY, MAX_SCROLL_SENSITIVITY)]
    [SerializeField] protected float scrollSensitivty = 0.5f; //inventory scroll sensitivity, 0.1x-2x

    // Start is called before the first frame update
    void Start()
    {
        controls = new Dictionary<KeyCode, string>();

        for (int i = 0; i < Mathf.Min(actions.Length, buttons.Length); i++)
            if (!controls.ContainsKey(buttons[i]))
                controls.Add(buttons[i], actions[i]);
            else
                controls[buttons[i]] = actions[i];

        if (buttons.Length > controls.Count)
            Debug.LogWarning("Some defined buttons were mapped twice!");
        if (actions.Length > controls.Count)
            Debug.LogWarning("Some defined actions have gone unmapped!");
    }

    // Update is called once per frame
    void Update()
    {

    }

    /****************************************************
     * Maps a keyboard `button` to an `action`, replacing
     * the action already there. Returns the action
     * that got replaced if `button` is mapped to a new action,
     * or the empty string otherwise
     ****************************************************/
    public string MapButtonToAction(KeyCode button, string action)
    {
        if (controls.ContainsKey(button))
        {
            string replaced = controls[button];
            controls[button] = action;
            return replaced;
        }
        controls.Add(button, action);
        return "";
    }

    /*****************************************************************
     * Search the keyboard mappings for the button corresponding to
     * `action`; then returns the key, or `KeyCode.None` if no key
     * maps to `action`
     *****************************************************************/
    public List<KeyCode> GetKeyboardButtons(string action)
    {
        List<KeyCode> buttonList = new List<KeyCode>();
        foreach (KeyCode k in controls.Keys)
        {
            if (controls[k] == action)
                buttonList.Add(k);
        }
        return buttonList;
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
     * Miscellaneous Object Info
     ***************************/
    public string GetPresetName()
    {
        return presetName;
    }

    public void SetPresetName(string n)
    {
        presetName = n;
    }

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
