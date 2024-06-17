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
    public event Action<MoveAction> OnPlayerMove;
    public event Action<MoveAction> OnShearsMove;
    public event Action<MoveAction> OnPlantMove;
    public event Action<GrabAction> OnObjectGrabbed;
    public event Action<GrowAction> OnPlantGrow;
    public event Action<Vector2Int> OnDoorClose;
    public event Action<Vector2Int> OnDoorOpen;
    public event Action OnUndoEnd;
    public event Action OnResetEnd;

    public GameState initialGameState;
    public GameState currentState;
    public List<GameState> stateList;
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
        if (currentState.GetPlayer().objectHeld)
            if (currentState.GetPlayer().directionFacing == Vector2Int.left || currentState.GetPlayer().directionFacing == Vector2Int.right)
                TurnHoldingShear(currentState.GetPlayer().directionFacing, Vector2Int.up);
            else
                MoveHoldingShear(Vector2Int.up);
        else
            Move(Vector2Int.up);
    }

    private void MoveDown(InputAction.CallbackContext obj)
    {
        if (currentState.GetPlayer().objectHeld)
            if (currentState.GetPlayer().directionFacing == Vector2Int.left || currentState.GetPlayer().directionFacing == Vector2Int.right)
                TurnHoldingShear(currentState.GetPlayer().directionFacing, Vector2Int.down);
            else
                MoveHoldingShear(Vector2Int.down);
        else
            Move(Vector2Int.down);
    }
    private void MoveRight(InputAction.CallbackContext obj)
    {
        if (currentState.GetPlayer().objectHeld)
            if (currentState.GetPlayer().directionFacing == Vector2Int.up || currentState.GetPlayer().directionFacing == Vector2Int.down)
                TurnHoldingShear(currentState.GetPlayer().directionFacing, Vector2Int.right);
            else
                MoveHoldingShear(Vector2Int.right);
        else
            Move(Vector2Int.right);
    }
    private void MoveLeft(InputAction.CallbackContext obj)
    {
        if (currentState.GetPlayer().objectHeld)
            if (currentState.GetPlayer().directionFacing == Vector2Int.up || currentState.GetPlayer().directionFacing == Vector2Int.down)
                TurnHoldingShear(currentState.GetPlayer().directionFacing, Vector2Int.left);
            else
                MoveHoldingShear(Vector2Int.left);
        else
            Move(Vector2Int.left);
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        BeginMove();

        TLPlayer player = currentState.GetPlayer();
        Vector2Int curPos = currentState.GetPosOf(player);
        Vector2Int grabDirection = Vector2Int.zero;

        Debug.Log("Shears Direction: " + currentState.GetShearsAtPos(curPos + Vector2Int.up).directionFacing);
        

        if (player.objectHeld)
        {
            //player.objectHeld.SetHeld(false);
            //player.objectHeld = null;
            player.objectHeld = false;
            currentState.GetShearsAtPos(curPos + player.directionFacing).SetHeld(false);
        }
        else if (currentState.GetShearsAtPos(curPos + Vector2Int.up) != null && currentState.GetShearsAtPos(curPos + Vector2Int.up).directionFacing != Vector2Int.down)
            grabDirection = Vector2Int.up;
        else if (currentState.GetShearsAtPos(curPos + Vector2Int.down) != null && currentState.GetShearsAtPos(curPos + Vector2Int.down).directionFacing != Vector2Int.up)
            grabDirection = Vector2Int.down;
        else if (currentState.GetShearsAtPos(curPos + Vector2Int.left) != null && currentState.GetShearsAtPos(curPos + Vector2Int.left).directionFacing != Vector2Int.right)
            grabDirection = Vector2Int.left;
        else if (currentState.GetShearsAtPos(curPos + Vector2Int.right) != null && currentState.GetShearsAtPos(curPos + Vector2Int.right).directionFacing != Vector2Int.left)
            grabDirection = Vector2Int.right;
        else
            return;

        Debug.Log("Grab Direction: " + grabDirection);

        // If one of the checks passed to pick up an object
        if (grabDirection != Vector2Int.zero)
        {
            //player.objectHeld = currentState.GetShearsAtPos(curPos + grabDirection);
            player.objectHeld = true;
            currentState.GetShearsAtPos(curPos + grabDirection).SetHeld(true);
            player.directionFacing = grabDirection;
            OnPlayerMove?.Invoke(new MoveAction(curPos, curPos, grabDirection, player, currentState));
            OnObjectGrabbed?.Invoke(new GrabAction(grabDirection, player, currentState.GetShearsAtPos(curPos + grabDirection), currentState));
            Debug.Log("Player Holding: " + player.objectHeld);
        }

        EndMove();
    }

    private void Move(Vector2Int moveDir)
    {
        BeginMove();

        TLPlayer player = currentState.GetPlayer();
        Vector2Int curPos = currentState.GetPosOf(player);
        Vector2Int goalPos = curPos + moveDir;

        player.directionFacing = moveDir;

        // Check if the player is walking into a wall, door, or other object it cannot walk though
        if (currentState.GetWallAtPos(goalPos) != null || currentState.GetShearsAtPos(goalPos) != null)
        {
            OnPlayerMove?.Invoke(new MoveAction(curPos, curPos, moveDir, player, currentState));
            return;
        }
        if (currentState.GetDoorAtPos(goalPos) != null && !currentState.GetDoorAtPos(goalPos).IsOpen())
        {
            OnPlayerMove?.Invoke(new MoveAction(curPos, curPos, moveDir, player, currentState));
            return;
        }


        //6 
        if (currentState.GetPlantAtPos(goalPos) != null)
        {
            TLPlant[] plantGroup = currentState.GetPlantGroupAtPos(goalPos);
            bool canMove = MovePlantGroup(plantGroup, moveDir);

            if (canMove)
            {
                Debug.Log("1");
                currentState.MoveRelative(player, moveDir);
                OnPlayerMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, player, currentState));
                GrowPlant(goalPos + moveDir, moveDir);
            }
            else
            {
                OnPlayerMove?.Invoke(new MoveAction(curPos, curPos, moveDir, player, currentState));
                GrowPlant(goalPos, moveDir);
            }
        }
        else
        {
            OnPlayerMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, player, currentState));
            Debug.Log("2");
            currentState.MoveRelative(player, moveDir);
        }

        EndMove();
    }

    // A turn from right to up would have a starting Dir of Vector2Int.right and a ending dir of Vector2Int.up
    private void TurnHoldingShear(Vector2Int startingDir, Vector2Int endingDir)
    {
        BeginMove();

        TLPlayer player = currentState.GetPlayer();
        Vector2Int curPos = currentState.GetPosOf(player);
        TLShears shears = currentState.GetShearsAtPos(curPos + startingDir);

        currentState.MoveRelative(shears, curPos + endingDir);
        player.directionFacing = endingDir;
        OnPlayerMove?.Invoke(new MoveAction(curPos, curPos, endingDir, player, currentState));
        OnShearsMove?.Invoke(new MoveAction(curPos + startingDir, curPos + endingDir, endingDir, shears, currentState));

        EndMove();
    }

    private void MoveHoldingShear(Vector2Int moveDir)
    {
        BeginMove();

        TLPlayer player = currentState.GetPlayer();
        Vector2Int curPos = currentState.GetPosOf(player);
        TLShears shears = currentState.GetShearsAtPos(curPos + player.directionFacing);

        Debug.Log("3");
        currentState.MoveRelative(shears, curPos + moveDir);
        OnPlayerMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, player, currentState));
        OnShearsMove?.Invoke(new MoveAction(shears.curPos, shears.curPos + moveDir, moveDir, shears, currentState));

        EndMove();
    }

    // Returns if the plan group moved
    private bool MovePlantGroup(TLPlant[] plantGroup, Vector2Int moveDir)
    {
        foreach (var plant in plantGroup)
        {
            if (currentState.GetWallAtPos(currentState.GetPosOf(plant) + moveDir) != null || currentState.GetDoorAtPos(currentState.GetPosOf(plant) + moveDir) != null)
                return false;
        }

        foreach (var plant in plantGroup)
        {
            OnPlantMove?.Invoke(new MoveAction(plant.curPos, plant.curPos + moveDir, moveDir, plant, currentState));
            Debug.Log("4");
            currentState.MoveRelative(plant, moveDir);
        }

        return true;
    }

    private void GrowPlant(Vector2Int goalPos, Vector2Int moveDir)
    {
        Vector2Int desiredPlantGrowth = goalPos + moveDir;

        // If we are pushing into a dead plant, stop
        if (currentState.GetPlantAtPos(goalPos) != null && currentState.GetPlantAtPos(goalPos).isDead)
            return;

        while (currentState.GetPlantAtPos(desiredPlantGrowth) != null && !currentState.GetPlantAtPos(desiredPlantGrowth).isDead)
        {
            desiredPlantGrowth += moveDir;
        }
        if (currentState.GetWallAtPos(desiredPlantGrowth) == null && currentState.GetDoorAtPos(desiredPlantGrowth) == null && currentState.GetPlantAtPos(desiredPlantGrowth) == null)
        {
            TLPlant plant = new TLPlant(desiredPlantGrowth, false);
            OnPlantGrow?.Invoke(new GrowAction(desiredPlantGrowth, moveDir, plant, currentState));
            currentState.AddObject(plant);
        }
        else if (currentState.GetWallAtPos(desiredPlantGrowth) == null && currentState.GetDoorAtPos(desiredPlantGrowth) == null && currentState.GetPlantAtPos(desiredPlantGrowth).isDead)
        {
            TLPlant plant = currentState.GetPlantAtPos(desiredPlantGrowth);
            plant.isDead = false;
            OnPlantGrow?.Invoke(new GrowAction(desiredPlantGrowth, moveDir, plant, currentState));
        }
    }

    private void Undo(InputAction.CallbackContext obj)
    {
        //print("Begin Undo: " + stateList.Count);
        if (stateList.Count >= 2)
        {
            var lastState = stateList[stateList.Count - 2];
            stateList.RemoveAt(stateList.Count - 1);
            currentState = new GameState(lastState);
        }

        OnUndoEnd?.Invoke();
        //print("End Undo: " + stateList.Count);
    }

    private void Reset(InputAction.CallbackContext obj)
    {
        //print("Begin Reset: " + stateList.Count);
        if (!currentState.Equals(initialGameState))
        {
            stateList.Add(initialGameState);
            currentState = new GameState(initialGameState);
        }

        OnResetEnd?.Invoke();
        //print("End Reset: " + stateList.Count);
    }

    private void BeginMove()
    {
        OnMoveBegin?.Invoke();
        originalDoorStates = new Dictionary<TLDoor, bool>();
        foreach (var door in currentState.GetAllTLDoors())
        {
            originalDoorStates.Add(door, door.IsOpen());
        }
    }

    public void EndMove()
    {
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

        OnMoveEnd?.Invoke();

        if (!currentState.Equals(stateList[stateList.Count - 1]))
        {
            stateList.Add(currentState);
            currentState = new GameState(currentState);
        }

        if (currentState.GetDoorAtPos(currentState.GetPlayer().curPos) != null)
        {
            GameManager.Inst.CompleteLevel(SceneManager.GetActiveScene().name);
        }

        /*int potNum = 0;
        foreach (var pot in currentState.GetAllTLPots())
        {
            potNum += pot.IsFull();
        }
        print(potNum);*/
        print(currentState.ToString());
        //print("End Move: " + GameManager.Inst.stateList.Count);
    }


    private void DebugFinishLevel(InputAction.CallbackContext obj)
    {
        GameManager.Inst.CompleteLevel(SceneManager.GetActiveScene().name);
    }
}
