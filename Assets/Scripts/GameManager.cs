using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] public PlayerInput inputManager;
    [SerializeField] public string[] wallNames;
    [SerializeField] public string[] potNames;
    [SerializeField] public string[] pathNames;
    [SerializeField] public GeneralAnimator animator;

    public Dictionary<string, bool> levelsCompleted;
    public string currentLevel;

    public static GameManager Inst;
    public MovementManager movementManager;
    public MapManager mapManager;

    public event Action OnLevelEnter;
    public event Action OnMapEnter;

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
        movementManager = GetComponent<MovementManager>();
        mapManager = GetComponent<MapManager>();
        levelsCompleted = new Dictionary<string, bool>();

        if (SceneManager.GetActiveScene().name.Contains("Map"))
            OpenMap();
        else
            OpenLevel(SceneManager.GetActiveScene().name);
    }       

    private void SetMovementManagerFromScene()
    {
        movementManager.stateList = new List<GameState>();

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

        
        /*foreach (var anim in TLSignatures)
        {
            print(anim.gameObject.name + ": " + anim.transform.position.x + " " + anim.transform.position.y);
        }*/
        

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

        movementManager.initialGameState = new GameState(TlObjectList);
        movementManager.stateList.Add(movementManager.initialGameState);
        movementManager.currentState = new GameState(movementManager.initialGameState);
    }

    private void SetMapManagerFromScene()
    {
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

                    foreach (var pathName in pathNames)
                    {
                        if (tileMap.GetTile(tilePos) != null && pathName.Equals(tileMap.GetTile(tilePos).name))
                        {
                            Vector3 pos = tileMap.CellToLocal(tilePos);
                            TlObjectList.Add(new TLPath(new Vector2Int((int)pos.x, (int)pos.y), true));
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
            else if (TLSig is PlantSignature)
                TlObjectList.Add(new TLPlant(pos));
            else if (TLSig is DoorSignature)
                TlObjectList.Add(new TLDoor(pos));
            else if (TLSig is LevelSignature)
                TlObjectList.Add(new TLLevel(pos, ((LevelSignature)TLSig).levelName));
        }

        mapManager.currentState = new GameState(TlObjectList);
    }

    public void OpenLevel(string level)
    {
        inputManager.SwitchCurrentActionMap("Gameplay");

        currentLevel = level;
        if (!levelsCompleted.ContainsKey(level))
            levelsCompleted.Add(level, false);
        StartCoroutine(WaitTilLevelLoad(level));
    }

    private IEnumerator WaitTilLevelLoad(string level)
    {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        SetMovementManagerFromScene();
        OnLevelEnter?.Invoke();
    }

    public void OpenMap()
    {
        inputManager.SwitchCurrentActionMap("World Map");
        StartCoroutine(WaitTilMapLoad("World 1 Map"));
    }

    public void FinishLevel()
    {
        levelsCompleted[currentLevel] = true;
        OpenMap();
        //map.complete level
    }

    private IEnumerator WaitTilMapLoad(string map)
    {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(map, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        SetMapManagerFromScene();
        OnMapEnter?.Invoke();
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void DEBUG(string message)
    {
        print(message);
    }
}
