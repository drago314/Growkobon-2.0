using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField] public InputActionReference moveUp, moveDown, moveRight, moveLeft, undo, reset;
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject plantPrefab;
    [SerializeField] public GameObject doorPrefab;
    [SerializeField] public GameObject potPrefab;
    [SerializeField] public string[] wallNames;
    [SerializeField] public string[] doorNames;

    public static GameManager Inst;

    public GameState initialGameState;
    public GameState currentState;
    public List<GameState> stateList;

    private void Awake()
    {
        Inst = this; // TODO fix
    }

    private void Start()
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
                    if (tileMap.GetTile(tilePos) != null)
                        print(tileMap.GetTile(tilePos).name);

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
                }
            }
        }

        var TLAnimators = FindObjectsByType<TLAnimator>(FindObjectsSortMode.None);

        /*
        foreach (var anim in TLAnimators)
        {
            print(anim.GetType().ToString() + ": " + anim.transform.position.x + " " + anim.transform.position.y);
        }
        */

        foreach (var TLanim in TLAnimators)
        {
            Vector2Int pos = new Vector2Int((int)TLanim.gameObject.transform.position.x, (int)TLanim.gameObject.transform.position.y);
            if (TLanim is PlayerAnimator)
                TlObjectList.Add(new TLPlayer(pos));
            if (TLanim is PlantAnimator)
                TlObjectList.Add(new TLPlant(pos));
            if (TLanim is PotAnimator)
                TlObjectList.Add(new TLPot(pos));
        }

        initialGameState = new GameState(TlObjectList);
        stateList.Add(initialGameState);
        currentState = new GameState(initialGameState);
    }       

    public void GenerateState(GameState state)
    {
        var TLAnimators = FindObjectsByType<TLAnimator>(FindObjectsSortMode.None);
        foreach (var TLanim in TLAnimators)
        {
            if (TLanim is not DoorAnimator && TLanim is not PotAnimator)
                Destroy(TLanim.gameObject);
        }

        foreach (var TLObj in state.GetAllTLObjects())
        {
            if (TLObj is TLPlayer)
                Instantiate(playerPrefab, new Vector3(TLObj.curPos.x, TLObj.curPos.y, 0), Quaternion.identity);
            if (TLObj is TLPlant)
                Instantiate(plantPrefab, new Vector3(TLObj.curPos.x, TLObj.curPos.y, 0), Quaternion.identity);
        }
    }

    public void GenerateCurrentState()
    {
        if (!currentState.Equals(stateList[stateList.Count - 1]))
        {
            GenerateState(currentState);
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
            GenerateState(lastState);
            stateList.RemoveAt(stateList.Count - 1);
            currentState = new GameState(lastState);
        }
        //print("End Undo: " + stateList.Count);
    }

    public void Reset(InputAction.CallbackContext obj)
    {
        //print("Begin Reset: " + stateList.Count);
        if (!currentState.Equals(initialGameState))
        {
            GenerateState(initialGameState);
            stateList.Add(initialGameState);
            currentState = new GameState(initialGameState);
        }
        //print("End Reset: " + stateList.Count);
    }

    public void DEBUG(string message)
    {
        print(message);
    }
}
