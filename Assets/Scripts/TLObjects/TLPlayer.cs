using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLPlayer : TLObject
{
    public Vector2Int directionFacing;

    public TLPlayer(Vector2Int curPos) : base(curPos)
    {
        directionFacing = Vector2Int.right;
    }

    public TLPlayer(TLPlayer obj) : base(obj)
    {
        directionFacing = obj.directionFacing;
    }

    public override string GetName() { return "Player"; }
}
