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
    }
    /// <summary>
    /// Set a quest in the questlog
    /// </summary>
    [YarnCommand("setquest")]
    public void SetQuest(string questName)
    {
        QuestManager.instance.SetQuest(questName);
    }
    /// <summary>
    /// Set a quest that is in the questlog to completed
    /// </summary>
    [YarnCommand("completequest")]
    public void CompleteQuest(string questName)
    {
        QuestManager.instance.CompleteQuest(questName);
    }

    /// <summary>
    /// show the i cant say button
    /// </summary>
    /// <param name="characterName"></param>
    [YarnCommand("showicantsay")]
    public void ICantSay()
    {
        if (PlayerInputManager.instance.inAsk)
            refM.askICantSayButton.SetActive(true);
        else
            refM.iCantSayButton.SetActive(true);
    }
    /// <summary>
    /// Change speaking character's name
    /// </summary>
    /// <param name="characterName"></param>
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
    /// <summary>
    /// Set a character as a companion, who follows you around
    /// </summary>
    /// <param name="characterName"></param>
    [YarnCommand("settocompanion")]
    public void SetToCompanion(string characterName)
    {
        foreach (Companion companion in refM.npcParent.GetComponentsInChildren<Companion>())
            if (companion.characterName == characterName)
                companion.inParty = true;
    }
    /// <summary>
    /// Open Prompt Menu and related question
    /// </summary>
    /// <param name="promptQ"></param>
    [YarnCommand("displaypromptmenu")]
    public void DisplayPromptMenu(string promptQ)
    {
        PlayerInputManager.instance.DisplayPrompt(promptQ, refM.promptMenu,
            refM.promptAnswer, refM.promptBubbleParent.transform,
            PlayerInputManager.instance.currentPromptBubbles);
    }
    /// <summary>
    /// Open Ask Prompt Menu and related question
    /// </summary>
    /// <param name="promptQ"></param>
    [YarnCommand("displaypromptmenuask")]
    public void DisplayPromptMenuAsk(string promptQ)
    {

        PlayerInputManager.instance.DisplayPrompt(promptQ, refM.askField,
            refM.askPrompt, ReferenceManager.instance.askPromptBubbleParent.transform,
            PlayerInputManager.instance.currentPromptAskBubbles);
        //activate the "continue" button above the Ask button
        refM.askContinueButton.SetActive(true);
    }
    /// <summary>
    /// Goes through all subtags of the main tag 
    /// additionally takes the name of a value it is supposed to save
    /// </summary>
    /// <param name="lookingFor"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Check, wheter a node has already been visited. if not, save it in the list
    /// </summary>
    /// <param name="characterName"></param>
    /// <param name="nodeName"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Checks, wheter the dialogue is currently running, or if we just randomly asked a question outside the dialogue
    /// </summary>
    /// <returns></returns>
    public bool CheckIfOnlyAsk()
    {
        return DialogueManager.instance.isOnlyInAsk;
    }
    /// <summary>
    /// Check if "I can't say" was pressed, if yes, jump to node "ICantSay"
    /// </summary>
    /// <returns></returns>
    public bool ICantSay(DialogueRunner runner, string characterName, string nextNode)
    {
        if (iCantSay)
            inICantSay = true;
        StartCoroutine(ICantSayNextFrame(runner, characterName, nextNode));
        return false;
    }
    /// <summary>
    /// Continue is triggered twice when calling a new dialogue, whic essentially skips the first line of every new called dialogue
    /// because of that we wait 1 frame
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="characterName"></param>
    /// <param name="nextNode"></param>
    /// <returns></returns>
    IEnumerator ICantSayNextFrame(DialogueRunner runner, string characterName, string nextNode)
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        yield return delay;
        currentCharacterName = "";
        currentNextNodeName = "";
        //check if "I cant say" was pressed
        if (iCantSay)
        {
            iCantSay = false;
            currentCharacterName = characterName;
            currentNextNodeName = nextNode;
            WordUtilities.JumpToNode(runner, "ICantSay");
        }
    }
    /// <summary>
    /// is called when the node "icantsay" is done, then waits one frame (so there isnt any problem with the MarkLineComplete-Command)
    /// the goes to "nextcurrentnode", which is set in the "ICantSay()" function
    /// </summary>
    [YarnCommand("onnodecomplete")]
    public void OnNodeComplete()
    {
        StartCoroutine(OnNodeCompleteNextFrame());
    }
    IEnumerator OnNodeCompleteNextFrame()
    {
        DialogueRunner runner;
        if (PlayerInputManager.instance.inAsk)
            runner = refM.askRunner;
        else
            runner = refM.runner;

        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        yield return delay;

        //set next node
        WordUtilities.JumpToNode(runner, currentNextNodeName);
        currentCharacterName = "";
        currentNextNodeName = "";
        inICantSay = false;
    }
}
