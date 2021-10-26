
using System;
using UnityEngine;

public abstract class Command{
    public abstract void Execute();
    public abstract void Undo();

    public abstract String myToString();
}