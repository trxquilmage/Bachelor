using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestCase : Case
{
    GameObject contentCountObject;
    GameObject dropDownObject;
    Quest quest;
    VerticalLayoutGroupChildren layoutGroup;
    public GameObject wordParent;
    public bool isInCase = false;
    public bool dropDownOpen
    {
        get { return DropDownOpen; }
        set
        {
            DropDownOpen = value;
            ((QuestData)quest.data).dropDownOpen = value;
            Debug.Log("data : " + ((QuestData)quest.data).dropDownOpen);
        }
    }
    bool DropDownOpen;
    #region OVERRIDES
    public override void Initialize()
    {
        base.Initialize();
    }
    public override void InitializeValues()
    {
        base.InitializeValues();
        InitalizeVerticalLayoutGroup();
        InitializeQuestCountObject();
        maxContentAmount = refM.maxQuestAdditions;
        origin = WordInfo.Origin.QuestLog;
        scrollbar = null;
        InitializeHideObject();
        StartCoroutine(InitializeValuesAfterQuest());
    }
    public override void FillArrayWithContents()
    {
        if (contents == null)
            base.FillArrayWithContents();
    }
    //There is no scrollbar so these should be empty
    public override void ScrollThroughBubbles()
    {
    }
    public override void ResetScrollbar()
    {
    }
    public override void UpdateBubbleHeight()
    {
    }
    public override IEnumerator RescaleScrollbar(bool resetScrollbar)
    {
        yield return null;
    }
    public override void SaveBubble(Bubble bubble)
    {
        base.SaveBubble(bubble);
        ((Word)bubble).currentParent = this;
        ForceLayoutGroupUpdate();
    }
    public override void DeleteOutOfCase()
    {
        base.DeleteOutOfCase();
        WordClickManager.instance.currentWord.GetComponent<Word>().currentParent = null;
        ForceLayoutGroupUpdate();
    }
    #endregion
    /// <summary>
    /// Updates the layout group when a child is added, because the layout group doesnt do it on its own
    /// </summary>
    public void ForceLayoutGroupUpdate()
    {
        StartCoroutine(ForceLayoutGroupUp());
    }
    IEnumerator ForceLayoutGroupUp()
    {
        yield return new WaitForEndOfFrame();
        layoutGroup.ForceChildUpdate();
    }
    /// <summary>
    /// Initializes all Values that need to be initialized after the Quest Script was initialized
    /// </summary>
    /// <returns></returns>
    IEnumerator InitializeValuesAfterQuest()
    {
        yield return new WaitForEndOfFrame();
        quest = GetComponent<Quest>();
        if (quest.data.origin == WordInfo.Origin.QuestLog)
        {
            isInCase = true;
            layoutGroup = transform.parent.GetComponent<VerticalLayoutGroupChildren>();
            dropDownOpen = ((QuestData)quest.data).dropDownOpen;
            dropDownObject.GetComponent<OnClickFunctions>().relatedQuest = this;
        }
    }
    /// <summary>
    /// The current VerticalLayoutGroup would have problems with other icons that belong to the QuestBubble
    /// So we are creating it again, one layer lower
    /// </summary>
    void InitalizeVerticalLayoutGroup()
    {
        caseObject = transform.GetChild(1).gameObject;
        caseObject.transform.localPosition = Vector3.zero;
        listingParent = caseObject;
    }
    /// <summary>
    /// Initialize a small counter next to the quest
    /// </summary>
    void InitializeQuestCountObject()
    {
        float height = 0;
        if (wordParent != null)
            height = wordParent.GetComponent<RectTransform>().localPosition.y;

        contentCountObject = transform.GetChild(3).gameObject;
        contentCountObject.transform.localPosition = new Vector2(GetComponent<RectTransform>().sizeDelta.x + 8, height);
        contentCount = contentCountObject.GetComponentInChildren<TMP_Text>();
    }
    void InitializeHideObject()
    {
        float height = 0;
        if (wordParent != null)
            height = wordParent.GetComponent<RectTransform>().localPosition.y;
        dropDownObject = transform.GetChild(2).gameObject;
        dropDownObject.transform.localPosition = new Vector2(-dropDownObject.GetComponent<RectTransform>().sizeDelta.x - 5, height);

    }
    /// <summary>
    /// Takes the object that layouts the words added to this quest and fits its size to the bubble size
    /// </summary>
    public void ChangeAddedParentScale(bool initializing)
    {
        RectTransform rTwordParent = listingParent.GetComponent<RectTransform>();
        RectTransform rTquestParent = wordParent.GetComponent<RectTransform>();
        float height;

        if (initializing)
        {
            VerticalLayoutGroup questLayoutgroup = wordParent.GetComponent<VerticalLayoutGroup>();
            float broadestWidth = 0;
            height = 0;
            RectTransform currentBubble;

            foreach (Image wordSize in wordParent.GetComponentsInChildren<Image>())
            {
                if (wordSize != wordParent.GetComponent<Image>())
                {
                    currentBubble = wordSize.GetComponent<RectTransform>();
                    broadestWidth = (currentBubble.sizeDelta.x > broadestWidth) ? currentBubble.sizeDelta.x : broadestWidth;
                    height += wordSize.GetComponent<RectTransform>().sizeDelta.y;
                    height += questLayoutgroup.spacing;
                }
            }
            //set the quest parent to the width of the broadest width of the quest bubbles

            rTquestParent.sizeDelta = new Vector2(broadestWidth, rTquestParent.sizeDelta.y);
            //set the word parent to the same size
            rTwordParent.sizeDelta = rTquestParent.sizeDelta;
            // move the Indicator bc its too far right at the moment

            InitializeQuestCountObject();
        }
        InitalizeVerticalLayoutGroup();
        InitializeQuestCountObject();
        InitializeHideObject();
    }
}
