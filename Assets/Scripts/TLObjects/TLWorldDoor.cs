using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLWorldDoor : TLObject
{
    private int levelsRequired = 0;

    public TLWorldDoor(Vector2Int curPos, WorldDoorSignature doorSig) : base(curPos)
    {
        levelsRequired = doorSig.levelsRequired;
    }
    public TLWorldDoor(TLWorldDoor door) : base(door.GetPosition())
    {
        levelsRequired = door.levelsRequired;
    }

    public int GetLevelsRequired() { return levelsRequired; }

    public bool IsOpen()
    {
        var levelsCompleted = GameManager.Inst.levelsCompleted.Count;
        return levelsCompleted >= levelsRequired;
    }

    public bool JustOpened()
    {
        var levelsCompleted = GameManager.Inst.levelsCompleted.Count;
        return levelsCompleted == levelsRequired;
    }

    public override bool CanMove(TLObject pusher, Vector2Int moveDir)
    {
        if (pusher is TLPlayer && IsOpen())
            return true;

        return false;
    }

    public override string GetName() { return "World Door"; }
    public override bool Equals(TLObject obj)
    {
        return base.Equals(obj) && obj is TLWorldDoor;
    }
    public override TLObject Copy()
    {
        return new TLWorldDoor(this);
    }
}