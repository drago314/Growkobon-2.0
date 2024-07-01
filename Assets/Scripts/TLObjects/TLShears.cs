using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TLShears : TLHoldableObject
{
    private List<TLShears> stateList;

    private TLPlant plantSkewered = null;
    private bool currentlyCornerSpinning = false;

    public event Action<MoveAction> OnShearsMove;
    public event Action<SpinAction> OnShearsSpin;
    public Action<SkewerAction> OnPlantSkewered;
    public Action<SkewerAction> OnPlantUnskewered;
    public Action<InstantMoveRotatableObject> OnUndoOrReset;

    public TLShears(Vector2Int curPos, ShearSignature sig) : base(curPos, sig.directionFacing)
    {
        stateList = new List<TLShears>();
        stateList.Add(new TLShears(this));
    }

    private TLShears(TLShears obj) : base(obj)
    {
        Initialize(obj);
    }

    private void Initialize(TLShears obj)
    {
        base.Initialize(obj);
        plantSkewered = obj.plantSkewered;
        OnUndoOrReset?.Invoke(new InstantMoveRotatableObject(curPos, GetDirectionFacing(), this, GameManager.Inst.currentState)); ;
    }

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
        GameState currentState = GameManager.Inst.currentState;

        if (pusher is TLPlayer && GetDirectionFacing() == -1 * moveDir && !IsPlantSkewered())
            return false;
        if (currentState.IsTLOfTypeAtPos<TLPlayer>(curPos + moveDir))
            return true;
        if (currentState.IsTLOfTypeAtPos<TLPlant>(curPos + moveDir) && GetDirectionFacing() == moveDir)
            return true;

        if (IsPlantSkewered() && moveDir != -1 * GetDirectionFacing())
            return plantSkewered.CanMove(this, moveDir);

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

        if (pusher is TLPlant && IsPlantSkewered() && pusher == GetPlantSkewered()) // If we are be moved inside a moving plant block
        {
            if (!spinning)
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
            if (!spinning)
                OnShearsMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, this, GameManager.Inst.currentState));
            curPos = curPos + moveDir;
            currentState.Move(this, curPos - moveDir);
            return;
        }

        if (IsPlantSkewered())
        {
            if (pusher is TLPlayer && GetDirectionFacing() == -1 * moveDir)
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

        bool canMoveCorner = CanMove(spinner, endDir);

        if (!canMoveCorner && currentState.IsTLOfTypeAtPos<TLPlant>(cornerSpot))
        {
            currentState.GetTLOfTypeAtPos<TLPlant>(cornerSpot).Shear();
            return true;
        }
        else if (!canMoveCorner)
        {
            return false;
        }

        bool somethingChanged = currentState.IsTLOfTypeAtPos<TLMoveableObject>(cornerSpot);

        Move(spinner, endDir, true);

        // FINAL POSITION
        currentlyCornerSpinning = false;
        Rotate90Degrees(clockwise);

        bool canMoveFinal = CanMove(spinner, startDir * -1);

        if (!canMoveFinal)
            return somethingChanged;

        Move(spinner, startDir * -1, true);
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
            stateList = new List<TLShears>();
            stateList.Add(new TLShears(this));
            GameManager.Inst.currentState.AddObject(this);
        }
        else
        {
            Destroy();
        }
    }

    public override string GetName()
    {
        string result = "Shears facing " + GetDirectionFacing();
        if (IsPlantSkewered())
            result += " Skewering Plant";
        return result;
    }
}
