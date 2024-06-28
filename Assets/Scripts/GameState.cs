using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState 
{
    protected Dictionary<(int, int), List<TLObject>> posToTLObj;
    private int moveCount = 0;

    public GameState()
    {
        moveCount = 0;
        posToTLObj = new Dictionary<(int, int), List<TLObject>>();
    }

    public int GetMoveCount() { return moveCount; }
    public bool AtInitialState() { return moveCount == 0; }

    public void AddObject(TLObject TLObj)
    {
        (int, int) posTuple = (TLObj.GetPosition().x, TLObj.GetPosition().y);

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

    public void RemoveObject(TLObject TLObj, Vector2Int oldPos)
    {
        posToTLObj[(oldPos.x, oldPos.y)].Remove(TLObj);
    }

    public void ClearState()
    {
        posToTLObj = new Dictionary<(int, int), List<TLObject>>();
        moveCount = 0;
    }

    public void EndMove()
    {
        foreach (var obj in GetAllTLObjects())
            obj.EndMove();
        moveCount += 1;
    }

    public void Undo()
    {
        if (moveCount == 0)
            return;

        foreach (var obj in GetAllTLObjects())
        {
            obj.Undo();
        }
        moveCount -= 1;
    }

    public void Reset()
    {
        if (moveCount == 0)
            return;

        foreach (var obj in GetAllTLObjects())
        {
            obj.Reset();
        }
        moveCount = 0;
    }

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

    public List<T> GetAllOfTLType<T>() where T : TLObject
    {
        List<T> objectList = new List<T>();
        foreach (var obj in GetAllTLObjects())
        {
            if (obj is T)
                objectList.Add((T)obj);
        }
        return objectList;
    }

    public bool IsTLObjectAtPos(Vector2Int pos)
    {
        if (!posToTLObj.ContainsKey((pos.x, pos.y)))
            return false;
        return true;
    }

    public List<TLObject> GetTLObjectsAtPos(Vector2Int pos)
    {
        if (!posToTLObj.ContainsKey((pos.x, pos.y)))
            return null;
        return posToTLObj[(pos.x, pos.y)];
    }

    public bool IsTLOfTypeAtPos<T>(Vector2Int pos) where T : TLObject
    {
        if (GetTLObjectsAtPos(pos) == null)
            return false;
        foreach (var TLObj in GetTLObjectsAtPos(pos))
        {
            if (TLObj is T)
                return true;
        }
        return false;
    }

    public T GetTLOfTypeAtPos<T>(Vector2Int pos) where T : TLObject
    {
        if (GetTLObjectsAtPos(pos) == null)
            return null;
        foreach (var TLObj in GetTLObjectsAtPos(pos))
        {
            if (TLObj is T)
                return (T)TLObj;
        }
        return null;
    }

    public bool CanPush(TLObject pusher, Vector2Int moveDir)
    {
        if (!IsTLObjectAtPos(pusher.GetPosition() + moveDir))
            return true;

        bool objectCanMove = true;
        foreach (var obj in GetTLObjectsAtPos(pusher.GetPosition() + moveDir))
        {
            bool canMove = obj.CanMove(pusher, moveDir);
            if (!canMove)
            {
                objectCanMove = false;
                break;
            }
        }

        return objectCanMove;
    }

    public void Push(TLObject pusher, Vector2Int moveDir)
    {
        if (!IsTLObjectAtPos(pusher.GetPosition() + moveDir))
            return;

        TLObject[] objectsToMove = GetTLObjectsAtPos(pusher.GetPosition() + moveDir).ToArray();

        foreach (var obj in objectsToMove)
        {
            if (obj is TLMoveableObject && obj is not TLPlayer)
                ((TLMoveableObject)obj).Move(pusher, moveDir);
        }
    }

    public Vector2Int GetPosOf(TLObject TLObj)
    {
        return TLObj.GetPosition();
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
            TLPlant currentcheck = GetTLOfTypeAtPos<TLPlant>(plantPos + new Vector2Int(0, 1));
            if (currentcheck != null && !plantsList.Contains(currentcheck))
                plantsList.Add(currentcheck);

            currentcheck = GetTLOfTypeAtPos<TLPlant>(plantPos + new Vector2Int(0, -1));
            if (currentcheck != null && !plantsList.Contains(currentcheck))
                plantsList.Add(currentcheck);

            currentcheck = GetTLOfTypeAtPos<TLPlant>(plantPos + new Vector2Int(1, 0));
            if (currentcheck != null && !plantsList.Contains(currentcheck))
                plantsList.Add(currentcheck);

            currentcheck = GetTLOfTypeAtPos<TLPlant>(plantPos + new Vector2Int(-1, 0));
            if (currentcheck != null && !plantsList.Contains(currentcheck))
                plantsList.Add(currentcheck);
        }

        return plantsList.ToArray();
    }

    public TLPlant[]  GetPlantGroupAtPos(Vector2Int pos)
    {
        if (GetTLOfTypeAtPos<TLPlant>(pos) != null)
            return GetPlantGroupOf(GetTLOfTypeAtPos<TLPlant>(pos));
        else
            return null;
    }

    public void Move(TLMoveableObject obj, Vector2Int oldPos)
    {
        //GameManager.Inst.DEBUG("ORIGINAL " + obj.gameObject.name + ": " + GetPosOf(obj).x + " " + GetPosOf(obj).y);
        RemoveObject(obj, oldPos);
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
        string result = "MOVE COUNT: " + moveCount + "\n"; 
        result += "POS TO TLOBJ:\n";

        foreach (var kvp in posToTLObj)
        {
            if (kvp.Value.Count > 0)
            {
                result += "Obj: " + kvp.Key.Item1 + " " + kvp.Key.Item2 + " Value:";
                foreach (var TLObj in kvp.Value)
                {
                    result += " " + TLObj.GetName();
                    result += ",";
                }
                result += "\n";
            }
        }
        return result;
    }
}
