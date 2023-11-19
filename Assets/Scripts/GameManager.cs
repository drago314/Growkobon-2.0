using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] public string[] wallNames;
    [SerializeField] public string[] potNames;
    [SerializeField] public string[] pathNames;

    public Dictionary<string, bool> levelsCompleted;
    public string currentLevel;
    public string currentWorld;

    public static GameManager Inst;
    public MovementManager movementManager;
    public MapManager mapManager;
    public GeneralAnimator animator;
    public PlayerInput inputManager;


    public event Action OnLevelEnter;
    public event Action OnMapEnter;
    public event Action OnMapLoad;

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
        animator = GetComponent<GeneralAnimator>();
        inputManager = GetComponent<PlayerInput>();

        levelsCompleted = new Dictionary<string, bool>();

        if (SceneManager.GetActiveScene().name.Contains("Map"))
        {
            currentWorld = SceneManager.GetActiveScene().name;
            OpenNewMap(currentWorld);
        }
        else
        {
            currentLevel = SceneManager.GetActiveScene().name;
            OpenLevel(currentLevel);
        }
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
            if (TLSig is PlantSignature)
                TlObjectList.Add(new TLPlant(pos));
            if (TLSig is DoorSignature)
                TlObjectList.Add(new TLDoor(pos, (DoorSignature) TLSig));
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
                            TlObjectList.Add(new TLPath(new Vector2Int((int)pos.x, (int)pos.y), false));
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
            else if (TLSig is LevelSignature)
                TlObjectList.Add(new TLLevel(pos, (LevelSignature)TLSig));
            else if (TLSig is WorldSignature)
                TlObjectList.Add(new TLWorldExit(pos, (WorldSignature)TLSig));
        }

        mapManager.currentState = new GameState(TlObjectList);

        mapManager.exitToPathsUnlocked = new Dictionary<string, List<Vector2Int>>();
        foreach (var lvl in mapManager.currentState.GetAllTLLevels())
        {
            foreach (var pair in lvl.exitToPathsUnlocked)
            {
                Debug.Log(pair.Key);
                mapManager.exitToPathsUnlocked.Add(pair.Key, pair.Value);
                if (levelsCompleted.ContainsKey(lvl.levelName) && levelsCompleted[lvl.levelName] == true)
                {
                    foreach (var path in pair.Value)
                    {
                        if (mapManager.currentState.GetPathAtPos(path) != null)
                            mapManager.currentState.GetPathAtPos(path).unlocked = true;
                    }
                }
            }
        }
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

    public void OpenCurrentMap()
    {
        inputManager.SwitchCurrentActionMap("World Map");
        StartCoroutine(OpenCurrentMapCoroutine());
    }

    private IEnumerator OpenCurrentMapCoroutine()
    {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(currentWorld, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        OnMapEnter?.Invoke();
    }

    public void OpenNewMap(string mapName)
    {
        inputManager.SwitchCurrentActionMap("World Map");
        currentWorld = mapName;
        StartCoroutine(OpenNewMapCoroutine(mapName));
    }

    private IEnumerator OpenNewMapCoroutine(string map)
    {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(map, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        SetMapManagerFromScene();
        OnMapEnter?.Invoke();
    }

    public void FinishLevel(string levelExit)
    {
        levelsCompleted[levelExit] = true;
        inputManager.SwitchCurrentActionMap("World Map");
        StartCoroutine(FinishLevelCoroutine(levelExit));
    }

    private IEnumerator FinishLevelCoroutine(string levelExit)
    {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(currentWorld, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        OnMapLoad?.Invoke();
        mapManager.CompleteLevel(levelExit);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DEBUG(string message)
    {
        print(message);
    }
}
