using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
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
}
