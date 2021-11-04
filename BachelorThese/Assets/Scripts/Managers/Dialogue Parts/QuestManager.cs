using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    //Handles everything happening in the quest case
    public static QuestManager instance;
    bool alreadyOpen;
    public Word.WordData[] allQuests;
    ReferenceManager refM;
    public GameObject wordReplacement;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        refM = ReferenceManager.instance;
        allQuests = new Word.WordData[refM.maxQuestCount];
        UpdateQuestCount();
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
    }
    /// <summary>
    /// Open case manually through UI. will not close on AutomaticOpenCase(false)
    /// </summary>
    public void ManuallyOpenCase()
    {
        ReferenceManager.instance.questCase.SetActive(!ReferenceManager.instance.questCase.activeInHierarchy);
        ReloadQuests();
    }
    /// <summary>
    /// Reloads existing Quests.
    /// </summary>
    void ReloadQuests()
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
        foreach (Word.WordData word in allQuests)
        {
            if (word.name != null)
            {
                SpawnQuestInLog(word);
            }
        }
        UpdateQuestCount();
        RescaleScrollbar();
    }
    /// <summary>
    /// Updates the Quest-count text UI of the quest case
    /// </summary>
    public void UpdateQuestCount()
    {
        int wordCount = 0;
        foreach (Word.WordData word in allQuests)
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
    public void SaveQuest(Word wordItem)
    {
        Word.WordData word = wordItem.data;
        if (!CheckIfQuestInList(word))
        {
            bool foundASpot = false;
            for (int i = 0; i < ReferenceManager.instance.maxQuestCount; i++)
            {
                if (allQuests[i].name == null)
                {
                    allQuests[i] = word;
                    foundASpot = true;
                    break;
                }
            }
            //Reload
            ReloadQuests();
            WordUtilities.ReColorAllInteractableWords();

            if (!foundASpot) //word list full, put the word back into the dialogue
            {
                WordUtilities.ReturnWordIntoText(wordItem);
            }
        }
        else
        {
            //put the word back into the dialogue
            WordUtilities.ReturnWordIntoText(wordItem);
        }
    }
    /// <summary>
    /// Checks, if the Quest that is supposed to be saved is already in the list
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    bool CheckIfQuestInList(Word.WordData word)
    {
        bool inList = false;
        foreach (Word.WordData data in allQuests)
        {
            if (data.name != null && data.name == word.name)
            {
                inList = true;
            }
        }
        return inList;
    }
    /// <summary>
    /// Delete currentWord out of the QuestLog
    /// </summary>
    public void DeleteOutOfLog()
    {
        Word.WordData data = WordClickManager.instance.currentWord.GetComponent<Word>().data;
        int deleteInt = -1;
        int i = 0;
        foreach (Word.WordData word in allQuests)
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
            allQuests[deleteInt] = new Word.WordData();
        }
        else
            Debug.Log("The quest to delete couldne be found " + data.name);
        ReloadQuests();
    }
    /// <summary>
    /// Create a less visible version of the quest in the wordcase to indicate its position
    /// </summary>
    /// <param name="word"></param>
    public void SpawnQuestReplacement(Word word)
    {
        wordReplacement = SpawnQuestInLog(word.data);

        Color color = word.GetComponent<Image>().color;
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
    GameObject SpawnQuestInLog(Word.WordData data)
    {
        GameObject bubble = WordUtilities.CreateWord(data, Vector2.zero,Vector2.zero, WordInfo.Origin.QuestLog, true);
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
    void RescaleScrollbar()
    {
        int questCount = 0;
        foreach (Word.WordData data in allQuests)
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
        Mathf.Clamp(questCount, refM.spaceForQuestsOnCanvas, refM.scrollbarMaxSize);
        //between biggest size (1) and smallest we want (0.05f)
        float scrollbarSize = WordUtilities.Remap(questCount, refM.spaceForQuestsOnCanvas, refM.scrollbarMaxSize, 1, 0.05f);
        float overlappingWords = Mathf.Clamp(questCount - refM.spaceForQuestsOnCanvas, 0, Mathf.Infinity);
        refM.currQuestScrollbarDistance = overlappingWords * refM.questScrollbarDistance;
        scrollbar.size = scrollbarSize;
    }
    #endregion
}
