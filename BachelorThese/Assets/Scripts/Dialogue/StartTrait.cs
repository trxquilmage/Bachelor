using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartTrait : MonoBehaviour
{
    [HideInInspector] public string description;
    [HideInInspector] public string[] relatedWords;
    [HideInInspector] public string[] relatedCommands;
    [HideInInspector] public StartQuestion relatedQuestion;
    public Toggle toggle;
    public void Initialize(StartTraitData data)
    {
        toggle = GetComponent<Toggle>();
        description = data.description;
        relatedWords = data.relatedWords;
        relatedCommands = data.relatedCommands;

        //fill the object with the correct text 
        GetComponentInChildren<TMP_Text>().text = description;
    }
    public void Uncheck()
    {
        toggle.isOn = false;
    }
    public void OnCheck()
    {
        //is being turned off
        if (!toggle.isOn)
            relatedQuestion.EraseCurrentlySelected(this);
        //max count reached: uncheck again
        else if (relatedQuestion.maxCountReached)
        {
            relatedQuestion.ChangeCurrentlySelected(this);
            Uncheck();
        }
        //is being turned on (max count not reached)
        else
            relatedQuestion.ChangeCurrentlySelected(this);
    }
}
[System.Serializable]
public class StartTraitData
{
    public string description;
    public string[] relatedWords;
    public string[] relatedCommands;
}

