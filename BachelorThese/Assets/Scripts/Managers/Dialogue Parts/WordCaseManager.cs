using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class WordCaseManager : MonoBehaviour
{
    //Handles everything happening in the word case
    public static WordCaseManager instance;
    public GameObject wordReplacement;
    public int allTagIndex = 0;
    public int otherTagIndex = 3;
    [SerializeField] Image background;
    [HideInInspector]
    public string openTag
    {
        get { return OpenTag; }
        set
        {
            if (OpenTag == value)
            {
                OpenTag = value;
                OpenOnTag(false);
            }
            else
            {
                OpenTag = value;
                OpenOnTag(true);
            }
        }
    }
    //[HideInInspector] public bool overTrashcan;

    string OpenTag;
    ReferenceManager refM;
    public Image[] buttons;
    public Dictionary<string, Word.WordData[]> tagRelatedWords;
    bool alreadyOpen;
    int tagAmount; // number of all tags except for "all" & "quests"
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        refM = ReferenceManager.instance;
        tagRelatedWords = new Dictionary<string, Word.WordData[]>();
        openTag = refM.wordTags[allTagIndex].name;

        //initialize the dictionary of selcted words
        foreach (WordInfo.WordTag wordTags in refM.wordTags)
        {
            if (wordTags.name != refM.wordTags[allTagIndex].name || wordTags.name != refM.wordTags[QuestManager.instance.questTagIndex].name)
                tagRelatedWords.Add(wordTags.name, new Word.WordData[refM.maxWordsPerTag]);
        }
        tagAmount = tagRelatedWords.Count - 2; //minus "all" & "quests"

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
            if (ReferenceManager.instance.wordCase.activeInHierarchy)
            {
                alreadyOpen = true;
            }
            ReferenceManager.instance.wordCase.SetActive(true);
        }
        //close the case
        else
        {
            if (!alreadyOpen)
            {
                ReferenceManager.instance.wordCase.SetActive(false);
            }
        }
    }
    /// <summary>
    /// for manually opening the case
    /// </summary>
    public void ManuallyOpenCase()
    {
        ReferenceManager.instance.wordCase.SetActive(!ReferenceManager.instance.wordCase.activeInHierarchy);
        openTag = ReferenceManager.instance.wordTags[0].name;
    }
    /// <summary>
    /// save a word dragged into the case in the Dict tagRelatedWords
    /// </summary>
    /// <param name="word"></param>
    public void SaveWord(Word wordItem)
    {
        Word.WordData word = wordItem.data;

        if (CheckIfCanSaveWord(word.name, word.tag, out int index))
        {
            tagRelatedWords[openTag][index] = word;
        }
        else
        {
            WordUtilities.ReturnWordIntoText(wordItem);
        }

        //Reload
        OpenOnTag(false);
        EffectUtilities.ReColorAllInteractableWords();
    }
    public bool CheckIfCanSaveWord(string name, string tag, out int index)
    {
        index = -1;
        bool inWordList;
        if (ReferenceManager.instance.duplicateWords)
            inWordList = false;
        else
            inWordList = CheckIfWordInList(name, tag);
        if (!inWordList)
        {
            bool foundASpot = false;
            for (int i = 0; i < ReferenceManager.instance.maxWordsPerTag; i++)
            {
                if (tagRelatedWords[openTag][i].name == null)
                {
                    index = i;
                    foundASpot = true;
                    break;
                }
            }
            if (foundASpot)
                return true;
            return false;
        }
        return false;
    }
    /// <summary>
    /// For button Inputs: Changes the open tag
    /// </summary>
    /// <param name="info"></param>
    public void ChangeToTag(TagButtonInfo info)
    {
        openTag = info.wordTag.name;
    }
    /// <summary>
    /// Loads the Word Case on the given tag. removes the previous words.
    /// </summary>
    public void OpenOnTag(bool resetScrollbar)
    {
        // Set Background Color to Tag Color + a bit grey
        Color color = WordUtilities.MatchColorToTag(openTag);
        color = Color.Lerp(color, Color.grey, 0.35f);
        Color colorStandart = Color.Lerp(color, Color.white, 0.3f);
        background.color = color;

        if (refM.wordLimit.isActiveAndEnabled)
        {
            refM.wordLimit.GetComponentInParent<Image>().color = color;
            refM.bubbleScrollbar.GetComponent<Image>().color = color;
            refM.bubbleScrollbar.GetComponentsInChildren<Image>()[1].color = colorStandart;
            refM.buttonScrollbar.GetComponent<Image>().color = color;
            refM.buttonScrollbar.GetComponentsInChildren<Image>()[1].color = colorStandart;
        }

        // Unload Words of previous tag
        foreach (Transform word in refM.listingParent.GetComponentsInChildren<Transform>())
        {
            if (word.gameObject != refM.listingParent)
            {
                Destroy(word.gameObject);
            }
        }

        // Load words of current tag
        if (openTag != ReferenceManager.instance.wordTags[allTagIndex].name) // tag isnt "AllWords"
        {
            foreach (Word.WordData word in tagRelatedWords[openTag])
            {
                if (word.name != null)
                {
                    GameObject bubble = WordUtilities.CreateWord(word, Vector2.zero, Vector2.zero, WordInfo.Origin.WordCase, false);
                    //place bubbles correctly (for protoype: below each other until i find out how the UI is supposed to work)
                    bubble.transform.SetParent(refM.listingParent.transform);
                }

            }
        }
        else // go through ALL tags
        {
            foreach (WordInfo.WordTag tag in ReferenceManager.instance.wordTags)
            {
                if (tagRelatedWords.ContainsKey(tag.name))
                {
                    foreach (Word.WordData word in tagRelatedWords[tag.name])
                    {
                        if (word.name != null)
                        {
                            SpawnWordInCase(word);
                        }
                    }
                }
            }
        }
        DestroyWordReplacement();
        UpdateWordCount();
        RescaleScrollbar(GetTagWordCount(openTag));
        if (resetScrollbar)
            ResetScrollbar();
    }
    /// <summary>
    /// Put the word that was dragged out of the case back into the case
    /// </summary>
    public void PutWordBack(Word word, Transform parent)
    {
        word.transform.SetParent(parent);
        OpenOnTag(false);
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
        int wordCount = GetTagWordCount(openTag);
        if (openTag != ReferenceManager.instance.wordTags[0].name)
        {
            ReferenceManager.instance.wordLimit.text = wordCount.ToString() + "<b>/" + ReferenceManager.instance.maxWordsPerTag + "</b>";
        }
        else
        {
            ReferenceManager.instance.wordLimit.text = wordCount.ToString() + "<b> Words</b>";
        }
    }
    /// <summary>
    /// Checks if the given Word is in the word case already and doesnt need to be added
    /// </summary>
    /// <returns></returns>
    bool CheckIfWordInList(string word, string tag)
    {
        bool inList = false;
        foreach (Word.WordData data in tagRelatedWords[tag])
        {
            if (data.name != null && data.name == word)
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
        GameObject bubble = WordUtilities.CreateWord(word, Vector2.zero, Vector2.zero, WordInfo.Origin.WordCase, false);
        //place bubbles correctly (for protoype: below each other until i find out how the UI is supposed to work)
        bubble.transform.SetParent(ReferenceManager.instance.listingParent.transform);
        return bubble;
    }
    /// <summary>
    /// throw a word out of the case OR throw a quest out of the log
    /// </summary>
    public void TrashAWord()
    {
        Word.WordData data = WordClickManager.instance.currentWord.GetComponent<Word>().data;
        if (data.tag != ReferenceManager.instance.wordTags[QuestManager.instance.questTagIndex].name) // isnt quest
        {
            DeleteOutOfCase();
            WordClickManager.instance.DestroyCurrentWord();
            UpdateWordCount();
            RescaleScrollbar(GetTagWordCount(openTag));
            ResetScrollbar();
            DestroyWordReplacement();
        }
        else if (data.tag == ReferenceManager.instance.wordTags[QuestManager.instance.questTagIndex].name)
        {
            QuestManager.instance.DeleteOutOfLog();
            WordClickManager.instance.DestroyCurrentWord();
            QuestManager.instance.RescaleScrollbar();
            QuestManager.instance.ResetScrollbar();
            QuestManager.instance.DestroyQuestReplacement();
        }
    }
    /// <summary>
    /// Disable the ask and barter buttons (when in barter or question mode)
    /// </summary>
    /// <param name="setInactive"></param>
    public void DisableAskAndBarter(bool setInactive)
    {
        Color activeColor = ReferenceManager.instance.askColor;
        Color grey = ReferenceManager.instance.greyedOutColor;
        GameObject ask = ReferenceManager.instance.ask; ;
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
    #region Scrollbars
    /// <summary>
    /// Called, when tag-scrollbar is scrolled. move all buttons from left-right
    /// </summary>
    public void ScrollThroughButtons()
    {
        float value = ReferenceManager.instance.buttonScrollbar.GetComponent<Scrollbar>().value;
        ReferenceManager.instance.tagButtonParent.transform.localPosition = new Vector2(-value * ReferenceManager.instance.tagScrollbarDistance,
            ReferenceManager.instance.tagButtonParent.transform.localPosition.y);
    }
    /// <summary>
    /// Call when the bubble scrollbar is scrolled. moves the words up & down. resets on tag.
    /// </summary>
    public void ScrollThroughBubbles()
    {
        float value = ReferenceManager.instance.bubbleScrollbar.GetComponent<Scrollbar>().value;
        ReferenceManager.instance.listingParent.transform.localPosition = new Vector2(0, value * ReferenceManager.instance.currBubbleScrollbarDistance);
    }
    /// <summary>
    /// Resets on OpenOnTag(), so that we arent lost in the sauce
    /// </summary>
    void ResetScrollbar()
    {
        ReferenceManager.instance.bubbleScrollbar.value = 0;
    }
    /// <summary>
    /// Takes the ScrollBar and re-scales it according to the current amount of words
    /// </summary>
    /// <param name="bubbleCount"></param>
    void RescaleScrollbar(int bubbleCount)
    {
        Scrollbar scrollbar = refM.bubbleScrollbar;
        //if the value is smaller than what fits the canvas, it is irrelevant
        Mathf.Clamp(bubbleCount, refM.spaceForBubblesOnCanvas, refM.scrollbarMaxSize);
        //between biggest size (1) and smallest we want (0.05f)
        float scrollbarSize = WordUtilities.Remap(bubbleCount, refM.spaceForBubblesOnCanvas, refM.scrollbarMaxSize, 1, 0.05f);
        float overlappingWords = Mathf.Clamp(bubbleCount - refM.spaceForBubblesOnCanvas, 0, Mathf.Infinity);
        refM.currBubbleScrollbarDistance = overlappingWords * refM.bubbleScrollbarDistance;
        scrollbar.size = scrollbarSize;
    }
    #endregion
    /// <summary>
    /// Count the number of words that are in the current tag
    /// </summary>
    /// <param name="tag"></param>
    int GetTagWordCount(string tag)
    {
        int wordCount = 0;
        if (tag != ReferenceManager.instance.wordTags[0].name)
        {
            foreach (Word.WordData data in tagRelatedWords[tag])
            {
                if (data.name != null)
                    wordCount++;
            }
        }
        else
        {
            foreach (string allTags in tagRelatedWords.Keys)
            {
                if (allTags != ReferenceManager.instance.wordTags[0].name &&
                    allTags != ReferenceManager.instance.wordTags[1].name &&
                    allTags != ReferenceManager.instance.wordTags[2].name)
                {
                    wordCount += GetTagWordCount(allTags);
                }
            }
        }
        return wordCount;
    }
}
