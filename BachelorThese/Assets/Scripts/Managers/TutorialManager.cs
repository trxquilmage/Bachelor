using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    TransformValues defaultStartTransform;

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
        if (tutorialOn)
        {
            player = refM.player;
            EnableOrDisableHighlightingWords(false);
            EnableOrDisableAskButton(false);
        }
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