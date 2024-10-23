using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    [SerializeField] protected PlayerController player;
    [SerializeField] protected List<ControlPresetUI> presetList;
    [SerializeField] protected TMP_InputField presetNameBox;

    [Header("Edit Options")]
    [SerializeField] protected ButtonColorManipulation copyButton;
    [SerializeField] protected ButtonColorManipulation pasteButton;
    [SerializeField] protected ButtonColorManipulation resetButton;
    [SerializeField] protected MessageRotate clipboardText;

    [Header("First Preset Button")]
    [SerializeField] protected Button firstPresetButton;

    protected PlayerInputActions inputActions;
    protected int currentPreset = 0;
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
            currentPreset = i;

        foreach (ControlPresetUI preset in presetList)
            preset.gameObject.SetActive(false);

        presetList[currentPreset].gameObject.SetActive(true);
        DrawNameOverrideBox();

        if (presetList[currentPreset].IsDefaultPreset())
        {
            pasteButton.LockButton();
            resetButton.LockButton();
            presetNameBox.interactable = false;

        }
        else
        {
            if (clipboardNumber > -1)
                pasteButton.UnlockButton();
            else
                pasteButton.LockButton();

            resetButton.UnlockButton();
            presetNameBox.interactable = true;
        }
    }

    public void CopyControlScheme()
    {
        clipboardNumber = currentPreset;
        if (!presetList[currentPreset].IsDefaultPreset())
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
            ControlPresetSettings clipboardSettings = presetList[clipboardNumber].GetPresetSettings(); // source settings
            presetList[currentPreset].PastePresetSettings(clipboardSettings);

            string pathToPaste;
            for (int i = 0; i < actions.Count; i++)
            {
                switch (actions[i].name)
                {
                    case "Move":
                        for (int j = 0; j < 4; j++)
                        {
                            pathToPaste = presetList[clipboardNumber].GetBindingPathForAction("Move", j);
                            presetList[currentPreset].OverrideBindingPathForAction("Move", pathToPaste, j);
                        }
                        break;
                    case "Signature Attack":
                        int sourceOffset;
                        int destOffset;

                        // if source is simple:
                        if (clipboardSettings.GetSignatureActivationMode() == ControlPresetSettings.SignatureActivation.Simple)
                        {
                            // Source has signature as composite, want to copy binding
                            if (presetList[clipboardNumber].SignatureIsComposite(false))
                                sourceOffset = 1;
                            else // Source has signature not as composite, want to copy binding
                                sourceOffset = 0;
                            destOffset = 1; // paste to binding
                        }
                        else // source is combo
                        {
                            //assert(presetList[clipboardNumber].SignatureIsComposite(false))
                            // Source has signature as composite, want to copy modifier
                            sourceOffset = 0;
                            destOffset = 0; // paste to modifier
                        }

                        pathToPaste = presetList[clipboardNumber].GetBindingPathForAction("Signature Attack", sourceOffset);
                        presetList[currentPreset].OverrideBindingPathForAction("Signature Attack", pathToPaste, destOffset);
                        break;
                    case "Scroll Inventory":
                        for (int j = 0; j < 2; j++)
                        {
                            pathToPaste = presetList[clipboardNumber].GetBindingPathForAction("Scroll Inventory", j);
                            presetList[currentPreset].OverrideBindingPathForAction("Scroll Inventory", pathToPaste, j);
                        }
                        break;
                    default:
                        Debug.Log("copy pasting " + actions[i].name);
                        pathToPaste = presetList[clipboardNumber].GetBindingPathForAction(actions[i].name, 0);
                        presetList[currentPreset].OverrideBindingPathForAction(actions[i].name, pathToPaste, 0);
                        break;
                }
            }
        }
    }

    public void ResetControlScheme()
    {
        // TODO: Be VERY Careful about resetting (clear out both modifier and binding for signature attack!)
        List<RebindActionUI> buttons = presetList[currentPreset].GetButtons();
        foreach (RebindActionUI button in buttons)
        {
            button.ResetToDefault();
        }

        // when overriding settings:
        // If already simple, then all we need to clear is binding
        // If going from combo to simple, first reset clears modifier,
        //   then function call copies cleared modifier to binding
        presetList[currentPreset].ResetSettingsToDefault();
        presetList[currentPreset].ApplyNameOverride(null);
        DrawNameOverrideBox();
    }

    public void DrawNameOverrideBox()
    {
        presetNameBox.text = presetList[currentPreset].GetNameOverride();
        UpdatePlaceholderName();
    }

    public void ApplyNameOverrideBox()
    {
        presetList[currentPreset].ApplyNameOverride(presetNameBox.text);
        if (string.IsNullOrEmpty(presetNameBox.text))
        {
            presetNameBox.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = inputActions.controlSchemes[currentPreset].name;
            presetList[currentPreset].ApplyNameOverride(inputActions.controlSchemes[currentPreset].name);
        }
    }

    public void UpdatePlaceholderName()
    {
        if (string.IsNullOrEmpty(presetNameBox.text))
            presetNameBox.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = inputActions.controlSchemes[currentPreset].name;
    }

    public void ClearClipboard()
    {
        clipboardNumber = -1;
        clipboardText.StopRotation();
    }

    private void OnEnable()
    {
        firstPresetButton.onClick.Invoke();
    }

    private void OnDisable()
    {
        ClearClipboard();
        //PlayerPrefs.SetString("rebinds", inputActions.SaveBindingOverridesAsJson());
    }
}
