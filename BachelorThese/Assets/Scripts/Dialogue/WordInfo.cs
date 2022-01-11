using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordInfo
{
    public enum Origin
    {
        Dialogue, Ask, WordCase, QuestLog, Environment
    }
    [System.Serializable]
    public struct WordTag
    {
        public string name;
        public Color tagColor;
        public Button tagButton;
        public bool buttonIsActive;
    }
}


