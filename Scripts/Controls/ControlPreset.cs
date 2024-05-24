using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPreset : MonoBehaviour
{
    public string presetName;

    [Tooltip("Map an action to a keyboard button in parallel (buttons can have at most one action)")]
    [Header("Keyboard Controls Initializer")]
    [SerializeField] protected KeyCode[] keyboardButtons;
    [SerializeField] protected string[] keyboardActions;
    protected Dictionary<KeyCode, string> keyboardControls;

    //0 = Left Click
    //1 = Right Click
    //2 = Middle Click
    //3 = Scroll Up
    //4 = Scroll Down
    [Tooltip("Map an action to a mouse button in parallel (buttons can have at most one action)")]
    [Header("Mouse Controls Initializer")]
    [SerializeField] protected int[] mouseButtons;
    [SerializeField] protected string[] mouseActions;
    protected Dictionary<int, string> mouseControls;

    [Header("Other settings")]
    [SerializeField] protected SignatureActivation sigActivation;
    [SerializeField] protected MeleeAim meleeAim;
    [SerializeField] protected RangedAim rangedAim;
    [Range(1, 5)]
    [SerializeField] protected int rangedAssist;

    // Start is called before the first frame update
    void Start()
    {
        keyboardControls = new Dictionary<KeyCode, string>();
        mouseControls = new Dictionary<int, string>();

        for (int i = 0; i < Mathf.Min(keyboardActions.Length, keyboardButtons.Length); i++)
            if (!keyboardControls.ContainsKey(keyboardButtons[i]))
                keyboardControls.Add(keyboardButtons[i], keyboardActions[i]);
            else
                keyboardControls[keyboardButtons[i]] = keyboardActions[i];

        if (keyboardButtons.Length > keyboardControls.Count)
            Debug.LogWarning("Some defined keyboardButtons are going unused!");
        if (keyboardActions.Length > keyboardControls.Count)
            Debug.LogWarning("Some defined keyboardActions have not been mapped!");

        for (int i = 0; i < Mathf.Min(mouseActions.Length, mouseButtons.Length); i++)
            if (!mouseControls.ContainsKey(mouseButtons[i]) && !mouseControls.ContainsValue(mouseActions[i]))
                mouseControls.Add(mouseButtons[i], mouseActions[i]);
            else
                mouseControls[mouseButtons[i]] = mouseActions[i];

        if (mouseButtons.Length > mouseControls.Count)
            Debug.LogWarning("Some defined mouseButtons are going unused!");
        if (mouseActions.Length > mouseControls.Count)
            Debug.LogWarning("Some defined keyboardActions have not been mapped!");
    }

    // Update is called once per frame
    void Update()
    {

    }

    /****************************************************
     * Maps a keyboard `button` to an `action`, replacing
     * the action already there. Returns the action
     * that got replaced if `button` replaced an action,
     * or the empty string otherwise
     ****************************************************/
    public string SetActionControl(KeyCode button, string action)
    {
        if (keyboardControls.ContainsKey(button))
        {
            string replaced = keyboardControls[button];
            keyboardControls[button] = action;
            return replaced;
        }
        keyboardControls.Add(button, action);
        return "";
    }

    /****************************************************
     * Maps a mouse `button` to an `action`, replacing
     * the action already there. Returns the action
     * that got replaced if `button` replaced an action,
     * or the empty string otherwise
     ****************************************************/
    public string SetActionControl(int button, string action)
    {
        if (mouseControls.ContainsKey(button))
        {
            string replaced = mouseControls[button];
            mouseControls[button] = action;
            if (button < 0 || button > 4)
                replaced += " (Unknown Mouse Button " + button + ")";
            return replaced;
        }
        mouseControls.Add(button, action);
        if (button < 0 || button > 4)
            return "(Unknown Mouse Button " + button + ")";
        return "";
    }

    /*****************************************************************
     * Search the keyboard mappings for the button corresponding to
     * `action`; then returns the key, or `KeyCode.None` if no key
     * maps to `action`
     *****************************************************************/
    public List<KeyCode> GetKeyboardButton(string action)
    {
        List<KeyCode> buttonList = new List<KeyCode>();
        foreach (KeyCode k in keyboardControls.Keys)
        {
            if (keyboardControls[k] == action)
                buttonList.Add(k);
        }
        return buttonList;
    }

    /*****************************************************************
     * Search the mouse mappings for the button corresponding
     * to `action`; then returns the mouse, or `KeyCode.None` if no key
     * maps to `action`
     *****************************************************************/
    public List<int> GetMouseButton(string action)
    {
        List<int> buttonList = new List<int>();
        foreach (int m in mouseControls.Keys)
        {
            if (mouseActions[m] == action)
                buttonList.Add(m);
        }
        return buttonList;
    }

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
