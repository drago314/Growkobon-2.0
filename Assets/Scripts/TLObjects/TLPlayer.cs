using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLPlayer : TLObject
{
    public TLPlayer(Vector2Int curPos) : base(curPos)
    {
    }

    public override string GetName() { return "Player"; }
}
