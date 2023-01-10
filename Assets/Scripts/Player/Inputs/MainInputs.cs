//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Scripts/Player/Inputs/MainInputs.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @MainInputs : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @MainInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""MainInputs"",
    ""maps"": [
        {
            ""name"": ""Main"",
            ""id"": ""be7e90a4-2e19-4d5f-a977-8275278edb9b"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""030870ee-73fc-4ea6-9019-aefa4fae239a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""PassThrough"",
                    ""id"": ""b2996dab-8dfe-4c8f-8b92-8d1977d72d47"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""63b37192-9ab2-4470-9688-44f6566cad60"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Value"",
                    ""id"": ""5cefc2f3-ef8e-4225-ae42-128bd34bf1e3"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""PassThrough"",
                    ""id"": ""14b8d7b7-d1ef-4b3f-8af7-f176719619cb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""EscapeY"",
                    ""type"": ""Value"",
                    ""id"": ""2ad92a2b-f682-4e00-9655-a769b6dd8551"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""EscapeX"",
                    ""type"": ""Value"",
                    ""id"": ""7f863c60-5e2c-4bb9-9240-edb34ffbc00e"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""ZQSD"",
                    ""id"": ""233cd02a-2740-4349-b182-635356c62ccb"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""add7153f-e5f3-4434-83a3-e54e1c04de62"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c9c5a58d-a22f-4b6d-81be-c263de848f84"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a7dad3ee-727a-43a3-aa50-297b993e0baf"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a10d5be0-b102-411b-b331-9188f532a1f6"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""eb3e5d57-a497-40d1-946c-76b7ee2fb3ca"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""57405dc3-c564-4f28-a16a-8d7cd1ac9fdd"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""639aaf42-277e-476e-9d40-e786052266b5"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""adedfe6e-d753-4deb-acec-a051ff1b1769"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""68a89b69-0c50-42da-a779-cf78988eec05"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EscapeY"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""1441a61b-fdfe-4753-8128-b2f5b7a31bf6"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EscapeY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""7d4a3c19-08c4-4064-9f3b-f9c9b42306ed"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EscapeY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""54962370-fa14-4ed2-9376-6abad2c30be0"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EscapeX"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""c1ea4975-26bc-4609-a5ec-1cb49432fb54"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EscapeX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""70862400-60bd-4a31-9dbe-39b0e1333a97"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EscapeX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Main
        m_Main = asset.FindActionMap("Main", throwIfNotFound: true);
        m_Main_Move = m_Main.FindAction("Move", throwIfNotFound: true);
        m_Main_Crouch = m_Main.FindAction("Crouch", throwIfNotFound: true);
        m_Main_Jump = m_Main.FindAction("Jump", throwIfNotFound: true);
        m_Main_Zoom = m_Main.FindAction("Zoom", throwIfNotFound: true);
        m_Main_Run = m_Main.FindAction("Run", throwIfNotFound: true);
        m_Main_EscapeY = m_Main.FindAction("EscapeY", throwIfNotFound: true);
        m_Main_EscapeX = m_Main.FindAction("EscapeX", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Main
    private readonly InputActionMap m_Main;
    private IMainActions m_MainActionsCallbackInterface;
    private readonly InputAction m_Main_Move;
    private readonly InputAction m_Main_Crouch;
    private readonly InputAction m_Main_Jump;
    private readonly InputAction m_Main_Zoom;
    private readonly InputAction m_Main_Run;
    private readonly InputAction m_Main_EscapeY;
    private readonly InputAction m_Main_EscapeX;
    public struct MainActions
    {
        private @MainInputs m_Wrapper;
        public MainActions(@MainInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Main_Move;
        public InputAction @Crouch => m_Wrapper.m_Main_Crouch;
        public InputAction @Jump => m_Wrapper.m_Main_Jump;
        public InputAction @Zoom => m_Wrapper.m_Main_Zoom;
        public InputAction @Run => m_Wrapper.m_Main_Run;
        public InputAction @EscapeY => m_Wrapper.m_Main_EscapeY;
        public InputAction @EscapeX => m_Wrapper.m_Main_EscapeX;
        public InputActionMap Get() { return m_Wrapper.m_Main; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MainActions set) { return set.Get(); }
        public void SetCallbacks(IMainActions instance)
        {
            if (m_Wrapper.m_MainActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_MainActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnMove;
                @Crouch.started -= m_Wrapper.m_MainActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnCrouch;
                @Jump.started -= m_Wrapper.m_MainActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnJump;
                @Zoom.started -= m_Wrapper.m_MainActionsCallbackInterface.OnZoom;
                @Zoom.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnZoom;
                @Zoom.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnZoom;
                @Run.started -= m_Wrapper.m_MainActionsCallbackInterface.OnRun;
                @Run.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnRun;
                @Run.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnRun;
                @EscapeY.started -= m_Wrapper.m_MainActionsCallbackInterface.OnEscapeY;
                @EscapeY.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnEscapeY;
                @EscapeY.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnEscapeY;
                @EscapeX.started -= m_Wrapper.m_MainActionsCallbackInterface.OnEscapeX;
                @EscapeX.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnEscapeX;
                @EscapeX.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnEscapeX;
            }
            m_Wrapper.m_MainActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Zoom.started += instance.OnZoom;
                @Zoom.performed += instance.OnZoom;
                @Zoom.canceled += instance.OnZoom;
                @Run.started += instance.OnRun;
                @Run.performed += instance.OnRun;
                @Run.canceled += instance.OnRun;
                @EscapeY.started += instance.OnEscapeY;
                @EscapeY.performed += instance.OnEscapeY;
                @EscapeY.canceled += instance.OnEscapeY;
                @EscapeX.started += instance.OnEscapeX;
                @EscapeX.performed += instance.OnEscapeX;
                @EscapeX.canceled += instance.OnEscapeX;
            }
        }
    }
    public MainActions @Main => new MainActions(this);
    public interface IMainActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnZoom(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
        void OnEscapeY(InputAction.CallbackContext context);
        void OnEscapeX(InputAction.CallbackContext context);
    }
}
