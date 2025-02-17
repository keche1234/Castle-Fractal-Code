using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;

public class ControlPresetUI : MonoBehaviour
{
    [Range(0, 8)]
    [SerializeField] protected int controlSchemeIndex;
    [SerializeField] protected ControlPresetSettings settings;
    [SerializeField] protected bool isDefaultPreset;
    protected ControlPresetSettings.MeleeAim defaultMeleeAim;
    protected (ControlPresetSettings.RangedAim, int) defaultRangedAim;
    protected ControlPresetSettings.SignatureActivation defaultSignatureActivation;

    [Header("Action Buttons")]
    [SerializeField] protected List<RebindActionUI> buttons;

    [Header("Setting Texts")]
    [SerializeField] protected Text meleeAimText;
    [SerializeField] protected Text rangedAimText;
    [SerializeField] protected Text signatureActivationText;
    [SerializeField] protected TextMeshProUGUI rangedTitle;

    [Header("Setting Arrows")]
    [SerializeField] protected ButtonColorManipulation meleeLeft;
    [SerializeField] protected ButtonColorManipulation meleeRight;
    [SerializeField] protected ButtonColorManipulation rangedLeft;
    [SerializeField] protected ButtonColorManipulation rangedRight;
    [SerializeField] protected ButtonColorManipulation sigLeft;
    [SerializeField] protected ButtonColorManipulation sigRight;

    protected string nameOverride;
    protected PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        defaultMeleeAim = settings.GetMeleeAim();
        defaultRangedAim = (settings.GetRangedAim(), settings.GetRangedAssist());
        defaultSignatureActivation = settings.GetSignatureActivationMode();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<RebindActionUI> GetButtons()
    {
        return buttons;
    }

    public string GetControlSchemeName()
    {
        return inputActions.controlSchemes[controlSchemeIndex].name;
    }

    public bool IsDefaultPreset()
    {
        return isDefaultPreset;
    }

    public string GetNameOverride()
    {
        if (isDefaultPreset || nameOverride == null)
            return inputActions.controlSchemes[controlSchemeIndex].name;
        return nameOverride;
    }

    public void ApplyNameOverride(string text)
    {
        nameOverride = text;
    }

    /*****************************
     * CONTROL SETTINGS FUNCTIONS
     *****************************/
    public ControlPresetSettings GetPresetSettings()
    {
        return settings;
    }

    public string GetBindingPathForAction(string actionName, int offsetIndex = 0)
    {
        foreach (RebindActionUI button in buttons)
        {
            InputAction action = button.actionReference.action;
            if (action.name == actionName)
            {
                int startIndex = action.GetBindingIndex(GetControlSchemeName());
                if (startIndex + offsetIndex < 0 || startIndex + offsetIndex >= action.bindings.Count)
                {
                    Debug.LogError("Action Index of " + offsetIndex + " takes you out of range of the bindings!");
                    return "";
                }

                if (!action.bindings[startIndex + offsetIndex].groups.Contains(GetControlSchemeName()))
                    Debug.LogWarning("You are accessing a binding beyond the control scheme of this ControlPresetUI object.");

                return action.bindings[startIndex + offsetIndex].effectivePath;
            }
        }
        Debug.LogError("No action named " + actionName + " found!");
        return "";
    }

    public void OverrideBindingPathForAction(string actionName, string newPath, int offsetIndex = 0)
    {
        foreach (RebindActionUI button in buttons)
        {
            InputAction action = button.actionReference.action;
            if (action.name == actionName)
            {
                int startIndex = action.GetBindingIndex(GetControlSchemeName());

                if (startIndex + offsetIndex < 0 || startIndex + offsetIndex >= action.bindings.Count)
                {
                    Debug.LogError("Action Index of " + offsetIndex + " takes you out of range of the bindings!");
                    return;
                }

                if (!action.bindings[startIndex + offsetIndex].groups.Contains(GetControlSchemeName()))
                    Debug.LogWarning("You are accessing a binding beyond the control scheme of this ControlPresetUI object.");

                action.ApplyBindingOverride(startIndex + offsetIndex, newPath);
                return;
            }
        }
    }

    public bool SignatureIsComposite(bool returnOnFail)
    {
        foreach (RebindActionUI button in buttons)
        {
            InputAction action = button.actionReference.action;
            if (action.name == "Signature Attack")
            {
                int startIndex = action.GetBindingIndex(GetControlSchemeName());
                return action.bindings[startIndex].isPartOfComposite;
            }
        }
        Debug.LogError("Could not find Signature Attack button in Control Preset " + this);
        return returnOnFail;
    }

