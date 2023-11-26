using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLDoor : TLObject
{
    public int potsRequired = 0;
    public bool usesMultiPot = false;
    public string exitName = "Main Exit";

    public TLDoor(Vector2Int curPos, DoorSignature doorSig) : base(curPos)
    {
        potsRequired = doorSig.potsRequired;
        usesMultiPot = doorSig.usesMultiPots;
        exitName = doorSig.exitName;
    }

    public TLDoor(TLDoor obj) : base(obj)
    {
        potsRequired = obj.potsRequired;
        usesMultiPot = obj.usesMultiPot;
        exitName = obj.exitName;
    }

    public bool IsOpen()
    {
        var pots = GameManager.Inst.movementManager.currentState.GetAllTLPots();
        if (!usesMultiPot)
        {
            foreach (var pot in pots)
            {
                if (pot.IsFull() == 0)
                    return false;
            }
            return true;
        }
        else
        {
            int potTotal = 0;
            foreach (var pot in pots)
            {
                potTotal += pot.IsFull();
            }
            return potTotal == potsRequired;
        }
    }

    public override string GetName() { return exitName; }
}