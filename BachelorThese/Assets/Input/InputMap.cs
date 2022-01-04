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
                },
                {
                    ""name"": ""TalkCompanion"",
                    ""type"": ""Button"",
                    ""id"": ""b18da991-6a40-473d-b477-d473f171cc88"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Escape"",
                    ""type"": ""Button"",
                    ""id"": ""a2bdcde0-3633-426d-ba99-7f41ca49eb65"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""a9714ca3-adc5-4155-9565-5d8690a22d96"",
                    ""path"": ""<Keyboard>/alt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TalkCompanion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fbddf45f-ee60-41f8-9a9b-0803ed87a2ff"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TalkCompanion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1704b675-3ff6-4827-9ac4-bd69b15a5cf8"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Escape"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
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
                },
                {
                    ""name"": ""DoubleClick"",
                    ""type"": ""Button"",
                    ""id"": ""179e4688-0c29-42d7-87d3-9f0aa0a6589e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""Button"",
                    ""id"": ""6cffd136-ce5e-4baa-874f-2323d26ea4c5"",
                    ""expectedControlType"": ""Button"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""3d22b8ad-bfdd-4ce0-afc4-50b52f5c6bd2"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""MultiTap(tapTime=0.2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DoubleClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""7278f5aa-e09d-49ac-b918-44db85e20446"",
                    ""path"": ""1DAxis(minValue=-0.1,maxValue=0.1,whichSideWins=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""97cdfef3-58cc-46a6-a5a3-85be50707d27"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""624e1972-360c-496e-82f4-439e74beaf3b"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
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
        m_Player_TalkCompanion = m_Player.FindAction("TalkCompanion", throwIfNotFound: true);
        m_Player_Escape = m_Player.FindAction("Escape", throwIfNotFound: true);
        // Dialogue
        m_Dialogue = asset.FindActionMap("Dialogue", throwIfNotFound: true);
        m_Dialogue_Click = m_Dialogue.FindAction("Click", throwIfNotFound: true);
        m_Dialogue_MousePosition = m_Dialogue.FindAction("MousePosition", throwIfNotFound: true);
        m_Dialogue_DoubleClick = m_Dialogue.FindAction("DoubleClick", throwIfNotFound: true);
        m_Dialogue_Scroll = m_Dialogue.FindAction("Scroll", throwIfNotFound: true);
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
    private readonly InputAction m_Player_TalkCompanion;
    private readonly InputAction m_Player_Escape;
    public struct PlayerActions
    {
        private @InputMap m_Wrapper;
        public PlayerActions(@InputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @WalkUD => m_Wrapper.m_Player_WalkUD;
        public InputAction @WalkLR => m_Wrapper.m_Player_WalkLR;
        public InputAction @Talk => m_Wrapper.m_Player_Talk;
        public InputAction @TalkCompanion => m_Wrapper.m_Player_TalkCompanion;
        public InputAction @Escape => m_Wrapper.m_Player_Escape;
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
                @TalkCompanion.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTalkCompanion;
                @TalkCompanion.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTalkCompanion;
                @TalkCompanion.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTalkCompanion;
                @Escape.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEscape;
                @Escape.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEscape;
                @Escape.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEscape;
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
                @TalkCompanion.started += instance.OnTalkCompanion;
                @TalkCompanion.performed += instance.OnTalkCompanion;
                @TalkCompanion.canceled += instance.OnTalkCompanion;
                @Escape.started += instance.OnEscape;
                @Escape.performed += instance.OnEscape;
                @Escape.canceled += instance.OnEscape;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Dialogue
    private readonly InputActionMap m_Dialogue;
    private IDialogueActions m_DialogueActionsCallbackInterface;
    private readonly InputAction m_Dialogue_Click;
    private readonly InputAction m_Dialogue_MousePosition;
    private readonly InputAction m_Dialogue_DoubleClick;
    private readonly InputAction m_Dialogue_Scroll;
    public struct DialogueActions
    {
        private @InputMap m_Wrapper;
        public DialogueActions(@InputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @Click => m_Wrapper.m_Dialogue_Click;
        public InputAction @MousePosition => m_Wrapper.m_Dialogue_MousePosition;
        public InputAction @DoubleClick => m_Wrapper.m_Dialogue_DoubleClick;
        public InputAction @Scroll => m_Wrapper.m_Dialogue_Scroll;
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
                @DoubleClick.started -= m_Wrapper.m_DialogueActionsCallbackInterface.OnDoubleClick;
                @DoubleClick.performed -= m_Wrapper.m_DialogueActionsCallbackInterface.OnDoubleClick;
                @DoubleClick.canceled -= m_Wrapper.m_DialogueActionsCallbackInterface.OnDoubleClick;
                @Scroll.started -= m_Wrapper.m_DialogueActionsCallbackInterface.OnScroll;
                @Scroll.performed -= m_Wrapper.m_DialogueActionsCallbackInterface.OnScroll;
                @Scroll.canceled -= m_Wrapper.m_DialogueActionsCallbackInterface.OnScroll;
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
                @DoubleClick.started += instance.OnDoubleClick;
                @DoubleClick.performed += instance.OnDoubleClick;
                @DoubleClick.canceled += instance.OnDoubleClick;
                @Scroll.started += instance.OnScroll;
                @Scroll.performed += instance.OnScroll;
                @Scroll.canceled += instance.OnScroll;
            }
        }
    }
    public DialogueActions @Dialogue => new DialogueActions(this);
    public interface IPlayerActions
    {
        void OnWalkUD(InputAction.CallbackContext context);
        void OnWalkLR(InputAction.CallbackContext context);
        void OnTalk(InputAction.CallbackContext context);
        void OnTalkCompanion(InputAction.CallbackContext context);
        void OnEscape(InputAction.CallbackContext context);
    }
    public interface IDialogueActions
    {
        void OnClick(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnDoubleClick(InputAction.CallbackContext context);
        void OnScroll(InputAction.CallbackContext context);
    }
}
