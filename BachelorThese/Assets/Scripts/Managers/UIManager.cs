using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Handles UI Functions

    [HideInInspector] public bool isInteracting;
    public static UIManager instance;
    ReferenceManager refM;
    WordCaseManager wcM;
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
        wcM = WordCaseManager.instance;
        scaleFactor = refM.canvas.scaleFactor;
        ColorUI();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1)
        {
            timer = 0;
            if (refM.canvas.scaleFactor != scaleFactor)
            {
                OnScale();
            }
        }
    }
    /// <summary>
    /// Portray The E-Button Feedack On The Canvas
    /// </summary>
    /// <param name="target"></param>
    public void PortrayEButton(GameObject target)
    {
        if (target == null) //target == null : hide e button
        {
            refM.eButtonSprite.enabled = false;
            return;
        }
        //place e button correctly
        Vector2 targetInScreenSpace = Camera.main.WorldToScreenPoint(target.transform.position);
        refM.eButtonSprite.transform.position = targetInScreenSpace + Vector2.right * 40f;

        refM.eButtonSprite.enabled = true; //show e button
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
        scaleFactor = refM.canvas.scaleFactor;
        if (DialogueInputManager.instance.isActiveAndEnabled)
        {
            foreach (TMP_Text text in ReferenceManager.instance.interactableTextList)
                WordUtilities.ReColorAllInteractableWords();
        }

    }
    /// <summary>
    /// Gives the correct colors to all UI Elements
    /// </summary>
    void ColorUI()
    {
        //Declare variables
        Image[] buttons = WordCaseManager.instance.buttons;
        GameObject tagParent = refM.tagButtonParent;

        //activate UI elements
        refM.wordCase.SetActive(true);
        refM.nPCDialogueField.SetActive(true);
        refM.playerInputField.SetActive(true);
        refM.askField.SetActive(true);
        refM.askNPCField.SetActive(true);
        refM.questCase.SetActive(true);

        //color everything
        buttons = tagParent.GetComponentsInChildren<Image>();
        //[0] is spacing
        buttons[1].color = refM.allColor;
        buttons[2].color = refM.locationColor;
        buttons[3].color = refM.otherColor;
        buttons[4].color = refM.nameColor;
        buttons[5].color = refM.itemColor;
        refM.ask.GetComponent<Image>().color = refM.askColor;
        refM.barter.GetComponent<Image>().color = refM.askColor;
        refM.trashCan.GetComponent<Image>().color = refM.askColor;
        refM.wButton.GetComponent<Image>().color = refM.askColor;
        refM.qButton.GetComponent<Image>().color = refM.askColor;

        refM.playerInputField.GetComponent<Image>().color = refM.inputFieldColor;
        refM.nPCDialogueField.GetComponent<Image>().color = refM.textFieldColor;
        refM.askNPCField.GetComponent<Image>().color = refM.textFieldColor;
        refM.askField.GetComponent<Image>().color = refM.inputFieldColor;

        //color quest case (word case is colored on OpenOnTag())
        Color color = refM.questColor;
        Color greyColor = Color.Lerp(color, Color.grey, 0.35f);
        Color colorHighlight = Color.Lerp(color, Color.white, 0.3f);
        refM.questCase.GetComponent<Image>().color = greyColor;
        refM.questLimit.GetComponentInParent<Image>().color = greyColor;
        refM.questScrollbar.GetComponent<Image>().color = greyColor;
        refM.questScrollbar.transform.GetChild(0).GetComponentInChildren<Image>().color = greyColor;
        refM.questTrashCan.GetComponent<Image>().color = refM.askColor;
        
        refM.continueButton.GetComponent<Image>().color = refM.interactableButtonColor;
        refM.askButton.GetComponent<Image>().color = refM.interactableButtonColor;
        refM.askContinueButton.GetComponent<Image>().color = refM.interactableButtonColor;

        refM.nameField.GetComponent<Image>().color = refM.nameFieldColor;
        //go through all interactable texts and make them "normal color"
        foreach (TMP_Text text in GameObject.FindObjectsOfType<TMP_Text>())
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
}
