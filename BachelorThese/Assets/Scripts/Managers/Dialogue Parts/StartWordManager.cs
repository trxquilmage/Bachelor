using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartWordManager : MonoBehaviour
{
    public static StartWordManager instance;
    ReferenceManager refM;

    void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        refM = ReferenceManager.instance;

        //Only starts the game with this screen if its enabled
        if (refM.startWithStartWords)
            CallStartWordScreen();
    }
    public void CallStartWordScreen()
    {
        refM.mainStartWordParent.SetActive(true);
        refM.otherUIParent.SetActive(false);
        DialogueInputManager.instance.enabled = true;
        DialogueManager.instance.isInDialogue = true;
    }
    public void OnContinue(int i)
    {
        switch (i)
        {
            case 0:
                if (CheckForAllQuestionsAnswered())
                {
                    refM.questionParent.SetActive(false);
                    refM.traitParent.SetActive(true);
                    SaveStartWords(refM.questionParent, false);
                }
                break;
            case 1:
                refM.traitParent.SetActive(false);
                refM.generatorParent.SetActive(true);
                SaveStartWords(refM.traitParent, true);
                ShowSelectedWords();
                refM.otherUIParent.SetActive(true);
                refM.otherUIParent.transform.GetChild(1).gameObject.SetActive(false);
                break;
            case 2:
                refM.otherUIParent.transform.GetChild(1).gameObject.SetActive(true);
                EndStartWordScreen();
                break;
        }
    }
    /// <summary>
    ///  Go through all questions and check if they are answered
    /// </summary>
    /// <returns></returns>
    bool CheckForAllQuestionsAnswered()
    {
        foreach (StartQuestion question in refM.questionParent.GetComponentsInChildren<StartQuestion>())
        {
            if (!question.questionAnswered)
                return false;
        }
        return true;
    }
    /// <summary>
    /// Save the related words of the given answers.
    /// </summary>
    void SaveStartWords(GameObject parent, bool moreThanOneAnswer)
    {
        foreach (StartQuestion question in parent.GetComponentsInChildren<StartQuestion>())
        {
            //if there is only one possible answer to the question
            if (!moreThanOneAnswer)
            {
                refM.startWords.AddRange(question.currentlySelected.relatedWords);
                refM.startCommands.AddRange(question.currentlySelected.relatedCommands);
            }
            else
                foreach (StartTrait trait in question.allCurrentlySelected)
                {
                    if (trait != null)
                    {
                        refM.startWords.AddRange(trait.relatedWords);
                        refM.startCommands.AddRange(trait.relatedCommands);
                    }
                }
        }
    }
    /// <summary>
    /// Take all the words we got from the questions and portray them on the screen.
    /// </summary>
    void ShowSelectedWords()
    {
        string text = "";
        string spacing = "    ";
        refM.startWords = GoThroughGivenCommands(refM.startWords);
        foreach (string word in refM.startWords)
            text += word + spacing;
        refM.showWordsText.text = text;
        EffectUtilities.ReColorAllInteractableWords();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    List<string> GoThroughGivenCommands(List<string> startWords)
    {
        foreach (string command in refM.startCommands)
        {
            switch (command)
            {
                case "removeDoubles":
                    startWords = RemoveDoubles(startWords);
                    break;
                case "removeYes":
                    startWords = RemoveWord(startWords, "Yes");
                    break;
                default:
                    break;
            }
        }
        return startWords;
    }
    /// <summary>
    /// Takes all given words and removes the words that were added more than once
    /// </summary>
    /// <param name="startWords"></param>
    /// <returns></returns>
    List<string> RemoveDoubles(List<string> startWords)
    {
        List<string> controlList = new List<string>();
        for (int i = 0; i < startWords.Count; i++)
        {
            if (!controlList.Contains(startWords[i]))
                controlList.Add(startWords[i]);
        }
        return controlList;
    }
    /// <summary>
    /// Takes all given words and removes the word that is given as an argument
    /// </summary>
    /// <returns></returns>
    List<string> RemoveWord(List<string> startWords, string wordToRemove)
    {
        if (startWords.Contains(wordToRemove))
            startWords.Remove(wordToRemove);
        return startWords;
    }
    /// <summary>
    /// Close the StartWord Screen again, after everything is done.
    /// </summary>
    public void EndStartWordScreen()
    {
        refM.mainStartWordParent.SetActive(false);
        refM.generatorParent.SetActive(false);
        refM.otherUIParent.SetActive(true);
        DialogueInputManager.instance.enabled = false;
        DialogueManager.instance.isInDialogue = false;
    }
}
