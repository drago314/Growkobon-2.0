using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;

public class MovementManager : MonoBehaviour
{
    [SerializeField] public InputActionReference moveUp, moveDown, moveRight, moveLeft, interact, undo, reset, finishLevel;

    public event Action OnMoveBegin;
    public event Action OnMoveEnd;
    public event Action OnUndoBegin;
    public event Action OnUndoEnd;
    public event Action OnResetBegin;
    public event Action OnResetEnd;
    public event Action<GrowAction> OnPlantGrow;

    Dictionary<TLDoor, bool> originalDoorStates; //  TODO this is bad and should be fixed with new animation / game state system

    private void Start()
    {
        moveUp.action.performed += MoveUp;
        moveDown.action.performed += MoveDown;
        moveRight.action.performed += MoveRight;
        moveLeft.action.performed += MoveLeft;
        interact.action.performed += Interact;
        undo.action.performed += Undo;
        reset.action.performed += Reset;
        finishLevel.action.performed += DebugFinishLevel;
    }

    private void OnDestroy()
    {
        moveUp.action.performed -= MoveUp;
        moveDown.action.performed -= MoveDown;
        moveRight.action.performed -= MoveRight;
        moveLeft.action.performed -= MoveLeft;
        interact.action.performed -= Interact;
        undo.action.performed -= Undo;
        reset.action.performed -= Reset;
        finishLevel.action.performed -= DebugFinishLevel;
    }

    private void MoveUp(InputAction.CallbackContext obj)
    {
        GameState currentState = GameManager.Inst.currentState;
        if (currentState.GetPlayer().IsObjectHeld())
            if (currentState.GetPlayer().GetDirectionFacing() == Vector2Int.left || currentState.GetPlayer().GetDirectionFacing() == Vector2Int.right)
                TurnHoldingShear(currentState.GetPlayer().GetDirectionFacing(), Vector2Int.up);
            else
                MoveHoldingShear(Vector2Int.up);
        else
            Move(Vector2Int.up);
    }

    private void MoveDown(InputAction.CallbackContext obj)
    {
        GameState currentState = GameManager.Inst.currentState;
        if (currentState.GetPlayer().IsObjectHeld())
            if (currentState.GetPlayer().GetDirectionFacing() == Vector2Int.left || currentState.GetPlayer().GetDirectionFacing() == Vector2Int.right)
                TurnHoldingShear(currentState.GetPlayer().GetDirectionFacing() , Vector2Int.down);
            else
                MoveHoldingShear(Vector2Int.down);
        else
            Move(Vector2Int.down);
    }
    private void MoveRight(InputAction.CallbackContext obj)
    {
        GameState currentState = GameManager.Inst.currentState;
        if (currentState.GetPlayer().IsObjectHeld())
            if (currentState.GetPlayer().GetDirectionFacing() == Vector2Int.up || currentState.GetPlayer().GetDirectionFacing() == Vector2Int.down)
                TurnHoldingShear(currentState.GetPlayer().GetDirectionFacing(), Vector2Int.right);
            else
                MoveHoldingShear(Vector2Int.right);
        else
            Move(Vector2Int.right);
    }
    private void MoveLeft(InputAction.CallbackContext obj)
    {
        GameState currentState = GameManager.Inst.currentState;
        if (currentState.GetPlayer().IsObjectHeld())
            if (currentState.GetPlayer().GetDirectionFacing() == Vector2Int.up || currentState.GetPlayer().GetDirectionFacing() == Vector2Int.down)
                TurnHoldingShear(currentState.GetPlayer().GetDirectionFacing(), Vector2Int.left);
            else
                MoveHoldingShear(Vector2Int.left);
        else
            Move(Vector2Int.left);
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        BeginMove();

        GameState currentState = GameManager.Inst.currentState;

        TLPlayer player = currentState.GetPlayer();
        Vector2Int curPos = currentState.GetPosOf(player);
        Vector2Int grabDirection = Vector2Int.zero;        

        if (player.IsObjectHeld())
        {
            //player.objectHeld.SetHeld(false);
            //player.objectHeld = null;
            player.ReleaseObject();
        }
        else if (currentState.GetTLOfTypeAtPos<TLShears>(curPos + Vector2Int.up) != null && currentState.GetTLOfTypeAtPos<TLShears>(curPos + Vector2Int.up).GetDirectionFacing() != Vector2Int.down)
            grabDirection = Vector2Int.up;
        else if (currentState.GetTLOfTypeAtPos<TLShears>(curPos + Vector2Int.down) != null && currentState.GetTLOfTypeAtPos<TLShears>(curPos + Vector2Int.down).GetDirectionFacing() != Vector2Int.up)
            grabDirection = Vector2Int.down;
        else if (currentState.GetTLOfTypeAtPos<TLShears>(curPos + Vector2Int.left) != null && currentState.GetTLOfTypeAtPos<TLShears>(curPos + Vector2Int.left).GetDirectionFacing() != Vector2Int.right)
            grabDirection = Vector2Int.left;
        else if (currentState.GetTLOfTypeAtPos<TLShears>(curPos + Vector2Int.right) != null && currentState.GetTLOfTypeAtPos<TLShears>(curPos + Vector2Int.right).GetDirectionFacing() != Vector2Int.left)
            grabDirection = Vector2Int.right;
        else
            return;

        Debug.Log("Grab Direction: " + grabDirection);

        // If one of the checks passed to pick up an object
        if (grabDirection != Vector2Int.zero)
        {
            //player.objectHeld = currentState.GetShearsAtPos(curPos + grabDirection);
            player.PickupObject(currentState.GetTLOfTypeAtPos<TLShears>(curPos + grabDirection));
            Debug.Log("Player Holding: " + player.GetObjectHeld());
        }

        EndMove();
    }

