using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager instance;
    public GameObject selectedWordParent, selectedWordPrefab;
    void Awake()
    {
        instance = this;
    }
}
