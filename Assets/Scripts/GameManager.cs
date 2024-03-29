using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public bool activated = false;

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
    public LevelTransitioner levelTransitioner;

    public event Action OnLevelEnter;
    public event Action OnMapEnter;

    private void Awake()
    {
        if (Inst == null)
        {
            DontDestroyOnLoad(gameObject);
            Inst = this;
            activated = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
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
        foreach (var level in data.levelsCompleted)
        {
            levelsCompleted.Add(level, true);
        }
    }

    public void SaveData(GameData data)
    {
        data.currentWorld = currentWorld;

        data.levelsCompleted.Clear();
        foreach (var pair in levelsCompleted)
        {
            if (pair.Value)
                data.levelsCompleted.Add(pair.Key);
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
                TlObjectList.Add(new TLWorldPortal(pos, (WorldSignature)TLSig));
        }

        mapManager.currentState = new GameState(TlObjectList);

        mapManager.exitToPathsUnlocked = new Dictionary<string, List<Vector2Int>>();
        foreach (var lvl in mapManager.currentState.GetAllTLLevels())
        {
            foreach (var pair in lvl.exitToPathsUnlocked)
            {
                mapManager.exitToPathsUnlocked.Add(pair.Key, pair.Value);
                if (levelsCompleted.ContainsKey(pair.Key) && levelsCompleted[pair.Key])
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
        foreach (var world in mapManager.currentState.GetAllTLWorlds())
        { 
            foreach (var path in world.pathsBeginningUnlocked)
            {
                if (mapManager.currentState.GetPathAtPos(path) != null)
                    mapManager.currentState.GetPathAtPos(path).unlocked = true;
                if (mapManager.currentState.GetLevelAtPos(path) != null)
                    mapManager.currentState.GetLevelAtPos(path).unlocked = true;
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
        inputManager.SwitchCurrentActionMap("No Control");

        currentLevel = level;

        levelTransitioner.StartLevelTransition();
        yield return new WaitForSeconds(40f / 60f);
        var asyncLoadLevel = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        levelTransitioner.EndLevelTransition();

        SetMovementManagerFromScene();
        OnLevelEnter?.Invoke();

        yield return new WaitForSeconds(20f / 60f);
        inputManager.SwitchCurrentActionMap("Gameplay");
    }

    public void CompleteLevel(string levelExit)
    {
        StartCoroutine(CompleteLevelAsync(levelExit));
    }
    private IEnumerator CompleteLevelAsync(string levelExit)
    {
        Debug.Log("Finish Level: " + levelExit);
        inputManager.SwitchCurrentActionMap("No Control");

        levelTransitioner.StartLevelTransition();
        yield return new WaitForSeconds(40f / 60f);
        var asyncLoadLevel = SceneManager.LoadSceneAsync(currentWorld, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        levelTransitioner.EndLevelTransition();
        SetMapManagerFromScene();

        if (levelsCompleted.ContainsKey(levelExit))
            levelsCompleted[levelExit] = true;
        else
            levelsCompleted.Add(levelExit, true);
        DataPersistenceManager.instance.SaveGame();

        foreach (var level in mapManager.currentState.GetAllTLLevels())
        {
            if (level.levelName == currentLevel)
                mapManager.currentState.Move(mapManager.currentState.GetPlayer(), level.curPos);
        }

        OnMapEnter?.Invoke();
        mapManager.CompleteLevel(levelExit);

        yield return new WaitForSeconds(20f / 60f);
        inputManager.SwitchCurrentActionMap("World Map");
    }

    public void OpenMap(string mapName, Vector2Int pos)
    {
        StartCoroutine(OpenMapAsync(mapName, pos));
    }
    private IEnumerator OpenMapAsync(string mapName, Vector2Int pos)
    {
        Debug.Log("Open Map: " + mapName);
        inputManager.SwitchCurrentActionMap("No Control");

        currentWorld = mapName;

        levelTransitioner.StartLevelTransition();
        yield return new WaitForSeconds(40f / 60f);
        var asyncLoadLevel = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        levelTransitioner.EndLevelTransition();

        SetMapManagerFromScene();
        mapManager.currentState.Move(mapManager.currentState.GetPlayer(), pos);
        OnMapEnter?.Invoke();

        yield return new WaitForSeconds(20f / 60f);
        inputManager.SwitchCurrentActionMap("World Map");
    }

    public void OpenMap(string mapName, string levelName)
    {
        StartCoroutine(OpenMapAsync(mapName, levelName));
    }
    private IEnumerator OpenMapAsync(string mapName, string levelName)
    {
        Debug.Log("Open Map: " + mapName + ", " + levelName);

        inputManager.SwitchCurrentActionMap("No Control");

        currentWorld = mapName;

        levelTransitioner.StartLevelTransition();
        yield return new WaitForSeconds(40f / 60f);
        var asyncLoadLevel = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        levelTransitioner.EndLevelTransition();

        SetMapManagerFromScene();
        foreach (var level in mapManager.currentState.GetAllTLLevels())
        {
            if (level.levelName == levelName)
                mapManager.currentState.Move(mapManager.currentState.GetPlayer(), level.curPos);
        }
        foreach (var world in mapManager.currentState.GetAllTLWorlds())
        {
            if (world.worldToTravelTo == levelName)
                mapManager.currentState.Move(mapManager.currentState.GetPlayer(), world.curPos);
        }
        OnMapEnter?.Invoke();

        yield return new WaitForSeconds(20f / 60f);
        inputManager.SwitchCurrentActionMap("World Map");
    }

    public void OpenMap(string mapName)
    {
        StartCoroutine(OpenMapAsync(mapName));
    }
    private IEnumerator OpenMapAsync(string mapName)
    {
        Debug.Log("Open Map: " + mapName);

        inputManager.SwitchCurrentActionMap("No Control");

        currentWorld = mapName;

        levelTransitioner.StartLevelTransition();
        yield return new WaitForSeconds(40f / 60f);
        var asyncLoadLevel = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        levelTransitioner.EndLevelTransition();

        SetMapManagerFromScene();
        OnMapEnter?.Invoke();

        yield return new WaitForSeconds(20f / 60f);
        inputManager.SwitchCurrentActionMap("World Map");
    }

    public void OpenTitleScreen()
    {
        StartCoroutine(OpenTitleScreenAsync());
    }
    private IEnumerator OpenTitleScreenAsync()
    {
        levelTransitioner.StartLevelTransition();
        yield return new WaitForSeconds(40f / 60f);
        var asyncLoadLevel = SceneManager.LoadSceneAsync("Title Screen", LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        levelTransitioner.EndLevelTransition();
    }

    public void QuitGame()
    {
        DataPersistenceManager.instance.SaveGame();
        Application.Quit();
    }

    public void DEBUG(string message)
    {
        print(message);
    }
}
