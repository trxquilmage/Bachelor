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
                ReloadContents(false);
            }
            else
            {
                OpenTag = value;
                ReloadContents(true);
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
        //override tag && isnt quest
        else if (overrideTag != null && overrideTag != refM.wordTags[refM.questTagIndex].name)
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
        //override tag, but not a quest
        else if (overrideTag != null && overrideTag != refM.wordTags[refM.questTagIndex].name)
        {
            BubbleData[] data = tagRelatedWords[overrideTag];
            overrideTag = null;
            return data;
        }
        //override tag is quest or no override tag
        else if (overrideTag != null || tagRelatedWords.ContainsKey(openTag))
        {
            overrideTag = null;
            return tagRelatedWords[openTag];
        }
        else
            return null;
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
            //if the tag isnt "all" or "quest"
            if (wordTags.name != refM.wordTags[refM.allTagIndex].name && wordTags.name != refM.wordTags[refM.questTagIndex].name)
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
    public override void ReloadContents(bool resetScrollbar)
    {
        ChangeCaseColor();
        base.ReloadContents(resetScrollbar);
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
            foreach (WordInfo.WordTag tag in ReferenceManager.instance.wordTags)
            {
                if (tagRelatedWords.ContainsKey(tag.name))
                {
                    foreach (BubbleData data in tagRelatedWords[tag.name])
                    {
                        if (data.name != null)
                        {
                            SpawnBubbleInCase((WordData)data);
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Updates the wordcount text UI of the word case
    /// </summary>
    public override void UpdateContentCount()
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
    public override void SaveBubble(Bubble bubble)
    {
        ((WordData)bubble.data).currentParent = null;
        ((WordData)bubble.data).origin = origin;
        base.SaveBubble(bubble);
    }
    #endregion

    /// <summary>
    /// For button Inputs: Changes the open tag
    /// </summary>
    /// <param name="info"></param>
    public void ChangeToTag(TagButtonInfo info)
    {
        openTag = info.wordTag.name;
    }
    /// <summary>
    /// Change the color of the background, scrollbar & WordCounter according to the tag
    /// </summary>
    public void ChangeCaseColor()
    {
        // Set Background Color to Tag Color + a bit grey
        Color color = WordUtilities.MatchColorToTag(openTag);
        color = Color.Lerp(color, Color.grey, 0.35f);
        Color colorStandart = Color.Lerp(color, Color.white, 0.3f);
        caseObject.GetComponent<Image>().color = color;
        if (refM.wordLimit.isActiveAndEnabled)
        {
            refM.wordLimit.GetComponentInParent<Image>().color = color;
            refM.bubbleScrollbar.GetComponent<Image>().color = color;
            refM.bubbleScrollbar.GetComponentsInChildren<Image>()[1].color = colorStandart;
            refM.buttonScrollbar.GetComponent<Image>().color = color;
            refM.buttonScrollbar.GetComponentsInChildren<Image>()[1].color = colorStandart;
        }
    }

    #region Scrollbars
    /// <summary>
    /// Called, when tag-scrollbar is scrolled. move all buttons from left-right
    /// </summary>
    public void ScrollThroughButtons()
    {
        float value = ReferenceManager.instance.buttonScrollbar.GetComponent<Scrollbar>().value;
        float posX = Mathf.Lerp(0, -UIManager.instance.buttonWidth, value);
        ReferenceManager.instance.tagButtonParent.transform.localPosition = new Vector2(posX, ReferenceManager.instance.tagButtonParent.transform.localPosition.y);
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
