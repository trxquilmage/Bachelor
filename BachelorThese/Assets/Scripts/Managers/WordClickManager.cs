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
    public GameObject wordLastHighlighted;
    public GameObject currentWord
    {
        get
        {
            return CurrentWord;
        }
        set
        {
            CurrentWord = value;
            if (value == null)
                DialogueInputManager.instance.continueEnabledDrag = true;
            else
                DialogueInputManager.instance.continueEnabledDrag = false;
        }
    }
    public GameObject CurrentWord;
    WordLookupReader wlReader;
    GameObject[] activeWords = new GameObject[20];

    void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        wlReader = WordLookupReader.instance;
    }
    /// <summary>
    /// Ckeck if the word is actually in the keywords list, then skip to WordUtilities.CreateWord()
    /// </summary>
    /// <param name="sentWord"></param>
    /// <param name="wordPos"></param>
    public void CheckWord(string sentWord, Vector2 wordPos, TMP_WordInfo wordInfo)
    {
        //check if the sent word is actually in the keyword list
        if (wlReader.wordTag.ContainsKey(sentWord))
        {
            // Create Word Data to send
            Word.WordData data = new Word.WordData();
            data.name = sentWord;
            data.tag = WordUtilities.StringToTag(wlReader.wordTag[sentWord][0]);
            data.tagInfo = wlReader.wordTag[sentWord];
            wordLastHighlighted = WordUtilities.CreateWord(data, wordPos, wordInfo, WordInfo.Origin.Dialogue);
            AddToArray(activeWords, wordLastHighlighted);
        }
    }
    /// <summary>
    /// Destroy the word that is currently selected by the mouse
    /// </summary>
    public void DestroyCurrentWord()
    {
        TMP_Text text = currentWord.GetComponent<Word>().relatedText;
        // Destroy the Word
        Destroy(currentWord);
        currentWord = null;
        // Set the word color back to interactable
        WordUtilities.ReColorAllInteractableWords();
    }
    /// <summary>
    /// Destroy the word you highlighted (after it is no longer hovered)
    /// </summary>
    public void DestroyLastHighlighted()
    {
        TMP_Text text = wordLastHighlighted.GetComponent<Word>().relatedText;
        // Destroy the Word
        Destroy(wordLastHighlighted);
        wordLastHighlighted = null;

        // Set the word color back to interactable
        WordUtilities.ReColorAllInteractableWords();
    }
    /// <summary>
    /// Destroy the buttons that have spawned this dialogue
    /// </summary>
    public void DestroyAllActiveWords()
    {
        if (activeWords.Length != 0)
        {
            for (int i = activeWords.Length - 1; i > 0; i--)
            {
                if (activeWords[i] != null)
                {
                    Destroy(activeWords[i]);
                    activeWords[i] = null;
                }
            }
        }
    }
    /// <summary>
    /// Goes through the array and places the Object in the first free Spot
    /// </summary>
    void AddToArray(GameObject[] array, GameObject toAdd)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
            {
                array[i] = toAdd;
            }
        }
    }
}
