using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState 
{
    private Dictionary<(int, int), List<TLObject>> posToTLObj;
    private Dictionary<TLObject, Vector2Int> TLObjToPos;

    public GameState(TLObject[] TLObjects)
    {
        posToTLObj = new Dictionary<(int, int), List<TLObject>>();
        TLObjToPos = new Dictionary<TLObject, Vector2Int>();
        foreach (var TLObj in TLObjects)
        {
            AddObject(TLObj, new Vector2Int((int) TLObj.transform.position.x, (int)TLObj.transform.position.y));
        }

        //DEBUG
        /*
        foreach (var kvp in stateDict)
        {
            GameManager.Inst.DEBUG("Key = " + kvp.Key +  "Value = ");
            foreach (var val in kvp.Value)
            {
                GameManager.Inst.DEBUG(val.ToString());
            }
        }*/
    }

    public GameState(GameState prevState)
    { 
    }

    public List<TLObject> GetTLObjectsAtPos(Vector2Int pos)
    {
        if (!posToTLObj.ContainsKey((pos.x, pos.y)))
            return null;
        return posToTLObj[(pos.x, pos.y)];
    }

    public TLPlant GetPlantAtPos(Vector2Int pos)
    {
        if (GetTLObjectsAtPos(pos) == null)
            return null;
        foreach (var TLObj in GetTLObjectsAtPos(pos))
        {
            if (TLObj is TLPlant)
                return (TLPlant) TLObj;
        }
        return null;
    }
    public TLWall GetWallAtPos(Vector2Int pos)
    {
        if (GetTLObjectsAtPos(pos) == null)
            return null;
        foreach (var TLObj in GetTLObjectsAtPos(pos))
        {
            if (TLObj is TLWall)
                return (TLWall)TLObj;
        }
        return null;
    }

    public Vector2Int GetPosOf(TLObject TLObj)
    {
        return TLObjToPos[TLObj];
    }

    public TLPlant[] GetPlantGroupOf(TLPlant plant)
    {
        List<TLPlant> plantsList = new List<TLPlant>();

        plantsList.Add(plant);

        for (int i = 0; i < plantsList.Count; i++)
        {
            Vector2Int plantPos = GetPosOf(plantsList[i]);
            //GameManager.Inst.DEBUG("" + plantPos.x + " " + plantPos.y);
            //GameManager.Inst.DEBUG("" + plantsList.Count);
            TLPlant currentcheck = GetPlantAtPos(plantPos + new Vector2Int(0, 1));
            if (currentcheck != null && !plantsList.Contains(currentcheck))
                plantsList.Add(currentcheck);

            currentcheck = GetPlantAtPos(plantPos + new Vector2Int(0, -1));
            if (currentcheck != null && !plantsList.Contains(currentcheck))
                plantsList.Add(currentcheck);

            currentcheck = GetPlantAtPos(plantPos + new Vector2Int(1, 0));
            if (currentcheck != null && !plantsList.Contains(currentcheck))
                plantsList.Add(currentcheck);

            currentcheck = GetPlantAtPos(plantPos + new Vector2Int(-1, 0));
            if (currentcheck != null && !plantsList.Contains(currentcheck))
                plantsList.Add(currentcheck);
        }

        return plantsList.ToArray();
    }

    public TLPlant[]  GetPlantGroupAtPos(Vector2Int pos)
    {
        if (GetPlantAtPos(pos) != null)
            return GetPlantGroupOf(GetPlantAtPos(pos));
        else
            return null;
    }

    public void AddObject(TLObject TLObj, Vector2Int pos)
    {
        (int, int) posTuple = ((int)TLObj.transform.position.x, (int)TLObj.transform.position.y);

        if (posToTLObj.ContainsKey(posTuple))
        {
            posToTLObj[posTuple].Add(TLObj);
        }
        else
        {
            List<TLObject> key = new List<TLObject>();
            key.Add(TLObj);
            posToTLObj.Add(posTuple, key);
        }

        TLObjToPos.Add(TLObj, pos);
    }

    public void RemoveObject(TLObject TLObj)
    {
        posToTLObj[(GetPosOf(TLObj).x, GetPosOf(TLObj).y)].Remove(TLObj);
        TLObjToPos.Remove(TLObj);
    }

    public void MoveRelative(TLObject obj, Vector2Int posChange)
    {
        Vector2Int newPos = GetPosOf(obj) + posChange;
        Move(obj, newPos);
    }

    public void Move(TLObject obj, Vector2Int newPos)
    {
        //GameManager.Inst.DEBUG("ORIGINAL " + obj.gameObject.name + ": " + GetPosOf(obj).x + " " + GetPosOf(obj).y);
        RemoveObject(obj);
        AddObject(obj, newPos);
        //GameManager.Inst.DEBUG("FINAL " + obj.gameObject.name + ": " + GetPosOf(obj).x + " " + GetPosOf(obj).y);
    }

    public override string ToString()
    {
        string result = "TLOBJ TO POS:\n";
        foreach (var kvp in TLObjToPos)
        {
            result += "Obj: " + kvp.Key.name + " Value: " + kvp.Value + "\n";
        }

        result += "POS TO TLOBJ:\n";
        foreach (var kvp in posToTLObj)
        {
            result += "Obj: " + kvp.Key.Item1 + " " + kvp.Key.Item2 + " Value:";
            foreach (var TLObj in kvp.Value)
            {
                result += " " + TLObj.name;
            }
            result += "\n";
        }
        return result;
    }
}
