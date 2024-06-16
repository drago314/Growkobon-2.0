using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLPlant : TLObject
{
    public bool isDead;

    public TLPlant(Vector2Int curPos, PlantSignature sig) : base(curPos)
    {
        isDead = sig.isDead;
    }

    public TLPlant(Vector2Int curPos, bool isDead) : base(curPos)
    {
        this.isDead = isDead;
    }

    public TLPlant(TLPlant obj) : base(obj)
    {
        isDead = obj.isDead;
    }

    public override string GetName() { return "Plant"; }
}
