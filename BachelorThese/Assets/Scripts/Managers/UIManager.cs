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
    public void SwitchTrashImage(bool open)
    {
        WordCaseManager.instance.overTrashcan = open;
        if (open)
        {
            refM.trashCan.GetComponentsInChildren<Image>()[1].sprite = refM.trashCanImage02;
        }
        else
        {
            refM.trashCan.GetComponentsInChildren<Image>()[1].sprite = refM.trashCanImage01;
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

        //color everything
        buttons = tagParent.GetComponentsInChildren<Image>();
        buttons[0].color = refM.allColor;
        buttons[1].color = refM.locationColor;
        buttons[2].color = refM.generalColor;
        buttons[3].color = refM.nameColor;
        buttons[4].color = refM.itemColor;
        refM.ask.GetComponent<Image>().color = refM.askColor;
        refM.barter.GetComponent<Image>().color = refM.askColor;
        refM.trashCan.GetComponent<Image>().color = refM.askColor;
        refM.wButton.GetComponent<Image>().color = refM.askColor;

        refM.playerInputField.GetComponent<Image>().color = refM.inputFieldColor;
        refM.nPCDialogueField.GetComponent<Image>().color = refM.textFieldColor;
        refM.askNPCField.GetComponent<Image>().color = refM.textFieldColor;
        refM.askField.GetComponent<Image>().color = refM.inputFieldColor;
        
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
        ReferenceManager.instance.wordCase.SetActive(false);
        ReferenceManager.instance.nPCDialogueField.SetActive(false);
        ReferenceManager.instance.playerInputField.SetActive(false);
        ReferenceManager.instance.askField.SetActive(false);
        ReferenceManager.instance.askNPCField.SetActive(false);
    }
}
