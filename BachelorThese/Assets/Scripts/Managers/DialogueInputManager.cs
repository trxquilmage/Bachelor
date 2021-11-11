using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Yarn.Unity;
using TMPro;

public class DialogueInputManager : MonoBehaviour
{
    public static DialogueInputManager instance;

    public bool continueEnabledPrompt = true;
    public bool continueEnabledPromptAsk = true;
    public bool continueEnabledDrag = true;
    public bool continueEnabledAsk = true;
    public bool closeAWindow;
    public bool askTextFinished;

    WordClickManager wcManager;
    DialogueUI uiHandler;
    InputMap controls;
    bool textFinished;

    private void Awake()
    {
        instance = this;
        controls = new InputMap();
    }
    private void Start()
    {
        uiHandler = ReferenceManager.instance.standartDialogueUI;
        wcManager = WordClickManager.instance;
        controls.Dialogue.Click.performed += context => ContinueTextOnClick();
        controls.Dialogue.DoubleClick.performed += context => CheckDoubleClick();
    }

    /// <summary>
    /// disables continue for the time the ask menu is open
    /// </summary>
    /// <param name="open"></param>
    public void AskMenuOpen(bool open)
    {
        continueEnabledAsk = !open;
    }
    /// <summary>
    /// Called, when the Continue Button in the UI is pressed
    /// </summary>
    public void ContinueButton()
    {
        if (PlayerInputManager.instance.CheckForPromptsFilled(PlayerInputManager.instance.currentPromptBubbles) && continueEnabledAsk)
        {
            PlayerInputManager.instance.SaveGivenAnswer(PlayerInputManager.instance.currentPromptBubbles);
            Continue(uiHandler);
            continueEnabledPrompt = true;
            closeAWindow = true;
            PlayerInputManager.instance.DeleteAllPrompts(PlayerInputManager.instance.currentPromptBubbles);
            WordClickManager.instance.currentWord = null;
            WordCaseManager.instance.OpenOnTag(false); //Reload, so that the missing word comes back
            ReferenceManager.instance.playerInputField.SetActive(false);
        }
    }
    /// <summary>
    /// Called whenever a click happens
    /// </summary>
    void ContinueTextOnClick()
    {
        if (textFinished && continueEnabledPrompt
            && continueEnabledDrag && continueEnabledAsk)
        {
            Continue(uiHandler);
        }
        else if (!continueEnabledAsk && continueEnabledPromptAsk && askTextFinished) //continue the ask text instead
        {
            Continue(ReferenceManager.instance.askDialogueUI);
        }
    }
    /// <summary>
    /// Called, whenever a dialogue should be continued
    /// </summary>
    public void Continue(DialogueUI dialogue)
    {
        dialogue.MarkLineComplete();
        //Destroy all buttons you can find
        WordClickManager.instance.DestroyAllActiveWords();
    }
    /// <summary>
    /// When a line is not yet done (called in Dialogue Runner) -> disable continue for ContinueText()
    /// </summary>
    public void TextFinished()
    {
        textFinished = true;

        // Color all interactable words, force update, so there are no errors
        ReferenceManager.instance.interactableTextList[0].ForceMeshUpdate();
        EffectUtilities.ReColorAllInteractableWords();
    }
    /// <summary>
    /// When a line is done (called in Dialogue Runner) -> enable continue  for ContinueText()
    /// </summary>
    public void TextUnfinished()
    {
        textFinished = false;
    }
    /// <summary>
    /// Close the Prompt Menu, after pressing Continue
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public IEnumerator CloseAWindow(GameObject target)
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (!closeAWindow)
        {
            yield return delay;
        }
        target.SetActive(false);
        closeAWindow = false;
    }
    /// <summary>
    /// after a double click was performed, check if it was on a word. Save the word to the related box immediately.
    /// </summary>
    void CheckDoubleClick()
    {
        if (wcManager.wordLastHighlighted != null)
        {
            bool isQuest = wcManager.wordLastHighlighted.GetComponent<Word>().data.tag == WordInfo.WordTags.Quest;
            if (isQuest)
                QuestManager.instance.AutomaticOpenCase(true);
            else
                WordCaseManager.instance.AutomaticOpenCase(true);
            wcManager.SwitchFromHighlightedToCurrent();
            wcManager.currentWord.GetComponent<Word>().MoveToCase(isQuest);
        }
        //Check if above a word
        else
        {
            //Raycast For words that may be underneath the mouse pos
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = WordClickManager.instance.GetMousePos();
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            foreach(RaycastResult result in results)
            {
                if (result.gameObject.TryGetComponent<Word>(out Word word))
                {
                    bool isQuest = word.data.tag == WordInfo.WordTags.Quest;
                    word.MoveToCase(isQuest); //this will result in a wiggle animation
                    break;
                }
                else if (result.gameObject.transform.parent.TryGetComponent<Word>(out word))
                {
                    bool isQuest = word.data.tag == WordInfo.WordTags.Quest;
                    word.MoveToCase(isQuest); //this will result in a wiggle animation
                    break;
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
