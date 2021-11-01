using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class ReferenceManager : MonoBehaviour
{
    // Manager for general Object References

    public static ReferenceManager instance;

    public Colors colors;
    //public TextColors textColors;
    [HideInInspector] public Color normalColor;
    [HideInInspector] public Color interactableColor;
    [HideInInspector] public Color interactedColor;
    //public TagColors tagColors;
    [HideInInspector] public Color locationColor;
    [HideInInspector] public Color generalColor;
    [HideInInspector] public Color nameColor;
    [HideInInspector] public Color itemColor;
    [HideInInspector] public Color allColor;
    //public OtherColors otherColors;
    [HideInInspector] public Color shadowButtonColor; //Color that mixes into Button Shadows
    [HideInInspector] public Color askColor; //Color for ask & barter button (not set on start rn)
    [HideInInspector] public Color greyedOutColor; //greyed out color for ask & barter button
    [HideInInspector] public Color textFieldColor;
    [HideInInspector] public Color inputFieldColor;
    [HideInInspector] public Color nameFieldColor;
    [HideInInspector] public Color interactableButtonColor;
    [Header("Parents")]
    public GameObject selectedWordParent;
    public GameObject selectedWordParentAsk;
    public GameObject listingParent;
    public GameObject promptBubbleParent;
    public GameObject askPromptBubbleParent;
    public GameObject tagButtonParent;
    public GameObject npcParent;
    [Header("Dialogue Runner")]
    public DialogueRunner standartRunner;
    public DialogueUI standartDialogueUI;
    public DialogueRunner askRunner;
    public DialogueUI askDialogueUI;
    [Header("Prefabs")]
    public GameObject selectedWordPrefab;
    public GameObject promptBoxPrefab;
    [Header("Fields")]
    public GameObject nPCDialogueField;
    public GameObject playerInputField;
    public GameObject wordCase;
    public GameObject askField;
    public GameObject askNPCField;
    public Canvas dialogueCanvas;
    [Header("UI Elements")]
    public GameObject trashCan;
    public TMP_Text wordLimit;
    public GameObject ask;
    public GameObject barter;
    public GameObject wButton;
    public GameObject askContinueButton;
    public GameObject continueButton;
    public GameObject askButton;
    public TMP_Text[] interactableTextList;
    public Scrollbar buttonScrollbar;
    public Scrollbar bubbleScrollbar;
    public GameObject nameField;
    [Header("Dialogues")]
    public DialogueRunner runner;
    [Header("Images")]
    public Sprite trashCanImage01;
    public Sprite trashCanImage02;
    [Header("Prompt Related")]
    public TextMeshProUGUI promptQuestion;
    public TextMeshProUGUI promptAnswer;
    public GameObject promptMenu;
    public TMP_Text askPrompt;
    public TMP_Text askQuestion;
    public TMP_Text askNPCText;
    [Header("Non-Dialogue UI")]
    public Canvas canvas;
    public Image eButtonSprite;
    public GameObject worldMap;
    [Header("Environment")]
    public Transform[] allInteractableObjects;
    [Header("Game Data")]
    public int maxWordsPerTag;
    public int tagScrollbarDistance = 10;
    public int bubbleScrollbarDistance = 10;
    public int scrollbarSizeOnCanvas = 6;
    public int scrollbarMaxSize = 60;

    [HideInInspector] public float currScrollbarDistance = 0;
    void Awake()
    {
        instance = this;
        ReadColors();
    }
    /// <summary>
    /// Read out the color schemes from the current scriptable objects
    /// </summary>
    void ReadColors()
    {
        // Text Colors
        normalColor = colors.normalColor;
        interactableColor = colors.interactableColor;
        interactedColor = colors.interactedColor;
        // tag Colors
        locationColor = colors.locationColor;
        generalColor = colors.generalColor;
        nameColor = colors.nameColor;
        itemColor = colors.itemColor;
        allColor = colors.allColor;
        //other colors
        shadowButtonColor = colors.shadowButtonColor;
        askColor = colors.askColor;
        greyedOutColor = colors.greyedOutColor;
        textFieldColor = colors.textFieldColor;
        inputFieldColor = colors.inputFieldColor;
        nameFieldColor = colors.nameFieldColor;
        interactableButtonColor = colors.interactableButtonsColor;
    }
}
