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
    public override void Start()
    {
        base.Start();
    }
    public override void Initialize(string name, string[] tags, WordInfo.Origin origin, TMP_WordInfo wordInfo, Vector2 firstAndLastWordIndex)
    {
        wordParent = this.gameObject;
        base.Initialize(name, tags, origin, wordInfo, firstAndLastWordIndex, out BubbleData bubbleData);
        data = new WordData(bubbleData);

        //initialize the tag Object
        ((WordData)data).tagObj = new TagObject();
        ((WordData)data).tagObj.allGivenValues = new List<Yarn.Value>();
        ((WordData)data).tagObj.allGivenValues.Add(new Yarn.Value(data.name));

        int i = 0;
        foreach (string tag in tags)
        {
            if (i != 0)
            {
                Yarn.Value val = WordUtilities.TransformIntoYarnVal(tag);
                ((WordData)data).tagObj.allGivenValues.Add(val);
            }
            i++; //we dont want the location to be in this
        }

        InitializeBubbleShaping(firstAndLastWordIndex);
    }
    public override void Initialize(BubbleData bubbleData, Vector2 firstAndLastWordIndex)
    {
        wordParent = transform.GetChild(0).gameObject;
        base.Initialize(bubbleData, firstAndLastWordIndex);
        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAB");
        if (((WordData)bubbleData).currentParent == null)
            data.origin = WordCaseManager.instance.origin;
        else
        {
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            data.origin = QuestManager.instance.origin;
        }
        data = new WordData(bubbleData);
        ((WordData)data).tagObj = ((WordData)bubbleData).tagObj;
        ((WordData)data).bubbleData = ((WordData)bubbleData).bubbleData;
        ((WordData)data).currentParent = ((WordData)bubbleData).currentParent;
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
            if (((WordData)data).currentParent.GetContentCount() == 1)
            {
                ((WordData)data).currentParent.GetComponentInChildren<OnClickFunctions>().OpenCase(true);
            }
        }
        //in the same or a different quest already
        else
        {
            if (((WordData)data).currentParent != WordClickManager.instance.lastSavedQuestCase)
            {
                ((WordData)data).currentParent.DeleteOutOfCase();
                WordClickManager.instance.lastSavedQuestCase.SaveBubble(this);
                if (((WordData)data).currentParent.GetContentCount() == 1)
                {
                    ((WordData)data).currentParent.GetComponentInChildren<OnClickFunctions>().OpenCase(true);
                }
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
    public Bubble.TagObject tagObj
    {
        get
        {
            return TagObj;
        }
        set
        {
            TagObj = value;
            UpdateBubbleData();
        }
    }
    public BubbleData bubbleData
    {
        get
        {
            return BubbleData;
        }
        set
        {
            BubbleData = value;
            UpdateBubbleData();
        }
    }
    public QuestCase currentParent
    {
        get
        {
            return CurrentParent;
        }
        set
        {
            CurrentParent = value;
            UpdateBubbleData();
        }
    }

    Bubble.TagObject TagObj;
    BubbleData BubbleData;
    QuestCase CurrentParent;
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
    public override void UpdateBubbleData()
    {
        base.UpdateBubbleData();
        if (origin == WordInfo.Origin.WordCase)
        {
            WordCaseManager.instance.UpdateBubbleData(this);
        }
        else if (origin == WordInfo.Origin.QuestLog)
        {
            Debug.Log("RIGHT");
            if (currentParent != null)
            {
                Debug.Log(currentParent);
                currentParent.UpdateBubbleData(this);
            }

        }
    }
}

