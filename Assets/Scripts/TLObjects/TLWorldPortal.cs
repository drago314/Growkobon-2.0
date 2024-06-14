using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLWorldPortal : TLObject
{ 
    public string worldToTravelTo;
    public Vector2Int posToTravelTo;

    public TLWorldPortal(Vector2Int pos, WorldSignature sig) : base(pos)
    {
        worldToTravelTo = sig.worldToTravelTo;
        posToTravelTo = sig.posToTravelTo;
    }

    public TLWorldPortal(TLWorldPortal obj) : base(obj)
    {
        worldToTravelTo = obj.worldToTravelTo;
        posToTravelTo = obj.posToTravelTo;
    }

    public override string GetName() { return worldToTravelTo; }
}
