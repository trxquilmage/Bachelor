using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWordManager : MonoBehaviour
{
    public static StartWordManager instance;
    ReferenceManager refM;

    [Header("Input")]
    [SerializeField] StartTrait[] traits;
    [Header("References")]
    [SerializeField] GameObject mainParent;
    [SerializeField] GameObject questionParent;
    [SerializeField] GameObject traitParent;
    [SerializeField] GameObject wordParent;
    List<string> startWords = new List<string>();

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
        mainParent.SetActive(true);
        refM.otherUIParent.SetActive(false);
    }
    public void OnContinue(int i)
    {
        switch (i)
        {
            case 0:
                if (CheckForAllQuestionsAnswered())
                {
                    questionParent.SetActive(false);
                    traitParent.SetActive(true);
                    SaveStartWords();
                }
                break;
            case 1:
                if (CheckIfCorrectAmountOfTraitsGiven())
                {
                    traitParent.SetActive(false);
                    wordParent.SetActive(true);
                }
                break;
            case 2:
                EndStartWordScreen();
                break;
        }
    }
    bool CheckIfCorrectAmountOfTraitsGiven()
    {
        int traitCount = 0;
        foreach (StartTrait trait in traitParent.GetComponentsInChildren<StartTrait>())
        {
            if (trait.toggle.isOn)
                traitCount++;
        }
        if (traitCount > 5)
            return false;
        return true;
    }
    /// <summary>
    ///  Go through all questions and check if they are answered
    /// </summary>
    /// <returns></returns>
    bool CheckForAllQuestionsAnswered()
    {
        foreach (StartQuestion question in questionParent.GetComponentsInChildren<StartQuestion>())
        {
            if (!question.questionAnswered)
                return false;
        }
        return true;
    }
    /// <summary>
    /// Save the related words of the given answers.
    /// </summary>
    void SaveStartWords()
    {
        foreach (StartQuestion question in questionParent.GetComponentsInChildren<StartQuestion>())
        {
            startWords.AddRange(question.currentlySelected.relatedWords);
        }
    }
    /// <summary>
    /// Close the StartWord Screen again, after everything is done.
    /// </summary>
    public void EndStartWordScreen()
    {
        mainParent.SetActive(false);
        wordParent.SetActive(false);
        refM.otherUIParent.SetActive(true);
    }
}
