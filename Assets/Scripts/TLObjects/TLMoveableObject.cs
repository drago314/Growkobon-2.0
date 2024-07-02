using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TLMoveableObject : TLObject
{
    public TLMoveableObject(Vector2Int pos) : base(pos)
    {
        SetPos(pos);
    }

    public TLMoveableObject(TLMoveableObject obj) : base(obj)
    {
        SetPos(obj.GetPosition());
    }

    public abstract void SetPos(Vector2Int pos);
    public abstract void Move(TLObject pusher, Vector2Int moveDir);
    public abstract void EndMove(bool changeHappened);
    public abstract void Undo();
    public abstract void Reset();
}
