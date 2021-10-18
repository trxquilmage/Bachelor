using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using TMPro;

public class CommandManager : MonoBehaviour
{
    WordLookupReader wLookupReader;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI promptQuestion;
    [SerializeField] GameObject promptMenu;
    DialogueInputManager diManager;

    private void Start()
    {
        diManager = DialogueInputManager.instance;
        wLookupReader = WordLookupReader.instance;
    }
    [YarnCommand("showcharactername")]
    public void ShowCharacterName(string characterName)
    {
        nameText.text = characterName;
    }

    [YarnCommand("displaypromptmenu")]
    public void DisplayPromptMenu(string promptQ)
    {
        diManager.continueEnabled = false; //disable continue click
        if (wLookupReader.questionTag.ContainsKey(promptQ))
            promptQuestion.text = wLookupReader.questionTag[promptQ];
        else
            Debug.Log("The prompt {0} does not exist in the lookup table" + promptQ);
        promptMenu.SetActive(true);//show prompt menu
        //show required text prompt
        //make interactable
        StartCoroutine(diManager.CloseAWindow(promptMenu));// tell the diManager what window to close when done
    }
}