    private void Move(Vector2Int moveDir)
    {
        Debug.Log("BEGIN");
        print(GameManager.Inst.currentState.ToString());

        BeginMove();
        bool somethingChanged = TypicalMove(moveDir);
        if (somethingChanged)
            EndMove();
    }

    // Returns true if something changed
    private bool TypicalMove(Vector2Int moveDir)
    {
        GameState currentState = GameManager.Inst.currentState;

        TLPlayer player = currentState.GetPlayer();
        Vector2Int curPos = currentState.GetPosOf(player);
        Vector2Int goalPos = curPos + moveDir;

        // Check if the player is walking into a wall, door, or other object it cannot walk though
        if (currentState.GetTLOfTypeAtPos<TLWall>(goalPos) != null || currentState.GetTLOfTypeAtPos<TLShears>(goalPos) != null)
        {
            player.SetDirectionFacing(moveDir);
            return false;
        }
        if (currentState.GetTLOfTypeAtPos<TLDoor>(goalPos) != null && !currentState.GetTLOfTypeAtPos<TLDoor>(goalPos).IsOpen())
        {
            player.SetDirectionFacing(moveDir);
            return false;
        }


        //6 
        if (currentState.GetTLOfTypeAtPos<TLPlant>(goalPos) != null)
        {
            TLPlant[] plantGroup = currentState.GetPlantGroupAtPos(goalPos);
            bool canMove = MovePlantGroup(plantGroup, moveDir);

            if (canMove)
            {
                currentState.MoveRelative(player, moveDir);
                GrowPlant(goalPos + moveDir, moveDir);
            }
            else
            {
                player.SetDirectionFacing(moveDir);
                bool grewPlant = GrowPlant(goalPos, moveDir);
                if (!grewPlant)
                    return false;
            }
        }
        else
        {
            currentState.MoveRelative(player, moveDir);
        }

        return true;
    }

    // A turn from right to up would have a starting Dir of Vector2Int.right and a ending dir of Vector2Int.up
    private void TurnHoldingShear(Vector2Int startingDir, Vector2Int endingDir)
    {
        Debug.Log("BEGIN TURN");
        print(GameManager.Inst.currentState.ToString());

        BeginMove();

        GameState currentState = GameManager.Inst.currentState;

        TLPlayer player = currentState.GetPlayer();
        Vector2Int curPos = currentState.GetPosOf(player);
        TLShears shears = currentState.GetTLOfTypeAtPos<TLShears>(curPos + startingDir);

        Vector2Int cornerSpot = curPos + endingDir + startingDir;
        Vector2Int goalPos = curPos + endingDir;

        Debug.Log("CORNER: " + cornerSpot);
        Debug.Log("GOAL: " + goalPos);

        if (currentState.GetTLOfTypeAtPos<TLWall>(cornerSpot) != null || currentState.GetTLOfTypeAtPos<TLShears>(cornerSpot) != null || currentState.GetTLOfTypeAtPos<TLDoor>(cornerSpot) != null)
            return;
        if (currentState.GetTLOfTypeAtPos<TLWall>(goalPos) != null || currentState.GetTLOfTypeAtPos<TLShears>(goalPos) != null || currentState.GetTLOfTypeAtPos<TLDoor>(goalPos) != null)
            return;

        currentState.Move(shears, curPos + endingDir);
        player.SetDirectionFacing(endingDir);

        EndMove();
    }

