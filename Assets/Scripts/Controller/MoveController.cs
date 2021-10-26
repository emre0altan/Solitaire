using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class MoveController : MonoBehaviour
{
    public static MoveController Instance;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        commands = new List<Command>();
        commandStrings = new List<string>();
    }

    public List<Command> commands;
    public List<String> commandStrings;
    public int currentCommand = -1;
    public Text moveValue;

    private int totalMoves = 0;

    public void AddCommand(Command newCommand){
        newCommand.Execute();
        if (currentCommand != commands.Count - 1){
            currentCommand++;
            commands[currentCommand] = newCommand;
            commandStrings[currentCommand] = newCommand.myToString();
        }
        else{
            commands.Add(newCommand);
            commandStrings.Add(newCommand.myToString());
            currentCommand++;
        }

        totalMoves++;
        moveValue.text = totalMoves.ToString();
    }

    public void UndoCommand(){
        if(currentCommand < 0) return; 
        commands[currentCommand].Undo();
        currentCommand--;
        
        totalMoves++;
        moveValue.text = totalMoves.ToString();
    }

}
