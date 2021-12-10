using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class StartQuestion : MonoBehaviour
{
    public string question;
    public StartTraitData[] answers;
    [SerializeField] bool erasesLastOne = true;
    [HideInInspector] public StartTrait currentlySelected;
    [HideInInspector] public StartTrait[] allCurrentlySelected;
    [HideInInspector] public bool questionAnswered;
    [HideInInspector] public bool maxCountReached;
    ReferenceManager refM;

    int selectedCount
    {
        get { return SelectedCount; }
        set
        {
            SelectedCount = value;
            if (value == refM.maxStartWordTraitAmount)
                maxCountReached = true;
            else if (value < refM.maxStartWordTraitAmount)
                maxCountReached = false;
        }
    }
    int SelectedCount = 0;
    private void Start()
    {
        refM = ReferenceManager.instance;
        allCurrentlySelected = new StartTrait[refM.maxStartWordTraitAmount];
    }
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
        if (erasesLastOne)
        {
            // a new checkbox was selected
            //there was a checkbox already filled
            if (currentlySelected != null)
                currentlySelected.Uncheck();

            currentlySelected = newCurrentlySelected;
        }
        else if (selectedCount < allCurrentlySelected.Length)
        {
            for (int i = 0; i < allCurrentlySelected.Length; i++)
                if (allCurrentlySelected[i] == null)
                {
                    allCurrentlySelected[i] = newCurrentlySelected;
                    break;
                }
        }
        questionAnswered = true;
        selectedCount++;
    }
    public void EraseCurrentlySelected(StartTrait newCurrentlySelected)
    {
        //Checkbox was emptied
        if (newCurrentlySelected == currentlySelected || !erasesLastOne)
        {
            selectedCount--;
            if (erasesLastOne)
                currentlySelected = null;
            else if (selectedCount < allCurrentlySelected.Length)
            {
                for (int i = 0; i < allCurrentlySelected.Length; i++)
                    if (allCurrentlySelected[i] == newCurrentlySelected)
                    {
                        allCurrentlySelected[i] = null;
                        break;
                    }
            }
            questionAnswered = false;
        }
    }
}
