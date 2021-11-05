using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using TMPro;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    public PromptBubble[] currentPromptBubbles;
    public PromptBubble[] currentPromptAskBubbles;
    public Word.WordData givenAnswer;
    public Word.WordData givenAnswerAsk;
    public bool givenManualPrompt;
    DialogueInputManager diManager;
    WordLookupReader wlReader;
    ReferenceManager refM;
    InfoManager info;
    DialogueUI uiHandler;
    DialogueRunner runner;
    bool promptCurrentlyDisabled;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        currentPromptBubbles = new PromptBubble[3];
        currentPromptAskBubbles = new PromptBubble[3];
        diManager = DialogueInputManager.instance;
        wlReader = WordLookupReader.instance;
        refM = ReferenceManager.instance;
        info = InfoManager.instance;
        runner = ReferenceManager.instance.askRunner;
        uiHandler = ReferenceManager.instance.askDialogueUI;
        //disable askrunner
        ReferenceManager.instance.askRunner.gameObject.SetActive(false);
    }
    /// <summary>
    /// saves the Info given, after pressing continue
    /// </summary>
    public void SaveGivenAnswer(PromptBubble[] promptBubbles)
    {
        foreach (PromptBubble prompt in promptBubbles)
        {
            if (prompt != null && prompt.child != null)
            {
                if (promptBubbles == currentPromptBubbles)
                    givenAnswer = prompt.child.GetComponent<Word>().data;
                else if (promptBubbles == currentPromptAskBubbles)
                    givenAnswerAsk = prompt.child.GetComponent<Word>().data;
            }
        }
    }
    /// <summary>
    /// Checks, whether all prompts are filled at the moment
    /// </summary>
    public bool CheckForPromptsFilled(PromptBubble[] promptBubbles)
    {
        bool allFilled = true;
        foreach (PromptBubble prompt in promptBubbles)
        {
            if (prompt != null && prompt.child == null) //doesnt have a child
            {
                allFilled = false;
            }
        }
        return allFilled;
    }
    /// <summary>
    /// Save a prompt, after it was created
    /// </summary>
    /// <param name="bubble"></param>
    public void SavePrompt(PromptBubble bubble, PromptBubble[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
            {
                array[i] = bubble;
                break;
            }
        }
    }
    /// <summary>
    /// Delete all prompts on the screen
    /// </summary>
    public void DeleteAllPrompts(PromptBubble[] promptBubbles)
    {
        for (int i = 0; i < promptBubbles.Length; i++)
        {
            if (promptBubbles[i] != null)
            {
                Destroy(promptBubbles[i].gameObject);
                promptBubbles[i] = null;
            }
        }
    }
    /// <summary>
    /// Check what kind of answer/sub tag the text wants back, then save it in the given spot
    /// </summary>
    /// <param name="lookingFor"></param>
    /// <param name="saveIn"></param>
    /// <param name="isAsk"></param>
    /// <returns></returns>
    public Yarn.Value ReactToInput(string lookingFor, string NPCname, string saveIn, bool isAsk)
    {
        Word.WordData data;
        if (!isAsk)
        {
            data = givenAnswer;
        }
        else
        {
            data = givenAnswerAsk;
        }

        Word.TagObject tagObj = data.tagObj;
        Yarn.Value val = InfoManager.instance.FindValue(data, lookingFor);
        //save the required Info
        if (CheckIfShouldSave(saveIn)) { info.SaveInfo(saveIn, val, NPCname, tagObj); }

        return val;
    }
    /// <summary>
    /// wites out the given prompt & adds the prompt bubbles
    /// </summary>
    /// <param name="promptID"></param>
    /// <param name="promptMenu"></param>
    /// <param name="promptAnswer"></param>
    /// <param name="promptQuestion"></param>
    public void DisplayPrompt(string promptID, GameObject promptMenu, TMP_Text promptAnswer, Transform bubbleParent, PromptBubble[] saveIn)
    {
        //disable continue click
        if (saveIn == currentPromptBubbles)
        {
            diManager.continueEnabledPrompt = false;
        }
        else if (saveIn == currentPromptAskBubbles)
        {
            diManager.continueEnabledPromptAsk = false;
        }

        promptMenu.SetActive(true);//show prompt menu

        //if prompt exists
        if (wlReader.questionTag.ContainsKey(promptID))
        {
            promptAnswer.ForceMeshUpdate();
            //before filling in new text into the prompt menu, empty the array, as it doesnt do that
            promptAnswer.text = wlReader.questionTag[promptID][0];
            promptAnswer.ForceMeshUpdate();
        }
        else
            Debug.Log("The prompt {0} does not exist in the lookup table" + promptID);

        //show required text prompts OVER the text at |Tag|
        WordUtilities.CheckForPromptInputs(promptAnswer, promptAnswer.textInfo, bubbleParent, saveIn); // textinfo somehow gets deleted or something after that

        //make interactable
        StartCoroutine(diManager.CloseAWindow(promptMenu));// tell the diManager what window to close when done
    }
    /// <summary>
    /// Checks, wheter the data should be saved somewhere
    /// </summary>
    bool CheckIfShouldSave(string saveIn)
    {
        if (saveIn != "")
        {
            return true;
        }
        return false;
    }

    #region Ask Related
    /// <summary>
    /// called, when the "ask" button is pressed. opens the prompt "ASK"
    /// </summary>
    public void AskButton(string promptID)
    {
        //disable continue
        DialogueInputManager.instance.AskMenuOpen(true);
        // open the fake ask menu
        refM.askField.SetActive(true);
        // grey out ask and barter
        WordCaseManager.instance.DisableAskAndBarter(true);
        //Enable the 2nd DialogueRunner
        refM.askRunner.gameObject.SetActive(true);
        //start the second runner
        refM.askRunner.StartDialogue(DialogueManager.instance.currentTarget.askNode);
        //generate prompt bubble
        DisplayPrompt(promptID, refM.askField, refM.askPrompt,
            refM.askPromptBubbleParent.transform, currentPromptAskBubbles);
        // Temporarily disable any other active prompts
        TemporarilyClosePromptMenu(true);
        //Reload the word case, so any possibly missing words from other prompt inputs respawn
        WordCaseManager.instance.OpenOnTag(false);
        //disable the text below for the moment, because its causing quite some problems
        refM.interactableTextList[0].gameObject.SetActive(false);
    }
    /// <summary>
    /// When prompt was filled etc, find the correct way to answer.
    /// </summary>
    /// <param name="promptID"></param>
    public void SendAskButton()
    {
        if (CheckForPromptsFilled(currentPromptAskBubbles))
        {
            //check prompt bubble for content
            SaveGivenAnswer(currentPromptAskBubbles);
            //enable click continue
            DialogueInputManager.instance.continueEnabledPromptAsk = true;
            //Delete existing prompts
            DeleteAllPrompts(currentPromptAskBubbles);
            //Delete current word gets deleted, so empty the currentWord var
            WordClickManager.instance.currentWord = null;
            //Reload, so that the missing word comes back
            WordCaseManager.instance.OpenOnTag(false);
            //Jump to NPC.answer
            WordUtilities.JumpToNode(ReferenceManager.instance.askRunner, givenAnswerAsk.name);
            //Continue()
            DialogueInputManager.instance.Continue(ReferenceManager.instance.askDialogueUI);
            // Close Prompt Field
            ReferenceManager.instance.askField.SetActive(false);
        }
    }
    /// <summary>
    /// called, when the dialogue ends. Closes all Managers & windows
    /// </summary>
    public void OnQuestionDialogueEnded()
    {
        //enable continue
        DialogueInputManager.instance.AskMenuOpen(false);
        // close the fake ask menu
        ReferenceManager.instance.askField.SetActive(false);
        //set ask and barter buttons active again
        WordCaseManager.instance.DisableAskAndBarter(false);
        //disable 2nd runner again
        ReferenceManager.instance.askRunner.gameObject.SetActive(false);
        // if the prompt menu has been closed, reopen it
        TemporarilyClosePromptMenu(false);
        //enable the text below again
        refM.interactableTextList[0].gameObject.SetActive(true);
    }
    /// <summary>
    /// When an ask line is done (called in Dialogue Runner) -> enable continue for ContinueText()
    /// </summary>
    public void AskTextUnfinished()
    {
        DialogueInputManager.instance.askTextFinished = false;
    }
    /// <summary>
    /// When a line is done (called in Dialogue Runner) -> enable continue for ContinueText()
    /// </summary>
    public void AskTextFinished()
    {
        DialogueInputManager.instance.askTextFinished = true;

        // Color all interactable words, force update, so there are no errors
        ReferenceManager.instance.askNPCText.ForceMeshUpdate();
        WordUtilities.ReColorAllInteractableWords();
    }
    /// <summary>
    /// Called to continue on an ask prompt. disables itself afterwards.
    /// </summary>
    public void ContinueButtonAsk()
    {
        if (CheckForPromptsFilled(currentPromptAskBubbles))
        {
            SaveGivenAnswer(currentPromptAskBubbles);
            DialogueInputManager.instance.Continue(ReferenceManager.instance.askDialogueUI);
            DialogueInputManager.instance.continueEnabledPromptAsk = true;
            DeleteAllPrompts(currentPromptAskBubbles);
            WordClickManager.instance.currentWord = null;
            WordCaseManager.instance.OpenOnTag(false); //Reload, so that the missing word comes back
            ReferenceManager.instance.askField.SetActive(false);
        }
        ReferenceManager.instance.askContinueButton.SetActive(false);
    }
    /// <summary>
    /// when called close the prompt menu and possible related promps
    /// </summary>
    public void TemporarilyClosePromptMenu(bool close)
    {
        if (close && ReferenceManager.instance.playerInputField.activeInHierarchy)
        {
            promptCurrentlyDisabled = true;
            ReferenceManager.instance.playerInputField.SetActive(false);
            ReferenceManager.instance.promptBubbleParent.SetActive(false);
        }
        else if (!close && promptCurrentlyDisabled)
        {
            promptCurrentlyDisabled = false;
            ReferenceManager.instance.playerInputField.SetActive(true);
            ReferenceManager.instance.promptBubbleParent.SetActive(true);
        }
    }
    #endregion
}
