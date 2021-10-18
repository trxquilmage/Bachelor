using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordCaseManager : MonoBehaviour
{
    public static WordCaseManager instance;
    [SerializeField] GameObject wordCaseUI;
    private void Awake()
    {
        instance = this;
    }
    public void OpenCase(bool open)
    {
        // open the case
        if (open)
        {
            wordCaseUI.SetActive(true);
        }
        //close the case
        else
        {
            wordCaseUI.SetActive(false);
        }
    }
}
