using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLWorldDoor : TLObject
{
    public int levelsRequired = 0;

    public TLWorldDoor(Vector2Int curPos, WorldDoorSignature doorSig) : base(curPos)
    {
        levelsRequired = doorSig.levelsRequired;
    }

    public TLWorldDoor(TLWorldDoor obj) : base(obj)
    {
        levelsRequired = obj.levelsRequired;
    }

    public bool IsOpen()
    {
        var levelsCompleted = GameManager.Inst.levelsCompleted.Count;
        Debug.Log("Is Open: " + levelsRequired.ToString() + ", " + levelsCompleted.ToString());
        return levelsCompleted >= levelsRequired;
    }

    public bool JustOpened()
    {
        var levelsCompleted = GameManager.Inst.levelsCompleted.Count;
        Debug.Log("Just Opened " + levelsCompleted.ToString());
        return levelsCompleted == levelsRequired;
    }
}