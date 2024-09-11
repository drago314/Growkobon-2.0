using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class TLObject
{
    protected Vector2Int curPos;

    public TLObject(Vector2Int pos)
    {
        curPos = pos;
    }

    protected TLObject(TLObject obj)
    {
        curPos = obj.curPos;
    }

    public Vector2Int GetPosition() { return curPos; }

    public virtual bool Equals(TLObject obj)
    {
        return obj.curPos.Equals(this.curPos);
    }

    public abstract string GetName();
    public abstract bool CanMove(TLObject pusher, Vector2Int moveDir);
    public abstract TLObject Copy();
}
