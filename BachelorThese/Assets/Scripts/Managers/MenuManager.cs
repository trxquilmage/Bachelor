using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    SubMenu currentSubMenu;
    SubMenu[] allSubMenus;
    [HideInInspector] public bool inMenu;

    ReferenceManager refM;
    private void Awake()
    {
        instance = this;

    }
    private void Start()
    {
        refM = ReferenceManager.instance;
        allSubMenus = new SubMenu[2] {
            new SubMenu("credits", false, refM.creditsField, refM.creditsText, refM.creditsTexts, 0),
            new SubMenu("tutorial", false, refM.tutorialField, refM.tutorialText, refM.tutorialTexts, 0)
            };
    }
    public void LoadScene(int i)
    {
        SceneManager.LoadScene(i);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void EnterMenu()
    {
        inMenu = true;

        if (WordClickManager.instance.currentWord != null)
        {
            WordClickManager.instance.currentWord.GetComponent<Word>().OnDroppedReactToPosition();
        }
       
        Time.timeScale = 0;

        refM.menuField.SetActive(true);
    }

    public void ExitMenu()
    {
        Time.timeScale = 1;
        refM.menuField.SetActive(false);
        inMenu = false;
    }

    public void EnterSubMenu(int i)
    {
        currentSubMenu = allSubMenus[i];
        currentSubMenu.Enter();
    }

    public void ExitSubMenu()
    {
        currentSubMenu.Exit();
        currentSubMenu = new SubMenu();
    }

    public void PressedEsc()
    {
        if (!inMenu)
            EnterMenu();
        else if (currentSubMenu.name != null)
            currentSubMenu.Exit();
        else
            ExitMenu();
    }

    public void ButtonForward()
    {
        if (currentSubMenu.name != null)
            currentSubMenu.SwitchForward();
    }

    public void ButtonBack()
    {
        if (currentSubMenu.name != null)
            currentSubMenu.SwitchBack();
    }
}
public struct SubMenu
{
    public string name;
    public bool inSubMenu;
    public GameObject relatedField;
    public TMP_Text relatedText;
    public string[] relatedTexts;
    int textIndex
    {
        get { return TextIndex; }
        set
        {
            TextIndex = value;
            PortrayText();
        }
    }
    int TextIndex;

    public SubMenu(string s_name, bool s_inSubMenu, GameObject s_relatedField, TMP_Text s_relatedText, string[] s_relatedTexts, int s_textIndex)
    {
        name = s_name;
        inSubMenu = s_inSubMenu;
        relatedField = s_relatedField;
        relatedText = s_relatedText;
        relatedTexts = s_relatedTexts;
        TextIndex = s_textIndex;
        textIndex = s_textIndex;
    }

    public SubMenu(SubMenu subM)
    {
        name = subM.name;
        inSubMenu = subM.inSubMenu;
        relatedField = subM.relatedField;
        relatedText = subM.relatedText;
        relatedTexts = subM.relatedTexts;
        TextIndex = subM.TextIndex;
        textIndex = subM.textIndex;
    }

    void PortrayText()
    {
        relatedText.text = relatedTexts[textIndex];
    }
    public void SwitchBack()
    {
        if (textIndex != 0)
            textIndex--;
        else
            textIndex = relatedTexts.Length - 1;
    }
    public void SwitchForward()
    {
        if (textIndex < relatedTexts.Length - 1)
            textIndex++;
        else
            textIndex = 0;
    }
    public void Enter()
    {
        inSubMenu = true;
        ReferenceManager.instance.menuField.SetActive(false);
        relatedField.SetActive(true);
        textIndex = 0;
    }

    public void Exit()
    {
        relatedField.SetActive(false);
        ReferenceManager.instance.menuField.SetActive(true);
        inSubMenu = false;
    }
}
