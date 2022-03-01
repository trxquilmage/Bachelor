using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using TMPro;

public class CommandManager : MonoBehaviour
{
    public static CommandManager instance;
    InfoManager info;
    ReferenceManager refM;
    [HideInInspector] public bool iCantSay;
    [HideInInspector] public string currentCharacterName;
    [HideInInspector] public string lastNodeName;
    [HideInInspector] public string currentNextNodeName;
    bool inICantSay;
    void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        info = InfoManager.instance;
        refM = ReferenceManager.instance;
        // runner
        refM.runner.AddFunction("icantsay", 2, delegate (Yarn.Value[] parameters)
        {
            return ICantSay(refM.runner, parameters[0].AsString, parameters[1].AsString);
        });
        refM.runner.AddFunction("react", -1, delegate (Yarn.Value[] parameters)
        {
            if (parameters.Length == 2)
            {
                return ReactToAnswer(parameters[0].AsString, "", parameters[1].AsString);
            }
            else
            {
                return ReactToAnswer(parameters[0].AsString, parameters[1].AsString, parameters[2].AsString);
            }
        });
        refM.runner.AddFunction("getinfo", 2, delegate (Yarn.Value[] parameters)
        {
            return GetInfo(parameters[0].AsString, (int)parameters[1].AsNumber);
        });
        refM.runner.AddFunction("isvisited", 2, delegate (Yarn.Value[] parameters)
        {
            return GetVisited(parameters[0].AsString, parameters[1].AsString);
        });
        refM.runner.AddFunction("isquestactive", 1, delegate (Yarn.Value[] parameters)
        {
            return CheckIfIsQuestActive(parameters[0].AsString);
        });

