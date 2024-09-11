using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLWall : TLObject
{
    public TLWall(Vector2Int curPos) : base(curPos)
    {
    }

    public TLWall(TLWall obj) : base(obj)
    {
    }


    public override bool CanMove(TLObject pusher, Vector2Int moveDir)
    {
        return false;
    }

    public override string GetName() { return "Wall"; }
    public override bool Equals(TLObject obj)
    {
        return base.Equals(obj) && obj is TLWall;
    }
    public override TLObject Copy()
    {
        return new TLWall(this);
    }
}
