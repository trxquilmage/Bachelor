using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class WordClickManager : MonoBehaviour
{
    // Manages the currently clicked/dragged word

    public static WordClickManager instance;
    WordLookupReader wlReader;
    public GameObject currentWord;

    void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        wlReader = WordLookupReader.instance;
    }
    public void SendWord(string sentWord, Vector2 mousePos)
    {
        //check if the sent word is actually in the keyword list
        if (wlReader.wordTag.ContainsKey(sentWord))
        {
            currentWord = WordUtilities.CreateWord(sentWord, wlReader.wordTag[sentWord], mousePos);
        }
    }
    public void DestroyCurrentWord()
    {
        Destroy(currentWord);
        currentWord = null;
    }
}
