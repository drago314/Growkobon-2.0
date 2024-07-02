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

    public string GetWorldToTravelTo() { return worldToTravelTo; }
    public Vector2Int GetPosToTravelTo() { return posToTravelTo; }

    public override bool CanMove(TLObject pusher, Vector2Int moveDir)
    {
        return true;
    }

    public override string GetName() { return worldToTravelTo; }
}
