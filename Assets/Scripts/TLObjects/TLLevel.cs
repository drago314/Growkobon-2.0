using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TLLevel : TLObject
{
    private string levelName;
    private bool isCompleted;

    public Action OnCompletion;

    public TLLevel(Vector2Int pos, LevelSignature sig, bool isCompleted) : base(pos)
    {
        levelName = sig.levelName;
    }
    
    public string GetLevelName() { return levelName; }

    public bool IsCompleted() { return isCompleted; }
    public void SetCompleted(bool completed)
    {
        if (completed && !isCompleted)
        {
            OnCompletion?.Invoke();
            isCompleted = completed;
        }
    }

    public override void EndMove() { return; }
    public override void Undo() { return; }
    public override void Reset() { return; }

    public override string GetName() { return levelName; }
}
