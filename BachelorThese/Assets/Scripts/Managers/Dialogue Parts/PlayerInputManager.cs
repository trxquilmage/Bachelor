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

    public void SaveGivenAnswer()
    {
        //get the list we should save the answer into
        if (CheckForAnyActivePromptBubbleParents(out PromptBubble[] promptBubbles))
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

    public bool CheckIfThereAreAnyPromptBubbles(out PromptBubble[] pB)
    {
        List<PromptBubble> pBList = new List<PromptBubble>();
        if (CheckForAnyActivePromptBubbleParents(out PromptBubble[] promptBubbles))
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

    public bool CheckIfThereArePromptBubblesWithTag(string tag, out PromptBubble pB)
    {
        pB = null;
        if (CheckIfThereAreAnyPromptBubbles(out PromptBubble[] promptBubbles))
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

    public bool CheckForAnyActivePromptBubbleParents(out PromptBubble[] pBParent)
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

    public bool CheckIfAllActivePromptsAreFilled()
    {
        bool allFilled = true;
        if (CheckForAnyActivePromptBubbleParents(out PromptBubble[] promptBubbles))
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

    public void SavePromptAfterCreation(PromptBubble bubble)
    {
        if (CheckForAnyActivePromptBubbleParents(out PromptBubble[] array))
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
    public void DeleteAllActivePrompts(PromptBubble[] promptBubbles)
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

        if (data.wordData != null)
        {
            Bubble.TagObject tagObj = data.wordData.tagObj;
            val = InfoManager.instance.FindValue(data.wordData, lookingFor);

            if (CheckIfShouldSave(saveIn))
            {
                info.SaveInfo(saveIn, val, NPCname, tagObj);
            }
        }

        return val;
    }
    public void GeneratePromptBubble(string promptID, GameObject promptMenu, TMP_Text promptAnswer, Transform bubbleParent, PromptBubble[] saveIn)
    {
        bool isAsk = saveIn == currentPromptAskBubbles;
        if (isAsk)
            diManager.continueHandler.OnAskPromptStart();
        else
            diManager.continueHandler.OnPromptStart();


        promptMenu.SetActive(true);

        if (DoesPromptIDExist(promptID))
        {
            promptAnswer.ForceMeshUpdate();
            promptAnswer.text = wlReader.questionTag[promptID][0];
            promptAnswer.ForceMeshUpdate();
        }
        else
            Debug.Log("The prompt {0} does not exist in the lookup table" + promptID);

        string subtags = (wlReader.questionTag[promptID].Length > 1) ? wlReader.questionTag[promptID][1] : "";

        GameObject prompt = WordUtilities.CheckForPromptInputsAndCreatePrompt(promptAnswer, promptAnswer.textInfo, bubbleParent, subtags);

        OnPromptStart(prompt);
        StartCoroutine(diManager.ClosePromptMenuAfterContinue(promptMenu));
    }

    void OnPromptStart(GameObject prompt)
    {
        if (prompt != null)
            WordCaseManager.instance.StartGreyOut(prompt);
    }

    public void OnPromptEnd()
    {
        WordCaseManager.instance.EndGreyOut();
    }

    bool DoesPromptIDExist(string promptID)
    {
        return wlReader.questionTag.ContainsKey(promptID);
    }
    bool CheckIfShouldSave(string saveIn)
    {
        if (saveIn != "")
        {
            return true;
        }
        return false;
    }

    #region Ask Related
    public void OpenAskField()
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
        if (CheckIfAllActivePromptsAreFilled())
        {
            SaveGivenAnswer();
            diManager.continueHandler.OnAskPromptEnd();
            DeleteAllActivePrompts(currentPromptAskBubbles);
            WordClickManager.instance.currentWord = null;
            WordCaseManager.instance.EndGreyOut();
            WordCaseManager.instance.ReloadContents();
            WordUtilities.JumpToNode(refM.askRunner, givenAnswerAsk.bubbleData.name);
            DialogueInputManager.instance.Continue(ReferenceManager.instance.askDialogueUI);
            AbortAskButton(false);
            refM.askField.SetActive(false);
            refM.askICantSayButton.SetActive(false);
        }
    }

    public void OnAskEnded(bool wasAborted)
    {
        diManager.continueHandler.OnEndAsk();
        refM.askNPCField.SetActive(false);
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
        if (wasAborted)
        {
            diManager.continueHandler.OnAskPromptEnd();
            WordClickManager.instance.currentWord = null;
            WordCaseManager.instance.EndGreyOut();
            WordCaseManager.instance.ReloadContents();
            refM.askField.SetActive(false);
            refM.askICantSayButton.SetActive(false);
            DeleteAllActivePrompts(currentPromptAskBubbles);
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
        DeleteAllActivePrompts(currentPromptAskBubbles);
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
        if (!CheckIfAllActivePromptsAreFilled())
            return;

        SaveGivenAnswer();
        diManager.Continue(ReferenceManager.instance.askDialogueUI);
        diManager.continueHandler.OnAskPromptEnd();
        DeleteAllActivePrompts(currentPromptAskBubbles);
        WordClickManager.instance.currentWord = null;
        WordCaseManager.instance.ReloadContents();
        ReferenceManager.instance.askField.SetActive(false);
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
