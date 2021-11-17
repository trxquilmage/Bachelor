using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pinwheel.UIEffects;
using UnityEngine.EventSystems;
using Yarn.Unity;

public class Quest : Bubble
{
    public QuestData questData; //is the same a data, but casted as QuestData
    GameObject questCountObject;
    GameObject dropDownObject;
    GameObject addedWordParent;

    public override void Initialize(string name, string[] tags, WordInfo.Origin origin, TMP_WordInfo wordInfo, Vector2 firstAndLastWordIndex)
    {
        base.Initialize(name, tags, origin, wordInfo, firstAndLastWordIndex, out BubbleData bubbleData);
        data = new QuestData(bubbleData);
        questData = (QuestData)data;
        InitializeBubbleShaping(firstAndLastWordIndex);
        InitalizeVerticalLayoutGroup();
        InitializeQuestCountObject();
        InitializeHideObject();
    }
    #region OVERRIDES
    public override void IsOverWordCase()
    {
        if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
        {
            IsOverNothing();
        }
        else if (data.origin == WordInfo.Origin.QuestLog)
        {
            // put it back
            AnimateMovementBackToCase(true);
        }
    }
    public override void IsOverQuestLog()
    {
        if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
        {
            //save it
            QuestManager.instance.SaveQuest(this);

            //close the case & Delete the UI word
            QuestManager.instance.AutomaticOpenCase(false);
            WordClickManager.instance.DestroyCurrentWord(this);

        }
        else if (data.origin == WordInfo.Origin.QuestLog)
        {
            IsOverNothing();
        }
    }
    public override void IsOverPlayerInput()
    {
        if (WordClickManager.instance.promptBubble.acceptsCurrentWord)
        {
            if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
            {
                //parent to word
                WordUtilities.ParentBubbleToPrompt(this.gameObject);
                //close wordCase
                WordCaseManager.instance.AutomaticOpenCase(false);
            }
            else if (data.origin == WordInfo.Origin.QuestLog)
            {
                // parent to word
                WordUtilities.ParentBubbleToPrompt(this.gameObject);
            }
        }
        else
            IsOverNothing();
    }
    public override void IsOverNothing()
    {
        //check if the mouse is above an NPC
        base.IsOverNothing();

        if (data.origin == WordInfo.Origin.QuestLog)
        {
            // put it back
            AnimateMovementBackToCase(true);
        }
    }
    public override void Unparent(Transform newParent, bool spawnWordReplacement, bool toCurrentWord)
    {
        base.Unparent(newParent, spawnWordReplacement, toCurrentWord);
        if (spawnWordReplacement)
        {
            if (data.origin == WordInfo.Origin.QuestLog)
                QuestManager.instance.SpawnQuestReplacement(this);
        }
    }
    #endregion

    public void SaveWordData(WordData wordData, int index)
    {
        questData.assignedWords[index] = wordData;
        UpdateQuestCountObject();
    }
    /// <summary>
    /// Check If there is enough space for the word and the word isnt there already
    /// </summary>
    public bool CheckIfWordFits(WordData wordData, out int index)
    {
        index = -1;
        int i = 0;
        bool foundSpot = false;
        foreach (WordData wData in questData.assignedWords)
        {
            if (wData.name == null && index == -1)
            {
                index = i;
                foundSpot = true;
            }
            if (wData.name == wordData.name)
            {
                return false;
            }
            i++;
        }
        return foundSpot;
    }
    /// <summary>
    /// The current VerticalLayoutGroup would have problems with other icons that belong to the QuestBubble
    /// So we are creating it again, one layer lower
    /// </summary>
    void InitalizeVerticalLayoutGroup()
    {
        addedWordParent = transform.GetChild(1).gameObject;
    }
    /// <summary>
    /// Initialize a small counter next to the quest
    /// </summary>
    void InitializeQuestCountObject()
    {
        questCountObject = transform.GetChild(3).gameObject;
        questCountObject.transform.localPosition = new Vector2(GetComponent<RectTransform>().sizeDelta.x + 8, 0);
        UpdateQuestCountObject();
    }
    void InitializeHideObject()
    {
        dropDownObject = transform.GetChild(2).gameObject;
        dropDownObject.transform.localPosition = new Vector2(-dropDownObject.GetComponent<RectTransform>().sizeDelta.x - 5, 0);
        dropDownObject.GetComponent<OnClickFunctions>().relatedQuest = this;
    }
    /// <summary>
    /// Update the text of the object next to the Quest
    /// </summary>
    void UpdateQuestCountObject()
    {
        int i = 0;
        foreach (WordData wData in questData.assignedWords)
        {
            if (wData.name != null)
            {
                i++;
            }
        }
        // make object show "x/5"
        questCountObject.GetComponentInChildren<TMP_Text>().text =
            i.ToString() + "<b>/" + ReferenceManager.instance.maxQuestAdditions.ToString() + "</b>";
    }
    public void OpenDropDown()
    {

    }
    public void CloseDropDown()
    {

    }
}
public class QuestData : BubbleData
{
    public BubbleData bubbleData;
    public WordData[] assignedWords;
    public string[] relevantWords;
    public QuestData(BubbleData data) : base()
    {
        name = data.name;
        tag = data.tag;
        tagInfo = data.tagInfo;
        origin = data.origin;
        lineLengths = data.lineLengths;
        isLongWord = data.isLongWord;
        bubbleData = data;

        if (name != null)
        {
            int maxQuestAdditions = ReferenceManager.instance.maxQuestAdditions;
            assignedWords = new WordData[maxQuestAdditions];
            relevantWords = new string[maxQuestAdditions];
            for (int i = 0; i < maxQuestAdditions; i++)
            {
                assignedWords[i] = new WordData(new BubbleData());
                if (data.tagInfo.Length > i + 1)
                    relevantWords[i] = data.tagInfo[i + 1];
                else
                    relevantWords[i] = null;
            }
        }

    }
}
