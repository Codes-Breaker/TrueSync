//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Script/Input/PlayerOneMovementActions.inputactions
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

public partial class @PlayerOneMovementActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerOneMovementActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerOneMovementActions"",
    ""maps"": [
        {
            ""name"": ""Action"",
            ""id"": ""65086328-0fba-4ced-a9ad-281a5db3ede1"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""9da37897-9fab-42a1-b5d8-6fefc98ba3e4"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Charge"",
                    ""type"": ""PassThrough"",
                    ""id"": ""7f7ed9af-28be-4f44-aed9-9d05eb3bb529"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""InteractWeapon"",
                    ""type"": ""PassThrough"",
                    ""id"": ""6473c0cb-4db6-41fb-b1c9-73f9cf8429eb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""PassThrough"",
                    ""id"": ""395df206-48e0-47b2-82cb-544ea04bcc9a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Brake"",
                    ""type"": ""PassThrough"",
                    ""id"": ""20bd0e20-5025-49b5-8753-cffc26d25774"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""e433ff48-5b56-434a-9d5f-aac3ec1979d2"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b364e981-6687-4e5b-ae41-baa2ed80f202"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3e07b6c1-3629-4a07-88a3-407227a8ea2a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e09524dc-d1a6-4ea6-bd02-af6506fb40fc"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""5bec65bd-978d-4bd3-890e-bae44bc0dbb1"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""54f8b90b-dfae-4f39-9cb1-dac960fa9a6d"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""38e52786-05bc-4d25-919e-62430a71fba1"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": ""GamePad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7127d6f5-dd11-482b-991f-89a44f33f107"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Charge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1ae9372c-6005-42ba-a412-68f2bcd2d9ac"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse"",
                    ""action"": ""Charge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2328309c-0e96-43ed-bce5-93c3ecababa8"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""InteractWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1c67c89d-97a7-46a9-8e9e-75e610c5d9db"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse"",
                    ""action"": ""InteractWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d5272e57-ce24-4a4c-813b-e19de82d9065"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""Charge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2a3b1e1b-b66d-471d-a936-97d0813687cd"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""InteractWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7b35eeec-e402-4a90-abd6-d5044d5d7530"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e461a733-d73a-4321-a43a-60d9e1396128"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""Brake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Action1"",
            ""id"": ""76df637c-0631-41e0-8e7d-07a688d9f9a2"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""fb80aeb8-3520-4a54-b8d7-09ba67fb9f29"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Charge"",
                    ""type"": ""PassThrough"",
                    ""id"": ""08d53de2-d4e0-4ca8-810e-bf07aa63b10c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""InteractWeapon"",
                    ""type"": ""PassThrough"",
                    ""id"": ""7075835a-54b8-402a-934d-0e3f51d4feda"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""a0347474-8f94-4ecb-8d7a-069d43944b03"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""3abe6dcc-61d2-406f-93ec-fc48f764eb13"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""7a9afce1-d71f-47fa-8ab4-42ec815ee46e"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""82393ca7-9c57-4ac5-bc8d-9124e2ce6e7e"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""bed08c31-f262-4a7b-8583-1beb2ae3b7f0"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""b0b2de99-02a2-46a0-accd-702140b6e288"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""af1f0a7d-f2a7-4ab9-a78d-508f0f2d003c"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Charge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fc918f71-6363-4cee-ab43-a706d82cfff1"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse"",
                    ""action"": ""Charge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""064a1cce-82df-44bc-9962-c6dd6e4c6207"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""InteractWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2a719346-d8e2-4541-9d90-4e51bcd6d3a7"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse"",
                    ""action"": ""InteractWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e753ece0-bf8f-47d2-8b82-817336161cf6"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6a9e7e68-8b72-4961-8d3d-f63b6a646af2"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""Charge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e2ae922a-2253-4d46-ac0d-35d1c241eff5"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""InteractWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""GamePad"",
            ""bindingGroup"": ""GamePad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Mouse"",
            ""bindingGroup"": ""Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Action
        m_Action = asset.FindActionMap("Action", throwIfNotFound: true);
        m_Action_Movement = m_Action.FindAction("Movement", throwIfNotFound: true);
        m_Action_Charge = m_Action.FindAction("Charge", throwIfNotFound: true);
        m_Action_InteractWeapon = m_Action.FindAction("InteractWeapon", throwIfNotFound: true);
        m_Action_Jump = m_Action.FindAction("Jump", throwIfNotFound: true);
        m_Action_Brake = m_Action.FindAction("Brake", throwIfNotFound: true);
        // Action1
        m_Action1 = asset.FindActionMap("Action1", throwIfNotFound: true);
        m_Action1_Movement = m_Action1.FindAction("Movement", throwIfNotFound: true);
        m_Action1_Charge = m_Action1.FindAction("Charge", throwIfNotFound: true);
        m_Action1_InteractWeapon = m_Action1.FindAction("InteractWeapon", throwIfNotFound: true);
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

    // Action
    private readonly InputActionMap m_Action;
    private IActionActions m_ActionActionsCallbackInterface;
    private readonly InputAction m_Action_Movement;
    private readonly InputAction m_Action_Charge;
    private readonly InputAction m_Action_InteractWeapon;
    private readonly InputAction m_Action_Jump;
    private readonly InputAction m_Action_Brake;
    public struct ActionActions
    {
        private @PlayerOneMovementActions m_Wrapper;
        public ActionActions(@PlayerOneMovementActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Action_Movement;
        public InputAction @Charge => m_Wrapper.m_Action_Charge;
        public InputAction @InteractWeapon => m_Wrapper.m_Action_InteractWeapon;
        public InputAction @Jump => m_Wrapper.m_Action_Jump;
        public InputAction @Brake => m_Wrapper.m_Action_Brake;
        public InputActionMap Get() { return m_Wrapper.m_Action; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ActionActions set) { return set.Get(); }
        public void SetCallbacks(IActionActions instance)
        {
            if (m_Wrapper.m_ActionActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_ActionActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_ActionActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_ActionActionsCallbackInterface.OnMovement;
                @Charge.started -= m_Wrapper.m_ActionActionsCallbackInterface.OnCharge;
                @Charge.performed -= m_Wrapper.m_ActionActionsCallbackInterface.OnCharge;
                @Charge.canceled -= m_Wrapper.m_ActionActionsCallbackInterface.OnCharge;
                @InteractWeapon.started -= m_Wrapper.m_ActionActionsCallbackInterface.OnInteractWeapon;
                @InteractWeapon.performed -= m_Wrapper.m_ActionActionsCallbackInterface.OnInteractWeapon;
                @InteractWeapon.canceled -= m_Wrapper.m_ActionActionsCallbackInterface.OnInteractWeapon;
                @Jump.started -= m_Wrapper.m_ActionActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_ActionActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_ActionActionsCallbackInterface.OnJump;
                @Brake.started -= m_Wrapper.m_ActionActionsCallbackInterface.OnBrake;
                @Brake.performed -= m_Wrapper.m_ActionActionsCallbackInterface.OnBrake;
                @Brake.canceled -= m_Wrapper.m_ActionActionsCallbackInterface.OnBrake;
            }
            m_Wrapper.m_ActionActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Charge.started += instance.OnCharge;
                @Charge.performed += instance.OnCharge;
                @Charge.canceled += instance.OnCharge;
                @InteractWeapon.started += instance.OnInteractWeapon;
                @InteractWeapon.performed += instance.OnInteractWeapon;
                @InteractWeapon.canceled += instance.OnInteractWeapon;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Brake.started += instance.OnBrake;
                @Brake.performed += instance.OnBrake;
                @Brake.canceled += instance.OnBrake;
            }
        }
    }
    public ActionActions @Action => new ActionActions(this);

    // Action1
    private readonly InputActionMap m_Action1;
    private IAction1Actions m_Action1ActionsCallbackInterface;
    private readonly InputAction m_Action1_Movement;
    private readonly InputAction m_Action1_Charge;
    private readonly InputAction m_Action1_InteractWeapon;
    public struct Action1Actions
    {
        private @PlayerOneMovementActions m_Wrapper;
        public Action1Actions(@PlayerOneMovementActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Action1_Movement;
        public InputAction @Charge => m_Wrapper.m_Action1_Charge;
        public InputAction @InteractWeapon => m_Wrapper.m_Action1_InteractWeapon;
        public InputActionMap Get() { return m_Wrapper.m_Action1; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Action1Actions set) { return set.Get(); }
        public void SetCallbacks(IAction1Actions instance)
        {
            if (m_Wrapper.m_Action1ActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_Action1ActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_Action1ActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_Action1ActionsCallbackInterface.OnMovement;
                @Charge.started -= m_Wrapper.m_Action1ActionsCallbackInterface.OnCharge;
                @Charge.performed -= m_Wrapper.m_Action1ActionsCallbackInterface.OnCharge;
                @Charge.canceled -= m_Wrapper.m_Action1ActionsCallbackInterface.OnCharge;
                @InteractWeapon.started -= m_Wrapper.m_Action1ActionsCallbackInterface.OnInteractWeapon;
                @InteractWeapon.performed -= m_Wrapper.m_Action1ActionsCallbackInterface.OnInteractWeapon;
                @InteractWeapon.canceled -= m_Wrapper.m_Action1ActionsCallbackInterface.OnInteractWeapon;
            }
            m_Wrapper.m_Action1ActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Charge.started += instance.OnCharge;
                @Charge.performed += instance.OnCharge;
                @Charge.canceled += instance.OnCharge;
                @InteractWeapon.started += instance.OnInteractWeapon;
                @InteractWeapon.performed += instance.OnInteractWeapon;
                @InteractWeapon.canceled += instance.OnInteractWeapon;
            }
        }
    }
    public Action1Actions @Action1 => new Action1Actions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    private int m_GamePadSchemeIndex = -1;
    public InputControlScheme GamePadScheme
    {
        get
        {
            if (m_GamePadSchemeIndex == -1) m_GamePadSchemeIndex = asset.FindControlSchemeIndex("GamePad");
            return asset.controlSchemes[m_GamePadSchemeIndex];
        }
    }
    private int m_MouseSchemeIndex = -1;
    public InputControlScheme MouseScheme
    {
        get
        {
            if (m_MouseSchemeIndex == -1) m_MouseSchemeIndex = asset.FindControlSchemeIndex("Mouse");
            return asset.controlSchemes[m_MouseSchemeIndex];
        }
    }
    public interface IActionActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnCharge(InputAction.CallbackContext context);
        void OnInteractWeapon(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnBrake(InputAction.CallbackContext context);
    }
    public interface IAction1Actions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnCharge(InputAction.CallbackContext context);
        void OnInteractWeapon(InputAction.CallbackContext context);
    }
}
