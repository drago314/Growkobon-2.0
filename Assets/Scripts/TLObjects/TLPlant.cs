using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TLPlant : TLMoveableObject
{
    private List<TLPlant> stateList;

    private bool isAlive;
    private bool isSkewered;
    private bool checkedForPush = false;
    private bool pushed = false;

    public Action<MoveAction> OnPlantMove;
    public Action<MoveAction> OnUndoOrReset;
    public Action OnPlantSkewered;
    public Action OnPlantUnskewered;
    public Action OnPlantCornerSpinSheared;
    public Action OnPlantDeath;
    public Action OnPlantRegrowth;

    public TLPlant(Vector2Int curPos, PlantSignature sig) : base(curPos)
    {
        isAlive = sig.isAlive;
        isSkewered = false;
        stateList = new List<TLPlant>();
        stateList.Add(new TLPlant(this));
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
        isAlive = obj.isAlive;
        isSkewered = obj.isSkewered;
        curPos = obj.curPos;
        OnUndoOrReset?.Invoke(new MoveAction(curPos, curPos, Vector2Int.zero, this, GameManager.Inst.currentState));
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

        TLPlant[] plantGroup = currentState.GetPlantGroupAtPos(curPos);

        foreach (var plant in plantGroup)
            plant.pushed = true;

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
            stateList = new List<TLPlant>();
            stateList.Add(new TLPlant(this));
            GameManager.Inst.currentState.AddObject(this);
        }
        else
        {
            Destroy();
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
