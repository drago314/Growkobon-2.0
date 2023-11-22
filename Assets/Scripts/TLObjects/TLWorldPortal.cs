using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLWorldPortal : TLObject
{ 
    public string worldToTravelTo;

    public TLWorldPortal(Vector2Int pos, WorldSignature sig) : base(pos)
    {
        worldToTravelTo = sig.worldToTravelTo;
    }

    public TLWorldPortal(TLWorldPortal obj) : base(obj)
    {
        worldToTravelTo = obj.worldToTravelTo;
    }

    public override string GetName() { return worldToTravelTo; }
}