    public void PastePresetSettings(ControlPresetSettings source)
    {
        settings.SetMeleeAim(source.GetMeleeAim());
        meleeAimText.text = settings.GetMeleeAimString();
        CheckMeleeArrows();

        settings.SetRangedAim(source.GetRangedAim(), source.GetRangedAssist());
        rangedAimText.text = settings.GetRangedAimString();
        CheckRangeAssist();

        settings.SetSignatureActivationMode(source.GetSignatureActivationMode());
        signatureActivationText.text = settings.GetSigActivationString();
        UpdateSignatureActivation();
        CheckSignatureArrows();
    }

    public void ResetSettingsToDefault()
    {
        settings.SetMeleeAim(defaultMeleeAim);
        meleeAimText.text = settings.GetMeleeAimString();
        CheckMeleeArrows();

        settings.SetRangedAim(defaultRangedAim.Item1, defaultRangedAim.Item2);
        rangedAimText.text = settings.GetRangedAimString();
        CheckRangeAssist();

        settings.SetSignatureActivationMode(defaultSignatureActivation);
        signatureActivationText.text = settings.GetSigActivationString();
        UpdateSignatureActivation();
        CheckSignatureArrows();
    }

    /*******************************/
    public void IncrementMeleeAim()
    {
        settings.IncremenetMeleeAim();
        meleeAimText.text = settings.GetMeleeAimString();

        //enable left button
        meleeLeft.UnlockButton();
        meleeRight.LockButton();
    }

    public void DecrementMeleeAim()
    {
        settings.DecrementMeleeAim();
        meleeAimText.text = settings.GetMeleeAimString();

        meleeLeft.LockButton();
        meleeRight.UnlockButton();
    }

    protected void CheckMeleeArrows()
    {
        switch (settings.GetMeleeAim())
        {
            case ControlPresetSettings.MeleeAim.Manual:
                meleeLeft.LockButton();
                meleeRight.UnlockButton();
                break;
            default:
                meleeLeft.UnlockButton();
                meleeRight.LockButton();
                break;
        }
    }

    /*******************************/
    public void IncrementRangedAim()
    {
        settings.IncrementRangedAim();
        rangedAimText.text = settings.GetRangedAimString();
        CheckRangeAssist();
    }

    public void DecrementRangedAim()
    {
        settings.DecrementRangedAim();
        rangedAimText.text = settings.GetRangedAimString();
        CheckRangeAssist();
    }

    protected void CheckRangeAssist()
    {
        rangedTitle.text = "Ranged Aim";
        if (settings.GetRangedAssist() >= 1 && settings.GetRangedAssist() <= ControlPresetSettings.GetMaxRangedAssist())
        {
            rangedTitle.text += " (Assisted)";
            rangedLeft.UnlockButton();
            rangedRight.UnlockButton();
        }
        else if (settings.GetRangedAim() == ControlPresetSettings.RangedAim.Manual)
        {
            rangedLeft.LockButton();
            rangedRight.UnlockButton();
        }
        else // Ranged Aim is Auto
        {
            rangedLeft.UnlockButton();
            rangedRight.LockButton();
        }
    }

    /*******************************/
    public void IncrementSignatureActivation()
    {
        settings.IncrementSignatureActivation();
        signatureActivationText.text = settings.GetSigActivationString();
        UpdateSignatureActivation();

        sigLeft.UnlockButton();
        sigRight.LockButton();
    }
    public void DecrementSignatureActivation()
    {
        settings.DecrementSignatureActivation();
        signatureActivationText.text = settings.GetSigActivationString();
        UpdateSignatureActivation();

        sigLeft.LockButton();
        sigRight.UnlockButton();
    }

