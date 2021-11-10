using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class NPC : MonoBehaviour
{
    
    public string characterName = "";
    public string talkToNode = "";
    public string askNode = "";

    [Header("Optional")]
    public YarnProgram scriptToLoad;
    public YarnProgram askScript;

    void Start()
    {
        if (scriptToLoad != null)
        {
            DialogueRunner dialogueRunner = FindObjectOfType<DialogueRunner>();
            dialogueRunner.Add(scriptToLoad);
        }
    }
}

