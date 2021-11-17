using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    //Handles everything happening in the quest case
    public static QuestManager instance;
    bool alreadyOpen;
    public QuestData[] allQuests;
    ReferenceManager refM;
    public GameObject wordReplacement;
    public int questTagIndex = 2; //in the tag list, the number of the Quest Tag
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        refM = ReferenceManager.instance;
        FillAllQuests();
        UpdateQuestCount();
    }
    /// <summary>
    /// As QuestData needas a Constructor, we cant just create an empty array
    /// </summary>
    void FillAllQuests()
    {
        allQuests = new QuestData[refM.maxQuestCount];
        for (int i = 0; i < refM.maxQuestCount; i++)
        {
            allQuests[i] = new QuestData(new BubbleData());
        }
    }
    /// <summary>
    /// Opens case and memorizes, if it was open before. If not, it will close on false.
    /// </summary>
    /// <param name="open"></param>
    public void AutomaticOpenCase(bool open)
    {
        // open the case
        if (open)
        {
            if (ReferenceManager.instance.questCase.activeInHierarchy)
            {
                alreadyOpen = true;
            }
            ReferenceManager.instance.questCase.SetActive(true);
        }
        //close the case
        else
        {
            if (!alreadyOpen)
            {
                ReferenceManager.instance.questCase.SetActive(false);
            }
        }
        ResetScrollbar();
    }
    /// <summary>
    /// Open case manually through UI. will not close on AutomaticOpenCase(false)
    /// </summary>
    public void ManuallyOpenCase()
    {
        ReferenceManager.instance.questCase.SetActive(!ReferenceManager.instance.questCase.activeInHierarchy);
        ReloadQuests();
        ResetScrollbar();
    }
    /// <summary>
    /// Reloads existing Quests.
    /// </summary>
    public void ReloadQuests()
    {
        // Delete all possibly active bubbles
        foreach (Transform word in refM.questListingParent.GetComponentsInChildren<Transform>())
        {
            if (word.gameObject != refM.questListingParent)
            {
                Destroy(word.gameObject);
            }
        }

        // Spawn words again
        foreach (QuestData word in allQuests)
        {
            if (word.name != null)
            {
                SpawnQuestInLog(word);
            }
        }
        UpdateQuestCount();
        RescaleScrollbar();
        DestroyQuestReplacement();
    }
    /// <summary>
    /// Updates the Quest-count text UI of the quest case
    /// </summary>
    public void UpdateQuestCount()
    {
        int wordCount = 0;
        foreach (QuestData word in allQuests)
        {
            if (word.name != null)
                wordCount++;
        }
        ReferenceManager.instance.questLimit.text =
            wordCount.ToString() + "<b>/" + ReferenceManager.instance.maxQuestCount + "</b>";
    }
    /// <summary>
    /// Saves quests that are dragged onto the quest log
    /// </summary>
    /// <param name="wordItem"></param>
    public void SaveQuest(Quest wordItem)
    {
        QuestData word = wordItem.questData;
        if (CheckIfCanSaveQuest(word.name, out int index))
        {
            allQuests[index] = word;
        }
        else
        {
            //put the word back into the dialogue
            WordUtilities.ReturnWordIntoText(wordItem);
        }
        //Reload
        ReloadQuests();
        EffectUtilities.ReColorAllInteractableWords();
    }
    /// <summary>
    /// Checks, if the Quest that is supposed to be saved is already in the list
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    bool CheckIfQuestInList(string name)
    {
        bool inList = false;
        foreach (QuestData data in allQuests)
        {
            if (data.name != null && data.name == name)
            {
                inList = true;
            }
        }
        return inList;
    }
    public bool CheckIfCanSaveQuest(string name, out int index)
    {
        index = -1;
        bool inQuestList;
        if (ReferenceManager.instance.duplicateWords)
            inQuestList = false;
        else
            inQuestList = CheckIfQuestInList(name);

        if (!inQuestList)
        {
            bool foundASpot = false;
            for (int i = 0; i < ReferenceManager.instance.maxQuestCount; i++)
            {
                if (allQuests[i].name == null)
                {
                    index = i;
                    foundASpot = true;
                    break;
                }
            }
            if (foundASpot)
                return true;
            return false;
        }
        return false;
    }
    /// <summary>
    /// Delete currentWord out of the QuestLog
    /// </summary>
    public void DeleteOutOfLog()
    {
        QuestData data = WordClickManager.instance.currentWord.GetComponent<Quest>().questData;
        int deleteInt = -1;
        int i = 0;
        foreach (QuestData word in allQuests)
        {
            if (word.name == data.name)
            {
                deleteInt = i;
                break;
            }
            i++;
        }
        if (deleteInt > -1)
        {
            allQuests[deleteInt] = new QuestData(new BubbleData());
        }
        else
            Debug.Log("The quest to delete couldne be found " + data.name);
        ReloadQuests();
    }
    /// <summary>
    /// Create a less visible version of the quest in the wordcase to indicate its position
    /// </summary>
    /// <param name="word"></param>
    public void SpawnQuestReplacement(Quest word)
    {
        wordReplacement = SpawnQuestInLog(word.questData);
        Color color = refM.wordTags[questTagIndex].tagColor;
        color.a = 0.3f;
        foreach (Image img in wordReplacement.GetComponentsInChildren<Image>())
        {
            img.color = color;
        }
    }
    /// <summary>
    /// Destory the transparent bubble, that gives feedback that the word is still being dragged
    /// </summary>
    public void DestroyQuestReplacement()
    {
        if (wordReplacement != null)
        {
            Destroy(wordReplacement);
            wordReplacement = null;
        }
    }
    /// <summary>
    /// Create a word that is spawned in the quest log
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    GameObject SpawnQuestInLog(QuestData data)
    {
        //MISSING: QUEST
        GameObject bubble = WordUtilities.CreateWord(data, Vector2.zero, Vector2.zero, WordInfo.Origin.QuestLog);
        //place bubbles correctly (for protoype: below each other until i find out how the UI is supposed to work)
        bubble.transform.SetParent(refM.questListingParent.transform);
        return bubble;
    }
    #region Scrollbar
    /// <summary>
    /// Called when the quest scrollbar is scrolled. moves the words up & down. resets on tag.
    /// </summary>
    public void ScrollThroughQuests()
    {
        float value = ReferenceManager.instance.questScrollbar.GetComponent<Scrollbar>().value;
        //currquestscrollbardistance is being added to whenever a quest is spawned
        ReferenceManager.instance.questListingParent.transform.localPosition = new Vector2(0, value * ReferenceManager.instance.currQuestScrollbarDistance);
    }
    /// <summary>
    /// Takes the ScrollBar and re-scales it according to the current amount of words
    /// </summary>
    /// <param name="bubbleCount"></param>
    public void RescaleScrollbar()
    {
        int questCount = 0;
        foreach (QuestData data in allQuests)
        {
            if (data.lineLengths != null)
            {
                questCount += data.lineLengths.Length;
            }
            else
            {
                questCount++;
            }
        }
        Scrollbar scrollbar = refM.questScrollbar;
        //if the value is smaller than what fits the canvas, it is irrelevant
        Mathf.Clamp(questCount, refM.spaceForQuestsOnCanvas, refM.bubbleScreenHeightMaxSize);
        //between biggest size (1) and smallest we want (0.05f)
        float scrollbarSize = WordUtilities.Remap(questCount, refM.spaceForQuestsOnCanvas, refM.bubbleScreenHeightMaxSize, 1, 0.05f);
        float overlappingWords = Mathf.Clamp(questCount - refM.spaceForQuestsOnCanvas, 0, Mathf.Infinity);
        refM.currQuestScrollbarDistance = overlappingWords * refM.questScrollbarDistance;
        scrollbar.size = scrollbarSize;
    }
    public void ResetScrollbar()
    {
        refM.questScrollbar.value = 0;
    }
    #endregion
}
