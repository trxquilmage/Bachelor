using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public static class WordUtilities
{
    /// <summary>
    /// Match the word to a color fitting the tag
    /// </summary>
    /// <param name="word"></param>
    public static void MatchColorToTag(GameObject word, WordInfo.WordTags tag)
    {
        word.GetComponent<Image>().color = Color.red;
    }

    /// <summary>
    /// Creates a word-Object at the mouse's position
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tag"></param>
    /// <param name="screenMousePos"></param>
    public static GameObject CreateWord(string name, string tag, Vector2 screenMousePos)
    {
        ReferenceManager refHandler = ReferenceManager.instance;
        GameObject word = GameObject.Instantiate(refHandler.selectedWordPrefab, screenMousePos, Quaternion.identity);
        word.transform.SetParent(refHandler.selectedWordParent.transform);
        Word wordScript = word.AddComponent<Word>();
        wordScript.Initialize(name, StringToTag(tag));
        return word;
    }
    /// <summary>
    /// Turn A String into a WordInfo.WordTags enum
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static WordInfo.WordTags StringToTag(string tag)
    {
        switch (tag)
        {
            case "Location":
                return WordInfo.WordTags.Location;
            default:
                Debug.Log("Tag Doesnt exist: " + tag);
                return WordInfo.WordTags.None; //for Debug Reasons
        }
    }
}
