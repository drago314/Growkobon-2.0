using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLDoor : TLObject
{
    public TLDoor(Vector2Int curPos) : base(curPos)
    {
    }

    public TLDoor(TLDoor obj) : base(obj)
    {
    }

    public bool IsOpen()
    {
        var pots = GameManager.Inst.movementManager.currentState.GetAllTLPots();
        foreach (var pot in pots)
        {
            if (pot.IsFull() != 1)
                return false;
        }
        return true;
    }

    public override string GetName() { return "Door"; }
}