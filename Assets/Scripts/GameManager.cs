using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField] public InputActionReference moveUp, moveDown, moveRight, moveLeft, undo, reset;
    [SerializeField] public string[] wallNames;
    [SerializeField] public string[] doorNames;
    [SerializeField] public string[] potNames;
    [SerializeField] public GeneralAnimator animator;

    public static GameManager Inst;

    public event System.Action OnUndoEnd;
    public event System.Action OnResetEnd;

    public GameState initialGameState;
    public GameState currentState;
    public List<GameState> stateList;

    private void Awake()
    {
        if (Inst == null)
        {
            DontDestroyOnLoad(gameObject);
            Inst = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        stateList = new List<GameState>();
        undo.action.performed += Undo;
        reset.action.performed += Reset;

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
                    foreach (var doorName in doorNames)
                    {
                        if (tileMap.GetTile(tilePos) != null && doorName.Equals(tileMap.GetTile(tilePos).name))
                        {
                            Vector3 pos = tileMap.CellToLocal(tilePos);
                            TlObjectList.Add(new TLDoor(new Vector2Int((int)pos.x, (int)pos.y)));
                        }
                    }
                    foreach (var potName in potNames)
                    {
                        if (tileMap.GetTile(tilePos) != null && potName.Equals(tileMap.GetTile(tilePos).name))
                        {
                            Vector3 pos = tileMap.CellToLocal(tilePos);
                            TlObjectList.Add(new TLPot(new Vector2Int((int)pos.x, (int)pos.y)));
                        }
                    }
                }
            }
        }

        var TLSignatures = FindObjectsByType<TLSignature>(FindObjectsSortMode.None);

        /*
        foreach (var anim in TLAnimators)
        {
            print(anim.GetType().ToString() + ": " + anim.transform.position.x + " " + anim.transform.position.y);
        }
        */

        foreach (var TLSig in TLSignatures)
        {
            Vector2Int pos = new Vector2Int((int)TLSig.gameObject.transform.position.x, (int)TLSig.gameObject.transform.position.y);
            if (TLSig is PlayerSignature)
                TlObjectList.Add(new TLPlayer(pos));
            if (TLSig is PlantSignature)
                TlObjectList.Add(new TLPlant(pos));
            if (TLSig is DoorSignature)
                TlObjectList.Add(new TLDoor(pos));
        }

        initialGameState = new GameState(TlObjectList);
        stateList.Add(initialGameState);
        currentState = new GameState(initialGameState);
    }       

    public void GenerateState(GameState state) 
    {
        // TODO Add MoveableTLObject class
        var TLSignatures = FindObjectsByType<TLSignature>(FindObjectsSortMode.None);
        foreach (var TLSig in TLSignatures)
        {
            if (TLSig is MoveableObjectSignature)
                Destroy(TLSig.gameObject);
        }

        foreach (var TLObj in state.GetAllTLObjects())
        {
            if (TLObj is TLPlayer)
                animator.InstantiatePlayer((TLPlayer)TLObj);
            if (TLObj is TLPlant)
                animator.InstantiatePlant((TLPlant)TLObj);
        }
    }

    public void GenerateCurrentState()
    {
        if (!currentState.Equals(stateList[stateList.Count - 1]))
        {
            stateList.Add(currentState);
            currentState = new GameState(currentState);
            GenerateState(currentState);
        }
    }

    public void EndMove()
    {
        if (!currentState.Equals(stateList[stateList.Count - 1]))
        {
            stateList.Add(currentState);
            currentState = new GameState(currentState);
        }
    }

    public void Undo(InputAction.CallbackContext obj)
    {
        //print("Begin Undo: " + stateList.Count);
        if (stateList.Count >= 2)
        {
            var lastState = stateList[stateList.Count - 2];
            stateList.RemoveAt(stateList.Count - 1);
            currentState = new GameState(lastState);
            GenerateState(currentState);
        }

        OnUndoEnd?.Invoke();
        //print("End Undo: " + stateList.Count);
    }

    public void Reset(InputAction.CallbackContext obj)
    {
        //print("Begin Reset: " + stateList.Count);
        if (!currentState.Equals(initialGameState))
        {
            stateList.Add(initialGameState);
            currentState = new GameState(initialGameState);
            GenerateState(currentState);
        }

        OnResetEnd?.Invoke();
        //print("End Reset: " + stateList.Count);
    }

    public void DEBUG(string message)
    {
        print(message);
    }
}
