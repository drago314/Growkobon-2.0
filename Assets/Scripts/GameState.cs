using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState 
{
    protected Dictionary<(int, int), List<TLObject>> posToTLObj;

    public GameState(List<TLObject> TLObjects)
    {
        posToTLObj = new Dictionary<(int, int), List<TLObject>>();
        foreach (var TLObj in TLObjects)
        {
            //TODO MAKE BETTER
            if (TLObj is TLPlayer)
            {
                AddObject(new TLPlayer(TLObj.curPos));
            }
            else if (TLObj is TLPlant)
            {
                AddObject(new TLPlant(TLObj.curPos));
            }
            else if (TLObj is TLWall)
            {
                AddObject(new TLWall(TLObj.curPos));    
            }
            else if (TLObj is TLDoor)
            {
                AddObject(new TLDoor(TLObj.curPos));
            }
            else if (TLObj is TLPot)
            {
                AddObject(new TLPot(TLObj.curPos));
            }
            else if (TLObj is TLPath)
            {
                AddObject(new TLPath((TLPath)TLObj));
            }
            else if (TLObj is TLLevel)
            {
                AddObject(new TLLevel((TLLevel)TLObj));
            }
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

    public GameState(GameState prevState) : this (prevState.GetAllTLObjects()) {}

    public List<TLObject> GetAllTLObjects()
    {
        var list = new List<TLObject>();
        foreach (var objList in posToTLObj.Values)
        {
            foreach (var obj in objList)
            {
                list.Add(obj);
            }
        }
        return list;
    }

    public TLPlayer GetPlayer()
    {
        foreach (var obj in GetAllTLObjects())
        {
            if (obj is TLPlayer)
                return (TLPlayer)obj;
        }

        GameManager.Inst.DEBUG("NO PLAYER FOUND");
        return null;
    }

    public List<TLPot> GetAllTLPots()
    {
        List<TLPot> potList = new List<TLPot>();
        foreach (var obj in GetAllTLObjects())
        {
            if (obj is TLPot)
                potList.Add((TLPot)obj);
        }
        return potList;
    }

    public List<TLDoor> GetAllTLDoors()
    {
        List<TLDoor> doorList = new List<TLDoor>();
        foreach (var obj in GetAllTLObjects())
        {
            if (obj is TLDoor)
                doorList.Add((TLDoor)obj);
        }
        return doorList;
    }

    public List<TLLevel> GetAllTLLevels()
    {
        List<TLLevel> levelList = new List<TLLevel>();
        foreach (var obj in GetAllTLObjects())
        {
            if (obj is TLLevel)
                levelList.Add((TLLevel)obj);
        }
        return levelList;
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

    public TLDoor GetDoorAtPos(Vector2Int pos)
    {
        if (GetTLObjectsAtPos(pos) == null)
            return null;
        foreach (var TLObj in GetTLObjectsAtPos(pos))
        {
            if (TLObj is TLDoor)
                return (TLDoor)TLObj;
        }
        return null;
    }

    public TLPot GetPotAtPos(Vector2Int pos)
    {
        if (GetTLObjectsAtPos(pos) == null)
            return null;
        foreach (var TLObj in GetTLObjectsAtPos(pos))
        {
            if (TLObj is TLPot)
                return (TLPot)TLObj;
        }
        return null;
    }

    public TLPath GetPathAtPos(Vector2Int pos)
    {
        if (GetTLObjectsAtPos(pos) == null)
            return null;
        foreach (var TLObj in GetTLObjectsAtPos(pos))
        {
            if (TLObj is TLPath)
                return (TLPath)TLObj;
        }
        return null;
    }

    public TLLevel GetLevelAtPos(Vector2Int pos)
    {
        if (GetTLObjectsAtPos(pos) == null)
            return null;
        foreach (var TLObj in GetTLObjectsAtPos(pos))
        {
            if (TLObj is TLLevel)
                return (TLLevel)TLObj;
        }
        return null;
    }

    public Vector2Int GetPosOf(TLObject TLObj)
    {
        return TLObj.curPos;
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

    public void AddObject(TLObject TLObj)
    {
        (int, int) posTuple = (TLObj.curPos.x, TLObj.curPos.y);

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
    }

    public void RemoveObject(TLObject TLObj)
    {
        posToTLObj[(GetPosOf(TLObj).x, GetPosOf(TLObj).y)].Remove(TLObj);
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
        obj.curPos = newPos;
        AddObject(obj);
        //GameManager.Inst.DEBUG("FINAL " + obj.gameObject.name + ": " + GetPosOf(obj).x + " " + GetPosOf(obj).y);
    }

    public bool Equals(GameState state)
    {
        foreach (var TLObj in GetAllTLObjects())
        {
            bool found = false;
            foreach (var TLObj2 in state.GetAllTLObjects())
            {
                if (TLObj.Equals(TLObj2))
                {
                    found = true;
                    break;
                }
            }
            if (found == false)
                return false;
        }

        return true;
    }

    public override string ToString()
    {
        string result = "POS TO TLOBJ:\n";

        foreach (var kvp in posToTLObj)
        {
            if (kvp.Value.Count > 0)
            {
                result += "Obj: " + kvp.Key.Item1 + " " + kvp.Key.Item2 + " Value:";
                foreach (var TLObj in kvp.Value)
                {
                    result += " " + TLObj.GetName();
                }
                result += "\n";
            }
        }
        return result;
    }
}
