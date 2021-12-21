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
    [HideInInspector] public Color inListColor;
    //public OtherColors otherColors;
    [HideInInspector] public Color shadowButtonColor; //Color that mixes into Button Shadows
    [HideInInspector] public Color askColor; //Color for ask & barter button (not set on start rn)
    [HideInInspector] public Color greyedOutColor; //greyed out color for ask & barter button
    [HideInInspector] public Color textFieldColor;
    [HideInInspector] public Color inputFieldColor;
    [HideInInspector] public Color nameFieldColor;
    [HideInInspector] public Color interactableButtonColor;
    [HideInInspector] public Color limitColor;
    [HideInInspector] public Color headerColor;
    [HideInInspector] public Color trashColor;
    [HideInInspector] public Color highlightColor;
    [Header("Parents")]
    public GameObject selectedWordParent;
    public GameObject selectedWordParentAsk;
    public GameObject listingParent;
    public GameObject questListingParent;
    public GameObject promptBubbleParent;
    public GameObject askPromptBubbleParent;
    public GameObject tagButtonParent;
    public GameObject npcParent;
    public GameObject otherUIParent;
    [Header("Dialogue Runner")]
    public DialogueRunner standartRunner;
    public DialogueUI standartDialogueUI;
    public DialogueRunner askRunner;
    public DialogueUI askDialogueUI;
    [Header("Prefabs")]
    public GameObject selectedWordPrefab;
    public GameObject questBubblePrefab;
    public GameObject promptBoxPrefab;
    public GameObject buttonPrefab;
    public GameObject questCountPrefab;
    public GameObject dropDownPrefab;
    public GameObject starPrefab;
    [Header("Fields")]
    public GameObject nPCDialogueField;
    public GameObject playerInputField;
    public GameObject wordCase;
    public GameObject questCase;
    public GameObject askField;
    public GameObject askNPCField;
    public GameObject menuField;
    public GameObject tutorialField;
    public Canvas dialogueCanvas;
    [Header("UI Elements")]
    public GameObject trashCan;
    public GameObject questTrashCan;
    public TMP_Text wordLimit;
    public TMP_Text questLimit;
    public Image questJournal;
    public Image wordJournal;
    public GameObject ask;
    public GameObject abortAsk;
    public GameObject barter;
    public GameObject askContinueButton;
    public GameObject continueButton;
    public GameObject iCantSayButton;
    public GameObject askICantSayButton;
    public GameObject askButton;
    public GameObject feedbackTextOtherTag;
    public GameObject feedbackTextTrashYesNo;
    public Scrollbar buttonScrollbar;
    public Scrollbar bubbleScrollbar;
    public Scrollbar questScrollbar;
    public GameObject nameField;
    public GameObject npcDialogueTextBox;
    public GameObject npcDialogueTextBoxAsk;
    [Header("StartWords")]
    public GameObject mainStartWordParent;
    public GameObject questionParent;
    public GameObject traitParent;
    public GameObject generatorParent;
    public Image questionBackground;
    public Image traitBackground;
    public Image generatorBackground;
    public Image questionContinueButton;
    public Image traitContinueButton;
    public Image generatorContinueButton;
    public TMP_Text showWordsText;
    public List<string> startWords;
    public List<string> startCommands;
    [Header("TEXTS")]
    public TMP_Text[] interactableTextList;
    public int npcTextIndex = 0;
    public int characterNameIndex = 1;
    public int npcTextAskIndex = 2;
    [Header("UI Elements (Only For Color)")]
    public GameObject wButton;
    public GameObject qButton;
    [Header("Dialogues")]
    public DialogueRunner runner;
    [Header("Images")]
    public Sprite trashCanImage01;
    public Sprite trashCanImage02;
    [Header("Prompt Related")]
    public TextMeshProUGUI promptAnswer;
    public GameObject promptMenu;
    public TMP_Text askPrompt;
    public TMP_Text askNPCText;
    [Header("Non-Dialogue UI")]
    public Canvas canvas;
    public Image eButtonSprite;
    public Image fButtonSprite;
    public Image rightClickIcon;
    public Image rightClickAskIcon;
    public GameObject worldMap;
    [Header("Environment")]
    public GameObject player;
    public Transform[] allInteractableObjects;
    [Header("Game Data")]
    public int maxWordsPerTag;
    public int questScrollbarDistance = 10;
    public int spaceForQuestsOnCanvas = 7;
    public int bubbleScreenHeightMaxSize = 60;
    public int maxLongWordLength = 3;
    public int maxQuestCount = 5;
    public int maxQuestAdditions = 5;
    public int interactionRadius = 5;
    public int maxStartWordTraitAmount = 5;
    public string sceneNumber = "001";
    [Header("Game Options")]
    public bool allWordsInteractable = false;
    public bool blockListOn = false;
    public bool noGreyOut = false;
    public bool duplicateWords = false;
    public bool startWithStartWords = false;
    [Header("Tags")]
    public WordInfo.WordTag[] wordTags;
    public int allTagIndex = 0;
    public int questTagIndex = 2;
    public int otherTagIndex = 3;
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
        inListColor = colors.inListColor;
        // tag Colors
        for(int i = 0; i < wordTags.Length; i++)
        {
            wordTags[i].tagColor = colors.generalTagColors[i];
        }
        //other colors
        shadowButtonColor = colors.shadowButtonColor;
        askColor = colors.askColor;
        greyedOutColor = colors.greyedOutColor;
        textFieldColor = colors.textFieldColor;
        inputFieldColor = colors.inputFieldColor;
        nameFieldColor = colors.nameFieldColor;
        interactableButtonColor = colors.interactableButtonsColor;
        limitColor = colors.limitColor;
        headerColor = colors.headerColor;
        trashColor = colors.trashColor;
        highlightColor = colors.highlightColor;
    }
}
