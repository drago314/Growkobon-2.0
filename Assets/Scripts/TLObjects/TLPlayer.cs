using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TLPlayer : TLMoveableObject
{
    private List<TLPlayer> stateList;

    private Vector2Int directionFacing;
    private TLHoldableObject objectHeld = null;

    public Action<InteractAction> OnObjectPickedUp;
    public Action<InteractAction> OnObjectPutDown;
    public Action<MoveAction> OnPlayerMove;
    public Action<SpinAction> OnPlayerSpin;
    public Action<MoveAction, InteractAction> OnUndoOrReset;

    public TLPlayer(Vector2Int curPos) : base(curPos)
    {
        directionFacing = Vector2Int.right;
        stateList = new List<TLPlayer>();
        stateList.Add(new TLPlayer(this));
    }
    
    private TLPlayer(TLPlayer obj) : base(obj)
    {
        Initialize(obj);
    }

    private void Initialize(TLPlayer obj)
    {
        curPos = obj.curPos;
        directionFacing = obj.directionFacing;
        objectHeld = obj.objectHeld;
        OnUndoOrReset?.Invoke(new MoveAction(curPos, curPos, directionFacing, this, GameManager.Inst.currentState), 
            new InteractAction(directionFacing, this, objectHeld, GameManager.Inst.currentState));
    }

    public override string GetName() {
        string name = "Player";
        if (objectHeld != null)
        {
            name += " is holding ";
            name += objectHeld.GetName();
        }
        else
        {
            name += " not holding";
        }
        return name; 
    }

    public bool IsObjectHeld() { return objectHeld != null; }
    public TLHoldableObject GetObjectHeld() { return objectHeld; }

    public void PickupObject(TLHoldableObject objectHeld)
    {
        Vector2Int grabDirection = objectHeld.GetPosition() - GetPosition();
        SetDirectionFacing(grabDirection);
        this.objectHeld = objectHeld;
        objectHeld.SetHeld(true);
        OnObjectPickedUp?.Invoke(new InteractAction(grabDirection, this, objectHeld, GameManager.Inst.currentState));
    }

    public void ReleaseObject()
    {
        if (objectHeld == null)
            return;

        Vector2Int releaseDirection = objectHeld.GetPosition() - GetPosition();
        objectHeld.SetHeld(false);
        OnObjectPutDown?.Invoke(new InteractAction(releaseDirection, this, objectHeld, GameManager.Inst.currentState));
        objectHeld = null;
    }

    public Vector2Int GetDirectionFacing() { return directionFacing; }

    public void SetDirectionFacing(Vector2Int dir)
    {
        directionFacing = dir;
        OnPlayerMove?.Invoke(new MoveAction(curPos, curPos, directionFacing, this, GameManager.Inst.currentState));
    }

    public override void SetPos(Vector2Int pos)
    {
        OnPlayerMove?.Invoke(new MoveAction(curPos, pos, pos - curPos, this, GameManager.Inst.currentState));
        curPos = pos;
    }

    public override bool CanMove(TLObject pusher, Vector2Int moveDir)
    {
        GameState currentState = GameManager.Inst.currentState;

        if (!IsObjectHeld() || objectHeld.GetPosition() == curPos + moveDir)
            return currentState.CanPush(this, moveDir);
        else
            return currentState.CanPush(this, moveDir) && currentState.CanPush(objectHeld, moveDir);
    }

    public override void Move(TLObject pusher, Vector2Int moveDir)
    {
        GameState currentState = GameManager.Inst.currentState;

        if (!IsObjectHeld() || objectHeld.GetPosition() == curPos + moveDir)
            currentState.Push(this, moveDir);
        else
        {
            currentState.Push(this, moveDir);
            currentState.Pull(this, objectHeld.GetPosition(), moveDir);
        }


        OnPlayerMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, this, GameManager.Inst.currentState));
        curPos = curPos + moveDir;
        GameManager.Inst.currentState.Move(this, curPos - moveDir);
    }

    public bool SpinHeldObject(bool clockwise, Vector2Int startDir, Vector2Int endDir)
    {
        if (!IsObjectHeld())
        {
            Debug.LogError("Spin Held Object Called when No Object is Held");
            return false;
        }

        Vector2Int originalPos = objectHeld.GetPosition();
        Debug.Log("original pos: " + originalPos);

        bool somethingChanged = objectHeld.SpinMove(this, clockwise, startDir, endDir);

        Debug.Log("new pos: " + objectHeld.GetPosition());
        if (objectHeld.GetPosition() != originalPos)
        {
            OnPlayerSpin?.Invoke(new SpinAction(curPos, curPos, startDir, endDir, clockwise, this, GameManager.Inst.currentState));
            directionFacing = endDir;
        }

        return somethingChanged;
    }

    public override void EndMove(bool changeHappened)
    {
        if (changeHappened)
            stateList.Add(new TLPlayer(this));
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
            stateList = new List<TLPlayer>();
            stateList.Add(new TLPlayer(this));
            GameManager.Inst.currentState.AddObject(this);
        }
        else
        {
            Destroy();
        }
    }

    private void PrintStateList()
    {
        string result = "Current Player StateList:\n";
        foreach (var state in stateList)
        {
            result += state.curPos + "\n";
        }
        Debug.Log(result);
    }
}
