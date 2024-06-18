using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLPot : TLObject
{
    private int potNumber = 1;

    public TLPot(Vector2Int curPos, int potNum) : base(curPos)
    {
        potNumber = potNum;
    }

    public int GetPotNumber() { return potNumber; }

    public int IsFull()
    {
        if (GameManager.Inst.currentState.GetTLOfTypeAtPos<TLPlant>(curPos) != null)
            return potNumber;
        else
            return 0;
    }

    public override void EndMove() { return; }
    public override void Undo() { return; }
    public override void Reset() { return; }
    public override string GetName() { return "Pot"; }
}
