using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class WordCaseManager : Case
{
    //Handles everything happening in the word case
    public static WordCaseManager instance;

    [HideInInspector]
    public string openTag
    {
        get { return OpenTag; }
        set
        {
            if (OpenTag == value)
            {
                OpenTag = value;
                ReloadContents();
            }
            else
            {
                MakeOtherTagProminent(OpenTag, value);
                OpenTag = value;
                ReloadContents();
                ResetScrollbar();
            }
        }
    }

    string OpenTag;
    [HideInInspector]
    public string overrideTag
    {
        get { return OverrideTag; }
        set
        {
            OverrideTag = value;
        }
    }
    string OverrideTag = null;
    [HideInInspector] public Dictionary<string, BubbleData[]> tagRelatedWords;
    bool onlyShowFavorites;

    public void Start()
    {
        Initialize();
    }

    #region OVERRIDES
    public override void Awake()
    {
        instance = this;
    }
    public override void Initialize()
    {
        base.Initialize();
    }
    public override void SetContents(BubbleData[] value)
    {
        //tag is "AllWords"
        if (overrideTag == refM.wordTags[refM.allTagIndex].name || overrideTag == null && openTag == refM.wordTags[refM.allTagIndex].name)
        {
            //i dont think anything needs to happen here rn?
        }
        //override tag
        else if (overrideTag != null)
        {
            tagRelatedWords[overrideTag] = value;
            overrideTag = null;
        }
        else
            tagRelatedWords[openTag] = value;
    }
    public override BubbleData[] GetContents()
    {
        //get contents: all words
        if (overrideTag == refM.wordTags[refM.allTagIndex].name || overrideTag == null && openTag == refM.wordTags[refM.allTagIndex].name)
        {
            List<BubbleData> allContents = new List<BubbleData>();
            foreach (BubbleData[] dataSets in tagRelatedWords.Values)
            {
                foreach (BubbleData data in dataSets)
                    allContents.Add(data);
            }
            return allContents.ToArray();
        }
        //override tag
        else if (overrideTag != null)
        {
            BubbleData[] data = tagRelatedWords[overrideTag];
            overrideTag = null;
            return data;
        }
        //no override tag
        else if (tagRelatedWords.ContainsKey(openTag))
        {
            overrideTag = null;
            return tagRelatedWords[openTag];
        }
        else
            return null;
    }
    protected override void CheckIfANewTagIsIncludedAndAddButton(string tagName)
    {
        if (CheckIfTagIsNew(tagName))
            ShowTagButton(tagName);
    }
    protected override void CheckIfATagIsEmptyDueToDelete(string tagName)
    {
        if (CheckIfTagIsEmpty(tagName))
        {
            HideTagButton(tagName);
            IfHiddenTagIsOpenJumpToAll(tagName);
        }
    }
    protected bool CheckIfTagIsNew(string tagName)
    {
        if (!WordUtilities.GetTag(tagName).buttonIsActive)
            return true;
        return false;
    }
    public override void UpdateBubbleData(BubbleData data)
    {
        int index = FindIndexOfBubbleDataInRelatedTagList(data);
        if (index != -1)
        {
            tagRelatedWords[data.tag][index] = data;
        }
    }
    public int FindIndexOfBubbleDataInRelatedTagList(BubbleData data)
    {
        int index = -1;
        int i = -1;
        foreach (BubbleData contentData in tagRelatedWords[data.tag])
        {
            i++;
            if (contentData.name == data.name)
            {
                index = i;
            }
        }
        return index;
    }
    protected void ShowTagButton(string tagName)
    {
        WordUtilities.GetTag(tagName).tagButton.GetComponent<TagButtonInfo>().SetActive();
    }
    public bool CheckIfTagIsEmpty(string tagName)
    {
        bool isEmpty = true;
        for (int i = 0; i < tagRelatedWords[tagName].Length; i++)
        {
            if (tagRelatedWords[tagName][i].name != null)
                isEmpty = false;
        }
        if (isEmpty)
            return true;
        return false;
    }
    public void HideTagButton(string tagName)
    {
        WordUtilities.GetTag(tagName).tagButton.GetComponent<TagButtonInfo>().SetInactive();
    }
    void IfHiddenTagIsOpenJumpToAll(string tagName)
    {
        if (tagName == openTag)
            ChangeToTag(refM.wordTags[refM.allTagIndex].tagButton.GetComponent<TagButtonInfo>());
    }
    public override void RearrangeContents()
    {
        base.RearrangeContents();
    }
    public override void InitializeValues()
    {
        base.InitializeValues();
        tagRelatedWords = new Dictionary<string, BubbleData[]>();
        caseObject = refM.wordCase;
        journalImage = refM.wordJournal;
        listingParent = refM.listingParent;
        contentCount = refM.wordLimit;
        maxContentAmount = refM.maxWordsPerTag;
        origin = WordInfo.Origin.WordCase;
        scrollbar = refM.bubbleScrollbar;
        openTag = refM.wordTags[refM.allTagIndex].name;
    }
    /// <summary>
    /// Fills the Dictionary TagRelatedWords, as WordData needs a constructor to actually be spawned
    /// </summary>
    public override void FillArrayWithContents()
    {
        //initialize the dictionary of selcted words
        foreach (WordInfo.WordTag wordTags in refM.wordTags)
        {
            //if the tag isnt "all"
            if (wordTags.name != refM.wordTags[refM.allTagIndex].name)
            {
                BubbleData[] dataList = new BubbleData[refM.maxWordsPerTag];
                for (int i = 0; i < refM.maxWordsPerTag; i++)
                {
                    dataList[i] = new BubbleData();
                }
                tagRelatedWords.Add(wordTags.name, dataList);
            }
        }
    }
    /// <summary>
    /// for manually opening the case
    /// </summary>
    public override void ManuallyOpenCase()
    {
        base.ManuallyOpenCase();
        openTag = ReferenceManager.instance.wordTags[refM.allTagIndex].name;
    }
    /// <summary>
    /// Loads the Word Case on the given tag. removes the previous words.
    /// </summary>
    public override void ReloadContents()
    {
        ChangeCaseColor();
        base.ReloadContents();
    }
    public override void SpawnContents()
    {
        RearrangeContents();
        // Load words of current tag
        // if tag isnt "AllWords"
        if (openTag != refM.wordTags[refM.allTagIndex].name)
        {
            foreach (BubbleData data in contents)
            {
                if (data.name != null)
                {
                    SpawnBubbleInCase((WordData)data);
                }

            }
        }
        // go through ALL tags
        else
        {
            List<BubbleData> allWordsTemp = new List<BubbleData>();
            foreach (WordInfo.WordTag tag in ReferenceManager.instance.wordTags)
            {
                if (tagRelatedWords.ContainsKey(tag.name))
                {
                    allWordsTemp.AddRange(tagRelatedWords[tag.name]);
                }
            }
            RearrangeContents(ref allWordsTemp);
            foreach (BubbleData data in allWordsTemp)
            {
                if (data.name != null)
                    if (onlyShowFavorites && data.isFavorite || !onlyShowFavorites)
                        SpawnBubbleInCase((WordData)data);
            }
        }
    }
    public override void UpdateContentCount()
    {
        int wordCount = GetTagWordCount(openTag);
        if (openTag != refM.wordTags[refM.allTagIndex].name)
        {
            refM.wordLimit.text = wordCount.ToString() + "<b>/" + refM.maxWordsPerTag + "</b>";
            if (wordCount == refM.maxWordsPerTag)
                refM.wordLimit.text = "<b><color=red>" + refM.wordLimit.text ;
        }
        else
        {
            refM.wordLimit.text = wordCount.ToString() + "<b> Words</b>";
        }
    }
    public override void TryToSaveTheBubble(Bubble bubble)
    {
        ((WordData)bubble.data).origin = origin;
        base.TryToSaveTheBubble(bubble);
    }
    #endregion
    public void ChangeToTag(TagButtonInfo info)
    {
        onlyShowFavorites = false;
        openTag = info.wordTag.name;
    }
    public void ChangeToFavorites()
    {
        onlyShowFavorites = true;
        openTag = refM.wordTags[refM.allTagIndex].name;
    }
    public int GetFavoritesCount()
    {
        int count = 0;
        foreach (WordInfo.WordTag tag in ReferenceManager.instance.wordTags)
        {
            if (tagRelatedWords.ContainsKey(tag.name))
            {
                foreach (BubbleData data in tagRelatedWords[tag.name])
                    if (data.isFavorite)
                        count ++;
            }
        }
        return count;
    }
    public void ChangeCaseColor()
    {
        // Set Background Color to Tag Color + a bit grey
        Color color = WordUtilities.MatchColorToTag(openTag, "");
        Color highlightColor = Color.Lerp(color, ReferenceManager.instance.highlightColor, 0.35f);
        ReferenceManager.instance.wordJournal.GetComponent<Image>().color = highlightColor;
    }
    int GetTagWordCount(string tag)
    {
        int wordCount = 0;
        if (tag != refM.wordTags[refM.allTagIndex].name)
        {
            foreach (BubbleData data in tagRelatedWords[tag])
            {
                if (data.name != null)
                    wordCount++;
            }
        }
        else
        {
            foreach (string allTags in tagRelatedWords.Keys)
            {
                if (allTags != ReferenceManager.instance.wordTags[refM.allTagIndex].name &&
                    allTags != ReferenceManager.instance.wordTags[refM.otherTagIndex].name)
                {
                    wordCount += GetTagWordCount(allTags);
                }
            }
        }
        return wordCount;
    }
    void MakeOtherTagProminent(string oldTag, string newTag)
    {
        if (oldTag != "" && oldTag != null)
        {
            if (!onlyShowFavorites)
                WordUtilities.GetTag(oldTag).tagButton.GetComponent<TagButtonInfo>().MakeTagButtonSmaller();
            else if (oldTag == refM.wordTags[refM.allTagIndex].name)
            {
                refM.favoritesButton.GetComponent<TagButtonInfo>().MakeTagButtonSmaller();
                if (oldTag != newTag)
                    onlyShowFavorites = false;
            }
        }
        if (newTag != "" && newTag != null)
        {
            if (!onlyShowFavorites)
                WordUtilities.GetTag(newTag).tagButton.GetComponent<TagButtonInfo>().MakeTagButtonProminent();
            else if (newTag == refM.wordTags[refM.allTagIndex].name)
            {
                refM.favoritesButton.GetComponent<TagButtonInfo>().MakeTagButtonProminent();
            }
        }
    }
    public override void ChangeScrollbarValue(float scrollValue)
    {
        if (WordClickManager.instance.mouseOverUIObject == "wordCase")
            base.ChangeScrollbarValue(scrollValue);
    }
}
