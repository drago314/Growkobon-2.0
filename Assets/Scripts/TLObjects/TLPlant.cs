using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TLPlant : TLMoveableObject
{
    private List<TLPlant> stateList;
    private TLPlant initialState;
    private bool deleted = false;
    public Action OnDoneWithObject;

    private bool isAlive;
    private bool isSkewered;
    private bool checkedForPush = false;
    private bool pushed = false;

    public Action<MoveAction> OnPlantMove;
    public Action OnPlantSkewered;
    public Action OnPlantUnskewered;
    public Action OnPlantCornerSpinSheared;
    public Action OnPlantDeath;
    public Action OnPlantRegrowth;

    public TLPlant(Vector2Int curPos, PlantSignature sig) : base(curPos)
    {
        isAlive = sig.isAlive;
        isSkewered = false;
        initialState = new TLPlant(this);
        stateList = new List<TLPlant>();
        stateList.Add(initialState);
    }

    public TLPlant(Vector2Int curPos, bool isAlive) : base(curPos)
    {
        this.isAlive = isAlive;
        isSkewered = false;
        stateList = new List<TLPlant>();
    }

    private TLPlant(TLPlant obj) : base(obj)
    {
        Initialize(obj);
    }


    public void Initialize(TLPlant obj)
    {
        deleted = obj.deleted;
        isAlive = obj.isAlive;
        isSkewered = obj.isSkewered;
        curPos = obj.curPos;
    }

    public bool IsAlive() { return isAlive; }

    public void SetAlive(bool isAlive)
    {
        if (this.isAlive && !isAlive)
        {
            this.isAlive = false;
            OnPlantDeath?.Invoke();
        }
        else if (!this.isAlive && isAlive)
        {
            this.isAlive = true;
            OnPlantRegrowth?.Invoke();
        }
    }

    public bool IsDeleted() { return deleted; }
    public bool HasBeenCheckedForPush() { return checkedForPush; }
    public bool HasBeenPushed() { return pushed; }
    public bool IsSkewered() { return isSkewered; }

    public void SetSkewered(bool isSkewered)
    {
        if (this.isSkewered && !isSkewered)
        {
            this.isSkewered = false;
            OnPlantUnskewered?.Invoke();
        }
        else if (!this.isSkewered && isSkewered)
        {
            this.isSkewered = true;
            isAlive = false;
            OnPlantSkewered?.Invoke();
        }
    }

    public void Shear()
    {
        isAlive = false;
        OnPlantCornerSpinSheared?.Invoke();
    }


    public override void SetPos(Vector2Int pos)
    {
        OnPlantMove?.Invoke(new MoveAction(curPos, pos, pos - curPos, this, GameManager.Inst.currentState));
        curPos = pos;
    }

    public override bool CanMove(TLObject pusher, Vector2Int moveDir)
    {
        GameState currentState = GameManager.Inst.currentState;

        if (checkedForPush)
            return true;

        if (pusher is TLShears && ((TLShears)pusher).GetDirectionFacing() == moveDir && !((TLShears)pusher).IsPlantSkewered())
        {
            TLShears shears = (TLShears)pusher;
            if (!shears.IsCurrentlyCornerSpinning())
                return true;
        }

        if (pusher is TLTrimmers && ((TLTrimmers)pusher).IsHeld())
            return true;

        TLPlant[] plantGroup = currentState.GetPlantGroupAtPos(curPos);

        foreach (var plant in plantGroup)
        {
            bool plantCanMove = true;
            if (!currentState.IsTLOfTypeAtPos<TLPlant>(plant.GetPosition() + moveDir))
                plantCanMove = currentState.CanPush(plant, moveDir);
            plant.checkedForPush = true;
            if (!plantCanMove)
                return false;
        }

        return true;
    }

    public override void Move(TLObject pusher, Vector2Int moveDir)
    {
        Debug.Log("Move Plant Called By " + pusher.GetName() + ": " + pusher.GetPosition());

        if (pushed)
            return;

        Debug.Log("Plant was not already pushed");

        GameState currentState = GameManager.Inst.currentState;

        if (pusher is TLShears && ((TLShears)pusher).GetDirectionFacing() == moveDir && !((TLShears)pusher).IsPlantSkewered())
        {
            TLShears shears = (TLShears)pusher;
            if (!shears.IsCurrentlyCornerSpinning())
            {
                shears.SkewerPlant(this);
                return;
            }
            else
            {
                isAlive = false;
                OnPlantCornerSpinSheared?.Invoke();
            }
        }

        if (pusher is TLTrimmers && ((TLTrimmers)pusher).IsHeld())
        {
            return;
        }

        TLPlant[] plantGroup = currentState.GetPlantGroupAtPos(curPos);

        foreach (var plant in plantGroup)
            plant.pushed = true;

        foreach (var plant in plantGroup) // Check which shears will unskewer from plant in the movement
        {
            if (plant.IsSkewered() && currentState.GetTLOfTypeAtPos<TLShears>(plant.GetPosition()) != pusher
                && currentState.GetTLOfTypeAtPos<TLShears>(plant.GetPosition()).GetDirectionFacing() == moveDir
                && !currentState.IsTLOfTypeAtPos<TLPlant>(plant.GetPosition() - moveDir))
                currentState.GetTLOfTypeAtPos<TLShears>(plant.GetPosition()).UnskewerPlant();
        }

        foreach (var plant in plantGroup)
        {
            bool wasSkewered = plant.IsSkewered();
            if (!currentState.IsTLOfTypeAtPos<TLPlant>(plant.GetPosition() + moveDir))
                currentState.Push(plant, moveDir);
            if (wasSkewered && currentState.GetTLOfTypeAtPos<TLShears>(plant.GetPosition()) != pusher)
                currentState.GetTLOfTypeAtPos<TLShears>(plant.GetPosition()).Move(plant, moveDir);
            plant.GroupedMove(moveDir);
        }
    }

    public void GroupedMove(Vector2Int moveDir)
    {
        OnPlantMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, this, GameManager.Inst.currentState));
        curPos = curPos + moveDir;
        GameManager.Inst.currentState.Move(this, curPos - moveDir);
    }

    public override void EndMove(bool changeHappened)
    {
        if (changeHappened)
            stateList.Add(new TLPlant(this));
        pushed = false;
        checkedForPush = false;
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
            if(stateList[stateList.Count - 2].IsDeleted())
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
            stateList.Add(new TLPlant(this));
            GameManager.Inst.currentState.AddObject(this);
        }
        else if (deleted)
        {
            stateList.Add(new TLPlant(this));
        }
        else
        {
            GameManager.Inst.currentState.DeleteObject(this);
            deleted = true;
            stateList.Add(new TLPlant(this));
        }
    }

    private void PrintStateList()
    {
        string result = "Current Plant StateList:\n";
        foreach (var state in stateList)
        {
            result += state.curPos + "\n";
        }
        Debug.Log(result);
    }

    public override string GetName() 
    {
        string result = "";
        if (IsAlive())
            result += "Alive ";
        else
            result += "Dead ";
        result += "Plant";

        if (IsSkewered())
            result += " is skewered";

        return result; 
    }
}
