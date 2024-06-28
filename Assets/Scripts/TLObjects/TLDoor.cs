using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TLDoor : TLObject
{
    private int potsRequired = 0;
    private bool usesMultiPot = false;

    public TLDoor(Vector2Int curPos, DoorSignature doorSig) : base(curPos)
    {
        potsRequired = doorSig.potsRequired;
        usesMultiPot = doorSig.usesMultiPots;
    }

    public bool IsOpen()
    {
        var pots = GameManager.Inst.currentState.GetAllOfTLType<TLPot>();
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

    public override bool CanMove(TLObject pusher, Vector2Int moveDir)
    {
        if (pusher is TLPlayer && IsOpen())
            return true;

        return false;
    }

    public bool UsesMultiPot() { return usesMultiPot; }
    public int GetPotsRequired() { return potsRequired; }

    public override void Undo() { return; }
    public override void Reset() { return; }
    public override void EndMove() { return; }

    public override string GetName() { return "Door";  }
}