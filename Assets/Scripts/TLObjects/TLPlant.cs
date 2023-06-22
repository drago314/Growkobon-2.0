using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLPlant : TLObject
{
    public TLPlant(Vector2Int curPos) : base(curPos)
    {
    }

    public TLPlant(TLPlant obj) : base(obj)
    {
    }

    public override string GetName() { return "Plant"; }
}
