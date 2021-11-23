using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : Case
{
    //Handles everything happening in the quest case
    public static QuestManager instance;


    #region OVERRIDES
    public override void Awake()
    {
        instance = this;
    }
    public override void Initialize()
    {
        base.Initialize();
    }
    public override void InitializeValues()
    {
        base.InitializeValues();
        caseObject = refM.questCase;
        listingParent = refM.questListingParent;
        contentCount = refM.questLimit;
        maxContentAmount = refM.maxQuestCount;
        origin = WordInfo.Origin.QuestLog;
        scrollbar = refM.questScrollbar;
    }
    public override void ReloadContents(bool resetScrollbar)
    {
        UpdateContentList();
        //then use this data to reload the set
        base.ReloadContents(resetScrollbar);
    }
    public void UpdateContentList()
    {
        //save the "contents" array of each questCase into the fitting bubbleData that is located on the same quest
        foreach (QuestCase questCase in refM.questListingParent.GetComponentsInChildren<QuestCase>())
        {
            questCase.SaveContentsToQuest();
        }
    }
    #endregion
    public void Start()
    {
        Initialize();
    }
}