    private void MoveHoldingShear(Vector2Int moveDir)
    {
        Debug.Log("BEGIN MOVE");
        print(GameManager.Inst.currentState.ToString());

        BeginMove();

        GameState currentState = GameManager.Inst.currentState;

        TLPlayer player = currentState.GetPlayer();
        Vector2Int curPos = currentState.GetPosOf(player);
        TLShears shears = currentState.GetTLOfTypeAtPos<TLShears>(curPos + player.GetDirectionFacing());

        Vector2Int goalPosPlayer = curPos + moveDir;
        Vector2Int goalPosShears = shears.GetPosition() + moveDir;

        if (moveDir == goalPosPlayer - goalPosShears) // If moving away from shear / pulling shear
        {
            Debug.Log("PULL");
            bool somethingChanged = TypicalMove(moveDir);
            bool playerMoved = curPos != player.GetPosition();
            if (playerMoved)
                currentState.MoveRelative(shears, moveDir);
            if (somethingChanged)
                EndMove();
        }
        else
        {
            Debug.Log("PUSH");
            if (currentState.GetTLOfTypeAtPos<TLWall>(goalPosShears) != null || currentState.GetTLOfTypeAtPos<TLShears>(goalPosShears) != null || currentState.GetTLOfTypeAtPos<TLDoor>(goalPosShears) != null)
                return;

            currentState.MoveRelative(shears, moveDir);
            currentState.MoveRelative(player, moveDir);
            EndMove();
        }
    }

    // Returns if the plan group moved
    private bool MovePlantGroup(TLPlant[] plantGroup, Vector2Int moveDir)
    {
        GameState currentState = GameManager.Inst.currentState;

        foreach (var plant in plantGroup)
        {
            if (currentState.GetTLOfTypeAtPos<TLWall>(currentState.GetPosOf(plant) + moveDir) != null || currentState.GetTLOfTypeAtPos<TLDoor>(currentState.GetPosOf(plant) + moveDir) != null)
                return false;
        }

        foreach (var plant in plantGroup)
        {
            currentState.MoveRelative(plant, moveDir);
        }

        return true;
    }

    private bool GrowPlant(Vector2Int goalPos, Vector2Int moveDir)
    {
        GameState currentState = GameManager.Inst.currentState;

        Vector2Int desiredPlantGrowth = goalPos + moveDir;

        // If we are pushing into a dead plant, stop
        if (currentState.GetTLOfTypeAtPos<TLPlant>(goalPos) != null && !currentState.GetTLOfTypeAtPos<TLPlant>(goalPos).IsAlive())
            return false;

        while (currentState.GetTLOfTypeAtPos<TLPlant>(desiredPlantGrowth) != null && currentState.GetTLOfTypeAtPos<TLPlant>(desiredPlantGrowth).IsAlive())
        {
            desiredPlantGrowth += moveDir;
        }
        if (currentState.GetTLOfTypeAtPos<TLWall>(desiredPlantGrowth) == null && currentState.GetTLOfTypeAtPos<TLDoor>(desiredPlantGrowth) == null && currentState.GetTLOfTypeAtPos<TLPlant>(desiredPlantGrowth) == null)
        {
            TLPlant plant = new TLPlant(desiredPlantGrowth, true);
            currentState.AddObject(plant);
            OnPlantGrow?.Invoke(new GrowAction(desiredPlantGrowth, moveDir, plant, currentState));
            return true;
        }
        else if (currentState.GetTLOfTypeAtPos<TLWall>(desiredPlantGrowth) == null && currentState.GetTLOfTypeAtPos<TLDoor>(desiredPlantGrowth) == null && !currentState.GetTLOfTypeAtPos<TLPlant>(desiredPlantGrowth).IsAlive())
        {
            TLPlant plant = currentState.GetTLOfTypeAtPos<TLPlant>(desiredPlantGrowth);
            plant.SetAlive(true);
            return true;
        }

        return false;
    }

    private void Undo(InputAction.CallbackContext obj)
    {
        OnUndoBegin?.Invoke();

        GameManager.Inst.currentState.Undo();
        print(GameManager.Inst.currentState.ToString());

        OnUndoEnd?.Invoke();
    }

    private void Reset(InputAction.CallbackContext obj)
    {
        //print("Begin Reset: " + stateList.Count);
        OnResetBegin?.Invoke();

        GameManager.Inst.currentState.Reset();
        print(GameManager.Inst.currentState.ToString());

        OnResetEnd?.Invoke();
        //print("End Reset: " + stateList.Count);
    }

    private void BeginMove()
    {
        OnMoveBegin?.Invoke();
    }

    public void EndMove()
    {
        GameState currentState = GameManager.Inst.currentState;

        if (currentState.GetTLOfTypeAtPos<TLDoor>(currentState.GetPlayer().GetPosition()) != null)
        {
            GameManager.Inst.CompleteLevel(SceneManager.GetActiveScene().name);
        }

        OnMoveEnd?.Invoke();
        currentState.EndMove();

        /*int potNum = 0;
        foreach (var pot in currentState.GetAllTLPots())
        {
            potNum += pot.IsFull();
        }
        print(potNum);*/
        Debug.Log("END");
        print(GameManager.Inst.currentState.ToString());
        //print("End Move: " + GameManager.Inst.stateList.Count);
    }


    private void DebugFinishLevel(InputAction.CallbackContext obj)
    {
        GameManager.Inst.CompleteLevel(SceneManager.GetActiveScene().name);
    }
}
