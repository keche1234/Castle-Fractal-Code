using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    protected PlayerInputActions inputActions;
    protected int currentPresent = 0;
    protected int clipboard = -1;
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
    }

    public void CopyControlScheme()
    {
        clipboard = currentPresent;
    }

    public void PasteControlScheme()
    {
        if (clipboard > -1)
        {
            string bindingToPaste;
            foreach (InputAction action in actions)
            {
                bindingToPaste = action.GetBindingDisplayString(0, inputActions.controlSchemes[clipboard].name);
                action.ApplyBindingOverride(bindingToPaste, inputActions.controlSchemes[currentPresent].name);
            }

            presetList[currentPresent].PastePresetSettings(presetList[clipboard].GetPresetSettings());
        }
    }

    public void ResetControlScheme()
    {
        List<RebindActionUI> buttons = presetList[currentPresent].GetButtons();
        foreach (RebindActionUI button in buttons)
            button.ResetToDefault();
    }

    public void ClearClipboard()
    {
        clipboard = -1;
    }

    //TODO: Load From Json
    //TODO: Save to Json File
}
