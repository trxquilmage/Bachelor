using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [HideInInspector] public bool inMenu;
    [HideInInspector] public bool inTutorial;
    int tutorialTextIndex
    {
        get { return TutorialTextIndex; }
        set
        {
            TutorialTextIndex = value;
            PortrayText();
        }
    }
    int TutorialTextIndex = 0;
    ReferenceManager refM;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        refM = ReferenceManager.instance;
    }
    public void LoadScene(int i)
    {
        SceneManager.LoadScene(i);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    /// <summary>
    /// Called when pressing "Esc" while in the game
    /// stops the time and opens the menu
    /// </summary>
    public void EnterMenu()
    {
        inMenu = true;
        //if currentWord -> act as if the word was let go of
        if (WordClickManager.instance.currentWord != null)
        {
            WordClickManager.instance.currentWord.GetComponent<Word>().OnDroppedReactToPosition();
        }
        //stop unity time
        Time.timeScale = 0;

        //open menu
        refM.menuField.SetActive(true);
    }
    /// <summary>
    /// Called on clicking "Continue" in the menu or pressing "Esc" a second time
    /// closes the menu and resumes time
    /// </summary>
    public void ExitMenu()
    {
        //resume time
        Time.timeScale = 1;

        //close menu
        refM.menuField.SetActive(false);
        inMenu = false;
    }
    /// <summary>
    /// called, when the tutorial button is pressed, opens the tutorial
    /// </summary>
    public void EnterTutorial()
    {
        inTutorial = true;
        //close menu
        refM.menuField.SetActive(false);
        //open tutorial
        refM.tutorialField.SetActive(true);
        //set tutorial text to int 0 and portray the correct text
        tutorialTextIndex = 0;
    }
    /// <summary>
    /// called when the tutorial is closed. re-opens the menu & closes the tutorial
    /// </summary>
    public void ExitTutorial()
    {
        //close tutorial
        refM.tutorialField.SetActive(false);
        //open menu
        refM.menuField.SetActive(true);
        inTutorial = false;
    }
    /// <summary>
    /// Checks whether the menu should be opened or closed.
    /// if the tutorial is open, it closes the tutorial instead
    /// </summary>
    public void PressedEsc()
    {
        if (!inMenu)
            EnterMenu();
        else if (inTutorial)
            ExitTutorial();
        else
            ExitMenu();
    }
    /// <summary>
    /// Take the tutorial text at tutorialTextIndex and portray it in the tutorial text field
    /// </summary>
    /// <param name="i"></param>
    void PortrayText()
    {
        refM.tutorialText.text = refM.tutorialTexts[tutorialTextIndex];
    }
    /// <summary>
    /// pressed to change the tutorial text to the next one
    /// </summary>
    public void ButtonForward()
    {
        if (tutorialTextIndex < refM.tutorialTexts.Length - 1)
            tutorialTextIndex++;
        else
            tutorialTextIndex = 0;
    }
    /// <summary>
    /// pressed to change the tutorial text to the previous one
    /// </summary>
    public void ButtonBack()
    {
        if (tutorialTextIndex != 0)
            tutorialTextIndex--;
        else
            tutorialTextIndex = refM.tutorialTexts.Length - 1;
    }
}
