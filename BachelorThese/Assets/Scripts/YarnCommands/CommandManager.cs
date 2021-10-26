using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using TMPro;

public class CommandManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI promptQuestion;
    [SerializeField] TextMeshProUGUI promptAnswer;
    [SerializeField] GameObject promptMenu;
    DialogueInputManager diManager;
    WordLookupReader wlReader;

    private void Start()
    {
        diManager = DialogueInputManager.instance;
        wlReader = WordLookupReader.instance;
        Yarn.ReturningFunction funct = delegate (Yarn.Value[] parameters)
        {
            return ReactToAnswer(parameters[0].AsString);
        };
        // registers a function "react()" in Yarn
        ReferenceManager.instance.runner.AddFunction("react", 1, funct);
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
        //disable continue click
        diManager.continueEnabledPrompt = false;
        //if prompt exists
        if (wlReader.questionTag.ContainsKey(promptQ))
        {
            promptQuestion.text = wlReader.questionTag[promptQ][0];
            promptAnswer.text = @wlReader.questionTag[promptQ][1];
        }
        else
            Debug.Log("The prompt {0} does not exist in the lookup table" + promptQ);
        promptMenu.SetActive(true);//show prompt menu
        //show required text prompts OVER the text at <Tag>
        WordUtilities.CheckForPromptInputs(promptAnswer);

        //make interactable
        StartCoroutine(diManager.CloseAWindow(promptMenu));// tell the diManager what window to close when done
    }
    /// <summary>
    /// Goes through all subtags of the main tag 
    /// </summary>
    /// <param name="lookingFor"></param>
    /// <returns></returns>
    public string ReactToAnswer(string lookingFor)
    {
        string answer = "";
        Word.WordData data = PlayerInputManager.instance.givenAnswer;
        switch (data.tag)
        {
            case WordInfo.WordTags.Location:
                switch (lookingFor)
                {
                    case "position":
                        answer = data.tagObj.loc.position;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
        return answer;
    }
}
