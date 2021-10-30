using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class WordClickManager : MonoBehaviour
{
    // Manages the currently clicked/dragged word

    public static WordClickManager instance;
    public GameObject wordLastHighlighted;
    public GameObject currentWord
    {
        get
        {
            return CurrentWord;
        }
        set
        {
            CurrentWord = value;
            if (value == null)
                DialogueInputManager.instance.continueEnabledDrag = true;
            else
                DialogueInputManager.instance.continueEnabledDrag = false;
        }
    }
    public GameObject CurrentWord;
    WordLookupReader wlReader;
    GameObject[] activeWords = new GameObject[20];
    public string mouseOverUIObject;
    InputMap controls;
    public PromptBubble promptBubble;
    bool stillOnWord;

    void Awake()
    {
        instance = this;
        controls = new InputMap();
    }
    private void Start()
    {
        wlReader = WordLookupReader.instance;
        
    }
    private void Update()
    {
        GetMouseOverUI();
    }
    /// <summary>
    /// Ckeck if the word is actually in the keywords list, then skip to WordUtilities.CreateWord()
    /// </summary>
    /// <param name="sentWord"></param>
    /// <param name="wordPos"></param>
    public void CheckWord(string sentWord, Vector2 wordPos, TMP_WordInfo wordInfo)
    {
        //check if the sent word is actually in the keyword list
        if (wlReader.wordTag.ContainsKey(sentWord))
        {
            // Create Word Data to send
            Word.WordData data = new Word.WordData();
            data.name = sentWord;
            data.tag = WordUtilities.StringToTag(wlReader.wordTag[sentWord][0]);
            data.tagInfo = wlReader.wordTag[sentWord];
            wordLastHighlighted = WordUtilities.CreateWord(data, wordPos, wordInfo, WordInfo.Origin.Dialogue);
            AddToArray(activeWords, wordLastHighlighted);
        }
    }
    /// <summary>
    /// Destroy the word that is currently selected by the mouse
    /// </summary>
    public void DestroyCurrentWord()
    {
        TMP_Text text = currentWord.GetComponent<Word>().relatedText;
        // Destroy the Word
        Destroy(currentWord);
        currentWord = null;
        // Set the word color back to interactable
        WordUtilities.ReColorAllInteractableWords();
    }
    /// <summary>
    /// Destroy the word you highlighted (after it is no longer hovered)
    /// </summary>
    public void DestroyLastHighlighted()
    {
        TMP_Text text = wordLastHighlighted.GetComponent<Word>().relatedText;
        // Destroy the Word
        Destroy(wordLastHighlighted);
        wordLastHighlighted = null;

        // Set the word color back to interactable
        WordUtilities.ReColorAllInteractableWords();
    }
    /// <summary>
    /// Destroy the buttons that have spawned this dialogue
    /// </summary>
    public void DestroyAllActiveWords()
    {
        if (activeWords.Length != 0)
        {
            for (int i = activeWords.Length - 1; i > 0; i--)
            {
                if (activeWords[i] != null)
                {
                    Destroy(activeWords[i]);
                    activeWords[i] = null;
                }
            }
        }
    }
    /// <summary>
    /// Goes through the array and places the Object in the first free Spot
    /// </summary>
    void AddToArray(GameObject[] array, GameObject toAdd)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
            {
                array[i] = toAdd;
            }
        }
    }
    /// <summary>
    /// Raycasts for the UI Elements, checks, above which Category of UI Element 
    /// the mouse is currently Hovering over & saves it to mouseOverUIObject
    /// Then Checks the exact word the UI is hovering over & Creates a button
    /// </summary>
    public void GetMouseOverUI()
    {
        //initialization & Raycast For UI Elements
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = GetMousePos();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        //Check above which Category of UI Element it is currently Hovering
        mouseOverUIObject = "none";
        bool foundPB = false; //found a prompt bubble
        bool foundtC = false; //found the trash can
        foreach (RaycastResult uIObject in results)
        {
            //over the wordcase
            if (uIObject.gameObject == ReferenceManager.instance.wordCase)
                mouseOverUIObject = "wordCase";
            //over a promptbubble
            else if (uIObject.gameObject.TryGetComponent<PromptBubble>(out PromptBubble pB))
            {
                promptBubble = pB;
                foundPB = true;
                mouseOverUIObject = "playerInput";
                promptBubble.OnBubbleHover(true);
            }
            //over the trashcan
            else if (uIObject.gameObject == ReferenceManager.instance.trashCan)
            {
                foundtC = true;
                mouseOverUIObject = "trashCan";
                UIManager.instance.SwitchTrashImage(true);
            }
        }
        if (!foundPB) //if not hovering over prompt
        {
            if (promptBubble != null)
            {
                promptBubble.OnBubbleHover(false);
                promptBubble = null;
            }
        }
        if (!foundtC) //if not over trashCan
        {
            {
                UIManager.instance.SwitchTrashImage(false);
            }
        }
        stillOnWord = false;
        //Check for the exact word the mouse is hovering over
        foreach (RaycastResult uIObject in results)
        {
            
            foreach (TMP_Text text in ReferenceManager.instance.interactableTextList)
            {
                
                //if the mouse is currently over an Interactable text
                if (uIObject.gameObject == text.gameObject)
                {
                    int wordIndex = TMP_TextUtilities.FindIntersectingWord(text, eventDataCurrentPosition.position, eventDataCurrentPosition.enterEventCamera);
                    if (wordIndex != -1)
                    {
                        TMP_WordInfo wordInfo = text.textInfo.wordInfo[wordIndex];
                        TMP_CharacterInfo charInfo = text.textInfo.characterInfo[wordInfo.firstCharacterIndex];


                        //Get Color of the first character of the word
                        Color32[] currentCharacterColor = text.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
                        if (currentCharacterColor[charInfo.vertexIndex] == ReferenceManager.instance.interactableColor)
                        {
                            WordUtilities.CreateABubble(text, wordInfo);
                        }
                        //if the mouse is STILL over the created bubble, dont delete it this round
                        if (WordClickManager.instance.wordLastHighlighted != null &&
                            WordClickManager.instance.wordLastHighlighted.GetComponentInChildren<TMP_Text>().text == wordInfo.GetWord())
                        {
                            stillOnWord = true;
                        }
                    }
                }
            }
        }
        if (!stillOnWord && WordClickManager.instance.currentWord == null
            && WordClickManager.instance.wordLastHighlighted != null) //stopped Hovering over the iteractable word
        {
            WordClickManager.instance.DestroyLastHighlighted();
        }
    }
    /// <summary>
    /// Return the mouse position in screen space
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMousePos()
    {
        Vector2 mousePos = controls.Dialogue.MousePosition.ReadValue<Vector2>();
        return mousePos;
    }
    void OnEnable()
    {
        controls.Enable();
    }
    void OnDisable()
    {
        controls.Disable();
    }
}
