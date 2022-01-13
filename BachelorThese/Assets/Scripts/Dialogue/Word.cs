using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pinwheel.UIEffects;
using UnityEngine.EventSystems;
using UnityEngine.VFX;
using Yarn.Unity;

public class Word : Bubble
{
    public override void Start()
    {
        base.Start();
    }
    public override void Initialize(BubbleData inputData, WordInfo.Origin origin, TMP_WordInfo wordInfo, Vector2 firstAndLastWordIndex)
    {
        vfxParent = GetComponentInChildren<VisualEffect>().transform.parent.gameObject;
        relatedCase = WordCaseManager.instance;
        wordParent = this.gameObject;

        data = new BubbleData();

        //so that the strings arent reference types :)
        string[] tagsCopy = new string[inputData.tagInfo.Length];
        System.Array.Copy(inputData.tagInfo, tagsCopy, inputData.tagInfo.Length);

        base.Initialize(inputData, origin, wordInfo, firstAndLastWordIndex, out BubbleData bubbleData);
        
        data = new WordData(bubbleData);
        
        if (data.subtag == "Boolean")
        {
            data.permanentWord = true;
        }
        //initialize the tag Object
        ((WordData)data).tagObj = new TagObject();
        ((WordData)data).tagObj.allGivenValues = new List<Yarn.Value>();
        ((WordData)data).tagObj.allGivenValues.Add(new Yarn.Value(data.name));

        int i = 0;
        foreach (string tag in inputData.tagInfo)
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
        vfxParent = GetComponentInChildren<VisualEffect>().transform.parent.gameObject;
        wordParent = this.gameObject;
        data = bubbleData;

        base.Initialize(data, firstAndLastWordIndex);
        data = new WordData(data);

        if (data.subtag == "Boolean")
        {
            data.permanentWord = true;
        }

        ((WordData)data).tagObj = ((WordData)bubbleData).tagObj;
        ((WordData)data).bubbleData = ((WordData)bubbleData).bubbleData;
        InitializeBubbleShaping(firstAndLastWordIndex);
    }
    protected override void IsOverWordCase()
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
    }
    protected override void IsOverPlayerInput()
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
                OnEnterPromptBubble();
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
            AnimateMovementBackToCase();
        }
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }
    protected override void Unparent(Transform newParent, bool spawnWordReplacement, bool toCurrentWord)
    {
        base.Unparent(newParent, spawnWordReplacement, toCurrentWord);
        if (spawnWordReplacement)
        {
            if (data.origin == WordInfo.Origin.WordCase)
                WordCaseManager.instance.SpawnReplacement(this);
        }
    }
    protected override Vector2 GetCaseTargetPosition()
    {
        if (data.origin != WordInfo.Origin.QuestLog)
            return ReferenceManager.instance.wordCase.GetComponent<RectTransform>().rect.center +
                                (Vector2)WordUtilities.GlobalScreenToCanvasPosition(
                                    ReferenceManager.instance.wordCase.GetComponent<RectTransform>().position);
        else
            return ReferenceManager.instance.questCase.GetComponent<RectTransform>().rect.center +
                 (Vector2)WordUtilities.GlobalScreenToCanvasPosition(
                     ReferenceManager.instance.questCase.GetComponent<RectTransform>().position);
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

    Bubble.TagObject TagObj;
    BubbleData BubbleData;
    public WordData(BubbleData data) : base()
    {
        name = data.name;
        tag = data.tag;
        subtag = data.subtag;
        tagInfo = data.tagInfo;
        origin = data.origin;
        lineLengths = data.lineLengths;
        isLongWord = data.isLongWord;
        bubbleData = data;
        permanentWord = data.permanentWord;
        isFavorite = data.isFavorite;
    }
    public override void UpdateBubbleData()
    {
        base.UpdateBubbleData();
        if (origin == WordInfo.Origin.WordCase)
        {
            WordCaseManager.instance.UpdateBubbleData(this);
        }
    }
}

