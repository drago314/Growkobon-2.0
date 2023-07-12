using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLWorldExit : TLObject
{ 
    public string worldName;

    public TLWorldExit(Vector2Int pos, WorldSignature sig) : base(pos)
    {
        worldName = sig.worldName;
    }

    public TLWorldExit(TLWorldExit obj) : base(obj)
    {
        worldName = obj.worldName;
    }

    public override string GetName() { return worldName; }
}
