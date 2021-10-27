using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour{
    
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start(){
        Settings.Instance.SaveTicks();
    }

    
    public GameObject gameCanvasPrefab;
    public EventSystem eventSystem;
    [Header("Vertical")] public GameObject homeScreen;
    public GameObject settingsScreen, completedScreen, matchPlusScreen, currentGameCanvas;

    [Header("Horizontal")] public GameObject horizhomeScreen;
    public GameObject horizsettingsScreen, horizcompletedScreen, horizmatchPlusScreen, horizcurrentGameCanvas;

    public List<int> latestDeckOrder;

    public void SaveOrder(List<Card> cards){
        for (int i = 0; i < cards.Count; i++){
            latestDeckOrder.Add(int.Parse(cards[i].gameObject.name));
        }
    }
    
    public void NewMatch(){
        if(currentGameCanvas != null) DestroyImmediate(currentGameCanvas);
        currentGameCanvas = Instantiate(gameCanvasPrefab);
        currentGameCanvas.GetComponent<CardController>().m_EventSystem = eventSystem;
        homeScreen.SetActive(false);
    }
    
    public void Continue()
    {
        if (currentGameCanvas != null){
            homeScreen.SetActive(false);
        }
        else{
            NewMatch();
        }
    }

    public void Restart(){
        PlayerPrefs.SetInt("Restart",1);
        NewMatch();
    }
    
    public void Completed(int move, float time, int points)
    {
        completedScreen.SetActive(true);
    }
    
    
    public void LeaderBoard()
    {
        
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
    

    private void OnApplicationPause(bool pauseStatus){
        
        
    }
}
