using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Yarn.Unity;
using TMPro;

public class DialogueInputManager : MonoBehaviour
{
    [SerializeField] Color normalColor, interactableColor, interactedColor;
    [SerializeField] GameObject wordCase, playerInput, npcDialogueField, uiLog;
    [SerializeField] TMP_Text[] interactableTextList;
    public static DialogueInputManager instance;
    InputMap controls;
    bool textFinished;
    public bool continueEnabled = true;
    public bool closeAWindow;
    public string mouseOverUIObject;
    DialogueRunner runner;
    DialogueUI uiHandler;

    private void Awake()
    {
        instance = this;
        controls = new InputMap();
    }
    private void Start()
    {
        runner = FindObjectOfType<DialogueRunner>();
        uiHandler = FindObjectOfType<DialogueUI>();
        controls.Dialogue.Click.performed += context => ContinueText();
    }
    private void Update()
    {
        GetMouseOverUI();
    }
    void ContinueText()
    {
        if (textFinished && continueEnabled)
        {
            uiHandler.MarkLineComplete();
            //Destroy all non used buttons
        }
    }
    public void TextFinished()
    {
        textFinished = true;
        // Color all interactable words
        WordUtilities.ColorAllInteractableWords(interactableTextList[0], interactableColor, 
            WordLookupReader.instance.wordTag);
    }
    public void TextUnfinished()
    {
        textFinished = false;
    }
    public void ContinueButton()
    {
        uiHandler.MarkLineComplete();
        continueEnabled = true;
        closeAWindow = true;
        //Destroy all non used buttons
    }
    public Vector2 GetMousePos()
    {
        Vector2 mousePos = controls.Dialogue.MousePosition.ReadValue<Vector2>();
        return mousePos;
    }

    /// <summary>
    /// Raycasts for the UI Elements, checks, above which Category of UI Element 
    /// the mouse is currently Hovering over & saves it to mouseOverUIObject
    /// Then Checks the exact word the UI is hovering over & Creates a button
    /// </summary>
    public void GetMouseOverUI()
    {
        //initialization & Raycast For UI Elements
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = DialogueInputManager.instance.GetMousePos();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        //Check above which Category of UI Element it is currently Hovering
        mouseOverUIObject = "none";
        foreach (RaycastResult uIObject in results)
        {
            if (uIObject.gameObject == wordCase)
                mouseOverUIObject = "wordCase";
            else if (uIObject.gameObject == playerInput)
                mouseOverUIObject = "playerInput";
        }

        //Check for the exact word the mouse is hovering over
        foreach (RaycastResult uIObject in results)
        {
            foreach (TMP_Text text in interactableTextList)
            {
                //if the mouse is currently over an Interactable text
                if (uIObject.gameObject == text.gameObject)
                {
                    int wordIndex = TMP_TextUtilities.FindIntersectingWord(text, eventDataCurrentPosition.position, eventDataCurrentPosition.enterEventCamera);
                    if (wordIndex != -1)
                    {
                        TMP_WordInfo wordInfo = text.textInfo.wordInfo[wordIndex];
                        TMP_CharacterInfo charInfo = text.textInfo.characterInfo[wordInfo.firstCharacterIndex];
                        if (charInfo.color == interactableColor)
                        {
                            WordClickManager.instance.SendWord(wordInfo.GetWord(), eventDataCurrentPosition.position);
                            // Set the hovered word to interacted
                            WordUtilities.ColorAWord(text, wordInfo.firstCharacterIndex, 
                                wordInfo.lastCharacterIndex, interactedColor); 
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Close the Prompt Menu, after pressing Continue
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public IEnumerator CloseAWindow(GameObject target)
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (!closeAWindow)
        {
            yield return delay;
        }
        target.SetActive(false);
        closeAWindow = false;
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

}
