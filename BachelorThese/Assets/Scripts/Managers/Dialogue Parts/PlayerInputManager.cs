using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using TMPro;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    [HideInInspector] public PromptBubble[] currentPromptBubbles;
    [HideInInspector] public PromptBubble[] currentPromptAskBubbles;
    [HideInInspector] public AnswerData givenAnswer;
    [HideInInspector] public AnswerData givenAnswerAsk;
    [HideInInspector] public bool givenManualPrompt;
    [HideInInspector] public bool inAsk;
    DialogueInputManager diManager;
    WordLookupReader wlReader;
    ReferenceManager refM;
    InfoManager info;
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
        //disable askrunner
        ReferenceManager.instance.askRunner.gameObject.SetActive(false);
    }
    /// <summary>
    /// saves the Info given, after pressing continue
    /// </summary>
    public void SaveGivenAnswer()
    {
        //get the list we should save the answer into
        if (CheckForActivePromptBubbleParent(out PromptBubble[] promptBubbles))
        {
            //go through all prompts and look for one that is filled
            foreach (PromptBubble prompt in promptBubbles)
            {
                if (prompt != null && prompt.child != null)
                {
                    AnswerData answer = new AnswerData();
                    Bubble bubble = prompt.child.GetComponent<Bubble>();
                    if (bubble is Word)
                        answer = new AnswerData() { wordData = (WordData)bubble.data, bubbleData = bubble.data };
                    
                    if (promptBubbles == currentPromptBubbles)
                        givenAnswer = answer;
                    else if (promptBubbles == currentPromptAskBubbles)
                        givenAnswerAsk = answer;
                }
            }
        }
    }
    /// <summary>
    /// see if there are any prompt bubbles 
    /// </summary>
    public bool CheckForPrompts(out PromptBubble[] pB)
    {
        List<PromptBubble> pBList = new List<PromptBubble>();
        if (CheckForActivePromptBubbleParent(out PromptBubble[] promptBubbles))
        {
            foreach (PromptBubble prompt in promptBubbles)
            {
                if (prompt != null)
                {
                    pBList.Add(prompt);
                }
            }
            pB = pBList.ToArray();
            if (pB.Length > 0)
                return true;
        }
        pB = pBList.ToArray();
        return false;
    }
    /// <summary>
    /// See if there currently are any prompt bubbles with the fitting tag 
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="pB"></param>
    /// <returns></returns>
    public bool CheckForPromptsWithTag(string tag, out PromptBubble pB)
    {
        pB = null;
        if (CheckForPrompts(out PromptBubble[] promptBubbles))
        {
            foreach (PromptBubble bubble in promptBubbles)
            {
                if (bubble.CheckIfTagFits(tag))
                {
                    pB = bubble;
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// See if any promptbubbleparent is active right now
    /// </summary>
    /// <param name="pBParent"></param>
    /// <returns></returns>
    public bool CheckForActivePromptBubbleParent(out PromptBubble[] pBParent)
    {
        pBParent = null;
        if (inAsk)
            pBParent = currentPromptAskBubbles;
        else if (!inAsk && refM.playerInputField.activeInHierarchy) //no ask and in prompt currently
            pBParent = currentPromptBubbles;
        else
            return false;
        if (pBParent == null)
            return false;
        return true;
    }
    /// <summary>
    /// Checks, whether all prompts are filled at the moment
    /// </summary>
    public bool CheckIfAllPromptsAreFilled()
    {
        bool allFilled = true;
        if (CheckForActivePromptBubbleParent(out PromptBubble[] promptBubbles))
        {
            foreach (PromptBubble prompt in promptBubbles)
            {
                if (prompt != null && prompt.child == null) //doesnt have a child
                {
                    allFilled = false;
                }
            }
            return allFilled;
        }
        return false;
    }
    /// <summary>
    /// Save a prompt, after it was created
    /// </summary>
    /// <param name="bubble"></param>
    public void SavePrompt(PromptBubble bubble)
    {
        if (CheckForActivePromptBubbleParent(out PromptBubble[] array))
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
    public Yarn.Value ReactToInput(string lookingFor, string NPCname, string saveIn)
    {
        Yarn.Value val = null;
        AnswerData data;
        if (!inAsk)
            data = givenAnswer;
        else
            data = givenAnswerAsk;

        if (data.wordData != null) //is word
        {
            Bubble.TagObject tagObj = data.wordData.tagObj;
            val = InfoManager.instance.FindValue(data.wordData, lookingFor);
            //save the required Info
            if (CheckIfShouldSave(saveIn))
            {
                info.SaveInfo(saveIn, val, NPCname, tagObj);
            }
        }

        return val;
    }
    /// <summary>
    /// wites out the given prompt & adds the prompt bubbles
    /// </summary>
    /// <param name="promptID"></param>
    /// <param name="promptMenu"></param>
    /// <param name="promptAnswer"></param>
    /// <param name="promptQuestion"></param>
    public void GeneratePromptBubble(string promptID, GameObject promptMenu, TMP_Text promptAnswer, Transform bubbleParent, PromptBubble[] saveIn)
    {
        bool isAsk = (saveIn == currentPromptAskBubbles) ? true : false;
        if (isAsk)
            diManager.continueHandler.OnAskPromptStart();
        else
            diManager.continueHandler.OnPromptStart();
            

        promptMenu.SetActive(true);

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
        string subtags = (wlReader.questionTag[promptID].Length > 1) ? wlReader.questionTag[promptID][1] : "";
        WordUtilities.CheckForPromptInputs(promptAnswer, promptAnswer.textInfo, bubbleParent, subtags); // textinfo somehow gets deleted or something after that

        //make interactable
        StartCoroutine(diManager.ClosePromptMenuAfterContinue(promptMenu));// tell the diManager what window to close when done
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
    public void AskButton()
    {
        inAsk = true;
        if (!DialogueManager.instance.isInDialogue)
        {
            DialogueManager.instance.isOnlyInAsk = true;
            DialogueInputManager.instance.enabled = true;
        }

        string promptID = "AskAAA";
        diManager.continueHandler.OnStartAsk();
        refM.askField.SetActive(true);
        AskAndBarterButton(false);
        AbortAskButton(true);

        refM.askRunner.gameObject.SetActive(true);
        refM.askRunner.StartDialogue(DialogueManager.instance.currentTarget.askNode);
        GeneratePromptBubble(promptID, refM.askField, refM.askPrompt,
            refM.askPromptBubbleParent.transform, currentPromptAskBubbles);

        if (DialogueManager.instance.isInDialogue)
        {
            TemporarilyCloseNormalPromptMenu(true);
            refM.interactableTextList[0].gameObject.SetActive(false);
        }
        WordCaseManager.instance.ReloadContents();
    }

    public void SendAskButton()
    {
        if (CheckIfAllPromptsAreFilled())
        {
            SaveGivenAnswer();
            diManager.continueHandler.OnAskPromptEnd();
            //Delete existing prompts
            DeleteAllPrompts(currentPromptAskBubbles);
            //Delete current word gets deleted, so empty the currentWord var
            WordClickManager.instance.currentWord = null;
            //Reload, so that the missing word comes back
            WordCaseManager.instance.ReloadContents();
            //Jump to NPC.answer
            WordUtilities.JumpToNode(refM.askRunner, givenAnswerAsk.bubbleData.name);
            //Continue()
            DialogueInputManager.instance.Continue(ReferenceManager.instance.askDialogueUI);
            //make new button "abort ask" unavailable
            AbortAskButton(false);
            // Close Prompt Field
            refM.askField.SetActive(false);
            refM.askICantSayButton.SetActive(false);
        }
    }

    public void OnAskEnded(bool wasAborted)
    {
        diManager.continueHandler.OnEndAsk();
        refM.askField.SetActive(false);
        AskAndBarterButton(true);
        refM.askRunner.gameObject.SetActive(false);

        if (DialogueManager.instance.isInDialogue)
        {
            TemporarilyCloseNormalPromptMenu(false);
            refM.interactableTextList[0].gameObject.SetActive(true);
        }
        if (DialogueManager.instance.isOnlyInAsk)
        {
            DialogueManager.instance.isOnlyInAsk = false;
            DialogueManager.instance.currentTarget = null;
            DialogueInputManager.instance.enabled = false;
        }
        DialogueManager.instance.isOnlyInAsk = false;
        if (!wasAborted)
        {
            diManager.continueHandler.OnAskPromptEnd();
            DeleteAllPrompts(currentPromptAskBubbles);
            WordClickManager.instance.currentWord = null;
            WordCaseManager.instance.ReloadContents();
            refM.askField.SetActive(false);
            refM.askICantSayButton.SetActive(false);
            AbortAskButton(false);
        }
        inAsk = false;
    }
    public void CantSayAsk()
    {
        diManager.continueHandler.OnEndAsk();
        AskAndBarterButton(true);
        refM.askRunner.gameObject.SetActive(false);

        if (DialogueManager.instance.isInDialogue)
        {
            TemporarilyCloseNormalPromptMenu(false);
            refM.interactableTextList[0].gameObject.SetActive(true);
        }

        if (DialogueManager.instance.isOnlyInAsk)
        {
            DialogueManager.instance.isOnlyInAsk = false;
            DialogueManager.instance.currentTarget = null;
            diManager.enabled = false;
        }
        DialogueManager.instance.isOnlyInAsk = false;
        diManager.continueHandler.OnAskPromptEnd();
        DeleteAllPrompts(currentPromptAskBubbles);
        WordClickManager.instance.currentWord = null;
        WordCaseManager.instance.ReloadContents();
        // Close Prompt Field
        refM.askField.SetActive(false);
        refM.askICantSayButton.SetActive(false);
        inAsk = false;
    }
    public void OnAskTextStart()
    {
        diManager.continueHandler.OnAskTextStart();
    }
    public void OnAskTextFinished()
    {
        diManager.continueHandler.OnAskTextEnd();

        // Color all interactable words, force update, so there are no errors
        ReferenceManager.instance.askNPCText.ForceMeshUpdate();
        EffectUtilities.ReColorAllInteractableWords();
    }
    public void ContinueButtonAsk()
    {
        if (CheckIfAllPromptsAreFilled())
        {
            SaveGivenAnswer();
            diManager.Continue(ReferenceManager.instance.askDialogueUI);
            diManager.continueHandler.OnAskPromptEnd();
            DeleteAllPrompts(currentPromptAskBubbles);
            WordClickManager.instance.currentWord = null;
            WordCaseManager.instance.ReloadContents();
            ReferenceManager.instance.askField.SetActive(false);
        }
        ReferenceManager.instance.askContinueButton.SetActive(false);
    }
    /// <summary>
    /// when called close the prompt menu and possible related promps
    /// </summary>
    public void TemporarilyCloseNormalPromptMenu(bool close)
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
    void AbortAskButton(bool show)
    {
        refM.abortAsk.SetActive(show);
    }
    void AskAndBarterButton(bool show)
    {
        refM.ask.SetActive(show);
        refM.barter.SetActive(show);
    }
    #endregion
}
public class AnswerData
{
    public WordData wordData;
    public BubbleData bubbleData;
}
