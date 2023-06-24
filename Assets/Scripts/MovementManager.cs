using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementManager : MonoBehaviour
{
    public event System.Action OnMoveBegin;
    public event System.Action OnMoveEnd;
    public event System.Action<MoveAction> OnPlayerMove;
    public event System.Action<MoveAction> OnPlantMove;
    public event System.Action<GrowAction> OnPlantGrow;
    public event System.Action<Vector2Int> OnDoorClose;
    public event System.Action<Vector2Int> OnDoorOpen;

    private void Start()
    {
        GameManager.Inst.moveUp.action.performed += MoveUp;
        GameManager.Inst.moveDown.action.performed += MoveDown;
        GameManager.Inst.moveRight.action.performed += MoveRight;
        GameManager.Inst.moveLeft.action.performed += MoveLeft;
    }

    private void MoveUp(InputAction.CallbackContext obj)
    {
        Move(Vector2Int.up);
    }

    private void MoveDown(InputAction.CallbackContext obj)
    {
        Move(Vector2Int.down);
    }
    private void MoveRight(InputAction.CallbackContext obj)
    {
        Move(Vector2Int.right);
    }
    private void MoveLeft(InputAction.CallbackContext obj)
    {
        Move(Vector2Int.left);
    }

    private void Move(Vector2Int moveDir)
    {
        //print("Begin Move: " + GameManager.Inst.stateList.Count);
        OnMoveBegin?.Invoke();

        GameState state = GameManager.Inst.currentState;
        TLPlayer player = state.GetPlayer();
        Vector2Int curPos = state.GetPosOf(player);
        Vector2Int goalPos = curPos + moveDir;

        player.directionFacing = moveDir;
        Dictionary<TLDoor, bool> originalDoorStates = new Dictionary<TLDoor, bool>();
        foreach (var door in state.GetAllTLDoors())
        {
            originalDoorStates.Add(door, door.IsOpen());
        }


        //print(state.ToString());

        // 2
        if (state.GetWallAtPos(goalPos) != null)
        {
            OnPlayerMove?.Invoke(new MoveAction(curPos, curPos, moveDir, player));
            return;
        }
        if (state.GetDoorAtPos(goalPos) != null && !state.GetDoorAtPos(goalPos).IsOpen())
        {
            OnPlayerMove?.Invoke(new MoveAction(curPos, curPos, moveDir, player));
            return;
        }

        //6 
        //print("goal pos: " + goalPos);
        if (state.GetPlantAtPos(goalPos) != null)
        {
            TLPlant[] plantGroup = state.GetPlantGroupAtPos(goalPos);
            bool canMove = true;
            foreach (var plant in plantGroup)
            {
                if (state.GetWallAtPos(state.GetPosOf(plant) + moveDir) != null || state.GetDoorAtPos(state.GetPosOf(plant) + moveDir) != null)
                {
                    canMove = false;
                    break;
                }
            }
            if (canMove)
            {
                foreach (var plant in plantGroup)
                {
                    OnPlantMove?.Invoke(new MoveAction(plant.curPos, plant.curPos + moveDir, moveDir, plant));
                    state.MoveRelative(plant, moveDir);
                }
                OnPlayerMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, player));
                state.MoveRelative(player, moveDir);
            }
            else
            {
                OnPlayerMove?.Invoke(new MoveAction(curPos, curPos, moveDir, player));
            }
            GrowPlant(goalPos, moveDir);
        }
        else
        {
            OnPlayerMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, player));
            state.MoveRelative(player, moveDir);
        }

        //10
        foreach (var kvp in originalDoorStates)
        {
            if (kvp.Key.IsOpen() != kvp.Value)
            {
                if (kvp.Key.IsOpen())
                    OnDoorOpen?.Invoke(kvp.Key.curPos);
                else
                    OnDoorClose?.Invoke(kvp.Key.curPos);
            }
        }

        //11
        OnMoveEnd?.Invoke();
        GameManager.Inst.EndMove();
        
        //print(state.ToString());
        //print("End Move: " + GameManager.Inst.stateList.Count);
    }

    private void GrowPlant(Vector2Int goalPos, Vector2Int moveDir)
    {
        GameState state = GameManager.Inst.currentState;

        Vector2Int desiredPlantGrowth = goalPos + moveDir;
        while (state.GetPlantAtPos(desiredPlantGrowth) != null)
        {
            desiredPlantGrowth += moveDir;
        }
        if (state.GetWallAtPos(desiredPlantGrowth) == null && state.GetDoorAtPos(desiredPlantGrowth) == null)
        {
            TLPlant plant = new TLPlant(desiredPlantGrowth);
            OnPlantGrow?.Invoke(new GrowAction(desiredPlantGrowth, moveDir, plant));
            state.AddObject(plant);
        }
    }
}
