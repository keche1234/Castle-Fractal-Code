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

    [Header("Signature Activation Buttons")]
    [SerializeField] protected PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
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

    /*****************************
     * CONTROL SETTINGS FUNCTIONS
     *****************************/
    public ControlPresetSettings GetPresetSettings()
    {
        return settings;
    }

    public void PastePresetSettings(ControlPresetSettings source)
    {
        settings.SetMeleeAim(source.GetMeleeAim());
        meleeAimText.text = settings.GetMeleeAimString();

        settings.SetRangedAim(source.GetRangedAim(), source.GetRangedAssist());
        rangedAimText.text = settings.GetRangedAimString();
        CheckRangeAssist();

        settings.SetSignatureActivationMode(source.GetSignatureActivationMode());
        signatureActivationText.text = settings.GetSigActivationString();
        UpdateSignatureActivation();
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

    //public void UpdateSignatureActivation()
    //{
    //    //TODO: Use the buttons tied to signature and main attack to adjust
    //}

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

            for (int i = 0; i < signatureButton.actionReference.action.bindings.Count; i++)
            {
                Debug.Log(i + ") " + signatureButton.actionReference.action.bindings[i]);
            }
            Debug.Log("**************");

            if (signatureButton != null)
            {
                InputAction sigAction = signatureButton.actionReference.action;
                int startIndex = sigAction.GetBindingIndex(controlSchemeName);
                if (settings.GetSignatureActivationMode() == ControlPresetSettings.SignatureActivation.Simple)
                {
                    Debug.Log("Starting (" + startIndex + "): " + sigAction.bindings[startIndex].effectivePath + " + " + sigAction.bindings[startIndex + 1].effectivePath);
                    if (sigAction.bindings[startIndex].effectivePath != "") // There's a "true" composite here
                    {
                        Debug.Log("Make a new \"composite\" that's really just a button press.");

                        //TODO: Figure out how to apply override only to specific control scheme!
                        sigAction.ApplyBindingOverride(startIndex + 1, sigAction.bindings[startIndex].effectivePath);
                        sigAction.ApplyBindingOverride(startIndex, "");
                        signatureButton.bindingId = sigAction.bindings[startIndex + 1].id.ToString();

                        Debug.Log("New path: " + sigAction.bindings[startIndex].overridePath + " + " + sigAction.bindings[startIndex + 1].overridePath);
                    }
                }
                else
                {
                    if (attackButton != null)
                    {
                        InputAction mainAction = attackButton.actionReference.action;
                        if (sigAction.bindings[startIndex].effectivePath == "") //Changing from simple
                        {
                            //TODO: Figure out how to apply override only to specific control scheme!

                            Debug.Log("Changing from simple to combo (" + startIndex + "). Path is " + sigAction.bindings[startIndex + 1].effectivePath);
                            sigAction.ApplyBindingOverride(startIndex, sigAction.bindings[startIndex + 1].effectivePath);
                            sigAction.ApplyBindingOverride(startIndex + 1, mainAction.bindings[mainAction.GetBindingIndex(controlSchemeName)].effectivePath);
                            signatureButton.bindingId = sigAction.bindings[startIndex].id.ToString();
                            Debug.Log("New path: " + sigAction.bindings[startIndex].overridePath + " + " + sigAction.bindings[startIndex + 1].overridePath);
                        }
                        else // Already combo
                        {
                            //TODO: Figure out how to apply override only to specific control scheme!

                            Debug.Log("Already combo (" + startIndex + ").");
                            sigAction.ApplyBindingOverride(startIndex, sigAction.bindings[startIndex].effectivePath);
                            sigAction.ApplyBindingOverride(startIndex + 1, mainAction.bindings[mainAction.GetBindingIndex(controlSchemeName)].effectivePath);
                            Debug.Log("New path: " + sigAction.bindings[startIndex].overridePath + " + " + sigAction.bindings[startIndex + 1].overridePath);
                        }
                    }
                }
            }

            //InputAction temp = new InputAction();

            //if (settings.GetSignatureActivationMode() == ControlPresetSettings.SignatureActivation.Simple)
            //{
            //    int startIndex = signatureAttack.GetBindingIndex(controlSchemeName);
            //    //if (signatureAttack.bindings[startIndex].overridePath != null)
            //    //    Debug.Log("Starting (Override): " + signatureAttack.bindings[startIndex].overridePath + " + " + signatureAttack.bindings[startIndex + 1].overridePath);
            //    //else
            //        Debug.Log("Starting (" + startIndex + "): " + signatureAttack.bindings[startIndex].effectivePath + " + " + signatureAttack.bindings[startIndex + 1].effectivePath + "(" + signatureAttack.bindings[startIndex].id + ")");
            //    if (signatureAttack.bindings[startIndex].overridePath != "")
            //    {
            //        Debug.Log("Make a new \"composite\" that's really just a button press.");
            //        //if (signatureAttack.bindings[startIndex].overridePath != null)
            //        //{
            //        //    Debug.Log("Overridden!");
            //        //    signatureAttack.ApplyBindingOverride(startIndex + 1, signatureAttack.bindings[startIndex].overridePath);
            //        //}
            //        //else
            //            signatureAttack.ApplyBindingOverride(startIndex + 1, signatureAttack.bindings[startIndex].effectivePath);
            //        signatureAttack.ApplyBindingOverride(startIndex, "");
            //        Debug.Log("New path: " + signatureAttack.bindings[startIndex].overridePath + " + " + signatureAttack.bindings[startIndex + 1].overridePath);
            //    }
            //}
            //else
            //{

            //    if (signatureAttack.bindings[signatureAttack.GetBindingIndex(controlSchemeName)].isPartOfComposite)
            //    {
            //        //Update the Composite
            //        //Debug.Log("Update the composite");
            //        temp.AddCompositeBinding("OneModifier")
            //            .With("Binding", inputActions.Player.MainAttack.bindings[mainAttack.GetBindingIndex(controlSchemeName)].effectivePath)
            //            .With("Modifier", signatureAttack.bindings[signatureAttack.GetBindingIndex(controlSchemeName) + 1].effectivePath);
            //    }
            //    else
            //    {
            //        //Make a composite
            //        //Debug.Log("Make a new composite");
            //        temp.AddCompositeBinding("OneModifier")
            //            .With("Modifier", signatureAttack.bindings[signatureAttack.GetBindingIndex(controlSchemeName)].effectivePath)
            //            .With("Binding", inputActions.Player.MainAttack.bindings[mainAttack.GetBindingIndex(controlSchemeName)].effectivePath);
            //    }
            //    signatureAttack.ChangeBinding(signatureAttack.GetBindingIndex(controlSchemeName))
            //        .WithGroup(controlSchemeName)
            //        .WithPath(temp.bindings[0].effectivePath);
            //}

            //Debug.Log(signatureAttack.bindings[signatureAttack.GetBindingIndex(controlSchemeName)]);
        }
    }
}
