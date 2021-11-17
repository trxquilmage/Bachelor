using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickFunctions : MonoBehaviour
{
    [HideInInspector] public Quest relatedQuest;
    [SerializeField] Sprite normal, onClick;
    bool open = false;
    public void ChangeToOpen()
    {
        open = !open;
    }
    public void ChangeArrow()
    {
        if (open)
        {
            GetComponentsInChildren<Image>()[1].sprite = onClick;
        }
        else
        {
            GetComponentsInChildren<Image>()[1].sprite = normal;
        }
    }
    public void ChangeDropDownStatus()
    {
        if (open)
        {
            relatedQuest.OpenDropDown();
        }
        else
        {
            relatedQuest.CloseDropDown();
        }
    }
}
