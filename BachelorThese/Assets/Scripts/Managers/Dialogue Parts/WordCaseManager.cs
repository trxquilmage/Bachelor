using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class WordCaseManager : MonoBehaviour
{
    public static WordCaseManager instance;
    public GameObject wordReplacement;
    [SerializeField] Image background;
    [SerializeField] GameObject wordCaseUI;
    [HideInInspector]
    public WordInfo.WordTags openTag
    {
        get { return OpenTag; }
        set
        {
            OpenTag = value;
            OpenOnTag();
            UpdateWordCount();
        }
    }
    [HideInInspector] public bool overTrashcan;

    WordInfo.WordTags OpenTag;
    public Dictionary<WordInfo.WordTags, Word.WordData[]> tagRelatedWords;
    bool alreadyOpen;
    int tagAmount; // number of all tags except for "all" & "none"
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        tagRelatedWords = new Dictionary<WordInfo.WordTags, Word.WordData[]>();

        //initialize the dictionary of selcted words
        foreach (WordInfo.WordTags wordTags in (WordInfo.WordTags[])Enum.GetValues(typeof(WordInfo.WordTags)))
        {
            tagRelatedWords.Add(wordTags, new Word.WordData[ReferenceManager.instance.maxWordsPerTag]);
        }
        ColorButtonsCorrectly();
        tagAmount = tagRelatedWords.Count - 2; //minus "all" & "none"
    }
    /// <summary>
    /// Only for automatically opening the word case when draggin something in
    /// </summary>
    /// <param name="open"></param>
    public void AutomaticOpenCase(bool open)
    {
        // open the case
        if (open)
        {
            if (wordCaseUI.activeInHierarchy)
            {
                alreadyOpen = true;
            }
            wordCaseUI.SetActive(true);
        }
        //close the case
        else
        {
            if (!alreadyOpen)
            {
                wordCaseUI.SetActive(false);
            }
        }
    }
    /// <summary>
    /// for manually opening the case
    /// </summary>
    public void ManuallyOpenCase()
    {
        wordCaseUI.SetActive(!wordCaseUI.activeInHierarchy);
        openTag = WordInfo.WordTags.AllWords;
    }
    /// <summary>
    /// save a word dragged into the case in the Dict tagRelatedWords
    /// </summary>
    /// <param name="word"></param>
    public void SaveWord(Word wordItem)
    {
        Word.WordData word = wordItem.data;
        if (!CheckIfWordInList(word))
        {
            bool foundASpot = false;
            for (int i = 0; i < ReferenceManager.instance.maxWordsPerTag; i++)
            {
                if (tagRelatedWords[openTag][i].name == null)
                {
                    tagRelatedWords[openTag][i] = word;
                    foundASpot = true;
                    break;
                }
            }
            //Reload
            OpenOnTag();
            WordUtilities.ReColorAllInteractableWords();

            if (!foundASpot) //word list full, put the word back into the dialogue
            {
                WordUtilities.ReturnWordIntoText(wordItem);
            }
        }
        else
        {
            //put the word back into the dialogue
            WordUtilities.ReturnWordIntoText(wordItem);
        }
    }
    /// <summary>
    /// Loads the Word Case on the given tag. removes the previous words.
    /// </summary>
    public void OpenOnTag()
    {
        // Set Background Color to Tag Color + a bit grey
        Color color = WordUtilities.MatchColorToTag(openTag);
        color = new Color((color.r + 0.1f) > 1 ? 1 : (color.r + 0.1f),
            (color.g + 0.1f) > 1 ? 1 : (color.g + 0.1f),
            (color.b + 0.1f) > 1 ? 1 : (color.b + 0.1f), 1);
        background.color = color;
        ReferenceManager.instance.wordLimit.GetComponentInParent<Image>().color = color;
        // Unload Words of previous tag
        foreach (Transform word in ReferenceManager.instance.listingParent.GetComponentsInChildren<Transform>())
        {
            if (word.gameObject != ReferenceManager.instance.listingParent)
            {
                Destroy(word.gameObject);
            }
        }

        // Load words of current tag
        if (openTag != WordInfo.WordTags.AllWords) // go through ONE tag
        {
            foreach (Word.WordData word in tagRelatedWords[openTag])
            {
                if (word.name != null)
                {
                    GameObject bubble = WordUtilities.CreateWord(word, Vector2.zero, WordInfo.Origin.WordCase);
                    //place bubbles correctly (for protoype: below each other until i find out how the UI is supposed to work)
                    bubble.transform.SetParent(ReferenceManager.instance.listingParent.transform);
                }

            }
        }
        else // go through ALL tags
        {
            foreach (WordInfo.WordTags wordTags in (WordInfo.WordTags[])Enum.GetValues(typeof(WordInfo.WordTags)))
            {
                foreach (Word.WordData word in tagRelatedWords[wordTags])
                {
                    if (word.name != null)
                    {
                        SpawnWordInCase(word);
                    }
                }
            }
        }
        UpdateWordCount();
    }
    /// <summary>
    /// for UI buttons on the word case: change to the according case
    /// </summary>
    public void ChangeToTag(int i)
    {
        switch (i)
        {
            case 0:
                openTag = WordInfo.WordTags.AllWords;
                break;
            case 1:
                openTag = WordInfo.WordTags.Location;
                break;
            case 2:
                openTag = WordInfo.WordTags.General;
                break;
            case 3:
                openTag = WordInfo.WordTags.Name;
                break;
            default:
                Debug.Log("tag doesnt exist");
                break;
        }
    }
    /// <summary>
    /// Gives the correct colors to the tag-buttons
    /// </summary>
    void ColorButtonsCorrectly()
    {
        GameObject tagParent = ReferenceManager.instance.tagButtonParent;
        wordCaseUI.SetActive(true);
        Image[] buttons = tagParent.GetComponentsInChildren<Image>();
        buttons[0].color = ReferenceManager.instance.allColor;
        buttons[1].color = ReferenceManager.instance.locationColor;
        buttons[2].color = ReferenceManager.instance.generalColor;
        buttons[3].color = ReferenceManager.instance.nameColor;
        wordCaseUI.SetActive(false);
    }
    /// <summary>
    /// Put the word that was dragged out of the case back into the case
    /// </summary>
    public void PutWordBack(Word word)
    {
        word.transform.SetParent(ReferenceManager.instance.listingParent.transform);
    }
    /// <summary>
    /// Deletes the set word out of the word case
    /// </summary>
    /// <param name="data"></param>
    public void DeleteOutOfCase()
    {
        Word.WordData data = WordClickManager.instance.currentWord.GetComponent<Word>().data;
        int deleteInt = -1;
        int i = 0;
        foreach (Word.WordData word in tagRelatedWords[data.tag])
        {
            if (word.name == data.name)
            {
                deleteInt = i;
                break;
            }
            i++;
        }
        if (deleteInt > -1)
        {
            tagRelatedWords[data.tag][deleteInt] = new Word.WordData();
        }
        else
            Debug.Log("The word to delete couldne be found " + data.name);
    }
    /// <summary>
    /// Updates the wordcount text UI of the word case
    /// </summary>
    public void UpdateWordCount()
    {
        int wordCount = 0;
        if (openTag != WordInfo.WordTags.AllWords)
        {
            Word.WordData[] data = tagRelatedWords[openTag];
            foreach (Word.WordData word in data)
            {
                if (word.name != null)
                {
                    wordCount++;
                }
            }
            ReferenceManager.instance.wordLimit.text = wordCount.ToString() + "<b>/20</b>";
        }
        else
        {
            foreach (Word.WordData[] data in tagRelatedWords.Values)
            {
                foreach (Word.WordData word in data)
                {
                    if (word.name != null)
                    {
                        wordCount++;
                    }
                }
            }
            ReferenceManager.instance.wordLimit.text = wordCount.ToString() + "<b>/" + (ReferenceManager.instance.maxWordsPerTag * tagAmount).ToString() + "</b>";
        }
    }
    /// <summary>
    /// Checks if the given Word is in the word case already and doesnt need to be added
    /// </summary>
    /// <returns></returns>
    bool CheckIfWordInList(Word.WordData word)
    {
        bool inList = false;
        foreach (Word.WordData data in tagRelatedWords[word.tag])
        {
            if (data.name != null && data.name == word.name)
            {
                inList = true;
            }
        }
        return inList;
    }
    /// <summary>
    /// Create a less visible version of the word in the wordcase to inidate its position
    /// </summary>
    /// <param name="word"></param>
    public void SpawnWordReplacement(Word word)
    {
        wordReplacement = SpawnWordInCase(word.data);
        Color color = word.GetComponent<Image>().color;
        color.a = 0.3f;
        wordReplacement.GetComponent<Image>().color = color;
    }
    /// <summary>
    /// Destroy the currently active word replacement
    /// </summary>
    public void DestroyWordReplacement()
    {
        if (wordReplacement != null)
        {
            Destroy(wordReplacement);
            wordReplacement = null;
        }
    }
    /// <summary>
    /// create a word, that is to be placed in the word case
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public GameObject SpawnWordInCase(Word.WordData word)
    {
        GameObject bubble = WordUtilities.CreateWord(word, Vector2.zero, WordInfo.Origin.WordCase);
        //place bubbles correctly (for protoype: below each other until i find out how the UI is supposed to work)
        bubble.transform.SetParent(ReferenceManager.instance.listingParent.transform);
        return bubble;
    }
    /// <summary>
    /// throw a word out of the case
    /// </summary>
    public void TrashAWord()
    {
        DeleteOutOfCase();
        WordClickManager.instance.DestroyCurrentWord();
        UpdateWordCount();
    }
    /// <summary>
    /// Disable the ask and barter buttons (when in barter or question mode)
    /// </summary>
    /// <param name="setInactive"></param>
    public void DisableAskAndBarter(bool setInactive)
    {
        Color activeColor = ReferenceManager.instance.askColor;
        Color grey = ReferenceManager.instance.greyedOutColor;
        GameObject ask = ReferenceManager.instance.ask;;
        GameObject barter = ReferenceManager.instance.barter;
        if (setInactive) //turn off
        {
            ask.GetComponent<Button>().enabled = false;
            ask.GetComponent<Image>().color = Color.Lerp(activeColor, grey, 0.5f);
            barter.GetComponent<Button>().enabled = false;
            barter.GetComponent<Image>().color = Color.Lerp(activeColor, grey, 0.5f);
        }
        else //turn on
        {
            ask.GetComponent<Button>().enabled = true;
            ask.GetComponent<Image>().color = activeColor;
            barter.GetComponent<Button>().enabled = true;
            barter.GetComponent<Image>().color = activeColor;
        }
    }
}
