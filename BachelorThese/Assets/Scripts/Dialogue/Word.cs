using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pinwheel.UIEffects;
using UnityEngine.EventSystems;
using Yarn.Unity;

public class Word : Bubble
{
    public WordData wordData; //is the same a data, but casted as WordData
    public QuestCase currentParent;
    public override void Start()
    {
        base.Start();
    }
    public override void Initialize(string name, string[] tags, WordInfo.Origin origin, TMP_WordInfo wordInfo, Vector2 firstAndLastWordIndex)
    {
        wordParent = this.gameObject;
        base.Initialize(name, tags, origin, wordInfo, firstAndLastWordIndex, out BubbleData bubbleData);
        data = new WordData(bubbleData);
        wordData = (WordData)data;
        //initialize the tag Object
        wordData.tagObj = new TagObject();
        wordData.tagObj.allGivenValues = new List<Yarn.Value>();
        wordData.tagObj.allGivenValues.Add(new Yarn.Value(data.name));

        int i = 0;
        foreach (string tag in tags)
        {
            if (i != 0)
            {
                Yarn.Value val = WordUtilities.TransformIntoYarnVal(tag);
                wordData.tagObj.allGivenValues.Add(val);
            }
            i++; //we dont want the location to be in this
        }

        InitializeBubbleShaping(firstAndLastWordIndex);
    }
    public override void IsOverWordCase()
    {
        if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
        {
            //save it
            WordCaseManager.instance.SaveBubble(this);

            //close the case & Delete the UI word
            WordCaseManager.instance.AutomaticOpenCase(false);
            WordClickManager.instance.DestroyCurrentWord(this);
        }
        else if (data.origin == WordInfo.Origin.WordCase)
        {
            IsOverNothing();
        }
        else if (data.origin == WordInfo.Origin.QuestLog)
        {
            // this can happen if the word is a child of a quest
        }
    }
    public override void IsOverQuestLog()
    {
        if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
        {
            IsOverNothing();
        }
        else if (data.origin == WordInfo.Origin.WordCase)
        {
            IsOverNothing();
        }
        else if (data.origin == WordInfo.Origin.QuestLog)
        {
            // this can happen if the word is a child of a quest
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
            else if (data.origin == WordInfo.Origin.WordCase)
            {
                // parent to word
                WordUtilities.ParentBubbleToPrompt(this.gameObject);
            }
            else if (data.origin == WordInfo.Origin.QuestLog)
            {
                // this can happen if the word is a child of a quest
            }
        }
        else
            IsOverNothing();
    }
    public override void IsOverNothing()
    {
        //check if the mouse is above an NPC
        base.IsOverNothing();

        //this happens, regardless wheter a character was hit or not
        if (data.origin == WordInfo.Origin.WordCase)
        {
            // put it back
            AnimateMovementBackToCase(false);
        }
        else if (data.origin == WordInfo.Origin.QuestLog)
        {
            // this can happen if the word is a child of a quest
        }
    }
    public override void IsOverQuestCase()
    {
        base.IsOverQuestCase();

        //not in a quest already
        if (data.origin != WordInfo.Origin.QuestLog)
        {
            WordClickManager.instance.lastSavedQuestCase.SaveBubble(this);
            WordClickManager.instance.DestroyCurrentWord(this);
        }
        //in the same or a different quest already
        else
        {
            if (currentParent != WordClickManager.instance.lastSavedQuestCase)
            {
                currentParent.DeleteOutOfCase();
                WordClickManager.instance.lastSavedQuestCase.SaveBubble(this);
            }
        }
    }
    public override void Unparent(Transform newParent, bool spawnWordReplacement, bool toCurrentWord)
    {
        base.Unparent(newParent, spawnWordReplacement, toCurrentWord);
        if (spawnWordReplacement)
        {
            if (data.origin == WordInfo.Origin.WordCase)
                WordCaseManager.instance.SpawnReplacement(this);
        }
    }
}
public class WordData : BubbleData
{
    public Bubble.TagObject tagObj;
    public BubbleData bubbleData;
    public WordData(BubbleData data) : base()
    {
        name = data.name;
        tag = data.tag;
        tagInfo = data.tagInfo;
        origin = data.origin;
        lineLengths = data.lineLengths;
        isLongWord = data.isLongWord;
        bubbleData = data;
    }
}

