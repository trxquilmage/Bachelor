using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Yarn.Unity;
using TMPro;

public class DialogueInputManager : MonoBehaviour
{
    [SerializeField] GameObject wordCase, playerInput, npcDialogueField, uiLog;
    public TMP_Text[] interactableTextList;
    public static DialogueInputManager instance;
    public PromptBubble promptBubble;

    public bool continueEnabledPrompt = true;
    public bool continueEnabledDrag = true;
    public bool closeAWindow;
    public string mouseOverUIObject;

    DialogueRunner runner;
    DialogueUI uiHandler;
    InputMap controls;
    bool textFinished;
    bool stillOnWord;

    private void Awake()
    {
        instance = this;
        controls = new InputMap();
    }
    private void Start()
    {
        runner = FindObjectOfType<DialogueRunner>();
        uiHandler = FindObjectOfType<DialogueUI>();
        controls.Dialogue.Click.performed += context => ContinueTextOnClick();
    }
    private void Update()
    {
        GetMouseOverUI();
    }
    /// <summary>
    /// Called whenever a click happens
    /// </summary>
    void ContinueTextOnClick()
    {
        if (textFinished && continueEnabledPrompt && continueEnabledDrag)
        {
            Continue();
        }
    }
    /// <summary>
    /// Called, whenever a dialogue should be continued
    /// </summary>
    void Continue()
    {
        uiHandler.MarkLineComplete();
        //Destroy all buttons you can find
        WordClickManager.instance.DestroyAllActiveWords();
    }
    /// <summary>
    /// When a line is not yet done (called in Dialogue Runner) -> disable continue for ContinueText()
    /// </summary>
    public void TextFinished()
    {
        textFinished = true;

        // Color all interactable words, force update, so there are no errors
        interactableTextList[0].ForceMeshUpdate();
        WordUtilities.ColorAllInteractableWords(interactableTextList[0], WordLookupReader.instance.wordTag);
    }
    /// <summary>
    /// When a line is done (called in Dialogue Runner) -> enable continue  for ContinueText()
    /// </summary>
    public void TextUnfinished()
    {
        textFinished = false;
    }
    /// <summary>
    /// Called, when the Continue Button in the UI is pressed
    /// </summary>
    public void ContinueButton()
    {
        if (PlayerInputManager.instance.CheckForPromptsFilled())
        {
            PlayerInputManager.instance.ReactToInput();
            Continue();
            continueEnabledPrompt = true;
            closeAWindow = true;
            PlayerInputManager.instance.DeleteAllPrompts();
            WordClickManager.instance.currentWord = null;
        }
    }
    /// <summary>
    /// Return the mouse position in screen space
    /// </summary>
    /// <returns></returns>
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
        bool foundPB = false; //found a prompt bubble
        bool foundtC = false; //found the trash can
        foreach (RaycastResult uIObject in results)
        {
            //over the wordcase
            if (uIObject.gameObject == wordCase)
                mouseOverUIObject = "wordCase";
            //over a promptbubble
            else if (uIObject.gameObject.TryGetComponent<PromptBubble>(out PromptBubble pB))
            {
                promptBubble = pB;
                foundPB = true;
                mouseOverUIObject = "playerInput";
                promptBubble.OnBubbleHover(true);
            }
            //over the trashcan
            else if (uIObject.gameObject == ReferenceManager.instance.trashCan)
            {
                foundtC = true;
                mouseOverUIObject = "trashCan";
                UIManager.instance.SwitchTrashImage(true);
            }
        }
        if (!foundPB) //if not hovering over prompt
        {
            if (promptBubble != null)
            {
                promptBubble.OnBubbleHover(false);
                promptBubble = null;
            }
        }
        if (!foundtC) //if not over tC
        {
            {
                UIManager.instance.SwitchTrashImage(false);
            }
        }
        stillOnWord = false;
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


                        //Get Color of the first character of the word
                        Color32[] currentCharacterColor = text.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
                        if (currentCharacterColor[charInfo.vertexIndex] == ReferenceManager.instance.interactableColor)
                        {
                            WordUtilities.CreateABubble(text, wordInfo);
                        }
                        //if the mouse is STILL over the created bubble, dont delete it this round
                        if (WordClickManager.instance.wordLastHighlighted != null &&
                            WordClickManager.instance.wordLastHighlighted.GetComponentInChildren<TMP_Text>().text == wordInfo.GetWord())
                        {
                            stillOnWord = true;
                        }
                    }
                }
            }
        }
        if (!stillOnWord && WordClickManager.instance.currentWord == null
            && WordClickManager.instance.wordLastHighlighted != null) //stopped Hovering over the iteractable word
        {
            WordClickManager.instance.DestroyLastHighlighted();
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
