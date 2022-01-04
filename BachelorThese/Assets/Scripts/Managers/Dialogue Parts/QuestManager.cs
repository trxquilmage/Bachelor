using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    //Handles everything happening in the quest case
    public static QuestManager instance { get; private set; }
    void Awake()
    {
        instance = this;
    }
}
