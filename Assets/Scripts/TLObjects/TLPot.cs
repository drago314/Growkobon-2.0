using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLPot : TLObject
{
    public TLPot(Vector2Int curPos) : base(curPos)
    {
    }

    public TLPot(TLPot obj) : base(obj)
    {
    }

    public int IsFull()
    {
        if (GameManager.Inst.currentState.GetPlantAtPos(curPos) != null)
            return 1;
        else
            return 0;
    }

    public override string GetName() { return "Pot"; }
}
