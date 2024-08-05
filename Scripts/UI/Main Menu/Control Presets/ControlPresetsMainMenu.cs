using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;

public class ControlPresetsMainMenu : MonoBehaviour
{
    //[SerializeField] protected RebindActionUI upButton;
    //[SerializeField] protected RebindActionUI downButton;
    //[SerializeField] protected RebindActionUI leftButton;
    //[SerializeField] protected RebindActionUI rightButton;
    //[SerializeField] protected RebindActionUI signatureButton;
    //[SerializeField] protected List<RebindActionUI> buttons;
    //[SerializeField] protected List<ControlPresetSettings> presets;

    [SerializeField] protected List<ControlPresetUI> presetList;
    [SerializeField] protected ButtonColorManipulation copyButton;
    [SerializeField] protected ButtonColorManipulation pasteButton;
    [SerializeField] protected ButtonColorManipulation resetButton;
    [SerializeField] protected MessageRotate clipboardText;
    protected PlayerInputActions inputActions;
    protected int currentPresent = 0;
    protected int clipboardNumber = -1;
    protected List<InputAction> actions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        actions = new List<InputAction>();
        actions.Add(inputActions.Player.Move);
        actions.Add(inputActions.Player.MainAttack);
        actions.Add(inputActions.Player.Roll);
        actions.Add(inputActions.Player.SignatureAttack);
        actions.Add(inputActions.Player.ScrollInventory);
        actions.Add(inputActions.Player.DropWeapon);
        actions.Add(inputActions.Player.Inventory);
        actions.Add(inputActions.Player.Pause);
    }

    public void SetCurrentControlScheme(int i)
    {
        if (i >= 0 || i < presetList.Count)
            currentPresent = i;

        foreach (ControlPresetUI preset in presetList)
            preset.gameObject.SetActive(false);

        presetList[currentPresent].gameObject.SetActive(true);

        if (presetList[currentPresent].IsDefaultPreset())
        {
            pasteButton.LockButton();
            resetButton.LockButton();
        }
        else
        {
            if (clipboardNumber > -1)
                pasteButton.UnlockButton();
            else
                pasteButton.LockButton();

            resetButton.UnlockButton();
        }
    }

    public void CopyControlScheme()
    {
        clipboardNumber = currentPresent;
        if (!presetList[currentPresent].IsDefaultPreset())
            pasteButton.UnlockButton();
        else
            pasteButton.LockButton();

        List<string> messages = new List<string>();
        messages.Add((clipboardNumber + 1).ToString());
        clipboardText.SetMessageList(messages);
        clipboardText.RunRotation();
    }

    public void PasteControlScheme()
    {
        if (clipboardNumber > -1)
        {
            ControlPresetSettings clipboardSettings = presetList[clipboardNumber].GetPresetSettings();
            presetList[currentPresent].PastePresetSettings(clipboardSettings);

            string pathToPaste;
            for (int i = 0; i < actions.Count; i++)
            {
                switch (actions[i].name)
                {
                    case "Move":
                        for (int j = 0; j < 4; j++)
                        {
                            pathToPaste = presetList[clipboardNumber].GetBindingPathForAction("Move", j);
                            presetList[currentPresent].OverrideBindingPathForAction("Move", pathToPaste, j);
                        }
                        break;
                    case "Signature Attack":
                        int copyIndex;
                        int pasteIndex;

                        if (presetList[clipboardNumber].SignatureIsComposite(false))
                        {
                            copyIndex = 1;
                            if (clipboardSettings.GetSignatureActivationMode() == ControlPresetSettings.SignatureActivation.Simple)
                                pasteIndex = 1;
                            else
                                pasteIndex = 0;
                        }
                        else
                        {
                            copyIndex = 0;
                            pasteIndex = 1;
                        }

                        Debug.Log(copyIndex + " and " + pasteIndex);

                        pathToPaste = presetList[clipboardNumber].GetBindingPathForAction("Signature Attack", copyIndex);
                        presetList[currentPresent].OverrideBindingPathForAction("Signature Attack", pathToPaste, pasteIndex);
                        break;
                    case "Scroll Inventory":
                        for (int j = 0; j < 2; j++)
                        {
                            pathToPaste = presetList[clipboardNumber].GetBindingPathForAction("Scroll Inventory", j);
                            presetList[currentPresent].OverrideBindingPathForAction("Scroll Inventory", pathToPaste, j);
                        }
                        break;
                    default:
                        pathToPaste = presetList[clipboardNumber].GetBindingPathForAction(actions[i].name, 0);
                        presetList[currentPresent].OverrideBindingPathForAction(actions[i].name, pathToPaste, 0);
                        break;
                }
            }
        }
    }

    public void ResetControlScheme()
    {
        List<RebindActionUI> buttons = presetList[currentPresent].GetButtons();
        foreach (RebindActionUI button in buttons)
            button.ResetToDefault();
        presetList[currentPresent].ResetSettingsToDefault();
    }

    public void ClearClipboard()
    {
        clipboardNumber = -1;
    }

    //TODO: Load From Json
    //TODO: Save to Json File
}
