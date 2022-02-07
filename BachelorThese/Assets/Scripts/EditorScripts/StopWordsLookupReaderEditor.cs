using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StopWordsLookupReader))]
public class StopWordsLookupReaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StopWordsLookupReader myTarget = (StopWordsLookupReader)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Sort"))
        {
            myTarget.StartSorting();
        }
    }
}
