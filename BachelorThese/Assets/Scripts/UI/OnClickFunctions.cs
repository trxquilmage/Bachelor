using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickFunctions : MonoBehaviour
{
    [HideInInspector] public QuestCase relatedQuest
    {
        get { return RelatedQuest; }
        set
        {
            RelatedQuest = value;
            if (relatedQuest.dropDownOpen)
            {
                ChangeToOpen();
                ChangeArrow();
                ChangeDropDownStatus();
            }
        }
    }
    QuestCase RelatedQuest;
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
        relatedQuest.ManuallyOpenCase();
        relatedQuest.ForceLayoutGroupUpdate();
        relatedQuest.dropDownOpen = open;
        Debug.Log("open : " + open);

    }
}
