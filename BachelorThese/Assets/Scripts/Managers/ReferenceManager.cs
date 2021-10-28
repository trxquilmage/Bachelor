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

    [Header("Text Colors")]
    public Color normalColor;
    public Color interactableColor;
    public Color interactedColor;
    [Header("Tag Colors")]
    public Color locationColor;
    public Color generalColor;
    public Color nameColor;
    public Color allColor;
    [Header("Other Colors")]
    public Color shadowButtonColor; //Color that mixes into Button Shadows
    public Color askColor;
    public Color greyedOutColor;

    [Header("Parents")]
    public GameObject selectedWordParent;
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
    public Canvas dialogueCanvas;
    [Header("UI Elements")]
    public GameObject trashCan;
    public TMP_Text wordLimit;
    public GameObject ask;
    public GameObject barter;
    public GameObject askContinueButton;
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
    [Header("Game Data")]
    public int maxWordsPerTag;
    void Awake()
    {
        instance = this;
    }
}
