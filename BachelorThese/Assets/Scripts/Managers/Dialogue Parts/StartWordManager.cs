using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWordManager : MonoBehaviour
{
    public static StartWordManager instance;

    ReferenceManager refM;
    void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        refM = ReferenceManager.instance;

        //Only starts the game with this screen if its enabled
        if (refM.startWithStartWords)
            CallStartWordScreen();
    }
    public void CallStartWordScreen()
    {

    }
}
