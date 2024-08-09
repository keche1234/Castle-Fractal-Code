using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

////TODO: localization support

////TODO: deal with composites that have parts bound in different control schemes

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    /// <summary>
    /// A reusable component with a self-contained UI for rebinding a single action.
    /// </summary>
    public class RebindActionUI : MonoBehaviour
    {

        /// <summary>
        /// Current control preset
        /// </summary>
        public string currentControlPreset
        {
            get => m_CurrentControlPreset;
            set
            {
                m_CurrentControlPreset = value;
            }
        }

        /// <summary>
        /// Reference to the action that is to be rebound.
        /// </summary>
        public InputActionReference actionReference
        {
            get => m_Action;
            set
            {
                m_Action = value;
                UpdateActionLabel();
                UpdateBindingDisplay();
            }
        }

        /// <summary>
        /// ID (in string form) of the binding that is to be rebound on the action.
        /// </summary>
        /// <seealso cref="InputBinding.id"/>
        public string bindingId
        {
            get => m_BindingId;
            set
            {
                m_BindingId = value;
                UpdateBindingDisplay();
            }
        }

        ///<summary>
        /// A list of actions you can change with this one
        /// </summary>
        public List<InputActionReference> otherActions
        {
            get => m_OtherActions;
            set
            {

            }
        }

        /// <summary>
        /// A list of bindings that will also change with this one
        /// </summary>
        public List<string> otherBindings
        {
            get => m_OtherBindings;
            set
            {

            }
        }

        /// <summary>
        /// A list of binding indexes that this button can share controls with
        /// </summary>
        public List<string> permittedBindingDuplicates
        {
            get => m_PermittedBindingDuplicates;
            set
            {

            }
        }

        public InputBinding.DisplayStringOptions displayStringOptions
        {
            get => m_DisplayStringOptions;
            set
            {
                m_DisplayStringOptions = value;
                UpdateBindingDisplay();
            }
        }

        /// <summary>
        /// Text component that receives the name of the action. Optional.
        /// </summary>
        public Text actionLabel
        {
            get => m_ActionLabel;
            set
            {
                m_ActionLabel = value;
                UpdateActionLabel();
            }
        }

        /// <summary>
        /// Text component that receives the display string of the binding. Can be <c>null</c> in which
        /// case the component entirely relies on <see cref="updateBindingUIEvent"/>.
        /// </summary>
        public Text bindingText
        {
            get => m_BindingText;
            set
            {
                m_BindingText = value;
                UpdateBindingDisplay();
            }
        }

        /// <summary>
        /// Optional text component that receives a text prompt when waiting for a control to be actuated.
        /// </summary>
        /// <seealso cref="startRebindEvent"/>
        /// <seealso cref="rebindOverlay"/>
        public Text rebindPrompt
        {
            get => m_RebindText;
            set => m_RebindText = value;
        }

        /// <summary>
        /// Optional UI that is activated when an interactive rebind is started and deactivated when the rebind
        /// is finished. This is normally used to display an overlay over the current UI while the system is
        /// waiting for a control to be actuated.
        /// </summary>
        /// <remarks>
        /// If neither <see cref="rebindPrompt"/> nor <c>rebindOverlay</c> is set, the component will temporarily
        /// replaced the <see cref="bindingText"/> (if not <c>null</c>) with <c>"Waiting..."</c>.
        /// </remarks>
        /// <seealso cref="startRebindEvent"/>
        /// <seealso cref="rebindPrompt"/>
        public GameObject rebindOverlay
        {
            get => m_RebindOverlay;
            set => m_RebindOverlay = value;
        }

        /// <summary>
        /// Event that is triggered every time the UI updates to reflect the current binding.
        /// This can be used to tie custom visualizations to bindings.
        /// </summary>
        public UpdateBindingUIEvent updateBindingUIEvent
        {
            get
            {
                if (m_UpdateBindingUIEvent == null)
                    m_UpdateBindingUIEvent = new UpdateBindingUIEvent();
                return m_UpdateBindingUIEvent;
            }
        }

        /// <summary>
        /// Event that is triggered when an interactive rebind is started on the action.
        /// </summary>
        public InteractiveRebindEvent startRebindEvent
        {
            get
            {
                if (m_RebindStartEvent == null)
                    m_RebindStartEvent = new InteractiveRebindEvent();
                return m_RebindStartEvent;
            }
        }

        /// <summary>
        /// Event that is triggered when an interactive rebind has been completed or canceled.
        /// </summary>
        public InteractiveRebindEvent stopRebindEvent
        {
            get
            {
                if (m_RebindStopEvent == null)
                    m_RebindStopEvent = new InteractiveRebindEvent();
                return m_RebindStopEvent;
            }
        }

        /// <summary>
        /// When an interactive rebind is in progress, this is the rebind operation controller.
        /// Otherwise, it is <c>null</c>.
        /// </summary>
        public InputActionRebindingExtensions.RebindingOperation ongoingRebind => m_RebindOperation;

        /// <summary>
        /// Return the action and binding index for the binding that is targeted by the component
        /// according to
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingIndex"></param>
        /// <returns></returns>
        public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
        {
            bindingIndex = -1;

            action = m_Action?.action;
            if (action == null)
                return false;

            if (string.IsNullOrEmpty(m_BindingId))
                return false;

            // Look up binding index.
            var bindingId = new Guid(m_BindingId);
            bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
            if (bindingIndex == -1)
            {
                Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'", this);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Trigger a refresh of the currently displayed binding.
        /// </summary>
        public void UpdateBindingDisplay()
        {
            var displayString = string.Empty;
            var deviceLayoutName = default(string);
            var controlPath = default(string);

            // Get display string from action.
            var action = m_Action?.action;
            if (action != null)
            {
                var bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == m_BindingId);
                if (bindingIndex != -1)
                    //displayString = action.GetBindingDisplayString(InputBinding.MaskByGroup(currentControlPreset));
                    displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, displayStringOptions);
            }

            // Set on label (if any).
            if (m_BindingText != null)
                m_BindingText.text = displayString;

            // Give listeners a chance to configure UI in response.
            m_UpdateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
        }

        /// <summary>
        /// Remove currently applied binding overrides.
        /// </summary>
        public void ResetToDefault()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            ResetBinding(action, bindingIndex);

            //if (action.bindings[bindingIndex].isComposite)
            //{
            //    // It's a composite. Remove overrides from part bindings.
            //    for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
            //        action.RemoveBindingOverride(i);
            //}
            //else
            //{
            //    action.RemoveBindingOverride(bindingIndex);
            //}
            UpdateBindingDisplay();
        }

        private void ResetBinding(InputAction action, int bindingIndex)
        {
            InputBinding newBinding = action.bindings[bindingIndex];
            //string oldOverridePath = newBinding.overridePath;

            action.RemoveBindingOverride(bindingIndex);

            //foreach (InputAction otherAction in action.actionMap.actions)
            //{
            //    if (otherAction == action)
            //    {
            //        continue;
            //    }

            //    for (int i = 0; i < otherAction.bindings.Count; i++)
            //    {
            //        InputBinding binding = otherAction.bindings[i];
            //        if (binding.overridePath == newBinding.path)
            //        {
            //            otherAction.ApplyBindingOverride(i, "");
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Initiate an interactive rebind that lets the player actuate a control to choose a new binding
        /// for the action.
        /// </summary>
        public void StartInteractiveRebind()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            // If the binding is a composite, we need to rebind each part in turn.
            if (action.bindings[bindingIndex].isComposite)
            {
                var firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                    PerformInteractiveRebind(action, bindingIndex, allCompositeParts: true);
            }
            else
            {
                PerformInteractiveRebind(action, bindingIndex);
            }
        }

        private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            m_RebindOperation?.Cancel(); // Will null out m_RebindOperation.

            void CleanUp()
            {
                m_RebindOperation?.Dispose();
                m_RebindOperation = null;
            }

            //disable the action before use to prevent errors
            action.Disable();

            //store old action path
            string oldPath = action.bindings[bindingIndex].effectivePath;

            // Configure the rebind.
            m_RebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .WithCancelingThrough("<Keyboard>/escape")
                .WithControlsExcluding("<Mouse>/scroll/y")
                //.WithExpectedControlType("axis")
                .WithBindingGroup(currentControlPreset)
                .OnCancel(
                    operation =>
                    {
                        action.Enable();
                        m_RebindStopEvent?.Invoke(this, operation);
                        m_RebindOverlay?.SetActive(false);
                        UpdateBindingDisplay();
                        CleanUp();
                    })
                .OnComplete(
                    operation =>
                    {
                        action.Enable();
                        m_RebindOverlay?.SetActive(false);

                        for (int i = 0; i < 10; i++)
                        {
                            if (action.bindings[bindingIndex].effectivePath == "<Keyboard>/" + i.ToString()
                                || action.bindings[bindingIndex].effectivePath == "<Keyboard>/numpad" + i.ToString())
                            {
                                action.ApplyBindingOverride(bindingIndex, oldPath);
                                PerformInteractiveRebind(action, bindingIndex, allCompositeParts);
                                return;
                            }
                        }

                        if (action.bindings[bindingIndex].effectivePath == "<Keyboard>/delete")
                        {
                            action.ApplyBindingOverride(bindingIndex, "--");
                            CleanUp();
                        }

                        (string, int) dupe = CheckDuplicateBindings(action, bindingIndex, currentControlPreset, allCompositeParts);
                        if (dupe.Item1 != "")
                        {
                            InputAction oldAction = action.actionMap.FindAction(dupe.Item1);
                            if (dupe.Item2 > -1)
                                oldAction.ApplyBindingOverride(dupe.Item2, "--");
                            else
                                oldAction.ApplyBindingOverride("--");
                            CleanUp();
                        }

                        //Rebind other bindings tied to this

                        int elementCount = Mathf.Min(otherActions.Count, otherBindings.Count);
                        if (otherActions.Count - otherBindings.Count != 0)
                            Debug.LogError("Lists " + otherActions + " and " + otherBindings + " must be parallel! (Counts: " + otherActions.Count + " vs " + otherBindings + ")");

                        for (int i = 0; i < otherActions.Count; i++)
                        {
                            InputAction otherAction = otherActions[i].action;

                            if (string.IsNullOrEmpty(otherBindings[i]))
                                continue;

                            int otherBindingIndex = otherAction.bindings.IndexOf(x => x.id == new Guid(otherBindings[i]));

                            if (otherBindingIndex != -1)
                                otherAction.ApplyBindingOverride(otherBindingIndex, action.bindings[bindingIndex].effectivePath);
                            else
                                Debug.LogError($"Cannot find binding with ID '{otherBindingIndex}' on '{otherAction}'", this);
                        }

                        UpdateBindingDisplay();
                        CleanUp();

                        // If there's more composite parts we should bind, initiate a rebind for the next part.
                        if (allCompositeParts)
                        {
                            var nextBindingIndex = bindingIndex + 1;
                            if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                                PerformInteractiveRebind(action, nextBindingIndex, true);
                        }
                        m_RebindStopEvent?.Invoke(this, operation);
                    });

            // If it's a part binding, show the name of the part in the UI.
            var partName = default(string);
            if (action.bindings[bindingIndex].isPartOfComposite)
            {
                //Debug.Log(action.bindings[bindingIndex - 1].isComposite + " and " + "(" + (bindingIndex + 1 == action.bindings.Count) + " or " + "NOT-" + action.bindings[bindingIndex + 1].isPartOfComposite + ")");

                // Modifier
                if (action.bindings[bindingIndex - 1].isComposite
                    && (bindingIndex + 1 >= action.bindings.Count || action.bindings[bindingIndex + 1].isPartOfComposite)
                    && (bindingIndex + 2 >= action.bindings.Count || !action.bindings[bindingIndex + 2].isPartOfComposite))
                {
                    partName = $"Binding '{action.name}'. ";
                }
                else //Part of 2D Vector
                    partName = $"Binding '{action.bindings[bindingIndex].name}'. ";
            }
            else
            {
                partName = $"Binding '{action.name}'. ";
            }

            // Bring up rebind overlay, if we have one.
            m_RebindOverlay?.SetActive(true);
            if (m_RebindText != null)
            {
                var text = //!string.IsNullOrEmpty(m_RebindOperation.expectedControlType)
                           //? $"{partName}Waiting for {m_RebindOperation.expectedControlType} input..."
                    /*:*/ $"{partName}Waiting for input...\n(Press 'Delete' to clear, 'Escape' to cancel)";
                m_RebindText.text = text;
            }

            // If we have no rebind overlay and no callback but we have a binding text label,
            // temporarily set the binding text label to "<Waiting>".
            if (m_RebindOverlay == null && m_RebindText == null && m_RebindStartEvent == null && m_BindingText != null)
                m_BindingText.text = "<Waiting...>";

            // Give listeners a chance to act on the rebind starting.
            m_RebindStartEvent?.Invoke(this, m_RebindOperation);

            m_RebindOperation.Start();
        }

        ///<summary>
        /// Checks if any other actions are mapped to the same button as the `action` `bindingIndex` pair in this control scheme,
        /// And returns the action that is causing conflict
        ///</summary>
        private (string, int) CheckDuplicateBindings(InputAction action, int bindingIndex, string currentGroup, bool allCompositeParts = false)
        {
            InputBinding newBinding = action.bindings[bindingIndex];
            int currentIndex = -1; //use this to prevent duplicates within composite (if remapping individually)

            foreach (InputBinding binding in action.actionMap.bindings)
            {
                if (binding.action == newBinding.action) // if binding is the new binding we're checking against:
                {
                    currentIndex++;
                    if (binding.isPartOfComposite && currentIndex != bindingIndex) // Need to check against other buttons in composite
                    {
                        if (binding.effectivePath == newBinding.effectivePath && binding.groups.Contains(currentGroup))
                        {
                            Debug.Log("Duplicate binding found in composite: " + newBinding.effectivePath);
                            return (binding.action, currentIndex);
                        }
                    }
                    else // Single binding, or same part of composite
                    {
                        continue;
                    }
                }

                // Make sure binding is in the same group as newBinding (TODO: Composites get funky)
                //TODO: Allow main attack to share binding with Signature's binding if in Combo

                if (binding.effectivePath == newBinding.effectivePath && binding.groups.Contains(currentGroup)) //For Signature Moves
                {
                    if (!permittedBindingDuplicates.Contains(binding.id.ToString()))
                    {
                        Debug.Log("Duplicate binding found: " + binding.effectivePath + " (" + binding.action + ")");
                        int mapIndex = action.actionMap.GetBindingIndex(binding);
                        InputAction oldAction = action.actionMap.FindAction(action.actionMap.bindings[mapIndex].action);
                        return (binding.action, oldAction.GetBindingIndex(binding));
                    }
                    else
                    {
                        Debug.Log("Duplicate binding found but ALLOWED: " + binding.effectivePath + " (" + binding.action + ")");
                    }
                }
            }

            if (allCompositeParts) // check against all composite parts if rebinding composite as a whole
            {
                for (int i = 1; i < bindingIndex; i++)
                {
                    if (action.bindings[i].effectivePath == newBinding.overridePath && action.bindings[i].groups.Contains(currentGroup))
                    {
                        Debug.Log("Duplicate binding found: " + newBinding.overridePath);
                        return (action.bindings[i].action, i);
                    }
                }
            }

            return ("", -1);
        }

        protected void OnEnable()
        {
            if (s_RebindActionUIs == null)
                s_RebindActionUIs = new List<RebindActionUI>();
            s_RebindActionUIs.Add(this);
            if (s_RebindActionUIs.Count == 1)
                InputSystem.onActionChange += OnActionChange;
        }

        protected void OnDisable()
        {
            m_RebindOperation?.Dispose();
            m_RebindOperation = null;

            s_RebindActionUIs.Remove(this);
            if (s_RebindActionUIs.Count == 0)
            {
                s_RebindActionUIs = null;
                InputSystem.onActionChange -= OnActionChange;
            }
        }

        // When the action system re-resolves bindings, we want to update our UI in response. While this will
        // also trigger from changes we made ourselves, it ensures that we react to changes made elsewhere. If
        // the user changes keyboard layout, for example, we will get a BoundControlsChanged notification and
        // will update our UI to reflect the current keyboard layout.
        private static void OnActionChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.BoundControlsChanged)
                return;

            var action = obj as InputAction;
            var actionMap = action?.actionMap ?? obj as InputActionMap;
            var actionAsset = actionMap?.asset ?? obj as InputActionAsset;

            for (var i = 0; i < s_RebindActionUIs.Count; ++i)
            {
                var component = s_RebindActionUIs[i];
                var referencedAction = component.actionReference?.action;
                if (referencedAction == null)
                    continue;

                if (referencedAction == action ||
                    referencedAction.actionMap == actionMap ||
                    referencedAction.actionMap?.asset == actionAsset)
                    component.UpdateBindingDisplay();
            }
        }

        [Tooltip("The current Control Preset that this button is attached to.")]
        //[Range(1, 9)]
        [SerializeField]
        private string m_CurrentControlPreset;

        [Tooltip("Reference to action that is to be rebound from the UI.")]
        [SerializeField]
        private InputActionReference m_Action;

        [SerializeField]
        private string m_BindingId;

        [Tooltip("Reference to other actions that will be rebound.")]
        [SerializeField]
        private List<InputActionReference> m_OtherActions;

        [Tooltip("Parallel to `Other Actions` list.")]
        [SerializeField]
        private List<string> m_OtherBindings;

        //[Tooltip("Bindings that are also remapped")]
        //[SerializeField]
        //private List<string> m_OtherBindings;

        [Tooltip("Bindings that, if you set this action to the same control, will permit duplicates. This permission is one way.")]
        [SerializeField]
        private List<string> m_PermittedBindingDuplicates;

        [SerializeField]
        private InputBinding.DisplayStringOptions m_DisplayStringOptions;

        [Tooltip("Text label that will receive the name of the action. Optional. Set to None to have the "
            + "rebind UI not show a label for the action.")]
        [SerializeField]
        private Text m_ActionLabel;

        [Tooltip("Text label that will receive the current, formatted binding string.")]
        [SerializeField]
        private Text m_BindingText;

        [Tooltip("Optional UI that will be shown while a rebind is in progress.")]
        [SerializeField]
        private GameObject m_RebindOverlay;

        [Tooltip("Optional text label that will be updated with prompt for user input.")]
        [SerializeField]
        private Text m_RebindText;

        [Tooltip("Optional bool field which allows you to OVERRIDE the action label with your own text")]
        public bool m_OverRideActionLabel;

        [Tooltip("What text should be displayed for the action label?")]
        [SerializeField] private List<string> m_ActionLabelStrings;

        [Tooltip("Event that is triggered when the way the binding is display should be updated. This allows displaying "
            + "bindings in custom ways, e.g. using images instead of text.")]
        [SerializeField]
        private UpdateBindingUIEvent m_UpdateBindingUIEvent;

        [Tooltip("Event that is triggered when an interactive rebind is being initiated. This can be used, for example, "
            + "to implement custom UI behavior while a rebind is in progress. It can also be used to further "
            + "customize the rebind.")]
        [SerializeField]
        private InteractiveRebindEvent m_RebindStartEvent;

        [Tooltip("Event that is triggered when an interactive rebind is complete or has been aborted.")]
        [SerializeField]
        private InteractiveRebindEvent m_RebindStopEvent;

        private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;

        private static List<RebindActionUI> s_RebindActionUIs;

        // We want the label for the action name to update in edit mode, too, so
        // we kick that off from here.
#if UNITY_EDITOR
        protected void OnValidate()
        {
            UpdateActionLabel();
            UpdateBindingDisplay();
        }

#endif

        private void UpdateActionLabel()
        {
            if (m_ActionLabel != null)
            {
                var action = m_Action?.action;
                m_ActionLabel.text = "";

                if (m_OverRideActionLabel && m_ActionLabelStrings != null)
                {
                    if (m_ActionLabelStrings.Count >= 1)
                    {
                        m_ActionLabel.text += m_ActionLabelStrings[0];
                        for (int i = 1; i < m_ActionLabelStrings.Count; i++)
                            m_ActionLabel.text += "\n" + m_ActionLabelStrings[i];
                    }
                }
                else
                {
                    if (m_ActionLabel)
                        m_ActionLabel.text = action != null ? action.name : string.Empty;
                    m_ActionLabelStrings = null;
                }

                //m_ActionLabel.text = action != null ? action.name : string.Empty;
            }
        }

        [Serializable]
        public class UpdateBindingUIEvent : UnityEvent<RebindActionUI, string, string, string>
        {
        }

        [Serializable]
        public class InteractiveRebindEvent : UnityEvent<RebindActionUI, InputActionRebindingExtensions.RebindingOperation>
        {
        }
    }
}
