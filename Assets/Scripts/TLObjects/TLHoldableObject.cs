using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TLHoldableObject : TLMoveableObject
{
    private List<TLHoldableObject> stateList;

    private Vector2Int directionFacing;
    private bool held = false;

    public event Action OnPickedUp;
    public event Action OnRelease;
    public event Action<MoveAction> OnMove;
    public event Action<SpinAction> OnSpin;


    public TLHoldableObject(Vector2Int curPos, Vector2Int directionFacing) : base(curPos)
    {
        this.directionFacing = directionFacing;
    }

    protected TLHoldableObject(TLHoldableObject obj) : base(obj)
    {
        Initialize(obj);
    }

    private void Initialize(TLHoldableObject obj)
    {
        SetDirectionFacing(obj.directionFacing);
        Move(obj.GetPosition());
        SetHeld(obj.held);
    }

    public Vector2Int GetDirectionFacing() { return directionFacing; }
    public void SetDirectionFacing(Vector2Int dir)
    {
        directionFacing = dir;
        OnMove?.Invoke(new MoveAction(curPos, curPos, dir, this, GameManager.Inst.currentState));
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

    public override void Move(Vector2Int pos)
    {
        if ((curPos - pos).x == 0 || (curPos - pos).y == 0)
        {
            OnMove?.Invoke(new MoveAction(curPos, pos, pos - curPos, this, GameManager.Inst.currentState));
            curPos = pos;
        }
        else
        {
            Vector2Int moveDiff = pos - curPos;
            Vector2Int startDir;
            Vector2Int endDir;
            if (moveDiff == new Vector2Int(1, 1))
            {
                startDir = new Vector2Int(0, -1);
                endDir = new Vector2Int(1, 0);
            }
            else if (moveDiff == new Vector2Int(-1, 1))
            {
                startDir = new Vector2Int(1, 0);
                endDir = new Vector2Int(0, 1);
            }
            else if (moveDiff == new Vector2Int(-1, -1))
            {
                startDir = new Vector2Int(0, 1);
                endDir = new Vector2Int(-1, 0);
            }
            else
            {
                startDir = new Vector2Int(-1, 0);
                endDir = new Vector2Int(0, -1);
            }

            OnSpin?.Invoke(new SpinAction(curPos, pos, startDir, endDir, this, GameManager.Inst.currentState));
        }
    }

    public override void EndMove() 
    {
        stateList.Add(new TLHoldableObject(this));
    }

    public override void Undo()
    {
        if (GameManager.Inst.currentState.AtInitialState())
            return;

        GameManager.Inst.currentState.RemoveObject(this);

        if (stateList.Count >= 2)
        {
            Initialize(stateList[stateList.Count - 2]);
            stateList.RemoveAt(stateList.Count - 1);
            GameManager.Inst.currentState.AddObject(this);
        }
        else
            Destroy();
    }

    public override void Reset()
    {
        GameManager.Inst.currentState.RemoveObject(this);

        if (stateList.Count > GameManager.Inst.currentState.GetMoveCount())
        {
            Initialize(stateList[0]);
            stateList = new List<TLHoldableObject>();
            stateList.Add(new TLHoldableObject(this));
            GameManager.Inst.currentState.AddObject(this);
        }
        else
        {
            Destroy();
        }
    }

    public override string GetName() { return "Holdable Object"; }
}
