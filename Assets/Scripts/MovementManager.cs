using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System;

public class MovementManager : MonoBehaviour
{
    [SerializeField] public string[] wallNames;
    [SerializeField] public string[] potNames;

    [SerializeField] public InputActionReference moveUp, moveDown, moveRight, moveLeft, undo, reset, finishLevel;

    public event Action OnLevelEnter;
    public event Action OnMoveBegin;
    public event Action OnMoveEnd;
    public event Action<MoveAction> OnPlayerMove;
    public event Action<MoveAction> OnPlantMove;
    public event Action<GrowAction> OnPlantGrow;
    public event Action<Vector2Int> OnDoorClose;
    public event Action<Vector2Int> OnDoorOpen;
    public event Action OnUndoEnd;
    public event Action OnResetEnd;

    public GameState initialGameState;
    public GameState currentState;
    public List<GameState> stateList;

    private void Start()
    {
        moveUp.action.performed += MoveUp;
        moveDown.action.performed += MoveDown;
        moveRight.action.performed += MoveRight;
        moveLeft.action.performed += MoveLeft;
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
        undo.action.performed -= Undo;
        reset.action.performed -= Reset;
        finishLevel.action.performed -= DebugFinishLevel;
    }

    public void LoadLevel()
    {
        if (SceneManager.GetActiveScene().name.Contains("Level"))
        { 
            stateList = new List<GameState>();

            var TlObjectList = new List<TLObject>();

            var tileMaps = FindObjectsOfType<Tilemap>();
            foreach (var tileMap in tileMaps)
            {
                if (tileMap.gameObject.name != "Background Tilemap")
                {
                    BoundsInt bounds = tileMap.cellBounds;
                    foreach (Vector3Int tilePos in tileMap.cellBounds.allPositionsWithin)
                    {
                        //if (tileMap.GetTile(tilePos) != null)
                        //    print(tileMap.GetTile(tilePos).name);

                        foreach (var wallName in wallNames)
                        {
                            if (tileMap.GetTile(tilePos) != null && wallName.Equals(tileMap.GetTile(tilePos).name))
                            {
                                Vector3 pos = tileMap.CellToLocal(tilePos);
                                TlObjectList.Add(new TLWall(new Vector2Int((int)pos.x, (int)pos.y)));
                            }
                        }
                        for (int i = 0; i < potNames.Length; i++)
                        {
                            if (tileMap.GetTile(tilePos) != null && potNames[i].Equals(tileMap.GetTile(tilePos).name))
                            {
                                Vector3 pos = tileMap.CellToLocal(tilePos);
                                TlObjectList.Add(new TLPot(new Vector2Int((int)pos.x, (int)pos.y), i + 1));
                            }
                        }
                    }
                }
            }

            var TLSignatures = FindObjectsByType<TLSignature>(FindObjectsSortMode.None);


            /*foreach (var anim in TLSignatures)
            {
                print(anim.gameObject.name + ": " + anim.transform.position.x + " " + anim.transform.position.y);
            }*/


            foreach (var TLSig in TLSignatures)
            {
                Vector2Int pos = new Vector2Int((int)TLSig.gameObject.transform.position.x, (int)TLSig.gameObject.transform.position.y);
                if (TLSig is PlayerSignature)
                    TlObjectList.Add(new TLPlayer(pos));
                else if (TLSig is PlantSignature)
                    TlObjectList.Add(new TLPlant(pos));
                else if (TLSig is DoorSignature)
                    TlObjectList.Add(new TLDoor(pos, (DoorSignature)TLSig));
            }

            initialGameState = new GameState(TlObjectList);
            stateList.Add(initialGameState);
            currentState = new GameState(initialGameState);

            OnLevelEnter?.Invoke();
        }
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
        //print(currentState.ToString());

        OnMoveBegin?.Invoke();

        TLPlayer player = currentState.GetPlayer();
        Vector2Int curPos = currentState.GetPosOf(player);
        Vector2Int goalPos = curPos + moveDir;

        player.directionFacing = moveDir;
        Dictionary<TLDoor, bool> originalDoorStates = new Dictionary<TLDoor, bool>();
        foreach (var door in currentState.GetAllTLDoors())
        {
            originalDoorStates.Add(door, door.IsOpen());
        }


        // 2
        if (currentState.GetWallAtPos(goalPos) != null)
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
        //print("goal pos: " + goalPos);
        if (currentState.GetPlantAtPos(goalPos) != null)
        {
            TLPlant[] plantGroup = currentState.GetPlantGroupAtPos(goalPos);
            bool canMove = true;
            foreach (var plant in plantGroup)
            {
                if (currentState.GetWallAtPos(currentState.GetPosOf(plant) + moveDir) != null || currentState.GetDoorAtPos(currentState.GetPosOf(plant) + moveDir) != null)
                {
                    canMove = false;
                    break;
                }
            }
            if (canMove)
            {
                foreach (var plant in plantGroup)
                {
                    OnPlantMove?.Invoke(new MoveAction(plant.curPos, plant.curPos + moveDir, moveDir, plant, currentState));
                    currentState.MoveRelative(plant, moveDir);
                }
                OnPlayerMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, player, currentState));
                currentState.MoveRelative(player, moveDir);
            }
            else
            {
                OnPlayerMove?.Invoke(new MoveAction(curPos, curPos, moveDir, player, currentState));
            }
            GrowPlant(goalPos, moveDir);
        }
        else
        {
            OnPlayerMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, player, currentState));
            currentState.MoveRelative(player, moveDir);
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
        EndMove();

        if (currentState.GetDoorAtPos(currentState.GetPosOf(player)) != null)
        {
<<<<<<< Updated upstream
            GameManager.Inst.FinishLevel(SceneManager.GetActiveScene().name + " " + currentState.GetDoorAtPos(currentState.GetPosOf(player)).doorName);
=======
            GameManager.Inst.FinishLevel(currentState.GetDoorAtPos(currentState.GetPosOf(player)));
>>>>>>> Stashed changes
        }

        /*int potNum = 0;
        foreach (var pot in currentState.GetAllTLPots())
        {
            potNum += pot.IsFull();
        }
        print(potNum);*/
        //print(currentState.ToString());
        //print("End Move: " + GameManager.Inst.stateList.Count);
    }

    private void GrowPlant(Vector2Int goalPos, Vector2Int moveDir)
    {
        Vector2Int desiredPlantGrowth = goalPos + moveDir;
        while (currentState.GetPlantAtPos(desiredPlantGrowth) != null)
        {
            desiredPlantGrowth += moveDir;
        }
        if (currentState.GetWallAtPos(desiredPlantGrowth) == null && currentState.GetDoorAtPos(desiredPlantGrowth) == null)
        {
            TLPlant plant = new TLPlant(desiredPlantGrowth);
            OnPlantGrow?.Invoke(new GrowAction(desiredPlantGrowth, moveDir, plant, currentState));
            currentState.AddObject(plant);
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

    public void EndMove()
    {
        if (!currentState.Equals(stateList[stateList.Count - 1]))
        {
            stateList.Add(currentState);
            currentState = new GameState(currentState);
        }
    }


    private void DebugFinishLevel(InputAction.CallbackContext obj)
    {
<<<<<<< Updated upstream
        GameManager.Inst.FinishLevel(SceneManager.GetActiveScene().name + " " + currentState.GetAllTLDoors()[0].doorName);
=======
        GameManager.Inst.FinishLevel(currentState.GetAllTLDoors()[0]);
>>>>>>> Stashed changes
    }
}
