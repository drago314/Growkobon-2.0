using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLShears : TLHoldableObject
{
    public TLShears(Vector2Int curPos, ShearSignature sig) : base(curPos, sig.directionFacing)
    {
    }

    public TLShears(TLShears obj) : base(obj)
    {
    }

    public override string GetName() { return "Shears"; }
}
