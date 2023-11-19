using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] public string[] wallNames;
    [SerializeField] public string[] potNames;
    [SerializeField] public string[] pathNames;

    public SerializableDictionary<string, bool> levelsCompleted;
    public string currentLevel;
    public string currentWorld;

    public static GameManager Inst;
    public MovementManager movementManager;
    public MapManager mapManager;
    public GeneralAnimator animator;
    public PlayerInput inputManager;


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
        animator = GetComponent<GeneralAnimator>();
        inputManager = GetComponent<PlayerInput>();

        levelsCompleted = new SerializableDictionary<string, bool>();

        if (SceneManager.GetActiveScene().name.Contains("Title"))
        {
            inputManager.SwitchCurrentActionMap("Pause Menu");
        }
        else if (SceneManager.GetActiveScene().name.Contains("Map"))
        {
           OpenMap(SceneManager.GetActiveScene().name);
        }
        else
        {
            currentWorld = SceneManager.GetActiveScene().name.Substring(0, 7) + " Map"; 
            OpenLevel(SceneManager.GetActiveScene().name);
        }
    }       

    public void LoadData(GameData data)
    {
        currentWorld = data.currentWorld;

        levelsCompleted.Clear();
        foreach (var pair in data.levelsCompleted)
        {
            levelsCompleted.Add(pair.Key, pair.Value);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.currentWorld = currentWorld;

        data.levelsCompleted.Clear();
        foreach (var pair in levelsCompleted)
        {
            data.levelsCompleted.Add(pair.Key, pair.Value);
        }
    }

    public bool IsLevelComplete(string levelName)
    {
        bool completed = false;
        foreach (string key in levelsCompleted.Keys)
        {
            if (key.Contains(levelName))
                completed = true;
        }
        return completed;
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

        Debug.Log("Level Set");
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
                mapManager.exitToPathsUnlocked.Add(pair.Key, pair.Value);
                if (IsLevelComplete(lvl.levelName))
                {
                    foreach (var path in pair.Value)
                    {
                        if (mapManager.currentState.GetPathAtPos(path) != null)
                            mapManager.currentState.GetPathAtPos(path).unlocked = true;
                        if (mapManager.currentState.GetLevelAtPos(path) != null)
                            mapManager.currentState.GetLevelAtPos(path).unlocked = true;
                    }
                }
            }
        }

        Debug.Log("Map Set");
    }

    public void OpenLevel(string level)
    {
        StartCoroutine(OpenLevelAsync(level));
    }
    private IEnumerator OpenLevelAsync(string level)
    {
        Debug.Log("Open Level: " + level);

        currentLevel = level;
        var asyncLoadLevel = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        SetMovementManagerFromScene();
        inputManager.SwitchCurrentActionMap("Gameplay");
        OnLevelEnter?.Invoke();
    }

    public void FinishLevel(string levelExit)
    {
        StartCoroutine(FinishLevelAsync(levelExit));
    }
    private IEnumerator FinishLevelAsync(string levelExit)
    {
        Debug.Log("Finish Level: " + levelExit);

        var asyncLoadLevel = SceneManager.LoadSceneAsync(currentWorld, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        SetMapManagerFromScene();
        foreach (var level in mapManager.currentState.GetAllTLLevels())
        {
            if (level.levelName == currentLevel)
                mapManager.currentState.Move(mapManager.currentState.GetPlayer(), level.curPos);
        }
        if (levelsCompleted.ContainsKey(levelExit))
            levelsCompleted[levelExit] = true;
        else
            levelsCompleted.Add(levelExit, true);
        inputManager.SwitchCurrentActionMap("World Map");
        OnMapEnter?.Invoke();
        mapManager.CompleteLevel(levelExit);
    }

    public void OpenMap(string mapName, string levelName)
    {
        StartCoroutine(OpenMapAsync(mapName, levelName));
    }
    private IEnumerator OpenMapAsync(string mapName, string levelName)
    {
        Debug.Log("Open Map: " + mapName + ", " + levelName);

        currentWorld = mapName;
        var asyncLoadLevel = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        SetMapManagerFromScene();
        foreach (var level in mapManager.currentState.GetAllTLLevels())
        {
            if (level.levelName == levelName)
                mapManager.currentState.Move(mapManager.currentState.GetPlayer(), level.curPos);
        }
        inputManager.SwitchCurrentActionMap("World Map");
        OnMapEnter?.Invoke();
    }

    public void OpenMap(string mapName)
    {
        StartCoroutine(OpenMapAsync(mapName));
    }
    private IEnumerator OpenMapAsync(string mapName)
    {
        Debug.Log("Open Map: " + mapName);

        currentWorld = mapName;

        var asyncLoadLevel = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);

        SetMapManagerFromScene();
        inputManager.SwitchCurrentActionMap("World Map");
        OnMapEnter?.Invoke();
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
