using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TLPlant : TLMoveableObject
{
    private List<TLPlant> stateList;

    private bool isAlive;
    private bool isSkewered;

    public Action<MoveAction> OnPlantMove;
    public Action<MoveAction> OnUndoOrReset;
    public Action OnPlantSkewered;
    public Action OnPlantUnskewered;
    public Action OnPlantDeath;
    public Action OnPlantRegrowth;

    public TLPlant(Vector2Int curPos, PlantSignature sig) : base(curPos)
    {
        isAlive = sig.isAlive;
        stateList = new List<TLPlant>();
        stateList.Add(new TLPlant(this));
    }

    public TLPlant(Vector2Int curPos, bool isAlive) : base(curPos)
    {
        this.isAlive = isAlive;
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
            this.isAlive = false;
            OnPlantSkewered?.Invoke();
        }
    }

    public override void Move(Vector2Int pos)
    {
        OnPlantMove?.Invoke(new MoveAction(curPos, pos, pos - curPos, this, GameManager.Inst.currentState));
        curPos = pos;
    }

    public override void EndMove()
    {
        stateList.Add(new TLPlant(this));
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
        return result; 
    }
}