        // askRunner
        refM.askRunner.AddFunction("icantsay", 2, delegate (Yarn.Value[] parameters)
        {
            return ICantSay(refM.askRunner, parameters[0].AsString, parameters[1].AsString);
        });
        refM.askRunner.AddFunction("react", -1, delegate (Yarn.Value[] parameters)
        {
            if (parameters.Length == 2)
            {
                return ReactToAnswer(parameters[0].AsString, "", parameters[1].AsString);
            }
            else
            {
                return ReactToAnswer(parameters[0].AsString, parameters[1].AsString, parameters[2].AsString);
            }
        });
        refM.askRunner.AddFunction("getinfo", 2, delegate (Yarn.Value[] parameters)
        {
            return GetInfo(parameters[0].AsString, (int)parameters[1].AsNumber);
        });
        refM.askRunner.AddFunction("isvisited", 2, delegate (Yarn.Value[] parameters)
        {
            return GetVisited(parameters[0].AsString, parameters[1].AsString);
        });
        refM.askRunner.AddFunction("checkonlyask", 0, delegate (Yarn.Value[] parameters)
        {
            return CheckIfOnlyAsk();
        });
        refM.askRunner.AddFunction("isquestactive", 1, delegate (Yarn.Value[] parameters)
        {
            return CheckIfIsQuestActive(parameters[0].AsString);
        });
    }

    [YarnCommand("returnfromask")]
    public void ReturnFromAsk()
    {
        StartCoroutine(RedoGreyOutAfterAsk());
    }
    IEnumerator RedoGreyOutAfterAsk()
    {
        yield return new WaitForEndOfFrame();
        if (PlayerInputManager.instance.CheckIfThereAreAnyPromptBubbles(out PromptBubble[] promptBubbles))
        {
            WordCaseManager.instance.StartGreyOut(promptBubbles[0].gameObject);
            WordCaseManager.instance.ReloadContents();
        }
    }

    [YarnCommand("activateaskbutton")]
    public void ActivateAskButton()
    {
        TutorialManager.instance.EnableOrDisableAskButton(true);
    }

    [YarnCommand("activatewordhighlighting")]
    public void ActivateWordHighlighting()
    {
        TutorialManager.instance.EnableOrDisableHighlightingWords(true);
    }

    [YarnCommand("endtutorial")]
    public void EndTutorial()
    {
        TutorialManager.instance.EndTutorial();
    }

    [YarnCommand("setquest")]
    public void SetQuest(string questName)
    {
        QuestManager.instance.SetQuest(questName);
    }

    [YarnCommand("completequest")]
    public void CompleteQuest(string questName)
    {
        QuestManager.instance.CompleteQuest(questName);
    }

    public bool CheckIfIsQuestActive(string questName)
    {
        return QuestManager.instance.CheckIfQuestIsActive(questName);
    }

    [YarnCommand("showicantsay")]
    public void ICantSay()
    {
        if (PlayerInputManager.instance.inAsk)
            refM.askICantSayButton.SetActive(true);
        else
            refM.iCantSayButton.SetActive(true);
    }
    [YarnCommand("showcharactername")]
    public void ShowCharacterName(string[] characterName)
    {
        string name = "";
        foreach (string namePart in characterName)
        {
            name += namePart + " ";
        }
        refM.interactableTextList[refM.characterNameIndex].text = name;
    }
    [YarnCommand("settocompanion")]
    public void SetToCompanion(string characterName)
    {
        foreach (Companion companion in refM.npcParent.GetComponentsInChildren<Companion>())
            if (companion.characterName == characterName)
            {
                companion.JoinParty();
                break;
            }
    }
    [YarnCommand("displaypromptmenu")]
    public void DisplayPromptMenu(string promptQ)
    {
        PlayerInputManager.instance.GeneratePromptBubble(promptQ, refM.promptMenu,
            refM.promptAnswer, refM.promptBubbleParent.transform,
            PlayerInputManager.instance.currentPromptBubbles);
    }
    [YarnCommand("displaypromptmenuask")]
    public void DisplayPromptMenuAsk(string promptQ)
    {
        PlayerInputManager.instance.GeneratePromptBubble(promptQ, refM.askField,
            refM.askPrompt, ReferenceManager.instance.askPromptBubbleParent.transform,
            PlayerInputManager.instance.currentPromptAskBubbles);
        //activate the "continue" button above the Ask button
        refM.askContinueButton.SetActive(true);
    }
    public Yarn.Value ReactToAnswer(string lookingFor, string saveIn, string npcName)
    {
        if (!inICantSay)
            return PlayerInputManager.instance.ReactToInput(lookingFor, npcName, saveIn);
        return new Yarn.Value();
    }
    /// <summary>
    /// Get back an Info from the code. ChosenValue: 0 = get value; 1 = get the name of the npc who was told that info
    /// </summary>
    /// <param name="lookingFor"></param>
    /// <returns></returns>
    public Yarn.Value GetInfo(string lookingFor, int chosenValue)
    {
        return InfoManager.instance.GetInfo(lookingFor, chosenValue);
    }
    public bool GetVisited(string characterName, string nodeName)
    {
        bool nodeAlreadyExists = false;
        bool nameExists = false;
        //check if the Name already exists
        if (info.visitedNodes.ContainsKey(characterName)) //name exists
        {
            nameExists = true;
            for (int i = 0; i < info.visitedNodes[characterName].Count; i++)
            {
                if (info.visitedNodes[characterName][i] == nodeName)
                {
                    nodeAlreadyExists = true;
                }
            }
            //node wasnt in the list yet
            if (!nodeAlreadyExists)
            {
                info.visitedNodes[characterName].Add(nodeName);
            }
        }
        //name wasnt in list yet
        if (!nameExists)
        {
            info.visitedNodes.Add(characterName, new List<string>() { nodeName });
        }
        return nodeAlreadyExists;
    }
    public bool CheckIfOnlyAsk()
    {
        return DialogueManager.instance.isOnlyInAsk;
    }
    public bool ICantSay(DialogueRunner runner, string characterName, string nextNode)
    {
        if (iCantSay)
            inICantSay = true;
        StartCoroutine(ICantSayNextFrame(runner, characterName, nextNode));
        return false;
    }
    IEnumerator ICantSayNextFrame(DialogueRunner runner, string characterName, string nextNode) // Continue is triggered twice when calling a new dialogue, which essentially skips the first line of every new called dialogue
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        yield return delay;
        currentCharacterName = "";
        currentNextNodeName = "";

        if (iCantSay)
        {
            iCantSay = false;
            currentCharacterName = characterName;
            currentNextNodeName = nextNode;
            WordUtilities.JumpToNode(runner, "ICantSay");
        }
    }
    [YarnCommand("starttutorialstep")]
    public void DisableContinueTutorial(string tutorialStepNumber)
    {
        TutorialManager.instance.DisableContinueUntilTutorialStepIsDone(int.Parse(tutorialStepNumber));
    }

    [YarnCommand("tutorialaskquestionsdone")]
    public void TutorialDoneAskQuestions()
    {
        TutorialManager.instance.AskQuestionsDone();
    }

    [YarnCommand("onnodecomplete")]
    public void OnNodeComplete()
    {
        StartCoroutine(OnNodeCompleteNextFrame());
    }
    IEnumerator OnNodeCompleteNextFrame()
    {
        DialogueRunner runner = (PlayerInputManager.instance.inAsk) ? refM.askRunner: refM.runner;

        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        yield return delay;

        WordUtilities.JumpToNode(runner, currentNextNodeName);
        currentCharacterName = "";
        currentNextNodeName = "";
        inICantSay = false;
    }
}
