using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class WordLookupReader : MonoBehaviour
{
    public static WordLookupReader instance;
    string dataPath, allWords, allLongWords, allQuestions, allTags, allFillers, allQuests;

    public Dictionary<string, string[]> wordTag = new Dictionary<string, string[]>();
    public Dictionary<string[], string[]> longWordTagSingular = new Dictionary<string[], string[]>();
    public Dictionary<string, string[]> longWordTag = new Dictionary<string, string[]>();
    public Dictionary<string, string[]> questionTag = new Dictionary<string, string[]>();
    public Dictionary<string, string[]> fillerTag = new Dictionary<string, string[]>();
    public Dictionary<string, string> questDescriptions = new Dictionary<string, string>();
    public Dictionary<string, string[]> tagSubtag = new Dictionary<string, string[]>();
    List<string> blocked = new List<string>();
    List<TMP_WordInfo> currentWordList;
    List<string[]> currentReferenceWords;
    int currentLongWordIndex;
    ReferenceManager refM;
    private void Awake()
    {
        instance = this;
        questionTag = new Dictionary<string, string[]>();
    }
    private void Start()
    {
        AssignVariables();
        LookUpWord();
        LookUpLongWord();
        LookUpQuestion();
        LookUpTag();
        LookUpFiller();
        LookUpQuests();
    }
    void AssignVariables()
    {
        refM = ReferenceManager.instance;
        dataPath = Application.dataPath + "/Data/" + refM.sceneNumber;
        currentWordList = new List<TMP_WordInfo>();
        currentReferenceWords = new List<string[]>();
        blocked = StopWordsLookupReader.instance.blockedWords;
    }
    void LookUpQuestion()
    {
        string pathQuestion = dataPath + "/Questions.csv";
        allQuestions = File.ReadAllText(pathQuestion);
        string[] linesQ = allQuestions.Split("\n"[0]);
        foreach (string s in linesQ)
        {
            if (s != "")
            {
                string[] lineData = s.Trim().Split(";"[0]);
                string[] textPrompts = new string[2]{
                    lineData[1], lineData[2]
                };
                questionTag.Add(lineData[0], textPrompts);
            }
        }
    }
    void LookUpLongWord()
    {
        string pathLongWord = dataPath + "/LongWords.csv";
        allLongWords = File.ReadAllText(pathLongWord);
        string[] lines = allLongWords.Split("\n"[0]);
        foreach (string s in lines)
        {
            if (s != "")
            {
                string[] lineData = s.Trim().Split(";"[0]);
                string[] lineInfo = new string[lineData.Length - 1];
                for (int i = 0; i < lineInfo.Length; i++)
                {
                    lineInfo[i] = lineData[i + 1];
                }
                string[] allWords = lineData[0].Trim().Split(" "[0]);

                longWordTagSingular.Add(allWords, lineInfo);
                longWordTag.Add(lineData[0], lineInfo);
            }
        }

    }
    void LookUpWord()
    {
        string pathWord = dataPath + "/Words.csv";
        allWords = File.ReadAllText(pathWord);
        string[] lines = allWords.Split("\n"[0]);
        foreach (string s in lines)
        {
            if (s != "")
            {
                string[] lineData = s.Trim().Split(";"[0]);
                string[] lineInfo = new string[lineData.Length - 1];
                for (int i = 0; i < lineInfo.Length; i++)
                {
                    lineInfo[i] = lineData[i + 1];
                }
                wordTag.Add(lineData[0], lineInfo);
            }
        }

    }
    void LookUpFiller()
    {
        string pathFillers = dataPath + "/Fillers.csv";
        allFillers = File.ReadAllText(pathFillers);
        string[] lines = allFillers.Split("\n"[0]);
        foreach (string s in lines)
        {
            if (s != "")
            {
                string[] lineData = s.Trim().Split(";"[0]);
                string[] lineInfo = new string[lineData.Length - 1];
                for (int i = 0; i < lineInfo.Length; i++)
                {
                    lineInfo[i] = lineData[i + 1];
                }
                fillerTag.Add(lineData[0], lineInfo);
            }
        }

    }
    void LookUpTag()
    {
        string pathTagLookup = dataPath + "/Tag-Subtag.csv";
        allTags = File.ReadAllText(pathTagLookup);
        string[] lines = allTags.Split("\n"[0]);
        foreach (string s in lines)
        {
            if (s != "")
            {
                string[] lineData = s.Trim().Split(";"[0]);
                string[] lineInfo = new string[lineData.Length - 1];
                for (int i = 0; i < lineInfo.Length; i++)
                {
                    lineInfo[i] = lineData[i + 1];
                }
                tagSubtag.Add(lineData[0], lineInfo);
            }
        }
    }
    void LookUpQuests()
    {
        string pathTagLookup = dataPath + "/Quests.csv";
        allQuests = File.ReadAllText(pathTagLookup);
        string[] lines = allQuests.Split("\n"[0]);
        foreach (string s in lines)
        {
            if (s != "")
            {
                string[] lineData = s.Trim().Split(";"[0]);
                questDescriptions.Add(lineData[0], lineData[1]);
            }
        }
    }

    /// <summary>
    /// Returns the index of the word that is being looked for
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int CheckForSubtags(BubbleData data, string lookingFor)
    {
        //check the dictionary for the tag
        int i = 0;
        foreach (string subtag in tagSubtag[data.tag])
        {
            if (subtag == lookingFor)
                return i;
            i++;
        }
        //get the index list from it
        Debug.Log("Found nothing");
        return 0;
    }
    /// <summary>
    /// Checks if the given word is a short word, a long word or a filler word
    /// </summary>
    /// <param name="wordInfo"></param>
    /// <param name="wordCollection"></param>
    /// <returns></returns>
    public bool CheckForWord(TMP_WordInfo wordInfo, out TMP_WordInfo[] wordCollection, out bool isFillerWord)
    {
        wordCollection = null;
        bool isWord = false;
        isFillerWord = false;
        string word = WordUtilities.CapitalizeAllWordsInString(wordInfo.GetWord());

        // is this a word or is it something like "-" etc.?
        if (wordInfo.GetWord().Length == 1)
        {
            if (!char.IsLetter(wordInfo.GetWord()[0]))
            {
                currentReferenceWords = new List<string[]>();
                currentWordList = new List<TMP_WordInfo>();
                currentLongWordIndex = 0;
                return false;
            }
        }
        // is this part of a longer word (but not the first word)?
        if (currentWordList.Count != 0) //if a chain is checked right now, this has highest priority
        {
            currentLongWordIndex++;
            if (currentLongWordIndex < currentReferenceWords[0].Length)
            {
                //go through all long words that could POSSIBLY be this word
                //remove all words that do not fit after this new word has been added
                List<string[]> remainingReferenceWords = new List<string[]>();
                foreach (string[] referenceWord in currentReferenceWords)
                {
                    if (referenceWord[currentLongWordIndex] == word)
                    {
                        remainingReferenceWords.Add(referenceWord);
                    }
                }
                currentReferenceWords = remainingReferenceWords;

                // this could still be at least one word
                if (currentReferenceWords.Count > 0)
                {
                    currentWordList.Add(wordInfo);
                    //the word is done
                    if (currentWordList.Count == currentReferenceWords[0].Length)
                    {
                        isWord = true;
                        wordCollection = currentWordList.ToArray();
                        currentReferenceWords = new List<string[]>();
                        currentWordList = new List<TMP_WordInfo>();
                        currentLongWordIndex = 0;
                        return isWord;
                    }
                    else
                        return false;
                }
                // this wasnt actually a long word
                else
                {
                    currentReferenceWords = new List<string[]>();
                    currentWordList = new List<TMP_WordInfo>();
                    currentLongWordIndex = 0;
                }
            }
        }

        //is this the first word of a longer word chain?
        if (currentWordList.Count == 0)
        {
            foreach (string[] words in longWordTagSingular.Keys)
            {
                if (words[0] == word)
                {
                    currentReferenceWords.Add(words);
                }
            }
            if (currentReferenceWords.Count > 0)
            {
                currentWordList.Add(wordInfo);
                currentLongWordIndex = 0;
                return false;
            }
        }
        //is this just a regular word?
        if (wordTag.ContainsKey(word))
        {
            isWord = true;
            wordCollection = new TMP_WordInfo[] { wordInfo };
            //Might be temporary, but delete long word progress, when finding a short word
            currentReferenceWords = new List<string[]>();
            currentWordList = new List<TMP_WordInfo>();
            currentLongWordIndex = 0;
            return isWord;
        }
        // is this word on the block list?
        if (ReferenceManager.instance.blockListOn && BinaryListSearcher.SortedStringListContains(blocked, word, 0, blocked.Count))
        {
            currentReferenceWords = new List<string[]>();
            currentWordList = new List<TMP_WordInfo>();
            currentLongWordIndex = 0;
            return false;
        }

        //none of the above applied, so this is a filler word
        if (currentWordList.Count == 0)
        {
            // Game Mode: all Interactable Words
            if (ReferenceManager.instance.allWordsInteractable)
            {
                isWord = true;
                isFillerWord = true;
                wordCollection = new TMP_WordInfo[] { wordInfo };
                //delete long word progress if not already done
                currentReferenceWords = new List<string[]>();
                currentWordList = new List<TMP_WordInfo>();
                currentLongWordIndex = 0;
                return isWord;
            }
        }
        return isWord;
    }
}
public class StopWordSorter
{
    List<string> listToSort;
    public StopWordSorter(string[] arrayToSort)
    {
        Start(ArrayToList(arrayToSort));
    }
    public StopWordSorter(List<string> listToSort)
    {
        Start(listToSort);
    }
    void Start(List<string> listToSort)
    {
        AssignValues(listToSort);
        this.listToSort.Sort();
        RemoveDuplicates();
    }
    void AssignValues(List<string> listToSort)
    {
        this.listToSort = listToSort;
    }
    List<string> ArrayToList(string[] array)
    {
        List<string> list = new List<string>();
        foreach (string word in array)
            list.Add(word);
        return list;
    }
    void RemoveDuplicates()
    {
        List<string> noDuplicates = new List<string>();
        for (int i = 0; i < listToSort.Count - 1; i++)
        {
            if (listToSort[i] != listToSort[i + 1])
                noDuplicates.Add(listToSort[i]);
        }
        listToSort = noDuplicates;
    }
    public List<string> GetSortedList()
    {
        return listToSort;
    }
}
public static class BinaryListSearcher
{
    public static bool SortedStringListContains(List<string> listToSearch, string wordToFind, int startIndex, int endIndex)
    {
        if ((endIndex - startIndex) < 1.1f)
        {
            if (listToSearch[startIndex] == wordToFind)
                return true;
            else if (listToSearch[endIndex] == wordToFind)
                return true;
            else
                return false;
        }

        int middle = startIndex + (endIndex - startIndex) / 2;
        int comparisonMiddleAndFind = string.Compare(listToSearch[middle], wordToFind);

        if (comparisonMiddleAndFind < 0) // find is smaller than middle
            return SortedStringListContains(listToSearch, wordToFind, middle, endIndex);
        else if (comparisonMiddleAndFind > 0) //find is bigger than middle
            return SortedStringListContains(listToSearch, wordToFind, startIndex, middle);
        else //find is middle
            return true;
    }
}