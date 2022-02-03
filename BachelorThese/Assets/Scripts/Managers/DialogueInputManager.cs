using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Yarn.Unity;
using TMPro;

public class DialogueInputManager : MonoBehaviour
{
    public static DialogueInputManager instance;

    [HideInInspector] public bool closeAWindow;

    public ContinueHandler continueHandler;
    WordClickManager wcManager;
    ReferenceManager refM;
    DialogueUI uiHandler;
    InputMap controls;

    private void Awake()
    {
        instance = this;
        controls = new InputMap();
        continueHandler = new ContinueHandler();
    }
    private void Start()
    {
        uiHandler = ReferenceManager.instance.standartDialogueUI;
        wcManager = WordClickManager.instance;
        refM = ReferenceManager.instance;

        controls.Dialogue.Click.performed += context => ContinueTextOnClick();
        controls.Dialogue.DoubleClick.performed += context => CheckWhereDoubleClickHappened();
    }


    public void ContinueButton()
    {
        if (PlayerInputManager.instance.CheckIfAllActivePromptsAreFilled() && !PlayerInputManager.instance.inAsk)
        {
            PlayerInputManager.instance.SaveGivenAnswer();
            Continue(uiHandler);
            continueHandler.OnPromptEnd();
            closeAWindow = true;
            PlayerInputManager.instance.DeleteAllActivePrompts(PlayerInputManager.instance.currentPromptBubbles);
            WordClickManager.instance.currentWord = null;
            WordCaseManager.instance.ReloadContents();
            refM.playerInputField.SetActive(false);
            refM.iCantSayButton.SetActive(false);
        }
    }
    public void AbortContinueButton()
    {
        if (!PlayerInputManager.instance.inAsk)
        {
            Continue(uiHandler);
            continueHandler.OnPromptEnd();
            closeAWindow = true;
            PlayerInputManager.instance.DeleteAllActivePrompts(PlayerInputManager.instance.currentPromptBubbles);
            WordClickManager.instance.currentWord = null;
            WordCaseManager.instance.ReloadContents();
            refM.playerInputField.SetActive(false);
            refM.iCantSayButton.SetActive(false);
        }
    }
    /// <summary>
    /// Called whenever a click happens
    /// </summary>
    void ContinueTextOnClick()
    {
        if (continueHandler.CanContinueDialogue())
        {
            Continue(uiHandler);
            UIManager.instance.OnRightClicked(false);
        }
        else if (continueHandler.CanContinueAsk())
        {
            Continue(refM.askDialogueUI);
            UIManager.instance.OnRightClicked(true);
        }
        else //Shake no feedback for the text box
        {
            if (!PlayerInputManager.instance.inAsk)
                StartCoroutine(EffectUtilities.ShakeNo(refM.nPCDialogueField, 0.3f));
            else
                StartCoroutine(EffectUtilities.ShakeNo(refM.askNPCField, 0.3f));
        }
    }

    public void Continue(DialogueUI dialogue)
    {
        dialogue.MarkLineComplete();
        WordClickManager.instance.DestroyAllActiveWords();
    }

    public void OnLineStart()
    {
        continueHandler.OnTextStart();
    }

    public void OnLineFinished()
    {
        continueHandler.OnTextEnd();
        if (continueHandler.AreAnyPromptsOpen())
            UIManager.instance.StartFeedbackRightClickDialoge();

        ReferenceManager.instance.interactableTextList[0].ForceMeshUpdate();
        EffectUtilities.ReColorAllInteractableWords();
    }

    public IEnumerator ClosePromptMenuAfterContinue(GameObject target)
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (!closeAWindow)
        {
            yield return delay;
        }
        target.SetActive(false);
        closeAWindow = false;

        PlayerInputManager.instance.OnPromptEnd();
    }
    void CheckWhereDoubleClickHappened()
    {
        if (wcManager.wordLastHighlighted != null)
        {
            WordCaseManager.instance.AutomaticOpenCase(true);
            wcManager.SwitchFromHighlightedToCurrent();
            wcManager.currentWord.GetComponent<Bubble>().doubleClickHandler.OnDoubleClicked();
        }
        else
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = WordClickManager.instance.GetMousePos();
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.tag != "IgnoreImageCast")
                {
                    if (result.gameObject.TryGetComponent<Bubble>(out Bubble word))
                    {
                        word.doubleClickHandler.OnDoubleClicked(); //this will result in a wiggle animation
                        break;
                    }
                    else if (result.gameObject.transform.parent.TryGetComponent<Bubble>(out word))
                    {
                        word.doubleClickHandler.OnDoubleClicked(); //this will result in a wiggle animation
                        break;
                    }
                    else if (result.gameObject.transform.parent.parent != null && result.gameObject.transform.parent.parent.TryGetComponent<Bubble>(out word))
                    {
                        word.doubleClickHandler.OnDoubleClicked(); //this will result in a wiggle animation
                        break;
                    }
                }
            }
        }
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
}

public class ContinueHandler
{
    bool currentlyInAPrompt;
    bool currentlyInAnAskPrompt;
    bool currentlyDraggingAWord;
    bool currentlyInAnAsk;
    bool currentlyWritingAskText;
    bool currentlyWritingText;
    bool currentlyInTutorialSituation;

    public ContinueHandler()
    {
        currentlyInAPrompt = false;
        currentlyInAnAskPrompt = false;
        currentlyDraggingAWord = false;
        currentlyInAnAsk = false;
        currentlyWritingAskText = false;
        currentlyWritingText = false;
        currentlyInTutorialSituation = false;
    }

    public bool CanContinueDialogue()
    {
        return (!currentlyWritingText && !currentlyInAPrompt &&
        !currentlyDraggingAWord && !currentlyInAnAsk && !currentlyInTutorialSituation);
    }

    public bool CanContinueAsk()
    {
        return (currentlyInAnAsk && !currentlyInAnAskPrompt &&
            !currentlyWritingAskText && !currentlyDraggingAWord);
    }

    public bool AreAnyPromptsOpen()
    {
        return (currentlyInAPrompt && currentlyInAnAskPrompt);
    }
    public void OnStartAsk()
    {
        currentlyInAnAsk = true;
    }
    public void OnEndAsk()
    {
        currentlyInAnAsk = false;
    }
    public void OnPromptStart()
    {
        currentlyInAPrompt = true;
    }
    public void OnPromptEnd()
    {
        currentlyInAPrompt = false;
    }
    public void OnAskPromptStart()
    {
        currentlyInAnAskPrompt = true;
    }
    public void OnAskPromptEnd()
    {
        currentlyInAnAskPrompt = false;
    }
    public void OnTextStart()
    {
        currentlyWritingText = true;
    }
    public void OnTextEnd()
    {
        currentlyWritingText = false;
    }
    public void OnAskTextStart()
    {
        currentlyWritingAskText = true;
    }
    public void OnAskTextEnd()
    {
        currentlyWritingAskText = false;
    }
    public void OnStartDrag()
    {
        currentlyDraggingAWord = true;
    }
    public void OnEndDrag()
    {
        currentlyDraggingAWord = false;
    }
    public void OnStartTutorial()
    {
        currentlyInTutorialSituation = true;
    }
    public void OnEndTutorial()
    {
        currentlyInTutorialSituation = false;
    }
}