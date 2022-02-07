using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

[ExecuteInEditMode]
public class StopWordsLookupReader : MonoBehaviour
{
    public static StopWordsLookupReader instance;
    public List<string> blockedWords;

    string dataPath;
    string allBlocked;
    List<string> blocked;
    StopWordSorter bucketSort;
    void Awake()
    {
        instance = this;
    }
    public void StartSorting()
    {
        instance = this;
        dataPath = Application.dataPath + "/Data/" + "002";

        StartBucketSort();
    }
    void StartBucketSort()
    {
        blocked = new List<string>();
        blockedWords = new List<string>();
        LookUpBlocked();

        bucketSort = new StopWordSorter(blocked);
        blockedWords = bucketSort.GetSortedList();
    }
    void LookUpBlocked()
    {
        string pathBlocked = dataPath + "/StopWords.txt";
        allBlocked = File.ReadAllText(pathBlocked);
        string[] lines = allBlocked.Split("\n"[0]);
        foreach (string s in lines)
        {
            if (s == "")
                break;
            string line = s.Substring(0, s.Length - 1);
            line = WordUtilities.CapitalizeAllWordsInString(line);
            blocked.Add(line);
        }
    }
}
