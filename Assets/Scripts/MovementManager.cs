using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;

public class MovementManager : MonoBehaviour
{
    [SerializeField] public InputActionReference moveUp, moveDown, moveRight, moveLeft, interact, undo, reset, finishLevel;
    [SerializeField] private bool DebugCompleteOn;
    public event Action OnMoveBegin;
    public event Action OnMoveEnd;
    public event Action OnUndoBegin;
    public event Action OnUndoEnd;
    public event Action OnResetBegin;
    public event Action OnResetEnd;
    public event Action OnLevelCompleted;
    public event Action<GrowAction> OnPlantGrow;

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
        if (currentState.GetPlayer().IsObjectHeld() && currentState.GetPlayer().GetObjectHeld() is TLShears && !((TLShears)currentState.GetPlayer().GetObjectHeld()).IsPlantSkewered()
        && (currentState.GetPlayer().GetDirectionFacing() == Vector2Int.left || currentState.GetPlayer().GetDirectionFacing() == Vector2Int.right))
            TurnHoldingShear(currentState.GetPlayer().GetDirectionFacing() == Vector2Int.left, currentState.GetPlayer().GetDirectionFacing(), Vector2Int.up);
        else
            TypicalMove(Vector2Int.up);
    }

    private void MoveDown(InputAction.CallbackContext obj)
    {
        GameState currentState = GameManager.Inst.currentState;
        if (currentState.GetPlayer().IsObjectHeld() && currentState.GetPlayer().GetObjectHeld() is TLShears && !((TLShears)currentState.GetPlayer().GetObjectHeld()).IsPlantSkewered()
        && (currentState.GetPlayer().GetDirectionFacing() == Vector2Int.left || currentState.GetPlayer().GetDirectionFacing() == Vector2Int.right))
            TurnHoldingShear(currentState.GetPlayer().GetDirectionFacing() == Vector2Int.right, currentState.GetPlayer().GetDirectionFacing(), Vector2Int.down);
        else
            TypicalMove(Vector2Int.down);
    }
    private void MoveRight(InputAction.CallbackContext obj)
    {
        GameState currentState = GameManager.Inst.currentState;
        if (currentState.GetPlayer().IsObjectHeld() && currentState.GetPlayer().GetObjectHeld() is TLShears && !((TLShears)currentState.GetPlayer().GetObjectHeld()).IsPlantSkewered()
        && (currentState.GetPlayer().GetDirectionFacing() == Vector2Int.up || currentState.GetPlayer().GetDirectionFacing() == Vector2Int.down))
            TurnHoldingShear(currentState.GetPlayer().GetDirectionFacing() == Vector2Int.up, currentState.GetPlayer().GetDirectionFacing(), Vector2Int.right);
        else
            TypicalMove(Vector2Int.right);
    }
    private void MoveLeft(InputAction.CallbackContext obj)
    {
        GameState currentState = GameManager.Inst.currentState;
        if (currentState.GetPlayer().IsObjectHeld() && currentState.GetPlayer().GetObjectHeld() is TLShears && !((TLShears)currentState.GetPlayer().GetObjectHeld()).IsPlantSkewered()
        && (currentState.GetPlayer().GetDirectionFacing() == Vector2Int.up || currentState.GetPlayer().GetDirectionFacing() == Vector2Int.down))
            TurnHoldingShear(currentState.GetPlayer().GetDirectionFacing() == Vector2Int.down, currentState.GetPlayer().GetDirectionFacing(), Vector2Int.left);
        else
            TypicalMove(Vector2Int.left);
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        BeginMove();

        GameState currentState = GameManager.Inst.currentState;

        TLPlayer player = currentState.GetPlayer();
        Vector2Int curPos = currentState.GetPosOf(player);
        Vector2Int grabDirection = Vector2Int.zero;

        // Enter Level When in Map
        if (currentState.IsTLOfTypeAtPos<TLLevel>(curPos))
            GameManager.Inst.OpenLevel(currentState.GetTLOfTypeAtPos<TLLevel>(curPos).GetLevelName());
        
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
        {
            EndMove(false);
            return;
        }

        Debug.Log("Grab Direction: " + grabDirection);

        // If one of the checks passed to pick up an object
        if (grabDirection != Vector2Int.zero)
        {
            //player.objectHeld = currentState.GetShearsAtPos(curPos + grabDirection);
            player.PickupObject(currentState.GetTLOfTypeAtPos<TLShears>(curPos + grabDirection));
            Debug.Log("Player Holding: " + player.GetObjectHeld());
        }

        EndMove(true);
    }

    private void TypicalMove(Vector2Int moveDir)
    {
        GameState currentState = GameManager.Inst.currentState;

        Debug.Log("BEGIN");
        print(currentState.ToString());

        BeginMove();

        if (!currentState.GetPlayer().IsObjectHeld())
            GameManager.Inst.currentState.GetPlayer().SetDirectionFacing(moveDir);

        bool canMove = currentState.GetPlayer().CanMove(null, moveDir);
        Debug.Log("Can Move: " + canMove);

        bool attemptGrowPlant = false;
        Vector2Int addition = Vector2Int.zero;

        Vector2Int goal = currentState.GetPlayer().GetPosition() + moveDir;
        if (currentState.IsTLOfTypeAtPos<TLPlant>(goal))
            attemptGrowPlant = true;
        else if (currentState.IsTLOfTypeAtPos<TLShears>(goal) && currentState.IsTLOfTypeAtPos<TLPlant>(goal + moveDir))
        {
            attemptGrowPlant = true;
            addition = moveDir;
        }

        Debug.Log("Should Grow Plant: " + attemptGrowPlant);

        if (canMove)
            currentState.GetPlayer().Move(null, moveDir);

        bool plantGrown = false;
        if (attemptGrowPlant)
            plantGrown = GrowPlant(currentState.GetPlayer().GetPosition() + moveDir + addition, moveDir);

        EndMove(canMove || plantGrown);
    }

    // A turn from right to up would have a starting Dir of Vector2Int.right and a ending dir of Vector2Int.up
    private void TurnHoldingShear(bool clockwise, Vector2Int startingDir, Vector2Int endingDir)
    {
        Debug.Log("BEGIN TURN");
        print(GameManager.Inst.currentState.ToString());

        BeginMove();

        GameState currentState = GameManager.Inst.currentState;

        TLPlayer player = currentState.GetPlayer();

        bool somethingChanged = player.SpinHeldObject(clockwise, startingDir, endingDir);

        EndMove(somethingChanged);
    }


    public bool GrowPlant(Vector2Int goalPos, Vector2Int moveDir)
    {
        GameState currentState = GameManager.Inst.currentState;

        Vector2Int desiredPlantGrowth = goalPos + moveDir;

        // If we are pushing into a dead plant or there is no plant, stop
        if (!currentState.IsTLOfTypeAtPos<TLPlant>(goalPos) || !currentState.GetTLOfTypeAtPos<TLPlant>(goalPos).IsAlive())
            return false;

        // If this plant we think is a growth was actually "pushed" by a plant connected to it with shears then do not grow
        TLPlant[] plantGroup = currentState.GetPlantGroupAtPos(goalPos);
        TLPlayer player = currentState.GetPlayer();

        if (player.IsObjectHeld() && player.GetObjectHeld() is TLShears)
        {
            TLShears shearsHeld = (TLShears) player.GetObjectHeld();
            if (shearsHeld.IsPlantSkewered())
            {
                foreach (var plant in plantGroup)
                {
                    if (plant.IsSkewered() && shearsHeld.GetPlantSkewered() == plant)
                        return false;
                }
            }
        }

        while (currentState.IsTLOfTypeAtPos<TLPlant>(desiredPlantGrowth) && currentState.GetTLOfTypeAtPos<TLPlant>(desiredPlantGrowth).IsAlive())
        {
            desiredPlantGrowth += moveDir;
        }
        if (!currentState.IsTLOfTypeAtPos<TLWall>(desiredPlantGrowth) && !currentState.IsTLOfTypeAtPos<TLDoor>(desiredPlantGrowth) && !currentState.IsTLOfTypeAtPos<TLPlant>(desiredPlantGrowth))
        {
            if (currentState.IsTLOfTypeAtPos<TLShears>(desiredPlantGrowth) && currentState.GetTLOfTypeAtPos<TLShears>(desiredPlantGrowth).GetDirectionFacing() != -1 * moveDir)
                return GrowPlant(desiredPlantGrowth + moveDir, moveDir); // Grow through shears that aren't poking

            TLPlant plant = new TLPlant(desiredPlantGrowth, true);
            currentState.AddObject(plant);
            OnPlantGrow?.Invoke(new GrowAction(desiredPlantGrowth, moveDir, plant, currentState));

            if (currentState.IsTLOfTypeAtPos<TLShears>(desiredPlantGrowth))
                currentState.GetTLOfTypeAtPos<TLShears>(desiredPlantGrowth).SkewerPlant(plant);

            return true;
        }
        else if (!currentState.IsTLOfTypeAtPos<TLWall>(desiredPlantGrowth) && !currentState.IsTLOfTypeAtPos<TLDoor>(desiredPlantGrowth) && !currentState.GetTLOfTypeAtPos<TLPlant>(desiredPlantGrowth).IsAlive())
        {
            TLPlant plant = currentState.GetTLOfTypeAtPos<TLPlant>(desiredPlantGrowth);
            if (plant.IsSkewered())
                return false;
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

    public void EndMove(bool somethingChanged)
    {
        GameState currentState = GameManager.Inst.currentState;

        if (!somethingChanged)
        {
            currentState.EndMove(false);
            return;
        }

        if (currentState.GetTLOfTypeAtPos<TLDoor>(currentState.GetPlayer().GetPosition()) != null)
        {
            GameManager.Inst.CompleteLevel(SceneManager.GetActiveScene().name);
        }

        OnMoveEnd?.Invoke();
        currentState.EndMove(true);

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

    public void CompleteLevel(string levelName)
    {
        GameState currentState = GameManager.Inst.currentState;

        var TLLevels = currentState.GetAllOfTLType<TLLevel>();
        foreach (var level in TLLevels)
        {
            if (level.GetLevelName() == levelName)
            {
                level.SetCompleted(true);
                OnLevelCompleted?.Invoke();
                return;
            }
        }
    }

    private void DebugFinishLevel(InputAction.CallbackContext obj)
    {
        if (DebugCompleteOn && !GameManager.Inst.inMap)
            GameManager.Inst.CompleteLevel(SceneManager.GetActiveScene().name);
    }
}
