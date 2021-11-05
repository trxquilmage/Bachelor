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
    public void CheckWord(string sentWord, Vector2 wordPos, TMP_WordInfo wordInfo, Vector2 firstAndLastWordIndex, WordInfo.Origin origin)
    {
        //check if the sent word is actually in the keyword list
        if (wlReader.wordTag.ContainsKey(sentWord))
        {
            // Create Word Data to send
            Word.WordData data = new Word.WordData()
            {
                name = sentWord,
                tag = WordUtilities.StringToTag(wlReader.wordTag[sentWord][0]),
                tagInfo = wlReader.wordTag[sentWord]
            };
            if (wordLastHighlighted != null)
                DestroyLastHighlighted();

            wordLastHighlighted = WordUtilities.CreateWord(data, wordPos, wordInfo, firstAndLastWordIndex, origin, false);
            if (wordLastHighlighted != null)
                AddToArray(activeWords, wordLastHighlighted);
        }
        else if (wlReader.longWordTag.ContainsKey(sentWord))
        {
            // Create Word Data to send
            Word.WordData data = new Word.WordData()
            {
                name = sentWord,
                tag = WordUtilities.StringToTag(wlReader.longWordTag[sentWord][0]),
                tagInfo = wlReader.longWordTag[sentWord]
            };
            if (wordLastHighlighted != null)
                DestroyLastHighlighted();
            wordLastHighlighted = WordUtilities.CreateWord(data, wordPos, wordInfo, firstAndLastWordIndex, origin, true);
            if (wordLastHighlighted != null)
                AddToArray(activeWords, wordLastHighlighted);
        }
        else if (wlReader.fillerTag.ContainsKey(sentWord))
        {
            // Create Word Data to send
            Word.WordData data = new Word.WordData()
            {
                name = sentWord,
                tag = WordUtilities.StringToTag(wlReader.fillerTag[sentWord][0]),
                tagInfo = wlReader.fillerTag[sentWord]
            };
            if (wordLastHighlighted != null)
                DestroyLastHighlighted();
            wordLastHighlighted = WordUtilities.CreateWord(data, wordPos, wordInfo, firstAndLastWordIndex, origin, false);
            if (wordLastHighlighted != null)
                AddToArray(activeWords, wordLastHighlighted);
        }
        else // is filler word without entry
        {
            // Create Word Data to send
            Word.WordData data = new Word.WordData()
            {
                name = sentWord,
                tag = WordInfo.WordTags.Other,
                tagInfo = new string[] { "Other", "wrongInfo" }
            };
            if (wordLastHighlighted != null)
                DestroyLastHighlighted();
            wordLastHighlighted = WordUtilities.CreateWord(data, wordPos, wordInfo, firstAndLastWordIndex, origin, false);
            if (wordLastHighlighted != null)
                AddToArray(activeWords, wordLastHighlighted);
        }
    }

    /// <summary>
    /// Destroy the word that is currently selected by the mouse
    /// </summary>
    public void DestroyCurrentWord()
    {
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
            //over the trashcan
            if (uIObject.gameObject == ReferenceManager.instance.trashCan
                || uIObject.gameObject == ReferenceManager.instance.questTrashCan)
            {
                foundtC = true;
                mouseOverUIObject = "trashCan";
                UIManager.instance.SwitchTrashImage(true, uIObject.gameObject);
            }
            //over the wordcase
            else if (uIObject.gameObject == ReferenceManager.instance.wordCase)
                mouseOverUIObject = "wordCase";
            //over the questlog
            else if (uIObject.gameObject == ReferenceManager.instance.questCase)
                mouseOverUIObject = "questLog";
            //over a promptbubble
            else if (uIObject.gameObject.TryGetComponent<PromptBubble>(out PromptBubble pB))
            {
                promptBubble = pB;
                foundPB = true;
                mouseOverUIObject = "playerInput";
                promptBubble.OnBubbleHover(true);
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
                UIManager.instance.SwitchTrashImage(false, null);
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
                    FindWordsHoveredOver(text, eventDataCurrentPosition);
                }
            }
        }
        if (!stillOnWord && currentWord == null
            && wordLastHighlighted != null) //stopped Hovering over the iteractable word
        {
            DestroyLastHighlighted();
        }
    }
    /// <summary>
    /// Checks the text for colored words, finds the relating ones and creates a bubble
    /// </summary>
    /// <param name="text"></param>
    /// <param name="eventData"></param>
    void FindWordsHoveredOver(TMP_Text text, PointerEventData eventData)
    {
        int wordIndex = TMP_TextUtilities.FindIntersectingWord(text, eventData.position, eventData.enterEventCamera);
        if (wordIndex != -1) //the function above gives out -1 if they find nothing
        {
            TMP_WordInfo wordInfo = text.textInfo.wordInfo[wordIndex];
            TMP_CharacterInfo charInfo = text.textInfo.characterInfo[wordInfo.firstCharacterIndex];

            //Get Color of the first character of the word
            Color32[] currentCharacterColor = text.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
            if (currentCharacterColor[charInfo.vertexIndex] == ReferenceManager.instance.interactableColor)
            {
                TMP_WordInfo[] wordInfos = FindAllWordsToBubble(text, wordIndex);
                WordUtilities.CreateABubble(text, wordInfos); //Word info of the START word, not the hovered word
            }

            //if the mouse is STILL over the created bubble, dont delete it this round
            if (wordLastHighlighted != null)
            {
                //go through the signle words of a (possibly longer) word
                foreach (string word in wordLastHighlighted.GetComponentInChildren<Word>().data.name.Trim().Split(" "[0]))
                {
                    //if ANY of the words in the bubble are the word we are currently hovering over, dont destroy
                    if (word == wordInfo.GetWord())
                        stillOnWord = true;
                }
            }
        }
    }
    /// <summary>
    /// Takes one given wordInfo and finds all words that are part of that word into an TMP_WordInfo[]
    /// </summary>
    /// <param name="text"></param>
    /// <param name="wordInfoIndex"></param>
    /// <returns></returns>
    TMP_WordInfo[] FindAllWordsToBubble(TMP_Text text, int wordInfoIndex)
    {
        List<TMP_WordInfo[]> wordInfosList = new List<TMP_WordInfo[]>();
        //Check if the word is in the list of important single words
        if (WordLookupReader.instance.CheckForWord(text.textInfo.wordInfo[wordInfoIndex], out TMP_WordInfo[] wordInfos, out bool isFiller) && !isFiller)
        {
            return wordInfos;
        }

        //Check if the word is in the list of important long words
        //go from wordIndex - x to wordIndex + x
        int startIndex = wordInfoIndex - ReferenceManager.instance.maxLongWordLength;
        int endIndex = wordInfoIndex + ReferenceManager.instance.maxLongWordLength;
        for (int i = startIndex < 0 ? 0 : startIndex;
            i < (endIndex > text.textInfo.wordCount ? text.textInfo.wordCount : endIndex); i++)
        {
            if (WordLookupReader.instance.CheckForWord(text.textInfo.wordInfo[i], out wordInfos, out isFiller))
            {
                wordInfosList.Add(wordInfos);
            }
        }
        foreach (TMP_WordInfo[] infos in wordInfosList) // this should prevent single words in the area to accidentally get added
        {
            if (infos.Length > 1)
            {
                foreach (TMP_WordInfo info in infos) // is the word contained in this word list or did it by coincidence catch another word list?
                {
                    if (info.GetWord() == text.textInfo.wordInfo[wordInfoIndex].GetWord())
                    {
                        return infos;
                    }
                }
            }
        }
        //else
        if (WordLookupReader.instance.CheckForWord(text.textInfo.wordInfo[wordInfoIndex], out wordInfos, out isFiller) && isFiller)
        {
            return wordInfos;
        }

        Debug.Log("The hovered word couldnt be found.");
        return null;
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
