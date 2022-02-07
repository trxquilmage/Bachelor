using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class StopWordsLookupReader : MonoBehaviour
{
    public static StopWordsLookupReader instance;
    public List<string> blockedWords;

    string dataPath;
    string allBlocked;
    List<string> blocked;
    BucketSort bucketSort;
    void Awake()
    {
        /*instance = this;
        dataPath = Application.dataPath + "/Data/" + "002";

        LookUpBlocked();
        StartBucketSort();*/
    }
    void StartBucketSort()
    {
        blocked = new List<string>();
        blockedWords = new List<string>();

        bucketSort = new BucketSort(blocked, 0);
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
                return;

            string line = s.Substring(0, s.Length - 1);
            line = WordUtilities.CapitalizeAllWordsInString(line);
            blocked.Add(line);
        }
    }
}