    public void UpdateSignatureActivation()
    {
        if (inputActions != null)
        {
            InputAction temp = new InputAction();
            string controlSchemeName = GetControlSchemeName();

            InputAction mainAttack = inputActions.Player.MainAttack;
            InputAction signatureAttack = inputActions.Player.SignatureAttack;

            RebindActionUI attackButton = null;
            RebindActionUI signatureButton = null;

            foreach (RebindActionUI button in buttons)
            {
                if (button.actionReference.action.id == mainAttack.id)
                    attackButton = button;
                else if (button.actionReference.action.id == signatureAttack.id)
                    signatureButton = button;
            }

            // Update Signature Input
            if (signatureButton != null)
            {
                InputAction sigAction = signatureButton.actionReference.action;
                int startIndex = sigAction.GetBindingIndex(InputBinding.MaskByGroup(controlSchemeName));

                // Determine if signature button is currently modifier (combo) or binding (simple), by going back one
                bool isModifier = signatureButton.bindingId == sigAction.bindings[startIndex].id.ToString();

                // If modifier:
                if (isModifier)
                {
                    // If current settings are simple:
                    if (settings.GetSignatureActivationMode() == ControlPresetSettings.SignatureActivation.Simple)
                    {
                        // Move the modifier path to the binding path
                        // Clear the modifier
                        string modifierPath = sigAction.bindings[startIndex].effectivePath;
                        sigAction.ApplyBindingOverride(startIndex + 1, modifierPath);
                        sigAction.ApplyBindingOverride(startIndex, modifierPath);

                        // Set signatureButton bindingId to startIndex + 1 (binding)
                        signatureButton.bindingId = sigAction.bindings[startIndex + 1].id.ToString();

                        if (attackButton != null)
                            attackButton.permittedBindingDuplicates.Remove(sigAction.bindings[startIndex + 1].id.ToString());

                    }
                    else // Else: (combo)
                    {
                        // keep modifier as is
                        // look for attack button. If found:
                        if (attackButton != null)
                        {
                            InputAction atkAction = attackButton.actionReference.action;

                            // Set binding to current attack binding, overriding current "binding" binding
                            int attackIndex = atkAction.GetBindingIndex(InputBinding.MaskByGroup(controlSchemeName));
                            string attackPath = atkAction.bindings[attackIndex].effectivePath;
                            sigAction.ApplyBindingOverride(startIndex + 1, attackPath);
                        }
                    }
                }
                else // Else: (binding)
                {
                    // If current settings are combo:
                    if (settings.GetSignatureActivationMode() == ControlPresetSettings.SignatureActivation.Combo)
                    {
                        if (attackButton != null)
                        {
                            InputAction atkAction = attackButton.actionReference.action;
                            int attackIndex = atkAction.GetBindingIndex(InputBinding.MaskByGroup(controlSchemeName));

                            // Move the binding path to the modifier path
                            // Copy the attack button path to the binding path
                            string attackPath = atkAction.bindings[attackIndex].effectivePath;
                            string bindingPath = sigAction.bindings[startIndex + 1].effectivePath;
                            sigAction.ApplyBindingOverride(startIndex, bindingPath);
                            sigAction.ApplyBindingOverride(startIndex + 1, attackPath);

                            // Set signatureButton bindingId to startIndex (modifier)
                            attackButton.permittedBindingDuplicates.Add(sigAction.bindings[startIndex + 1].id.ToString());
                            signatureButton.bindingId = sigAction.bindings[startIndex].id.ToString();
                        }
                    }
                    else // Else: (simple)
                    {
                        // do nothing
                    }
                }
            }
        }
    }

    protected void CheckSignatureArrows()
    {
        switch (settings.GetSignatureActivationMode())
        {
            case ControlPresetSettings.SignatureActivation.Simple:
                sigLeft.LockButton();
                sigRight.UnlockButton();
                break;
            default:
                sigLeft.UnlockButton();
                sigRight.LockButton();
                break;
        }
    }

    private void OnEnable()
    {
        foreach (RebindActionUI r in buttons)
        {
            //Debug.Log("Updating button " + r);
            r.UpdateBindingDisplay();
        }

        if (!isDefaultPreset)
        {
            Debug.Log("Loading controls (in preset)");
            
            string loadSettings = PlayerPrefs.GetString("Preset " + controlSchemeIndex.ToString());
            if (!string.IsNullOrEmpty(loadSettings))
                JsonUtility.FromJsonOverwrite(loadSettings, settings);

            string loadName = PlayerPrefs.GetString("Preset Name Override " + controlSchemeIndex.ToString());
            if (!string.IsNullOrEmpty(loadName))
            {
                Wrapper<string> wrapperToRetrieve = new Wrapper<string>(null);
                JsonUtility.FromJsonOverwrite(loadName, wrapperToRetrieve);
                if (!string.IsNullOrEmpty(wrapperToRetrieve.value))
                    nameOverride = wrapperToRetrieve.value;
            }

            meleeAimText.text = settings.GetMeleeAimString();
            CheckMeleeArrows();

            rangedAimText.text = settings.GetRangedAimString();
            CheckRangeAssist();

            signatureActivationText.text = settings.GetSigActivationString();
            UpdateSignatureActivation();
            CheckSignatureArrows();
        }
    }

    private void OnDisable()
    {
        if (!isDefaultPreset)
        {
            //PlayerPrefs.SetString("rebinds", inputActions.SaveBindingOverridesAsJson());

            string newSettings = JsonUtility.ToJson(settings);
            PlayerPrefs.SetString("Preset " + controlSchemeIndex.ToString(), newSettings);

            string newName = JsonUtility.ToJson(new Wrapper<string>(nameOverride));
            PlayerPrefs.SetString("Preset Name Override " + controlSchemeIndex.ToString(), newName);
        }
    }
}
