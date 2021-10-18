using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    // Manager for inaccessible Object References
    public static ReferenceManager instance;
    public GameObject selectedWordParent, selectedWordPrefab;
    void Awake()
    {
        instance = this;
    }
}
