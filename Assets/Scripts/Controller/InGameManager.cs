using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{

    public void Settings(){
        GameManager.Instance.settingsScreen.SetActive(true);
    }
    
    public void Home(){
        GameManager.Instance.homeScreen.SetActive(true);
    }
    
    public void Plus(){
        GameManager.Instance.matchPlusScreen.SetActive(true);
    }
    
    public void Hint(){
        Helper.Instance.Help();
    }
    
    public void Leaderboard(){
        GameManager.Instance.LeaderBoard();
    }
    
}
