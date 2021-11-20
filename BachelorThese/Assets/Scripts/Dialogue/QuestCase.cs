using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestCase : Case
{
    GameObject contentCountObject;
    GameObject dropDownObject;
    public GameObject wordParent;
    public bool isInCase = false;
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
        if (origin == WordInfo.Origin.QuestLog)
            isInCase = true;
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
        ChangeAddedParentScale(false);
    }
    public override void DeleteOutOfCase()
    {
        base.DeleteOutOfCase();
        WordClickManager.instance.currentWord.GetComponent<Word>().currentParent = null;
        ChangeAddedParentScale(false);
    }
    #endregion

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
        dropDownObject.GetComponent<OnClickFunctions>().relatedQuest = this;
    }
    IEnumerator ChangeAnchorPoint()
    {
        RectTransform rT = GetComponent<RectTransform>();
        RectTransform rTwordParent = wordParent.GetComponent<RectTransform>();
        rT.pivot = Vector2.up;
        rT.localPosition += new Vector3(0, rT.sizeDelta.y, 0);

        yield return new WaitForEndOfFrame();

        float questLocalHeight = rT.sizeDelta.y - rTwordParent.sizeDelta.y;
        rTwordParent.localPosition = new Vector3(0, questLocalHeight, 0);
        InitalizeVerticalLayoutGroup();
        InitializeQuestCountObject();
        InitializeHideObject();
    }
    /// <summary>
    /// Takes the object that layouts the words added to this quest and fits its size to the bubble size
    /// </summary>
    public void ChangeAddedParentScale(bool initializing)
    {
        RectTransform rTwordParent = listingParent.GetComponent<RectTransform>();
        RectTransform rTquestParent = wordParent.GetComponent<RectTransform>();
        RectTransform generalParent = GetComponent<RectTransform>();
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

            rTquestParent.sizeDelta = new Vector2(broadestWidth, height);
            //set the word parent to the same size
            rTwordParent.sizeDelta = rTquestParent.sizeDelta;
            // move the Indicator bc its too far right at the moment

            InitializeQuestCountObject();
        }

        VerticalLayoutGroup wordLayoutgroup = listingParent.GetComponent<VerticalLayoutGroup>();
        height = rTwordParent.sizeDelta.y;

        // Get the height of all word bubbles
        if (wordLayoutgroup.isActiveAndEnabled)
        {
            foreach (Image wordSize in listingParent.GetComponentsInChildren<Image>())
            {
                if (wordSize != listingParent.GetComponent<Image>())
                {
                    height += wordSize.GetComponent<RectTransform>().sizeDelta.y;
                    height += wordLayoutgroup.spacing;
                }
            }
        }

        generalParent.sizeDelta = new Vector2(generalParent.sizeDelta.x, height);
        ChangeAnchorPoint();
    }
}
