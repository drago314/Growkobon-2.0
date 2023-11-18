using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour, IDataPersistence
{
<<<<<<< Updated upstream
=======
    [SerializeField] public string[] wallNames;
    [SerializeField] public string[] potNames;
    [SerializeField] public string[] pathNames;

    public Dictionary<string, bool> levelsCompleted;
    public Dictionary<string, bool[]> multiExitLevelsCompleted;
    public string currentLevel;
    public string currentWorld;

>>>>>>> Stashed changes
    public static GameManager Inst;
    public MovementManager movementManager;
    public MapManager mapManager;
    public GeneralAnimator animator;
    public PlayerInput inputManager;
    public DataPersistenceManager dataManager;

    public string currentWorld;
    public Dictionary<string, bool> levelsCompleted;
    private Vector2Int playerMapPosition;

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

        if (SceneManager.GetActiveScene().name.Contains("Map"))
        {
            currentWorld = SceneManager.GetActiveScene().name;
            OpenMap(currentWorld);
        }
        else
        {
            currentWorld = SceneManager.GetActiveScene().name.Substring(0, 7) + " Map";
            OpenLevel(SceneManager.GetActiveScene().name);
        }
    }

    private void OnEnable()
    {
        movementManager = GetComponent<MovementManager>();
        mapManager = GetComponent<MapManager>();
        animator = GetComponent<GeneralAnimator>();
        inputManager = GetComponent<PlayerInput>();
<<<<<<< Updated upstream
        dataManager = GetComponent<DataPersistenceManager>();
        dataManager.LoadGame();
=======

        levelsCompleted = new Dictionary<string, bool>();
        multiExitLevelsCompleted = new Dictionary<string, bool[]>();

        if (SceneManager.GetActiveScene().name.Contains("Map"))
        {
            currentWorld = SceneManager.GetActiveScene().name;
            OpenNewMap(currentWorld);
        }
        else
        {
            currentLevel = SceneManager.GetActiveScene().name;
            currentWorld = SceneManager.GetActiveScene().name.Substring(0, 7) + " Map";
            StartCoroutine(WaitTilLevelLoad(currentLevel));
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
            else if (TLSig is PlantSignature)
                TlObjectList.Add(new TLPlant(pos));
            else if (TLSig is MultiExitDoorSignature)
                TlObjectList.Add(new TLDoorMultiExit(pos, (MultiExitDoorSignature)TLSig));
            else if (TLSig is DoorSignature)
                TlObjectList.Add(new TLDoor(pos, (DoorSignature)TLSig));
        }

        movementManager.initialGameState = new GameState(TlObjectList);
        movementManager.stateList.Add(movementManager.initialGameState);
        movementManager.currentState = new GameState(movementManager.initialGameState);
>>>>>>> Stashed changes
    }

    public void LoadData(GameData data)
    {
<<<<<<< Updated upstream
        levelsCompleted = new Dictionary<string, bool>(data.levelsCompleted);
        currentWorld = data.currentWorld;
        playerMapPosition = data.playerPosition;
    }

    public void SaveData(ref GameData data)
    {
        foreach (var pair in levelsCompleted)
        {
            if (data.levelsCompleted.ContainsKey(pair.Key))
                data.levelsCompleted[pair.Key] = pair.Value;
            else
                data.levelsCompleted.Add(pair.Key, pair.Value);
        }

        data.currentWorld = currentWorld;
        data.playerPosition = playerMapPosition;
    }

    public void OpenLevel(string levelName)
    {
        inputManager.SwitchCurrentActionMap("Gameplay");
        try
        {
            playerMapPosition = mapManager.currentState.GetPlayer().curPos;
        }
        catch (Exception e)
        {
            Debug.Log("Likely started playing from inside a level. Shouldn't be an issue: " + e);
        }
        StartCoroutine(OpenLevelCoroutine(levelName));
=======
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

        mapManager.lvlToUnlockedPaths = new Dictionary<string, List<List<Vector2Int>>>();
        foreach (var lvl in mapManager.currentState.GetAllTLLevels())
        {
            mapManager.lvlToUnlockedPaths.Add(lvl.levelName, new List<List<Vector2Int>> { lvl.unlockablePaths });
            if (levelsCompleted.ContainsKey(lvl.levelName) && levelsCompleted[lvl.levelName] == true)
            {
                if (lvl is TLLevel2Exit)
                {
                    if (multiExitLevelsCompleted.ContainsKey(lvl.levelName) && multiExitLevelsCompleted[lvl.levelName][0])
                    {
                        foreach (var path in lvl.unlockablePaths)
                        {
                            if (mapManager.currentState.GetPathAtPos(path) != null)
                                mapManager.currentState.GetPathAtPos(path).unlocked = true;
                        }
                    }
                    else if (multiExitLevelsCompleted.ContainsKey(lvl.levelName) && multiExitLevelsCompleted[lvl.levelName][1])
                    {
                        foreach (var path in ((TLLevel2Exit)lvl).unlockablePaths2)
                        {
                            if (mapManager.currentState.GetPathAtPos(path) != null)
                                mapManager.currentState.GetPathAtPos(path).unlocked = true;
                        }
                    }
                }
                else
                {
                    foreach (var path in lvl.unlockablePaths)
                    {
                        if (mapManager.currentState.GetPathAtPos(path) != null)
                            mapManager.currentState.GetPathAtPos(path).unlocked = true;
                    }
                }
            }
        }
    }

    public void OpenLevel(TLLevel level)
    {
        inputManager.SwitchCurrentActionMap("Gameplay");

        currentLevel = level.levelName;
        if (!levelsCompleted.ContainsKey(currentLevel))
            levelsCompleted.Add(currentLevel, false);
        if (level is TLLevel2Exit && !multiExitLevelsCompleted.ContainsKey(currentLevel))
            multiExitLevelsCompleted.Add(currentLevel, new Boolean[] { false, false });
        StartCoroutine(WaitTilLevelLoad(currentLevel));
>>>>>>> Stashed changes
    }
    private IEnumerator OpenLevelCoroutine(string levelName)
    {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        movementManager.LoadLevel();
    }

    public void FinishLevel(string levelExit)
    {
        if (levelsCompleted.ContainsKey(levelExit))
            levelsCompleted[levelExit] = true;
        else
            levelsCompleted.Add(levelExit, true);

        inputManager.SwitchCurrentActionMap("World Map");
        StartCoroutine(FinishLevelCoroutine(currentWorld));
    }

    private IEnumerator FinishLevelCoroutine(string levelExit)
    {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(currentWorld, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        mapManager.LoadMap(playerMapPosition);
        mapManager.UnlockLevelExit(levelExit);
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
        mapManager.LoadMap();
        playerMapPosition = mapManager.currentState.GetPlayer().curPos;
    }

<<<<<<< Updated upstream
    public void OpenMap()
    {
        OpenMap(currentWorld);
    }
    public void OpenMap(string mapName)
    {
        inputManager.SwitchCurrentActionMap("World Map");
        currentWorld = mapName;
        StartCoroutine(OpenMapCoroutine(mapName));
    }
    private IEnumerator OpenMapCoroutine(string map)
=======
    public void FinishLevel(TLDoor door, bool isMultiExit)
    {
        print("hi");
        if (isMultiExit)
        {
            print(((TLDoorMultiExit)door).exitNumber);
            multiExitLevelsCompleted[currentLevel][((TLDoorMultiExit)door).exitNumber] = true;
        }

        levelsCompleted[currentLevel] = true;
        inputManager.SwitchCurrentActionMap("World Map");

        if (door is TLDoorMultiExit)
            StartCoroutine(FinishLevelCoroutine(((TLDoorMultiExit)door).exitNumber));
        else
            StartCoroutine(FinishLevelCoroutine(0));
    }

    private IEnumerator FinishLevelCoroutine(int doorExited)
>>>>>>> Stashed changes
    {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(map, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
<<<<<<< Updated upstream
        mapManager.LoadMap(playerMapPosition);
=======
        OnMapLoad?.Invoke();
        mapManager.CompleteLevel(currentLevel, doorExited);
>>>>>>> Stashed changes
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
