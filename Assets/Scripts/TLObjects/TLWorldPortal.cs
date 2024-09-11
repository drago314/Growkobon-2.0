using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLWorldPortal : TLObject
{ 
    private string worldToTravelTo;
    private Vector2Int posToTravelTo;

    public TLWorldPortal(Vector2Int pos, WorldSignature sig) : base(pos)
    {
        worldToTravelTo = sig.worldToTravelTo;
        posToTravelTo = sig.posToTravelTo;
    }
    public TLWorldPortal(TLWorldPortal portal) : base(portal)
    {
        worldToTravelTo = portal.worldToTravelTo;
        posToTravelTo = portal.posToTravelTo;
    }

    public string GetWorldToTravelTo() { return worldToTravelTo; }
    public Vector2Int GetPosToTravelTo() { return posToTravelTo; }

    public override bool CanMove(TLObject pusher, Vector2Int moveDir)
    {
        return true;
    }

    public override string GetName() { return worldToTravelTo; }

    public override bool Equals(TLObject obj)
    {
        return base.Equals(obj) && obj is TLWorldPortal && GetWorldToTravelTo() == ((TLWorldPortal)obj).GetWorldToTravelTo() && GetPosToTravelTo() == ((TLWorldPortal)obj).GetPosToTravelTo();
    }
    public override TLObject Copy()
    {
        return new TLWorldPortal(this);
    }
}
