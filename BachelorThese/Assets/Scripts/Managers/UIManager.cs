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
    WordCaseManager wcM;
    WordClickManager wclM;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        refM = ReferenceManager.instance;
        wcM = WordCaseManager.instance;
        wclM = WordClickManager.instance;
        scaleFactor = refM.canvas.scaleFactor;
        ColorUIOnGameStart();
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
    public void PortrayOrHideInputButtonFeedback(GameObject target, Image sprite)
    {
        if (target == null)
        {
            sprite.enabled = false;
            return;
        }

        Vector2 targetInScreenSpace = Camera.main.WorldToScreenPoint(target.transform.position) / refM.canvas.scaleFactor;
        sprite.transform.localPosition = targetInScreenSpace + Vector2.right * 70;

        sprite.enabled = true;
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
    void ColorUIOnGameStart()
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

        InitializeWordCaseTagSwitchButtons();

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

    void InitializeWordCaseTagSwitchButtons()
    {
        int i = 0;
        foreach (WordInfo.WordTag tag in refM.wordTags)
        {

            if (tag.name != refM.wordTags[refM.otherTagIndex].name)
            {
                InstantiateTagSwitchButton(i, tag);
            }
            i++;
        } //initialize the "other" tag last
        InstantiateTagSwitchButton(refM.otherTagIndex, refM.wordTags[refM.otherTagIndex]);

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
    void InstantiateTagSwitchButton(int i, WordInfo.WordTag tag)
    {
        GameObject button = GameObject.Instantiate(refM.buttonPrefab, Vector2.zero, Quaternion.identity);
        button.transform.SetParent(refM.tagButtonParent.transform, false);
        TagButtonInfo info = button.GetComponent<TagButtonInfo>();
        info.Initialize(i, tag);
    }
    public void StartFeedbackRightClickDialoge()
    {
        clickFeedbackIsRunning.refBool = true;
        StartCoroutine(EffectUtilities.AlphaWave(refM.rightClickIcon, clickFeedbackIsRunning));
        StartCoroutine(EffectUtilities.AlphaWave(refM.rightClickAskIcon, clickFeedbackIsRunning));
    }
    public void OnRightClicked(bool isInAsk)
    {
        FadeOutRightClickFeedback();
        StartCoroutine(LightUpFeedback(isInAsk));
    }
    void FadeOutRightClickFeedback()
    {
        clickFeedbackIsRunning.refBool = false;
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
        StartCoroutine(EffectUtilities.ColorObjectAndChildrenInGradient(rightClickIcon.gameObject, new Color[5] { startColor, new Color(), Color.Lerp(rightClickIcon.color, Color.red, 0.8f), new Color(), endColor }, 0.5f)); ;
    }
    void FillActiveEffects()
    {
        activeEffects = new RefBool[20];
        for (int i = 0; i < activeEffects.Length; i++)
        {
            activeEffects[i] = new RefBool();
        }
    }
    public void TrashAWord()
    {
        if (wclM.currentWord != null)
        {
            BubbleData data = wclM.currentWord.GetComponent<Bubble>().data;
            if (data is WordData && data.IsFromWordCase())
            {
                Bubble bubble = wclM.currentWord.GetComponent<Bubble>();
                if (!bubble.data.permanentWord)
                {
                    if (bubble.data.isFavorite)
                        CallSafetyCheck();
                    else
                        DeleteBubble();
                }
                else
                {
                    bubble.DroppedOverNothing();
                    BlendInUI(refM.warningTrashYesNo, 3);
                }
            }
        }
    }
    void CallSafetyCheck()
    {
        Time.timeScale = 0;
        refM.areYouSureField.SetActive(true);
    }
    public void ButtonCallAnswerAreYouSure(bool isAccepted)
    {
        Time.timeScale = 1;
        refM.areYouSureField.SetActive(false);
        if (isAccepted)
            DeleteBubble();
        else
            wclM.currentWord.GetComponent<Word>().DroppedOverNothing();
    }
    void DeleteBubble()
    {
        //spawn delete-vfx
        wclM.currentWord.GetComponent<Bubble>().CallEffect(1);

        wcM.DeleteOutOfCase();
        wclM.DestroyCurrentWord();
        wcM.UpdateContentCount();
        StartCoroutine(wcM.RescaleScrollbar());
        wcM.ResetScrollbar();
        wcM.DestroyReplacement();
    }
    public void ClickedICantSay()
    {
        CommandManager.instance.iCantSay = true;
    }
}
