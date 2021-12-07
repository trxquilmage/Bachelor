using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pinwheel.UIEffects;
using UnityEngine.EventSystems;
using UnityEngine.VFX;
using Yarn.Unity;

public class Quest : Bubble
{
    QuestCase questCase;

    public override void Start()
    {
        base.Start();
    }
    public override void Initialize(string name, string[] tags, WordInfo.Origin origin, TMP_WordInfo wordInfo, Vector2 firstAndLastWordIndex)
    {
        VisualEffect[] effects = GetComponentsInChildren<VisualEffect>();
        vfxParent = effects[effects.Length - 1].transform.parent.gameObject;
        relatedCase = QuestManager.instance;
        wordParent = transform.GetChild(0).gameObject;
        base.Initialize(name, tags, origin, wordInfo, firstAndLastWordIndex, out BubbleData bubbleData);

        //if the quest uses another text than the one written down, change what the text says.
        if (bubbleData.tagInfo[1] != "")
            relatedText.text = bubbleData.tagInfo[1];

        data = new QuestData(bubbleData);
        if (bubbleData is QuestData)
        {
            ((QuestData)data).dropDownOpen = ((QuestData)bubbleData).dropDownOpen;
        }
        questCase = GetComponent<QuestCase>();
        questCase.wordParent = wordParent;
        InitializeBubbleShaping(firstAndLastWordIndex);
    }
    public override void Initialize(BubbleData bubbleData, Vector2 firstAndLastWordIndex)
    {
        VisualEffect[] effects = GetComponentsInChildren<VisualEffect>();
        vfxParent = effects[effects.Length - 1].transform.parent.gameObject;
        wordParent = transform.GetChild(0).gameObject;
        base.Initialize(bubbleData, firstAndLastWordIndex);

        //if the quest uses another text than the one written down, change what the text says.
        if (data.tagInfo[1] != "")
            relatedText.text = data.tagInfo[1];

        data.origin = QuestManager.instance.origin;
        data = new QuestData(data);
        ((QuestData)data).dropDownOpen = ((QuestData)bubbleData).dropDownOpen;
        questCase = GetComponent<QuestCase>();
        questCase.wordParent = wordParent;
        InitializeBubbleShaping(firstAndLastWordIndex);
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
            AnimateMovementBackToCase();
        }
    }
    public override void IsOverQuestLog()
    {
        if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
        {
            //save it
            QuestManager.instance.SaveBubble(this);

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
            AnimateMovementBackToCase();
        }
    }
    public override void IsOverQuestCase()
    {
        //pretend to ignore the word 
        IsOverQuestLog();
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (data.origin == WordInfo.Origin.QuestLog)
        {
            QuestManager.instance.UpdateContentList();
            questCase.dropDownScript.OpenCase(false);
            questCase.EnableOrDisableDropDownObject(false);
        }
    }
    public override void Unparent(Transform newParent, bool spawnWordReplacement, bool toCurrentWord)
    {
        base.Unparent(newParent, spawnWordReplacement, toCurrentWord);
        if (spawnWordReplacement)
        {
            if (data.origin == WordInfo.Origin.QuestLog)
                QuestManager.instance.SpawnReplacement(this);
        }
    }
    public override Vector2 GetCaseTargetPosition()
    {
        return ReferenceManager.instance.questCase.GetComponent<RectTransform>().rect.center +
                 (Vector2)WordUtilities.GlobalScreenToCanvasPosition(
                     ReferenceManager.instance.questCase.GetComponent<RectTransform>().position);
    }
    #endregion
}
public class QuestData : BubbleData
{
    public BubbleData bubbleData
    {
        get { return BubbleData; }
        set
        {
            BubbleData = value;
            UpdateBubbleData();
        }
    }
    public BubbleData[] contents
    {
        get { return Contents; }
        set
        {
            Contents = value;
            UpdateBubbleData();
        }
    }
    public string[] relevantWords
    {
        get { return RelevantWords; }
        set
        {
            RelevantWords = value;
            UpdateBubbleData();
        }
    }
    public bool dropDownOpen
    {
        get { return DropDownOpen; }
        set
        {
            DropDownOpen = value;
            UpdateBubbleData();
        }
    }

    BubbleData BubbleData;
    BubbleData[] Contents;
    string[] RelevantWords;
    bool DropDownOpen;
    public QuestData(BubbleData data) : base()
    {
        name = data.name;
        tag = data.tag;
        tagInfo = data.tagInfo;
        origin = data.origin;
        lineLengths = data.lineLengths;
        isLongWord = data.isLongWord;
        bubbleData = data;
        permanentWord = data.permanentWord;
        isFavorite = data.isFavorite;

        if (name != null)
        {
            int maxQuestAdditions = ReferenceManager.instance.maxQuestAdditions;
            contents = new BubbleData[maxQuestAdditions];
            relevantWords = new string[maxQuestAdditions];
            for (int i = 0; i < maxQuestAdditions; i++)
            {
                contents[i] = new BubbleData();
                if (data.tagInfo.Length > i + 1)
                    relevantWords[i] = data.tagInfo[i + 1];
                else
                    relevantWords[i] = null;
            }
        }
    }
    public override void UpdateBubbleData()
    {
        base.UpdateBubbleData();
        if (this is QuestData)
            QuestManager.instance.UpdateBubbleData(this);
    }
}
