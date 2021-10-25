using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;





public class MoveController : MonoBehaviour
{
    public static MoveController Instance;
    public List<Command> commands;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        commands = new List<Command>();
    }

    public int currentCommand = -1;

    public void AddCommand(Command newCommand){
        newCommand.Execute();
        if (currentCommand != commands.Count - 1){
            currentCommand++;
            commands[currentCommand] = newCommand;
        }
        else{
            commands.Add(newCommand);
            currentCommand++;
        }
    }

    public void UndoCommand(){
        if(currentCommand < 0) return; 
        commands[currentCommand].Undo();
        currentCommand--;
    }

}
