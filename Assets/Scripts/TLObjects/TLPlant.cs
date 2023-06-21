using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLPlant : TLObject
{
    public TLPlant(Vector2Int curPos) : base(curPos)
    {
    }

    public override string GetName() { return "Plant"; }
}
