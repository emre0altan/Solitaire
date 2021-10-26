using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class MoveController : MonoBehaviour
{
    public static MoveController Instance;
    public List<Command> commands;
    public List<String> commandStrings;

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

    public int currentCommand = -1;

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
    }

    public void UndoCommand(){
        if(currentCommand < 0) return; 
        commands[currentCommand].Undo();
        currentCommand--;
    }

}
