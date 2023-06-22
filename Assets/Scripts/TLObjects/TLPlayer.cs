using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLPlayer : TLObject
{
    public TLPlayer(Vector2Int curPos) : base(curPos)
    {
    }

    public TLPlayer(TLPlayer obj) : base(obj)
    {
    }

    public override string GetName() { return "Player"; }
}
