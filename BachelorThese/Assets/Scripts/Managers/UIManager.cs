using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Handles UI Functions

    [HideInInspector] public bool isInteracting;
    public static UIManager instance;
    public RefBool[] activeEffects;
    [HideInInspector] public RefBool clickFeedbackIsRunning = new RefBool() { refBool = false };
    [HideInInspector] public float buttonWidth = 0;
    ReferenceManager refM;
    float scaleFactor = 0;
    float timer = 0;
    GameObject currentTrashCan;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        refM = ReferenceManager.instance;
        scaleFactor = refM.canvas.scaleFactor;
        ColorUI();
        FillActiveEffects();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.5f)
        {
            timer = 0;
            if (refM.dialogueCanvas.scaleFactor != scaleFactor)
            {
                OnScale();
            }
        }
    }
    /// <summary>
    /// Portray The E-Button Feedack On The Canvas
    /// </summary>
    /// <param name="target"></param>
    public void PortrayButton(GameObject target, Image sprite)
    {
        if (target == null) //target == null : hide button
        {
            sprite.enabled = false;
            return;
        }
        //place e button correctly
        Vector2 targetInScreenSpace = Camera.main.WorldToScreenPoint(target.transform.position) / refM.canvas.scaleFactor;
        sprite.transform.localPosition = targetInScreenSpace + Vector2.right * 80f;

        sprite.enabled = true; //show e button
    }
    /// <summary>
    /// Changes the trashcan icon and makes sure that putting the word back is disabled while over the can
    /// </summary>
    /// <param name="open"></param>
    public void SwitchTrashImage(bool open, GameObject trashCan)
    {
        if (open)
        {
            trashCan.GetComponentsInChildren<Image>()[1].sprite = refM.trashCanImage02;
            currentTrashCan = trashCan;
        }
        else if (currentTrashCan != null && !open)
        {
            currentTrashCan.GetComponentsInChildren<Image>()[1].sprite = refM.trashCanImage01;
            currentTrashCan = null;
        }
    }
    /// <summary>
    /// When the Canvas is scaled, color data is lost, so re-do it, whe scaled
    /// </summary>
    void OnScale()
    {
        scaleFactor = refM.dialogueCanvas.scaleFactor;
        foreach (TMP_Text text in ReferenceManager.instance.interactableTextList)
        {
            EffectUtilities.ReColorAllInteractableWords();
        }
    }
    /// <summary>
    /// Gives the correct colors to all UI Elements
    /// </summary>
    void ColorUI()
    {
        //Declare variables
        GameObject tagParent = refM.tagButtonParent;

        //activate UI elements
        refM.wordCase.SetActive(true);
        refM.nPCDialogueField.SetActive(true);
        refM.playerInputField.SetActive(true);
        refM.askField.SetActive(true);
        refM.askNPCField.SetActive(true);
        refM.questCase.SetActive(true);

        InitializeButtons();

        refM.ask.GetComponent<Image>().color = refM.askColor;
        refM.barter.GetComponent<Image>().color = refM.askColor;
        refM.trashCan.GetComponent<Image>().color = refM.trashColor;
        refM.wButton.GetComponent<Image>().color = refM.headerColor;
        refM.qButton.GetComponent<Image>().color = refM.headerColor;

        refM.playerInputField.GetComponent<Image>().color = refM.inputFieldColor;
        refM.npcDialogueTextBox.GetComponent<Image>().color = refM.textFieldColor;
        refM.npcDialogueTextBoxAsk.GetComponent<Image>().color = refM.textFieldColor;
        refM.askField.GetComponent<Image>().color = refM.inputFieldColor;

        //word case is colored on OpenOnTag()
        refM.wordLimit.GetComponentInParent<Image>().color = refM.limitColor;

        //startwords
        refM.questionBackground.color = refM.textFieldColor;
        refM.traitBackground.color = refM.textFieldColor;
        refM.generatorBackground.color = refM.textFieldColor;
        refM.questionContinueButton.color = refM.interactableButtonColor;
        refM.traitContinueButton.color = refM.interactableButtonColor;
        refM.generatorContinueButton.color = refM.interactableButtonColor;

        //buttons
        refM.continueButton.GetComponent<Image>().color = refM.interactableButtonColor;
        refM.askButton.GetComponent<Image>().color = refM.interactableButtonColor;
        refM.askContinueButton.GetComponent<Image>().color = refM.interactableButtonColor;

        refM.nameField.GetComponent<Image>().color = refM.nameFieldColor;
        //go through all interactable texts and make them "normal color"
        foreach (TMP_Text text in refM.interactableTextList)
        {
            text.color = refM.normalColor;
        }

        //deactivate ui elements
        refM.wordCase.SetActive(false);
        refM.nPCDialogueField.SetActive(false);
        refM.playerInputField.SetActive(false);
        refM.askField.SetActive(false);
        refM.askNPCField.SetActive(false);
        refM.questCase.SetActive(false);
    }
    /// <summary>
    /// Calls a Couroutine, that blends in the wished UI Element for "time"-seconds
    /// </summary>
    /// <param name="uiElement"></param>
    /// <param name="time"></param>
    public void BlendInUI(GameObject uiElement, float time)
    {
        StartCoroutine(BlendInUIElement(uiElement, time));
    }
    IEnumerator BlendInUIElement(GameObject uiElement, float time)
    {
        uiElement.SetActive(true);
        float currentAlpha;
        Color color;

        Image[] allImages = uiElement.GetComponentsInChildren<Image>();
        TMP_Text[] allTexts = uiElement.GetComponentsInChildren<TMP_Text>();

        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        float timer = 0;

        while (timer < time)
        {
            if (timer < 1)
                currentAlpha = WordUtilities.Remap(timer, 0, 1, 0, 1);
            else if (timer > time - 1)
                currentAlpha = WordUtilities.Remap(timer, time - 1, time, 1, 0);
            else
                currentAlpha = 1;
            timer += Time.deltaTime;
            foreach (TMP_Text text in allTexts)
            {
                color = text.color;
                color.a = currentAlpha;
                text.color = color;
            }
            foreach (Image image in allImages)
            {
                color = image.color;
                color.a = currentAlpha;
                image.color = color;
            }
            yield return delay;
        }
        uiElement.SetActive(false);
    }
    public void PlayVFX(VisualEffect vfx)
    {
        if (vfx != null)
            vfx.SendEvent("Start");
    }
    public IEnumerator DestroyVFX(VisualEffect vfx)
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        float timer = 0;

        //we wait until the lifetime is over
        while (vfx.GetFloat("lifetime") > timer)
        {
            timer += Time.deltaTime;
            yield return delay;
        }

        Destroy(vfx.gameObject);
    }

    /// <summary>
    /// Create All Buttons in the WordCaseManager that are used to change the current tag
    /// </summary>
    void InitializeButtons()
    {
        int i = 0;
        foreach (WordInfo.WordTag tag in refM.wordTags)
        {
            //initialize the "other" tag last
            if (tag.name != refM.wordTags[ReferenceManager.instance.otherTagIndex].name)
            {
                InstantiateButton(i, tag);
            }
            i++;
        }
        //initialize the "Other" tag last
        InstantiateButton(refM.otherTagIndex, refM.wordTags[refM.otherTagIndex]);

        //get the width of all buttons combined
        float padding = refM.tagButtonParent.GetComponent<VerticalLayoutGroup>().spacing;
        buttonWidth += padding;
        foreach (RectTransform rT in refM.tagButtonParent.GetComponentsInChildren<RectTransform>())
        {
            if (rT.gameObject.TryGetComponent<Button>(out Button button))
            {
                buttonWidth += rT.sizeDelta.x;
                buttonWidth += padding;
            }
        }
        //minus the size thats already on screen
        buttonWidth -= refM.tagButtonParent.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
    }
    /// <summary>
    /// Instantiate a Tag-Button (into the wordCase)
    /// </summary>
    /// <param name="i"></param>
    /// <param name="tag"></param>
    void InstantiateButton(int i, WordInfo.WordTag tag)
    {
        GameObject button = GameObject.Instantiate(refM.buttonPrefab, Vector2.zero, Quaternion.identity);
        button.transform.SetParent(refM.tagButtonParent.transform, false);
        TagButtonInfo info = button.GetComponent<TagButtonInfo>();
        info.Initialize(i, tag);
    }
    public void StartClickFeedback()
    {
        clickFeedbackIsRunning.refBool = true;
        StartCoroutine(EffectUtilities.AlphaWave(refM.rightClickIcon, clickFeedbackIsRunning));
        StartCoroutine(EffectUtilities.AlphaWave(refM.rightClickAskIcon, clickFeedbackIsRunning));
    }
    /// <summary>
    /// When a right click happened, fade out the current feedback to rightclick and briefly light it up
    /// </summary>
    /// <param name="isInAsk"></param>
    public void OnRightClicked(bool isInAsk)
    {
        //Fade out the click Feedback
        clickFeedbackIsRunning.refBool = false;
        StartCoroutine(LightUpFeedback(isInAsk));
    }
    IEnumerator LightUpFeedback(bool isInAsk)
    {
        yield return new WaitForEndOfFrame();
        Image rightClickIcon;
        if (!isInAsk)
            rightClickIcon = refM.rightClickIcon;
        else
            rightClickIcon = refM.rightClickAskIcon;

        Color startColor = rightClickIcon.color;
        startColor.a = 1;
        Color32 endColor = startColor;
        endColor.a = 1; //not zero bc otherwise its indistinguishable from new Color()
        StartCoroutine(EffectUtilities.ColorObjectInGradient(rightClickIcon.gameObject, new Color[5] { startColor, new Color(), Color.Lerp(rightClickIcon.color, Color.red, 0.8f), new Color(), endColor }, 0.5f)); ;
    }
    void FillActiveEffects()
    {
        activeEffects = new RefBool[20];
        for (int i = 0; i < activeEffects.Length; i++)
        {
            activeEffects[i] = new RefBool();
        }
    }
    /// <summary>
    /// throw a word out of the case OR throw a quest out of the log
    /// </summary>
    public void TrashAWord()
    {
        BubbleData data = WordClickManager.instance.currentWord.GetComponent<Bubble>().data;
        if (data is WordData && data.origin == WordInfo.Origin.WordCase)
        {
            Bubble bubble = WordClickManager.instance.currentWord.GetComponent<Bubble>();
            if (!bubble.data.permanentWord)
            {
                //spawn delete-vfx
                WordClickManager.instance.currentWord.GetComponent<Bubble>().CallEffect(1);

                WordCaseManager.instance.DeleteOutOfCase();
                WordClickManager.instance.DestroyCurrentWord();
                WordCaseManager.instance.UpdateContentCount();
                StartCoroutine(WordCaseManager.instance.RescaleScrollbar(true));
                WordCaseManager.instance.ResetScrollbar();
                WordCaseManager.instance.DestroyReplacement();
            }
            else
            {
                bubble.IsOverNothing();
                BlendInUI(refM.feedbackTextTrashYesNo, 3);
            }
        }
    }
    public void ClickedICantSay()
    {
        CommandManager.instance.iCantSay = true;
    }
}
