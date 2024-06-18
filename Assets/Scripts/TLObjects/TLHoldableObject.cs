using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class TLHoldableObject : TLMoveableObject
{
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

    protected void Initialize(TLHoldableObject obj)
    {
        directionFacing = obj.directionFacing;
        curPos = obj.curPos;
        held = obj.held;
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
        if ((curPos - pos).x == 0 || (curPos - pos).y == 0) // Straight move
        {
            OnMove?.Invoke(new MoveAction(curPos, pos, pos - curPos, this, GameManager.Inst.currentState));
        }
        else // Spin Move
        {
            Vector2Int moveDiff = pos - curPos;
            Vector2Int playerPos = GameManager.Inst.currentState.GetPlayer().GetPosition();
            Vector2Int fromStartToPlayerDiff = curPos - playerPos;
            Debug.Log("Player: " + playerPos);
            Debug.Log("Shears: " + curPos);
            Debug.Log("Diff: " + fromStartToPlayerDiff);
            Vector2Int startDir;
            bool clockwise;

            if (moveDiff == new Vector2Int(1, 1))
            {
                if (fromStartToPlayerDiff == Vector2Int.left)
                {
                    startDir = new Vector2Int(-1, 0);
                    clockwise = true;
                }
                else
                {
                    startDir = new Vector2Int(0, -1);
                    clockwise = false;
                }
            }
            else if (moveDiff == new Vector2Int(-1, 1))
            {
                if (fromStartToPlayerDiff == Vector2Int.down)
                {
                    startDir = new Vector2Int(0, -1);
                    clockwise = true;
                }
                else
                {
                    startDir = new Vector2Int(1, 0);
                    clockwise = false;
                }
            }
            else if (moveDiff == new Vector2Int(-1, -1))
            {
                if (fromStartToPlayerDiff == Vector2Int.right)
                {
                    startDir = new Vector2Int(1, 0);
                    clockwise = true;
                }
                else
                {
                    startDir = new Vector2Int(0, 1);
                    clockwise = false;
                }
            }
            else
            {
                if (fromStartToPlayerDiff == Vector2Int.up)
                {
                    startDir = new Vector2Int(0, 1);
                    clockwise = true;
                }
                else
                {
                    startDir = new Vector2Int(-1, 0);
                    clockwise = false;
                }
            }

            Vector2Int endDir = startDir + moveDiff;

            Rotate90Degrees(clockwise);
            OnSpin?.Invoke(new SpinAction(curPos, pos, startDir, endDir, clockwise, this, GameManager.Inst.currentState));
        }
        curPos = pos;
    }

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
