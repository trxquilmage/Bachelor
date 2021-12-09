using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class StartQuestion : MonoBehaviour
{
    public string question;
    public StartTraitData[] answers;
    public StartTrait currentlySelected;
    public bool questionAnswered;

    private void OnEnable()
    {
        if (question != "")
        {
            GetComponent<TMP_Text>().text = question;
            StartTrait[] startTraits = GetComponentsInChildren<StartTrait>();
            // fill this object with the given text
            for (int i = 0; i < startTraits.Length; i++)
            {
                if (i < answers.Length)
                {
                    startTraits[i].Initialize(answers[i]);
                    startTraits[i].relatedQuestion = this;
                }
                else
                    startTraits[i].gameObject.SetActive(false);
            }
        }
        //hide question
        else
        {
            this.gameObject.SetActive(false);
        }
    }
    public void ChangeCurrentlySelected(StartTrait newCurrentlySelected)
    {
        // a new checkbox was selected
            //there was a checkbox already filled
            if (currentlySelected != null)
                currentlySelected.Uncheck();

            currentlySelected = newCurrentlySelected;
            questionAnswered = true;
    }
    public void EraseCurrentlySelected(StartTrait newCurrentlySelected)
    {
        //Checkbox was emptied
        if (newCurrentlySelected == currentlySelected)
        {
            currentlySelected = null;
            questionAnswered = false;
        }
    }
}
