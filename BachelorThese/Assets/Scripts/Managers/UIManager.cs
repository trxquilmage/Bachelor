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
}
