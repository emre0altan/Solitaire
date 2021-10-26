using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScreen : MonoBehaviour
{
    private void Start(){
        Settings.Instance.SaveTicks();
    }

    public void NewMatch()
    {
        
    }

    public void Continue()
    {
        
    }   
    
    public void ExitGame()
    {
        Application.Quit();
    }

    public void LeaderBoard()
    {
        
    }
    
}
