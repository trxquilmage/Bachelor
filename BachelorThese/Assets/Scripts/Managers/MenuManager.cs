using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [HideInInspector] public bool inMenu;
    private void Awake()
    {
        instance = this;
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

        //stop unity time
        Time.timeScale = 0;

        //open menu
        ReferenceManager.instance.menuField.SetActive(true);
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
        ReferenceManager.instance.menuField.SetActive(false);
        inMenu = false;
    }
}
