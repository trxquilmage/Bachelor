// GENERATED AUTOMATICALLY FROM 'Assets/Input/InputMap.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMap : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMap()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMap"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""81d39cb8-5eb3-4d82-a25d-d1e703109ab8"",
            ""actions"": [
                {
                    ""name"": ""WalkUD"",
                    ""type"": ""Button"",
                    ""id"": ""0a94053d-4f1a-42f7-afc5-7a19bc3d49aa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""WalkLR"",
                    ""type"": ""Button"",
                    ""id"": ""7596f41f-fb45-4419-ac09-af27cd937f18"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Talk"",
                    ""type"": ""Button"",
                    ""id"": ""5fb3d4c9-6d9e-43c7-9289-465c269e5a40"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WalkUD"",
                    ""id"": ""13135840-5f90-4e22-b298-c81bbc1ab2e4"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WalkUD"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""00293f1a-1224-4f9b-b3d1-9ba75d6c9657"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WalkUD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""2399a7e5-fb82-4531-93ac-f629a02ad764"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WalkUD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""31562889-e4ef-4055-8c66-76458ee96ab2"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Talk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6bbbad5c-1fdc-4b7c-b784-35a39f1a2d91"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Talk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WalkLR"",
                    ""id"": ""01909831-ecb7-49aa-bace-9f66096d890c"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WalkLR"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""8d6bc6bf-e4b4-43d1-b884-1690f6af0071"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WalkLR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""a218eabe-9c30-4e35-aa35-7aeccc4a9427"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WalkLR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Dialogue"",
            ""id"": ""196d438c-070c-4120-af4a-da6e66590f38"",
            ""actions"": [
                {
                    ""name"": ""Click"",
                    ""type"": ""Button"",
                    ""id"": ""85ec8af9-98d1-40ab-8b70-6795cfb19749"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""114a3d5a-c1b6-4f5c-892f-863fc7f4c8d1"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ece10229-0985-4930-869b-769ab5a3342b"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f96fa0f8-2375-4034-af4b-78d681434b07"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2555510a-ffef-4c7a-b318-1ad4cb5f74b4"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_WalkUD = m_Player.FindAction("WalkUD", throwIfNotFound: true);
        m_Player_WalkLR = m_Player.FindAction("WalkLR", throwIfNotFound: true);
        m_Player_Talk = m_Player.FindAction("Talk", throwIfNotFound: true);
        // Dialogue
        m_Dialogue = asset.FindActionMap("Dialogue", throwIfNotFound: true);
        m_Dialogue_Click = m_Dialogue.FindAction("Click", throwIfNotFound: true);
        m_Dialogue_MousePosition = m_Dialogue.FindAction("MousePosition", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_WalkUD;
    private readonly InputAction m_Player_WalkLR;
    private readonly InputAction m_Player_Talk;
    public struct PlayerActions
    {
        private @InputMap m_Wrapper;
        public PlayerActions(@InputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @WalkUD => m_Wrapper.m_Player_WalkUD;
        public InputAction @WalkLR => m_Wrapper.m_Player_WalkLR;
        public InputAction @Talk => m_Wrapper.m_Player_Talk;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @WalkUD.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWalkUD;
                @WalkUD.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWalkUD;
                @WalkUD.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWalkUD;
                @WalkLR.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWalkLR;
                @WalkLR.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWalkLR;
                @WalkLR.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWalkLR;
                @Talk.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTalk;
                @Talk.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTalk;
                @Talk.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTalk;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @WalkUD.started += instance.OnWalkUD;
                @WalkUD.performed += instance.OnWalkUD;
                @WalkUD.canceled += instance.OnWalkUD;
                @WalkLR.started += instance.OnWalkLR;
                @WalkLR.performed += instance.OnWalkLR;
                @WalkLR.canceled += instance.OnWalkLR;
                @Talk.started += instance.OnTalk;
                @Talk.performed += instance.OnTalk;
                @Talk.canceled += instance.OnTalk;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Dialogue
    private readonly InputActionMap m_Dialogue;
    private IDialogueActions m_DialogueActionsCallbackInterface;
    private readonly InputAction m_Dialogue_Click;
    private readonly InputAction m_Dialogue_MousePosition;
    public struct DialogueActions
    {
        private @InputMap m_Wrapper;
        public DialogueActions(@InputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @Click => m_Wrapper.m_Dialogue_Click;
        public InputAction @MousePosition => m_Wrapper.m_Dialogue_MousePosition;
        public InputActionMap Get() { return m_Wrapper.m_Dialogue; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DialogueActions set) { return set.Get(); }
        public void SetCallbacks(IDialogueActions instance)
        {
            if (m_Wrapper.m_DialogueActionsCallbackInterface != null)
            {
                @Click.started -= m_Wrapper.m_DialogueActionsCallbackInterface.OnClick;
                @Click.performed -= m_Wrapper.m_DialogueActionsCallbackInterface.OnClick;
                @Click.canceled -= m_Wrapper.m_DialogueActionsCallbackInterface.OnClick;
                @MousePosition.started -= m_Wrapper.m_DialogueActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_DialogueActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_DialogueActionsCallbackInterface.OnMousePosition;
            }
            m_Wrapper.m_DialogueActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Click.started += instance.OnClick;
                @Click.performed += instance.OnClick;
                @Click.canceled += instance.OnClick;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
            }
        }
    }
    public DialogueActions @Dialogue => new DialogueActions(this);
    public interface IPlayerActions
    {
        void OnWalkUD(InputAction.CallbackContext context);
        void OnWalkLR(InputAction.CallbackContext context);
        void OnTalk(InputAction.CallbackContext context);
    }
    public interface IDialogueActions
    {
        void OnClick(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
    }
}