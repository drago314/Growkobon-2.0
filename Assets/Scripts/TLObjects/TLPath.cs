using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLPath : TLObject
{
    public bool unlocked;

    public TLPath(Vector2Int curPos, bool pathUnlocked) : base(curPos)
    {
        unlocked = pathUnlocked;
    }

    public TLPath(TLPath obj) : base(obj)
    {
        unlocked = obj.unlocked;
    }

    public override string GetName() { return "Path"; }
}
