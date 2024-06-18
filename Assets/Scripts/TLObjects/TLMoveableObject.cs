using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TLMoveableObject : TLObject
{
    public TLMoveableObject(Vector2Int pos) : base(pos)
    {
        Move(pos);
    }

    public TLMoveableObject(TLMoveableObject obj) : base(obj)
    {
        Move(obj.GetPosition());
    }

    public abstract void Move(Vector2Int pos);
}
