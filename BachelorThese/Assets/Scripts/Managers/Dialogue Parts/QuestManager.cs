using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    //Handles everything happening in the quest case
    public static QuestManager instance { get; private set; }

    float questTextHeight;

    Quest[] quests;
    ReferenceManager refM;
    Scrollbar scrollbar;
    GameObject listingParent;
    TMP_Text questText;

    void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        SetValues();
        InitializeValues();
    }
    void SetValues()
    {
        refM = ReferenceManager.instance;
        scrollbar = refM.questScrollbar;
        listingParent = refM.questListingParent;
        questText = refM.questText;
    }
    void InitializeValues()
    {
        quests = new Quest[refM.maxQuests];
    }
    public void SetQuest(string questName)
    {
        int setAtIndex = GetEmptyQuestSpot();
        if (setAtIndex != -1)
        {
            quests[setAtIndex] = new Quest(questName);
            AutomaticallyOpenLog();
            ScaleScrollbarNew();
        }
        else
            Debug.Log("No free spaces for quests at the moment. Try expanding maxQuests in the refM.");
    }
    public void CompleteQuest(string questName)
    {
        GetQuestReference(questName).isCompleted = true;
        UpdateText();
        AutomaticallyOpenLog();
    }
    public void OpenOrCloseLog()
    {
        refM.questCase.SetActive(!refM.questCase.activeInHierarchy);
        UpdateText();
        ResetScrollbar();
    }
    void AutomaticallyOpenLog()
    {
        refM.questCase.SetActive(true);
        UpdateText();
        ResetScrollbar();
    }
    ref Quest GetQuestReference(string questName)
    {
        for (int i = 0; i < quests.Length; i++)
            if (quests[i].questName == questName)
                return ref quests[i];
        Debug.Log("no reference found to quest " + questName);
        return ref quests[0];
    }
    Quest GetQuest(string questName)
    {
        for (int i = 0; i < quests.Length; i++)
            if (quests[i].questName == questName)
                return quests[i];
        Quest emptyQuest = new Quest("");
        return emptyQuest;
    }
    public bool CheckIfQuestIsActive(string questName)
    {
        Quest quest = GetQuest(questName);
        return quest.questName != "" ? !quest.isCompleted : false;
    }
    Quest[] GetAllQuests()
    {
        List<Quest> filledQuests = new List<Quest>();
        foreach (Quest quest in quests)
        {
            if (quest.questName != null)
            {
                filledQuests.Add(quest);
            }
        }
        return filledQuests.ToArray();
    }
    void UpdateText()
    {
        string spacing = "<br><br>";
        string greyedout = "<alpha=#90>";
        string normalColor = "<alpha=#FF>";

        refM.questText.text = "";
        foreach (Quest quest in GetAllQuests())
        {
            refM.questText.text += (quest.isCompleted) ? greyedout : normalColor;
            refM.questText.text += quest.questDescription;
            refM.questText.text += spacing;
        }
        EffectUtilities.ReColorAllInteractableWords();
    }
    int GetEmptyQuestSpot()
    {
        int i = 0;
        foreach (Quest quest in quests)
        {
            if (quest.questName == null)
            {
                return i;
            }
            i++;
        }
        return -1;
    }
    #region Scrollbar
    public void ChangeScrollbarValue(float scrollValue)
    {
        if (WordClickManager.instance.mouseOverUIObject == "questLog")
        {
            scrollbar.value -= (scrollValue * 0.001f);
        }
    }
    public void ScrollThroughBubbles()
    {
        if (questTextHeight > 0)
        {
            float value = scrollbar.value;
            float posY = Mathf.Lerp(0, questTextHeight, value);
            questText.transform.localPosition = new Vector2(questText.transform.localPosition.x, posY);
        }
    }
    void ResetScrollbar()
    {
        ReferenceManager.instance.bubbleScrollbar.value = 0;
    }
    void ScaleScrollbarNew()
    {
        StartCoroutine(RescaleScrollbar());
    }
    IEnumerator RescaleScrollbar()
    {
        yield return new WaitForEndOfFrame();

        UpdateQuestTextHeight();

        float addedHeight = Mathf.Clamp(questTextHeight, 0, refM.bubbleScreenHeightMaxSize);
        float scrollbarSize = WordUtilities.Remap(addedHeight, 0, refM.bubbleScreenHeightMaxSize, 1, 0.05f);
        scrollbar.size = scrollbarSize;
    }
    void UpdateQuestTextHeight()
    {
        questText.ForceMeshUpdate();

        TMP_TextInfo textInfo = questText.textInfo;
        int lastLetterIndex = textInfo.characterCount - 1;
        TMP_CharacterInfo firstLetter = textInfo.characterInfo[0];
        TMP_CharacterInfo lastLetter = textInfo.characterInfo[lastLetterIndex];

        float firstLetterHeight = firstLetter.topLeft.y;
        float lastLetterHeight = lastLetter.bottomRight.y;
        
        questTextHeight = firstLetterHeight - lastLetterHeight;
        float frameHeight = listingParent.GetComponent<RectTransform>().sizeDelta.y;

        questTextHeight -= frameHeight;
    }
    #endregion
}
public struct Quest
{
    public string questName;
    public string questDescription;
    public bool isCompleted;

    public Quest(string name)
    {
        questName = name;
        isCompleted = false;
        questDescription = "";
        questDescription = GetQuestDescription();
    }

    string GetQuestDescription()
    {
        string description = "";
        if (WordLookupReader.instance.questDescriptions.ContainsKey(questName))
            description = WordLookupReader.instance.questDescriptions[questName];
        else if (questName == "")
            description = "";
        else
            Debug.Log("No Quest in WordLookUpReader named " + questName);
        return description;
    }
}
