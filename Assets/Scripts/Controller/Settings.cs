using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour{
    public static Settings Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    public GameObject[] ticks;

    public void TickOption(int i){
        ticks[i].SetActive(!ticks[i].activeSelf);
        if (i == 0){
            PlayerPrefs.SetInt("Draw3",ticks[i].activeSelf?1:0);
        }
        else if (i == 1){
            PlayerPrefs.SetInt("VegasMode",ticks[i].activeSelf?1:0);
        }
        else if (i == 2){
            PlayerPrefs.SetInt("Help",ticks[i].activeSelf?1:0);
        }
        else if (i == 3){
            PlayerPrefs.SetInt("Timer",ticks[i].activeSelf?1:0);
        }
        else if (i == 4){
            PlayerPrefs.SetInt("Sounds",ticks[i].activeSelf?1:0);
        }
        else if (i == 5){
            PlayerPrefs.SetInt("LockOrientation",ticks[i].activeSelf?1:0);
        }
    }

    public void SaveTicks(){
        PlayerPrefs.SetInt("Draw3",ticks[0].activeSelf?1:0);
        PlayerPrefs.SetInt("VegasMode",ticks[1].activeSelf?1:0);
        PlayerPrefs.SetInt("Help",ticks[2].activeSelf?1:0);
        PlayerPrefs.SetInt("Timer",ticks[3].activeSelf?1:0);
        PlayerPrefs.SetInt("Sounds",ticks[4].activeSelf?1:0);
        PlayerPrefs.SetInt("LockOrientation",ticks[5].activeSelf?1:0);
    }
    
}
