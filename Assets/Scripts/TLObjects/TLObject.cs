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

    public virtual bool Equals(TLObject obj)
    {
        return obj.GetName().Equals(this.GetName()) && obj.curPos.Equals(this.curPos);
    }
}
