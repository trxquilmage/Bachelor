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
        SetInitialValues();

        data = new BubbleData();
        base.Initialize(inputData, origin, wordInfo, firstAndLastWordIndex, out BubbleData bubbleData);
        data = new WordData(bubbleData);

        CheckIfShouldSetAsPermanentWord();

        //initialize the tag Object
        ((WordData)data).tagObj = new TagObject();
        ((WordData)data).tagObj.allGivenValues = new List<Yarn.Value>();
        ((WordData)data).tagObj.allGivenValues.Add(new Yarn.Value(data.name));

        SaveTagInfoAsYarnValuesInTagObject();
        ShapeBubbleAccordingToSize(firstAndLastWordIndex, true);
        ScaleAllParentsToTheirCorrectSizes();
        MoveLinesAccordingToOffset(true);
        effects.ColorWordToTag();
    }
    public override void Initialize(BubbleData bubbleData, Vector2 firstAndLastWordIndex)
    {
        SetInitialValues();

        data = bubbleData;
        base.Initialize(data, firstAndLastWordIndex);
        data = new WordData(data);

        CheckIfShouldSetAsPermanentWord();

        ((WordData)data).tagObj = ((WordData)bubbleData).tagObj;
        ((WordData)data).bubbleData = ((WordData)bubbleData).bubbleData;

        ShapeBubbleAccordingToSize(firstAndLastWordIndex, false);
        ScaleAllParentsToTheirCorrectSizes();
        MoveLinesAccordingToOffset(false);

        EffectUtilities.ColorAllChildrenOfAnObject(wordParent, data.tag, data.subtag);
    }
    public override void DroppedOverWordCase()
    {
        if (data.IsNotFromACase())
        {
            WordCaseManager.instance.TryToSaveTheBubble(this);
            WordCaseManager.instance.AutomaticOpenCase(false);
            WordClickManager.instance.DestroyThisWordAndCheckIfCurrent(this);
        }
        else if (data.IsFromWordCase())
        {
            DroppedOverNothing();
        }
    }
    public override void DroppedOverPlayerInput()
    {
        if (WordClickManager.instance.promptBubble.acceptsCurrentWord)
        {
            if (data.IsNotFromACase())
            {
                WordUtilities.ParentBubbleToPromptOnDrop(this.gameObject);
                WordCaseManager.instance.AutomaticOpenCase(false);
            }
            else if (data.IsFromWordCase())
            {
                WordUtilities.ParentBubbleToPromptOnDrop(this.gameObject);
                OnEnterPromptBubble();
            }
        }
        else
            DroppedOverNothing();
    }
    public override void DroppedOverNothing()
    {
        base.DroppedOverNothing();

        if (data.IsFromWordCase())
        {
            doubleClickHandler.AnimateMovementBackToCase();
            placeholderHandler.RemoveReplacement();
        }
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }
    protected override void ParentToNewParent(Transform newParent, bool spawnWordReplacement, bool toCurrentWord)
    {
        base.ParentToNewParent(newParent, spawnWordReplacement, toCurrentWord);
        if (!spawnWordReplacement || !data.IsFromWordCase())
            return;

        placeholderHandler.SpawnReplacement();
    }
    public override Vector2 GetCaseTargetPosition()
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
    void SetInitialValues()
    {
        vfxParent = GetComponentInChildren<VisualEffect>().transform.parent.gameObject;
        relatedCase = WordCaseManager.instance;
        wordParent = this.gameObject;
        relatedText = transform.GetComponentInChildren<TMP_Text>();
        bubbleOffset = GetComponent<BubbleOffset>();
    }
    void CheckIfShouldSetAsPermanentWord()
    {
        data.permanentWord = (data.tag == refM.wordTags[refM.yesNoTagIndex].name) ? true : false;
    }
    void SaveTagInfoAsYarnValuesInTagObject()
    {
        int i = 0;
        foreach (string tag in data.tagInfo)
        {
            if (i != 0)
            {
                Yarn.Value val = WordUtilities.TransformIntoYarnVal(tag);
                ((WordData)data).tagObj.allGivenValues.Add(val);
            }
            i++; //we dont want the location to be in this
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
        if (IsFromWordCase())
        {
            WordCaseManager.instance.UpdateBubbleData(this);
        }
    }
}

