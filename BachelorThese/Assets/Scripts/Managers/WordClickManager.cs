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
    [HideInInspector] public GameObject wordLastHighlighted;
    [HideInInspector]
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
    [HideInInspector] public GameObject CurrentWord;
    WordLookupReader wlReader;
    [HideInInspector] public GameObject[] activeWords = new GameObject[20];
    [HideInInspector]
    public string mouseOverUIObject
    {
        get { return MouseOverUIObject; }
        set
        {
            // wasn't over promptbubble and now is
            if (MouseOverUIObject != "playerInput" && value == "playerInput")
            {
                CheckPromptBubbleForCurrentWord(lastSavedPromptBubble);
            }
            // was over prompt bubble and now isn't
            else if (MouseOverUIObject == "playerInput" && value != "playerInput")
            {
                if (promptBubble != null)
                {
                    promptBubble.OnBubbleHover(false);
                    promptBubble = null;
                }
            }
            // wasn't over trash can and now is
            if (MouseOverUIObject != "trashCan" && value == "trashCan")
            {
                UIManager.instance.SwitchTrashImage(true, lastSavedTrashCan);
            }
            // was over trash can and now isn't
            else if (MouseOverUIObject == "trashCan" && value != "trashCan")
            {
                UIManager.instance.SwitchTrashImage(false, lastSavedTrashCan);
            }
            MouseOverUIObject = value;
        }
    }
    string MouseOverUIObject;
    InputMap controls;
    [HideInInspector] public PromptBubble promptBubble;
    [HideInInspector] public PromptBubble lastSavedPromptBubble;
    [HideInInspector] public GameObject lastSavedTrashCan;
    bool stillOnWord;

    ReferenceManager refM;

    void Awake()
    {
        instance = this;
        controls = new InputMap();
    }
    private void Start()
    {
        wlReader = WordLookupReader.instance;
        refM = ReferenceManager.instance;
        controls.Dialogue.Scroll.performed += context => WordCaseManager.instance.ChangeScrollbarValue(context.ReadValue<float>());
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
    public void CheckWord(string sentWord, Vector3 wordPos, TMP_WordInfo wordInfo, Vector2 firstAndLastWordIndex, WordInfo.Origin origin)
    {
        //check if the sent word is actually in the keyword list
        if (wlReader.wordTag.ContainsKey(sentWord))
        {
            // Create Bubble Data to send
            BubbleData data = new BubbleData()
            {
                name = sentWord,
                tag = wlReader.wordTag[sentWord][0],
                subtag = wlReader.wordTag[sentWord][1],
                tagInfo = wlReader.wordTag[sentWord]
            };

            DestroyLastHighlighted();

            wordLastHighlighted = WordUtilities.CreateWord(data, wordPos, wordInfo, firstAndLastWordIndex, origin, false);
            if (wordLastHighlighted != null)
                WordUtilities.AddToArray(activeWords, wordLastHighlighted);
        }
        else if (wlReader.longWordTag.ContainsKey(sentWord))
        {
            // Create Word Data to send
            BubbleData data = new BubbleData()
            {
                name = sentWord,
                tag = wlReader.longWordTag[sentWord][0],
                subtag = wlReader.longWordTag[sentWord][1],
                tagInfo = wlReader.longWordTag[sentWord]
            };
            if (wordLastHighlighted != null)
                DestroyLastHighlighted();
            wordLastHighlighted = WordUtilities.CreateWord(data, wordPos, wordInfo, firstAndLastWordIndex, origin, false);
            if (wordLastHighlighted != null)
                WordUtilities.AddToArray(activeWords, wordLastHighlighted);
        }
        else if (wlReader.fillerTag.ContainsKey(sentWord))
        {
            // Create Word Data to send
            BubbleData data = new BubbleData()
            {
                name = sentWord,
                tag = wlReader.fillerTag[sentWord][0],
                subtag = wlReader.fillerTag[sentWord][1],
                tagInfo = wlReader.fillerTag[sentWord]
            };
            DestroyLastHighlighted();
            wordLastHighlighted = WordUtilities.CreateWord(data, wordPos, wordInfo, firstAndLastWordIndex, origin, false);
            if (wordLastHighlighted != null)
                WordUtilities.AddToArray(activeWords, wordLastHighlighted);
        }
        else // is filler word without entry
        {
            // Create Word Data to send
            BubbleData data = new BubbleData()
            {
                name = sentWord,
                tag = refM.wordTags[refM.otherTagIndex].name,
                subtag = "OtherA",
                tagInfo = new string[] { ReferenceManager.instance.wordTags[ReferenceManager.instance.otherTagIndex].name, "wrongInfo" }
            };
            DestroyLastHighlighted();
            wordLastHighlighted = WordUtilities.CreateWord(data, wordPos, wordInfo, firstAndLastWordIndex, origin, false);
            if (wordLastHighlighted != null)
                WordUtilities.AddToArray(activeWords, wordLastHighlighted);
        }
    }

    /// <summary>
    /// Destroy the word that is currently selected by the mouse
    /// </summary>
    public void DestroyCurrentWord()
    {
        if (currentWord != null)
        {
            // Destroy the Word
            Destroy(currentWord);
            currentWord = null;
            // Set the word color back to interactable
            EffectUtilities.ReColorAllInteractableWords();
        }
    }
    /// <summary>
    /// this additionally checks, if the word IS the current word and if not, just deletes this word instead
    /// </summary>
    /// <param name="wordForCheck"></param>
    public void DestroyCurrentWord(Bubble wordForCheck)
    {
        if (wordForCheck.gameObject == currentWord && currentWord != null)
        {
            // Destroy the Word
            Destroy(currentWord);
            currentWord = null;
            // Set the word color back to interactable
            EffectUtilities.ReColorAllInteractableWords();
        }
        else
        {
            Destroy(wordForCheck.gameObject);
            EffectUtilities.ReColorAllInteractableWords();
        }
    }
    /// <summary>
    /// Destroy the word you highlighted (after it is no longer hovered)
    /// </summary>
    public void DestroyLastHighlighted()
    {
        if (wordLastHighlighted != null)
        {
            // Destroy the Word
            Destroy(wordLastHighlighted);
            wordLastHighlighted = null;

            // Set the word color back to interactable
            EffectUtilities.ReColorAllInteractableWords();
        }
    }
    public void SwitchFromHighlightedToCurrent()
    {
        DestroyCurrentWord();
        currentWord = wordLastHighlighted;
        wordLastHighlighted = null;
    }
    public void SwitchFromCurrentToHighlight()
    {
        DestroyLastHighlighted();
        wordLastHighlighted = currentWord;
        currentWord = null;
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
    public void RemoveFromArray(GameObject[] array, GameObject toRemove)
    {
        if (array.Contains(toRemove))
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == toRemove)
                    array[i] = null;
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
        string currentlyOver = "none";
        foreach (RaycastResult uIObject in results)
        {
            //over the trashcan
            if (uIObject.gameObject == ReferenceManager.instance.trashCan)
            {
                lastSavedTrashCan = uIObject.gameObject;
                currentlyOver = "trashCan";
            }
            //over the wordcase
            else if (uIObject.gameObject == ReferenceManager.instance.wordJournal.gameObject && currentlyOver == "none")
            {
                currentlyOver = "wordCase";
            }
            //over a promptbubble
            else if (uIObject.gameObject.TryGetComponent<PromptBubble>(out PromptBubble pB))
            {
                lastSavedPromptBubble = pB;
                currentlyOver = "playerInput";
            }
        }
        mouseOverUIObject = currentlyOver;
        stillOnWord = false;

        //Check for the exact word the mouse is hovering over
        //dont spawn words, if you are currently dragging one
        if (currentWord == null || !currentWord.GetComponent<Bubble>().wasDragged)
        {
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
    }
    /// <summary>
    /// Checks the text for colored words, finds the relating ones and creates a bubble
    /// </summary>
    /// <param name="text"></param>
    /// <param name="eventData"></param>
    void FindWordsHoveredOver(TMP_Text text, PointerEventData eventData)
    {
        int wordIndex;
        bool overlayCanvas = text.gameObject.tag == "TextOnOverlayCanvas";

        Camera cameraMain = Camera.main;
        if (overlayCanvas)
            cameraMain = eventData.enterEventCamera;

        wordIndex = TMP_TextUtilities.FindIntersectingWord(text, eventData.position, cameraMain);

        if (wordIndex != -1) //the function above gives out -1 if they find nothing
        {
            TMP_WordInfo wordInfo = text.textInfo.wordInfo[wordIndex];
            TMP_CharacterInfo charInfo = text.textInfo.characterInfo[wordInfo.firstCharacterIndex];

            //Get Color of the first character of the word
            //if its interactable OR inList, you can instantiate a word from it (not from interactedColor though)
            Color32[] currentCharacterColor = text.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
            if (EffectUtilities.CompareColor32(currentCharacterColor[charInfo.vertexIndex], (Color32)ReferenceManager.instance.interactableColor) ||
                EffectUtilities.CompareColor32(currentCharacterColor[charInfo.vertexIndex], (Color32)ReferenceManager.instance.inListColor))
            {
                TMP_WordInfo[] wordInfos = FindAllWordsToBubble(text, wordIndex);
                WordUtilities.CreateABubble(text, wordInfos); //Word info of the START word, not the hovered word
            }

            //if the mouse is STILL over the created bubble, dont delete it this round
            if (wordLastHighlighted != null)
            {
                //go through the signle words of a (possibly longer) word
                foreach (string word in wordLastHighlighted.GetComponentInChildren<Bubble>().data.name.Trim().Split(" "[0]))
                {
                    //if ANY of the words in the bubble are the word we are currently hovering over, dont destroy
                    if (word == WordUtilities.CapitalizeAllWordsInString(wordInfo.GetWord()))
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
        foreach (TMP_WordInfo[] infos in wordInfosList)
        {
            foreach (TMP_WordInfo info in infos) // is the word contained in this word list or did it by coincidence catch another word list?
            {
                if (info.firstCharacterIndex == text.textInfo.wordInfo[wordInfoIndex].firstCharacterIndex)
                {
                    return infos;
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
    /// <summary>
    /// takes the current word, (pretends as) if the current word is hovering over it, checks if it fits
    /// </summary>
    public void CheckPromptBubbleForCurrentWord(PromptBubble pB)
    {
        promptBubble = pB;
        promptBubble.OnBubbleHover(true);
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
