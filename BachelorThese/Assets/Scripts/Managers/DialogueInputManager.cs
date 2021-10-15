using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueInputManager : MonoBehaviour
{
    public static DialogueInputManager instance;
    InputMap controls;
    bool textFinished;
    public bool continueEnabled = true;
    public bool closeAWindow;
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
    void ContinueText()
    {
        if (textFinished && continueEnabled)
        {
            uiHandler.MarkLineComplete();
        }
    }
    public void TextFinished()
    {
        textFinished = true;
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
    }
    public Vector2 GetMousePos()
    {
        Vector2 mousePos = controls.Dialogue.MousePosition.ReadValue<Vector2>();
        return mousePos;
    }
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
