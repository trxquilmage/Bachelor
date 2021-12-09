using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartTrait : MonoBehaviour
{
    public string description;
    public string[] relatedWords;
    public StartQuestion relatedQuestion;
    public Toggle toggle;
    public void Initialize(StartTraitData data)
    {
        toggle = GetComponent<Toggle>();
        description = data.description;
        relatedWords = data.relatedWords;

        //fill the object with the correct text 
        GetComponentInChildren<TMP_Text>().text = description;
    }
    public void Uncheck()
    {
        toggle.isOn = false;
    }
    public void OnCheck()
    {
        if (toggle.isOn)
            relatedQuestion.ChangeCurrentlySelected(this);
        else
            relatedQuestion.EraseCurrentlySelected(this);
    }
}
[System.Serializable]
public class StartTraitData
{
    public string description;
    public string[] relatedWords;
}

