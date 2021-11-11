using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordInfo
{
    // Lists all Info needed to Work with the words
    /*public enum WordTags
    {
        Location, Name, Item, Quest, Other, None, AllWords
    }*/
    public enum Origin
    {
        Dialogue, Ask, WordCase, QuestLog, Environment
    }
    [System.Serializable]
    public struct WordTag
    {
        public string name;
        public Color tagColor;
    }
}


