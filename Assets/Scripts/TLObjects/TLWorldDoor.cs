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
        var levelsCompleted = GameManager.Inst.levelsCompleted;
        return levelsCompleted.Count >= levelsRequired;
    }
}