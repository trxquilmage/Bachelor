using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    //Handles everything happening in the quest case
    public static QuestManager instance { get; private set; }

    Quest[] quests;
    ReferenceManager refM;
    void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        refM = ReferenceManager.instance;
        quests = new Quest[refM.maxQuests];
    }

    public void SetQuest(string questName)
    {
        int setAtIndex = GetEmptyQuestSpot();
        if (setAtIndex != -1)
        {
            quests[setAtIndex] = new Quest(questName);
            AutomaticallyOpenLog();
        }
        else
            Debug.Log("No free spaces for quests at the moment. Try expanding maxQuests in the refM.");
    }

    public void CompleteQuest(string questName)
    {
        GetQuest(questName).isCompleted = true;
        UpdateText();
        AutomaticallyOpenLog();
    }

    public void OpenOrCloseLog()
    {
        refM.questCase.SetActive(!refM.questCase.activeInHierarchy);
        UpdateText();
    }

    void AutomaticallyOpenLog()
    {
        refM.questCase.SetActive(true);
        UpdateText();
    }

    ref Quest GetQuest(string questName)
    {
        for (int i = 0; i < quests.Length; i++)
            if (quests[i].questName == questName)
                return ref quests[i];
        Debug.Log("Quest " + questName + 
            " was never active, but is currently being seached for. Is there an error somewhere?");
        return ref quests[0];
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
        else
            Debug.Log("No Quest in WordLookUpReader named " + questName);
        return description;
    }
}
