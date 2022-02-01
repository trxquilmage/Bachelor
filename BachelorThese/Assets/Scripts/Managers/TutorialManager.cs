using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    public bool askQuestionDone;

    TransformValues defaultStartTransform;
    Tutorial[] tutorials;

    GameObject player;
    ReferenceManager refM;
    bool tutorialOn = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetValues();
        if (tutorialOn)
            PlacePlayerIntoTutorial();
    }

    void SetValues()
    {
        refM = ReferenceManager.instance;
        tutorialOn = refM.startWithTutorial;
        tutorials = new Tutorial[] { new SaveAWord(), new AskAQuestion() };
        if (tutorialOn)
        {
            player = refM.player;
            EnableOrDisableHighlightingWords(false);
            EnableOrDisableAskButton(false);
        }
    }
    public void AskQuestionsDone()
    {
        askQuestionDone = true;
    }
    public void DisableContinueUntilTutorialStepIsDone(int tutorialStepNumber)
    {
        DialogueInputManager.instance.continueHandler.OnStartTutorial();
        StartCoroutine(WaitUntilTutorialConditionIsMet(tutorials[tutorialStepNumber]));
    }

    IEnumerator WaitUntilTutorialConditionIsMet(Tutorial tutorial)
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        bool tutorialIsDone = false;

        while(!tutorialIsDone)
        {
            tutorialIsDone = tutorial.CheckForCondition();
            yield return delay;
        }
        DialogueInputManager.instance.continueHandler.OnEndTutorial();
    }
    void PlacePlayerIntoTutorial()
    {
        SaveOldTransform();
        PlacePlayerAtPosition(refM.tutorialStartPosition);
    }
    void PlacePlayerAtPosition(TransformValues position)
    {
        position.SetTransform(player.transform);
    }
    void SaveOldTransform()
    {
        defaultStartTransform = new TransformValues(player.transform);
    }
    public void EnableOrDisableHighlightingWords(bool setActive)
    {
        refM.highlightedWordsEnabled = setActive;
    }
    public void EnableOrDisableAskButton(bool setActive)
    {
        refM.ask.SetActive(setActive);
    }
    public void EndTutorial()
    {
        PlacePlayerAtPosition(defaultStartTransform);
    }
}

public interface Tutorial
{
    public abstract bool CheckForCondition();
}

public class SaveAWord : Tutorial
{
    ReferenceManager refM;
    WordCaseManager wcM;
    public SaveAWord()
    {
        refM = ReferenceManager.instance;
        wcM = WordCaseManager.instance;
    }

    public bool CheckForCondition()
    {
        int wordCountInCase = wcM.GetTagWordCount(refM.wordTags[refM.allTagIndex].name);
        bool hasAtLeastOneWordSaved = wordCountInCase > 0;
        return hasAtLeastOneWordSaved;
    }
}

public class AskAQuestion : Tutorial
{
    ReferenceManager refM;
    TutorialManager tutorialManager;
    public AskAQuestion()
    {
        refM = ReferenceManager.instance;
        tutorialManager = TutorialManager.instance;
    }

    public bool CheckForCondition()
    {
        return tutorialManager.askQuestionDone;
    }
}