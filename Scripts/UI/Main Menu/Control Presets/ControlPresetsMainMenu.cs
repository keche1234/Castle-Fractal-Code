using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;

public class ControlPresetsMainMenu : MonoBehaviour
{
    [SerializeField] protected List<RebindActionUI> buttons;
    [SerializeField] protected List<ControlPresetSettings> presets;
    [SerializeField] protected PlayerInputActions inputActions;
    protected int currentScheme = 0;
    protected int clipboard = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCurrentControlScheme(int i)
    {
        if (i >= 0 || i < presets.Count)
            currentScheme = i;

        //TODO: For each button, change the binding and control scheme
    }

    public void CopyControlScheme()
    {
        clipboard = currentScheme;
    }

    public void PasteControlScheme()
    {
        if (clipboard > -1)
        {
            string bindingToPaste = inputActions.Player.Move.GetBindingDisplayString(0, inputActions.controlSchemes[clipboard].name);
            inputActions.Player.Move.ApplyBindingOverride(bindingToPaste, inputActions.controlSchemes[currentScheme].name);

            bindingToPaste = inputActions.Player.MainAttack.GetBindingDisplayString(0, inputActions.controlSchemes[clipboard].name);
            inputActions.Player.MainAttack.ApplyBindingOverride(bindingToPaste, inputActions.controlSchemes[currentScheme].name);

            bindingToPaste = inputActions.Player.Roll.GetBindingDisplayString(0, inputActions.controlSchemes[clipboard].name);
            inputActions.Player.Roll.ApplyBindingOverride(bindingToPaste, inputActions.controlSchemes[currentScheme].name);

            bindingToPaste = inputActions.Player.SignatureAttack.GetBindingDisplayString(0, inputActions.controlSchemes[clipboard].name);
            inputActions.Player.SignatureAttack.ApplyBindingOverride(bindingToPaste, inputActions.controlSchemes[currentScheme].name);

            bindingToPaste = inputActions.Player.ScrollInventory.GetBindingDisplayString(0, inputActions.controlSchemes[clipboard].name);
            inputActions.Player.ScrollInventory.ApplyBindingOverride(bindingToPaste, inputActions.controlSchemes[currentScheme].name);

            bindingToPaste = inputActions.Player.DropWeapon.GetBindingDisplayString(0, inputActions.controlSchemes[clipboard].name);
            inputActions.Player.DropWeapon.ApplyBindingOverride(bindingToPaste, inputActions.controlSchemes[currentScheme].name);

            bindingToPaste = inputActions.Player.Inventory.GetBindingDisplayString(0, inputActions.controlSchemes[clipboard].name);
            inputActions.Player.Inventory.ApplyBindingOverride(bindingToPaste, inputActions.controlSchemes[currentScheme].name);

            bindingToPaste = inputActions.Player.Pause.GetBindingDisplayString(0, inputActions.controlSchemes[clipboard].name);
            inputActions.Player.Pause.ApplyBindingOverride(bindingToPaste, inputActions.controlSchemes[currentScheme].name);
        }
    }

    public void ResetControlScheme()
    {
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
