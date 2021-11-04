using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using TMPro;

public class CommandManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    InfoManager info;

    private void Start()
    {
        info = InfoManager.instance;
        // runner
        ReferenceManager.instance.runner.AddFunction("react", -1, delegate (Yarn.Value[] parameters)
        {
            if (parameters.Length == 2)
            {
                return ReactToAnswer(parameters[0].AsString, "", parameters[1].AsString, false);
            }
            else
            {
                return ReactToAnswer(parameters[0].AsString, parameters[1].AsString, parameters[2].AsString, false);
            }
        });
        ReferenceManager.instance.runner.AddFunction("getinfo", 2, delegate (Yarn.Value[] parameters)
        {
            return GetInfo(parameters[0].AsString, (int)parameters[1].AsNumber);
        });
        ReferenceManager.instance.runner.AddFunction("isvisited", 2, delegate (Yarn.Value[] parameters)
        {
            return GetVisited(parameters[0].AsString, parameters[1].AsString);
        });

        // askRunner
        ReferenceManager.instance.askRunner.AddFunction("react", -1, delegate (Yarn.Value[] parameters)
        {
            if (parameters.Length == 2)
            {
                return ReactToAnswer(parameters[0].AsString, "", parameters[1].AsString, true);
            }
            else
            {
                return ReactToAnswer(parameters[0].AsString, parameters[1].AsString, parameters[2].AsString, true);
            }
        });
        ReferenceManager.instance.askRunner.AddFunction("getinfo", 2, delegate (Yarn.Value[] parameters)
        {
            return GetInfo(parameters[0].AsString, (int)parameters[1].AsNumber);
        });
        ReferenceManager.instance.askRunner.AddFunction("isvisited", 2, delegate (Yarn.Value[] parameters)
        {
            return GetVisited(parameters[0].AsString, parameters[1].AsString);
        });
    }
    /// <summary>
    /// Change speaking character's name
    /// </summary>
    /// <param name="characterName"></param>
    [YarnCommand("showcharactername")]
    public void ShowCharacterName(string characterName)
    {
        nameText.text = characterName;
    }

    /// <summary>
    /// Open Prompt Menu and related question
    /// </summary>
    /// <param name="promptQ"></param>
    [YarnCommand("displaypromptmenu")]
    public void DisplayPromptMenu(string promptQ)
    {
        PlayerInputManager.instance.DisplayPrompt(promptQ, ReferenceManager.instance.promptMenu,
            ReferenceManager.instance.promptAnswer, ReferenceManager.instance.promptBubbleParent.transform,
            PlayerInputManager.instance.currentPromptBubbles);
    }
    /// <summary>
    /// Open Ask Prompt Menu and related question
    /// </summary>
    /// <param name="promptQ"></param>
    [YarnCommand("displaypromptmenuask")]
    public void DisplayPromptMenuAsk(string promptQ)
    {
        PlayerInputManager.instance.DisplayPrompt(promptQ, ReferenceManager.instance.askField,
            ReferenceManager.instance.askPrompt, ReferenceManager.instance.askPromptBubbleParent.transform,
            PlayerInputManager.instance.currentPromptAskBubbles);
        //activate the "continue" button above the Ask button
        ReferenceManager.instance.askContinueButton.SetActive(true);
    }
    /// <summary>
    /// Goes through all subtags of the main tag 
    /// additionally takes the name of a value it is supposed to save
    /// </summary>
    /// <param name="lookingFor"></param>
    /// <returns></returns>
    public Yarn.Value ReactToAnswer(string lookingFor, string saveIn, string npcName, bool isAsk)
    {
        return PlayerInputManager.instance.ReactToInput(lookingFor, npcName, saveIn, isAsk);
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
}
