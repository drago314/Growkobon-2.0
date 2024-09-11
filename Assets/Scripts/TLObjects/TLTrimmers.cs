using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TLTrimmers : TLHoldableObject
{
    private List<TLTrimmers> stateList;
    private TLTrimmers initialState;
    private bool deleted = false;
    public Action OnDoneWithObject;

    public event Action<MoveAction> OnTrimmersMove;
    public event Action<SpinAction> OnTrimmersSpin;

    public TLTrimmers(Vector2Int curPos, TrimmerSignature sig) : base(curPos, sig.directionFacing)
    {
        initialState = new TLTrimmers(this);
        stateList = new List<TLTrimmers>();
        stateList.Add(initialState);
    }

    private TLTrimmers(TLTrimmers obj) : base(obj)
    {
        Initialize(obj);
    }

    private void Initialize(TLTrimmers obj)
    {
        base.Initialize(obj);
        deleted = obj.deleted;
    }

    public bool IsDeleted() { return deleted; }

    public Vector2Int GetDirectionFacingAdjacent()
    {
        List<Vector2Int> clockwiseList = new List<Vector2Int>();
        clockwiseList.Add(Vector2Int.up);
        clockwiseList.Add(Vector2Int.right);
        clockwiseList.Add(Vector2Int.down);
        clockwiseList.Add(Vector2Int.left);

        int index = clockwiseList.IndexOf(directionFacing);
        if (index == clockwiseList.Count - 1)
            return clockwiseList[0];
        else
            return clockwiseList[index + 1];
    }

    public override void SetPos(Vector2Int pos)
    {
        OnTrimmersMove?.Invoke(new MoveAction(curPos, pos, pos - curPos, this, GameManager.Inst.currentState));
        curPos = pos;
    }

    public override bool CanMove(TLObject pusher, Vector2Int moveDir)
    {
        GameState currentState = GameManager.Inst.currentState;

        if (pusher is TLPlayer && pusher.GetPosition() == GetPosition() + GetDirectionFacing() && GetDirectionFacing() == -1 * moveDir)
            return false;
        if (pusher is TLPlayer && pusher.GetPosition() == GetPosition() + GetDirectionFacingAdjacent() && GetDirectionFacingAdjacent() == -1 * moveDir)
            return false;

        return currentState.CanPush(this, moveDir);
    }

    public override void Move(TLObject pusher, Vector2Int moveDir)
    {
        GameState currentState = GameManager.Inst.currentState;
        currentState.Push(this, moveDir);
        OnTrimmersMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, this, currentState));
        curPos = curPos + moveDir;
        currentState.Move(this, curPos - moveDir);
    }


    public override bool SpinMove(TLObject spinner, bool clockwise, Vector2Int startDir, Vector2Int endDir)
    {
        GameState currentState = GameManager.Inst.currentState;
        Vector2Int startingPos = curPos;
        Vector2Int cornerSpot = curPos + endDir;
        Vector2Int goalPos = curPos + endDir - startDir;

        // CORNER
        bool canMoveCorner = CanMove(spinner, endDir);
        Debug.Log("Can Move Corner: " + canMoveCorner);
        bool somethingChanged = currentState.IsTLOfTypeAtPos<TLMoveableObject>(cornerSpot);
        Move(spinner, endDir);
        Debug.Log("After Corner: " + currentState.ToString());

        // FINAL POSITION
        Rotate90Degrees(clockwise);

        bool canMoveFinal = CanMove(spinner, startDir * -1);
        Debug.Log("Can Move Final: " + canMoveFinal);

        if (!canMoveFinal)
        {
            Move(spinner, endDir * -1);
            Rotate90Degrees(!clockwise);
            return somethingChanged;
        }

        Move(spinner, startDir * -1);
        OnTrimmersSpin?.Invoke(new SpinAction(startingPos, goalPos, startDir, endDir, clockwise, this, currentState));
        return true;
    }

    public override void EndMove(bool changeHappened)
    {
        if (changeHappened)
            stateList.Add(new TLTrimmers(this));
    }

    public override void Undo()
    {
        if (GameManager.Inst.currentState.AtInitialState())
            return;

        if (stateList.Count >= 2 && !deleted)
        {
            GameManager.Inst.currentState.RemoveObject(this);
            Initialize(stateList[stateList.Count - 2]);
            stateList.RemoveAt(stateList.Count - 1);
            GameManager.Inst.currentState.AddObject(this);
        }
        else if (stateList.Count >= 2 && deleted)
        {
            if (stateList[stateList.Count - 2].IsDeleted())
            {
                Initialize(stateList[stateList.Count - 2]);
                stateList.RemoveAt(stateList.Count - 1);
            }
            else
            {
                Initialize(stateList[stateList.Count - 2]);
                stateList.RemoveAt(stateList.Count - 1);
                GameManager.Inst.currentState.ReviveObject(this);
            }
        }
        else
        {
            GameManager.Inst.currentState.RemoveObject(this);
            OnDoneWithObject?.Invoke();
        }
    }

    public override void Reset()
    {
        if (initialState != null)
        {
            GameManager.Inst.currentState.RemoveObject(this);
            Initialize(initialState);
            stateList.Add(new TLTrimmers(this));
            GameManager.Inst.currentState.AddObject(this);
        }
        else if (deleted)
        {
            stateList.Add(new TLTrimmers(this));
        }
        else
        {
            GameManager.Inst.currentState.DeleteObject(this);
            deleted = true;
            stateList.Add(new TLTrimmers(this));
        }
    }

    public override string GetName()
    {
        string result = "Trimmers facing " + GetDirectionFacing() + " + " + GetDirectionFacingAdjacent();
        return result;
    }

    public override bool Equals(TLObject obj)
    {
        return base.Equals(obj) && obj is TLTrimmers && IsHeld() == ((TLTrimmers)obj).IsHeld() && GetDirectionFacing() == ((TLTrimmers)obj).GetDirectionFacing();
    }

    public override TLObject Copy()
    {
        return new TLTrimmers(this);
    }
}
