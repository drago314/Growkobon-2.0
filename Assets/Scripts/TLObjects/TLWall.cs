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

    public override void EndMove() { return; }
    public override void Undo() { return; }
    public override void Reset() { return; }

    public override string GetName() { return "Wall"; }
}
