using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TLShears : TLHoldableObject
{
    private List<TLShears> stateList;
    private TLShears initialState;
    private bool deleted = false;
    public Action OnDoneWithObject;

    private TLPlant plantSkewered = null;
    private bool currentlyCornerSpinning = false;

    public event Action<MoveAction> OnShearsMove;
    public event Action<SpinAction> OnShearsSpin;
    public Action<SkewerAction> OnPlantSkewered;
    public Action<SkewerAction> OnPlantUnskewered;

    public TLShears(Vector2Int curPos, ShearSignature sig) : base(curPos, sig.directionFacing)
    {
        initialState = new TLShears(this);
        stateList = new List<TLShears>();
        stateList.Add(initialState);
    }

    private TLShears(TLShears obj) : base(obj)
    {
        Initialize(obj);
    }

    private void Initialize(TLShears obj)
    {
        base.Initialize(obj);
        deleted = obj.deleted;
        plantSkewered = obj.plantSkewered;
    }

    public bool IsDeleted() { return deleted; }
    public bool IsCurrentlyCornerSpinning() { return currentlyCornerSpinning; }
    public bool IsPlantSkewered() { return plantSkewered != null; }
    public TLPlant GetPlantSkewered() { return plantSkewered; }

    public void SkewerPlant(TLPlant plant)
    {
        Debug.Log("Skewer Plant Called");
        plantSkewered = plant;
        Vector2Int skewerDirection = plantSkewered.GetPosition() - GetPosition();
        plant.SetSkewered(true);
        OnPlantSkewered?.Invoke(new SkewerAction(skewerDirection, GameManager.Inst.currentState.GetPlayer(), this, plantSkewered, GameManager.Inst.currentState));
    }

    public void UnskewerPlant()
    {
        if (plantSkewered == null)
            return;

        Vector2Int skewerDirection = plantSkewered.GetPosition() - GetPosition();
        plantSkewered.SetSkewered(false);
        OnPlantUnskewered?.Invoke(new SkewerAction(skewerDirection, GameManager.Inst.currentState.GetPlayer(), this, plantSkewered, GameManager.Inst.currentState));
        plantSkewered = null;
    }

    public override void SetPos(Vector2Int pos)
    {
        OnShearsMove?.Invoke(new MoveAction(curPos, pos, pos - curPos, this, GameManager.Inst.currentState));
        curPos = pos;
    }

    public override bool CanMove(TLObject pusher, Vector2Int moveDir)
    {
        return CanMove(pusher, moveDir, false, false);
    }

    public bool CanMove(TLObject pusher, Vector2Int moveDir, bool cornerSpinning, bool finalSpinning)
    {
        GameState currentState = GameManager.Inst.currentState;

        if (pusher is TLTrimmers && ((TLTrimmers)pusher).IsHeld())
            return true;
        if (pusher is TLPlayer && pusher.GetPosition() == GetPosition() + GetDirectionFacing() && GetDirectionFacing() == -1 * moveDir && !IsPlantSkewered() && !cornerSpinning && !finalSpinning)
            return false;
        if (!IsPlantSkewered() && currentState.IsTLOfTypeAtPos<TLPlayer>(curPos + moveDir))
            return true;
        if (!IsPlantSkewered() && currentState.IsTLOfTypeAtPos<TLPlant>(curPos + moveDir) && GetDirectionFacing() == moveDir && !cornerSpinning)
            return true;

        if (IsPlantSkewered() && moveDir != -1 * GetDirectionFacing())
            return plantSkewered.CanMove(this, moveDir);

        if (pusher is TLPlant && GetDirectionFacing() == -1 * moveDir)
            return true; 

        return currentState.CanPush(this, moveDir);
    }

    public override void Move(TLObject pusher, Vector2Int moveDir)
    {
        Move(pusher, moveDir, false);
    }

    public void Move(TLObject pusher, Vector2Int moveDir, bool spinning)
    {
        Debug.Log("Move Shears Called By " + pusher.GetName() + ": " + pusher.GetPosition());

        GameState currentState = GameManager.Inst.currentState;

        if (pusher is TLTrimmers && ((TLTrimmers)pusher).IsHeld())
        {
            return;
        }

        if (pusher is TLPlant && IsPlantSkewered() && pusher == GetPlantSkewered()) // If we are be moved inside a moving plant block
        {
            OnShearsMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, this, GameManager.Inst.currentState));
            curPos = curPos + moveDir;
            currentState.Move(this, curPos - moveDir);
            return;
        }

        if (pusher is TLPlant && IsPlantSkewered() && pusher != GetPlantSkewered())
        {
            Debug.LogError("THIS SHEAR WAS NOT PUSHED CORRECTLY!!");
            return;
        }

        if (pusher is TLPlant && !IsPlantSkewered() && GetDirectionFacing() == -1 * moveDir)
        {
            SkewerPlant((TLPlant)pusher);
            return;
        }

        if (pusher is TLPlant && !IsPlantSkewered())
        {
            currentState.Push(this, moveDir);
            OnShearsMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, this, GameManager.Inst.currentState));
            curPos = curPos + moveDir;
            currentState.Move(this, curPos - moveDir);
            return;
        }

        if (IsPlantSkewered())
        {
            if (pusher is TLPlayer && GetDirectionFacing() == -1 * moveDir && pusher.GetPosition() != GetPosition() - moveDir)
                UnskewerPlant();
            else
                plantSkewered.Move(this, moveDir);

        }

        if (!IsPlantSkewered())
            currentState.Push(this, moveDir);

        if (!spinning)
            OnShearsMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, this, GameManager.Inst.currentState));
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
        currentlyCornerSpinning = true;

        bool canMoveCorner = CanMove(spinner, endDir, true, false);
        Debug.Log("Can Move Corner: " + canMoveCorner);

        if (!canMoveCorner && currentState.IsTLOfTypeAtPos<TLPlant>(cornerSpot) && directionFacing == endDir)
        {
            currentState.GetTLOfTypeAtPos<TLPlant>(cornerSpot).Shear();
            return true;
        }
        else if (!canMoveCorner)
        {
            GameManager.Inst.movementManager.GrowPlant(cornerSpot, endDir);
            return false;
        }

        bool somethingChanged = currentState.IsTLOfTypeAtPos<TLMoveableObject>(cornerSpot);

        bool attemptGrowPlant = currentState.IsTLOfTypeAtPos<TLPlant>(cornerSpot);
        Move(spinner, endDir, true);
        if (attemptGrowPlant)
            GameManager.Inst.movementManager.GrowPlant(cornerSpot + endDir, endDir);

        Debug.Log("After Corner: " + currentState.ToString());

        // FINAL POSITION
        currentlyCornerSpinning = false;
        Rotate90Degrees(clockwise);

        bool canMoveFinal = CanMove(spinner, startDir * -1, false, true);
        Debug.Log("Can Move Final: " + canMoveFinal);

        if (!canMoveFinal)
        {
            GameManager.Inst.movementManager.GrowPlant(goalPos, -1 * startDir);
            Move(spinner, endDir * -1);
            Rotate90Degrees(!clockwise);
            return somethingChanged;
        }

        attemptGrowPlant = currentState.IsTLOfTypeAtPos<TLPlant>(goalPos);
        Move(spinner, startDir * -1, true);
        if (attemptGrowPlant)
            GameManager.Inst.movementManager.GrowPlant(goalPos - startDir, -1 * startDir);
        OnShearsSpin?.Invoke(new SpinAction(startingPos, goalPos, startDir, endDir, clockwise, this, currentState));
        return true;
    }

    public override void EndMove(bool changeHappened)
    {
        currentlyCornerSpinning = false;

        if (changeHappened)
            stateList.Add(new TLShears(this));
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
            stateList.Add(new TLShears(this));
            GameManager.Inst.currentState.AddObject(this);
        }
        else if (deleted)
        {
            stateList.Add(new TLShears(this));
        }
        else
        {
            GameManager.Inst.currentState.DeleteObject(this);
            deleted = true;
            stateList.Add(new TLShears(this));
        }
    }

    public override string GetName()
    {
        string result = "Shears facing " + GetDirectionFacing();
        if (IsPlantSkewered())
            result += " Skewering Plant";
        return result;
    }


    public override bool Equals(TLObject obj)
    {
        if (obj is TLShears && IsPlantSkewered() && ((TLShears)obj).IsPlantSkewered())
            return base.Equals(obj) && GetPlantSkewered().Equals(((TLShears)obj).GetPlantSkewered()) && GetDirectionFacing() == ((TLShears)obj).GetDirectionFacing();
        else if (obj is TLShears && !IsPlantSkewered() && !((TLShears)obj).IsPlantSkewered())
            return base.Equals(obj) && GetDirectionFacing() == ((TLShears)obj).GetDirectionFacing();
        else
            return false;
    }

    public override TLObject Copy()
    {
        return new TLShears(this);
    }
}
