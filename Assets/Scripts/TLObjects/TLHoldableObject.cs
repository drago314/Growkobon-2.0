using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TLHoldableObject : TLObject
{
    public Vector2Int directionFacing;
    public event Action OnPickedUp;
    public event Action OnRelease;
    private bool held = false;


    public TLHoldableObject(Vector2Int curPos, Vector2Int directionFacing) : base(curPos)
    {
        this.directionFacing = directionFacing;
    }

    public TLHoldableObject(TLShears obj) : base(obj)
    {
        directionFacing = obj.directionFacing;
        held = obj.held;
    }

    public bool IsHeld() { return held; }
    
    public void SetHeld(bool held)
    {
        if (held && !this.held)
        {
            this.held = true;
            OnPickedUp?.Invoke();
        }
        else if (!held && this.held)
        {
            this.held = false;
            OnRelease?.Invoke();
        }
    }

    public override string GetName() { return "Holdable Object"; }
}
