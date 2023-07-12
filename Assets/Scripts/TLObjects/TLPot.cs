using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLPot : TLObject
{
    public int potNumber = 1;

    public TLPot(Vector2Int curPos, int potNum) : base(curPos)
    {
        potNumber = potNum;
    }

    public TLPot(TLPot obj) : base(obj)
    {
        potNumber = obj.potNumber;
    }

    public int IsFull()
    {
        if (GameManager.Inst.movementManager.currentState.GetPlantAtPos(curPos) != null)
            return potNumber;
        else
            return 0;
    }

    public override string GetName() { return "Pot"; }
}
