using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TLShears : TLHoldableObject
{
    private List<TLShears> stateList;

    private TLPlant plantSkewered = null;

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

    public override void EndMove()
    {
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

    public override string GetName() { return "Shears facing " + GetDirectionFacing(); }
}
