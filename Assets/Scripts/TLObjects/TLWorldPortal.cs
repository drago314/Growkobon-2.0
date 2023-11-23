using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLWorldPortal : TLObject
{ 
    public string worldToTravelTo;
    public Vector2Int posToTravelTo;
    public List<Vector2Int> pathsBeginningUnlocked;

    public TLWorldPortal(Vector2Int pos, WorldSignature sig) : base(pos)
    {
        worldToTravelTo = sig.worldToTravelTo;
        posToTravelTo = sig.posToTravelTo;
        pathsBeginningUnlocked = sig.pathsBeginningUnlocked;
    }

    public TLWorldPortal(TLWorldPortal obj) : base(obj)
    {
        worldToTravelTo = obj.worldToTravelTo;
        posToTravelTo = obj.posToTravelTo;
        pathsBeginningUnlocked = obj.pathsBeginningUnlocked;
    }

    public override string GetName() { return worldToTravelTo; }
}
