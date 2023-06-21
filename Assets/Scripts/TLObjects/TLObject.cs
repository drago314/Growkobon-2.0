using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLObject
{
    public Vector2Int curPos;

    public TLObject(Vector2Int pos)
    {
        curPos = pos;
    }

    public virtual string GetName() { return "Generic"; }
}
