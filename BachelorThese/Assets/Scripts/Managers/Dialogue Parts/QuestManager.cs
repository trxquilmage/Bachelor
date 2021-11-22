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

    #endregion
    public void Start()
    {
        Initialize();
    }
}
