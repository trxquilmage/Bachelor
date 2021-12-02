using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using TMPro;

public class CommandManager : MonoBehaviour
{
    InfoManager info;
    ReferenceManager refM;
    public bool iCantSay;
    public string currentCharacterName;
    public string currentNextNodeName;
    bool inICantSay;
    private void Start()
    {
        info = InfoManager.instance;
        refM = ReferenceManager.instance;
        // runner
        refM.runner.AddFunction("icantsay", 2, delegate (Yarn.Value[] parameters)
        {
            ICantSay(refM.runner, parameters[0].AsString, parameters[1].AsString);
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
            ICantSay(refM.askRunner, parameters[0].AsString, parameters[1].AsString);
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
    /// Change speaking character's name
    /// </summary>
    /// <param name="characterName"></param>
    [YarnCommand("showcharactername")]
    public void ShowCharacterName(string characterName)
    {
        refM.interactableTextList[refM.characterNameIndex].text = characterName;
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
        return PlayerInputManager.instance.ReactToInput(lookingFor, npcName, saveIn);
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
    /// Check if "I can't say" was pressed, if yes, jump to node "
    /// </summary>
    /// <returns></returns>
    public void ICantSay(DialogueRunner runner, string characterName, string nextNode)
    {
        currentCharacterName = "";
        currentNextNodeName = "";
        //check if "I cant say" was pressed
        if (iCantSay)
        {
            iCantSay = false;
            inICantSay = true;
            currentCharacterName = characterName;
            currentNextNodeName = nextNode;
            WordUtilities.JumpToNode(runner, characterName + "." + "ICantSay");
            //Start coroutine, that waits until I can't say is completed
        }
    }
    public void OnNodeComplete(DialogueRunner runner)
    {
        if (inICantSay && runner.CurrentNodeName.Trim().Split("."[0])[1] == "ICantSay")
        {
            //set next node
            WordUtilities.JumpToNode(runner, currentCharacterName + "." + currentNextNodeName);
            currentCharacterName = "";
            currentNextNodeName = "";
            inICantSay = false;
        }
    }
}
