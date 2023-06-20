using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState 
{
    private Dictionary<(int, int), TLObject[]> stateDict; //TODO good name

    public GameState(TLObject[] TLObjects)
    {
        stateDict = new Dictionary<(int, int), TLObject[]>();
        foreach (var TLObj in TLObjects)
        {
            (int, int) posTuple = ((int) TLObj.transform.position.x, (int) TLObj.transform.position.y);
            
            if (stateDict.ContainsKey(posTuple))
            {
                TLObject[] tempArr = stateDict[posTuple];
                TLObject[] newArr = new TLObject[tempArr.Length + 1];
                for (int i = 0; i < tempArr.Length; i++)
                {
                    newArr[i] = tempArr[i];
                }
                newArr[tempArr.Length] = TLObj;
                stateDict.Remove(posTuple);
                stateDict.Add(posTuple, newArr);
            }
            else
            {
                TLObject[] key = {TLObj};
                stateDict.Add(posTuple, key);
            }
        }

        //DEBUG
        foreach (var kvp in stateDict)
        {
            GameManager.Inst.DEBUG("Key = " + kvp.Key +  "Value = ");
            foreach (var val in kvp.Value)
            {
                GameManager.Inst.DEBUG(val.ToString());
            }
        }
    }

    public GameState(GameState prevState)
    {
    }
}
