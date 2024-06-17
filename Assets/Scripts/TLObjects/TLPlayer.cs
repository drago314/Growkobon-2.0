using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLPlayer : TLObject
{
    public Vector2Int directionFacing;
    public bool objectHeld = false;

    public TLPlayer(Vector2Int curPos) : base(curPos)
    {
        directionFacing = Vector2Int.right;
    }

    public TLPlayer(TLPlayer obj) : base(obj)
    {
        directionFacing = obj.directionFacing;
        objectHeld = obj.objectHeld;
    }

    public override string GetName() {
        string name = "Player";
        if (objectHeld)
        {
            name += "is holding";
        }
        else
        {
            name += " not holding";
        }
        return name; 
    }
}
