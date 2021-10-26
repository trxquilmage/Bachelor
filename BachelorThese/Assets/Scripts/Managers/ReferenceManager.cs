using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class ReferenceManager : MonoBehaviour
{
    // Manager for general Object References

    public static ReferenceManager instance;
    
    [Header("Text Colors")]
    public Color normalColor;
    public Color interactableColor;
    public Color interactedColor;
    [Header("Tag Colors")]
    public Color locationColor;
    public Color itemColor;
    public Color nameColor;
    public Color allColor;
    [Header("Other Colors")]
    public Color shadowButtonColor; //Color that mixes into Button Shadows

    [Header("Parents")]
    public GameObject selectedWordParent;
    public GameObject listingParent;
    public GameObject promptBubbleParent;
    public GameObject tagButtonParent;
    [Header("Prefabs")]
    public GameObject selectedWordPrefab;
    public GameObject promptBoxPrefab;
    [Header("Fields")]
    public GameObject nPCDialogueField;
    public GameObject wordCase;
    public Canvas dialogueCanvas;
    [Header("UI Elements")]
    public GameObject trashCan;
    public TMP_Text wordLimit;
    [Header("Dialogues")]
    public DialogueRunner runner;
    [Header("Images")]
    public Sprite trashCanImage01;
    public Sprite trashCanImage02;

    [Header("Game Data")]
    public int maxWordsPerTag;
    void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        
    }
}
