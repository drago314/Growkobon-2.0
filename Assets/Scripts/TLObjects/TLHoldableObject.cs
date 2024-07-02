using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class TLHoldableObject : TLMoveableObject
{
    protected Vector2Int directionFacing;
    protected bool held = false;

    public event Action OnPickedUp;
    public event Action OnRelease;

    public TLHoldableObject(Vector2Int curPos, Vector2Int directionFacing) : base(curPos)
    {
        this.directionFacing = directionFacing;
    }

    protected TLHoldableObject(TLHoldableObject obj) : base(obj)
    {
        Initialize(obj);
    }

    protected void Initialize(TLHoldableObject obj)
    {
        directionFacing = obj.directionFacing;
        curPos = obj.curPos;
        held = obj.held;
    }

    public Vector2Int GetDirectionFacing() { return directionFacing; }

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

    public abstract bool SpinMove(TLObject spinner, bool clockwise, Vector2Int startDir, Vector2Int endDir);

    public void Rotate90Degrees(bool clockwise)
    {
        List<Vector2Int> clockwiseList = new List<Vector2Int>();
        clockwiseList.Add(Vector2Int.up);
        clockwiseList.Add(Vector2Int.right);
        clockwiseList.Add(Vector2Int.down);
        clockwiseList.Add(Vector2Int.left);

        if (clockwise)
        {
            int index = clockwiseList.IndexOf(directionFacing);
            if (index == clockwiseList.Count - 1)
                directionFacing = clockwiseList[0];
            else
                directionFacing = clockwiseList[index + 1];

        }
        else
        {
            int index = clockwiseList.IndexOf(directionFacing);
            if (index == 0)
                directionFacing = clockwiseList[clockwiseList.Count - 1];
            else
                directionFacing = clockwiseList[index - 1];
        }
    }
}
