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

    public List<string> levelsCompleted;
    public string currentLevel;
    public string currentWorld;
    public bool inMap = true;

    public static GameManager Inst;
    public MovementManager movementManager;
    public PlantCreationAnimator animator;
    public PlayerInput inputManager;
    public LevelTransitioner levelTransitioner;
    public SoundFXManager soundFXManager;

    public GameState currentState;

    public event Action<GameState> OnLevelEnter;
    public event Action<GameState> OnMapEnter;

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
        levelsCompleted = new List<string>();

        if (SceneManager.GetActiveScene().name.Contains("Title"))
        {
            inputManager.SwitchCurrentActionMap("Pause Menu");
        }
        else if (SceneManager.GetActiveScene().name.Contains("Map"))
        {
            OpenMap(SceneManager.GetActiveScene().name);
            inMap = true;
        }
        else
        {
            OpenLevel(SceneManager.GetActiveScene().name);
            currentWorld = "World 1 Map"; // TODO FIX
            inMap = false;
        }
    }       

    public void LoadData(GameData data)
    {
        currentWorld = data.currentWorld;

        levelsCompleted.Clear();
        foreach (var level in data.levelsCompleted)
        {
            levelsCompleted.Add(level);
        }
    }

    public void SaveData(GameData data)
    {
        data.currentWorld = currentWorld;

        data.levelsCompleted.Clear();
        foreach (var level in levelsCompleted)
        {
            data.levelsCompleted.Add(level);
        }
    }

    public bool IsLevelComplete(string levelName)
    {
        bool completed = false;
        foreach (string level in levelsCompleted)
        {
            if (levelName == level)
                completed = true;
        }
        return completed;
    }

    private void SetMovementManagerFromScene()
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
        foreach (var TLSig in TLSignatures)
        {
            Vector2Int pos = new Vector2Int((int)TLSig.gameObject.transform.position.x, (int)TLSig.gameObject.transform.position.y);
            if (TLSig is PlayerSignature)
                TlObjectList.Add(new TLPlayer(pos));
            if (TLSig is PlantSignature)
                TlObjectList.Add(new TLPlant(pos, (PlantSignature) TLSig));
            if (TLSig is DoorSignature)
                TlObjectList.Add(new TLDoor(pos, (DoorSignature) TLSig));
            if (TLSig is ShearSignature)
                TlObjectList.Add(new TLShears(pos, (ShearSignature)TLSig));
        }

        currentState = new GameState();
        foreach (var TLObj in TlObjectList)
        {
            currentState.AddObject(TLObj);
        }

        Debug.Log("Level Set");
        Debug.Log(currentState.ToString());
        PrintLevelsCompleted();
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

                    foreach (var wallName in wallNames)
                    {
                        if (tileMap.GetTile(tilePos) != null && wallName.Equals(tileMap.GetTile(tilePos).name))
                        {
                            Vector3 pos = tileMap.CellToLocal(tilePos);
                            TlObjectList.Add(new TLWall(new Vector2Int((int)pos.x, (int)pos.y)));
                        }
                    }
                }
            }
        }

        var TLSignatures = FindObjectsByType<TLSignature>(FindObjectsSortMode.None);
        foreach (var TLSig in TLSignatures)
        {
            Vector2Int pos = new Vector2Int((int)TLSig.gameObject.transform.position.x, (int)TLSig.gameObject.transform.position.y);
            if (TLSig is PlayerSignature)
                TlObjectList.Add(new TLPlayer(pos));
            else if (TLSig is LevelSignature)
                TlObjectList.Add(new TLLevel(pos, (LevelSignature)TLSig, IsLevelComplete(((LevelSignature)TLSig).levelName)));
            else if (TLSig is WorldSignature)
                TlObjectList.Add(new TLWorldPortal(pos, (WorldSignature)TLSig));
            else if (TLSig is WorldDoorSignature)
                TlObjectList.Add(new TLWorldDoor(pos, (WorldDoorSignature)TLSig));
        }

        currentState = new GameState();
        foreach (var TLObj in TlObjectList)
        {
            currentState.AddObject(TLObj);
        }

        Debug.Log(currentState.ToString());
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
        inMap = false;

        levelTransitioner.StartLevelTransition(level);
        yield return new WaitForSeconds(40f / 60f);
        var asyncLoadLevel = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        levelTransitioner.EndLevelTransition();

        SetMovementManagerFromScene();
        OnLevelEnter?.Invoke(currentState);

        yield return new WaitForSeconds(20f / 60f);
        inputManager.SwitchCurrentActionMap("Gameplay");
    }

    public void CompleteLevel(string levelName)
    {
        StartCoroutine(CompleteLevelAsync(levelName));
    }
    private IEnumerator CompleteLevelAsync(string levelName)
    {
        inputManager.SwitchCurrentActionMap("No Control");

        inMap = true;

        levelTransitioner.StartLevelTransition(levelName);
        yield return new WaitForSeconds(40f / 60f);
        var asyncLoadLevel = SceneManager.LoadSceneAsync(currentWorld, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        levelTransitioner.EndLevelTransition();
        SetMapManagerFromScene();

        foreach (var level in currentState.GetAllOfTLType<TLLevel>())
        {
            if (level.GetLevelName() == levelName)
                currentState.GetPlayer().SetInitialPos(level.GetPosition());
        }

        OnMapEnter?.Invoke(currentState);

        if (!levelsCompleted.Contains(levelName))
        {
            levelsCompleted.Add(levelName);
            DataPersistenceManager.instance.SaveGame();
            movementManager.CompleteLevel(levelName);
        }

        Debug.Log("Finished Map Setup");
        Debug.Log(currentState.ToString());
        PrintLevelsCompleted();

        yield return new WaitForSeconds(20f / 60f);
        inputManager.SwitchCurrentActionMap("Gameplay");
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
        inMap = true;

        levelTransitioner.StartLevelTransition(mapName);
        yield return new WaitForSeconds(40f / 60f);
        var asyncLoadLevel = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        levelTransitioner.EndLevelTransition();

        SetMapManagerFromScene();
        currentState.GetPlayer().SetInitialPos(pos);
        OnMapEnter?.Invoke(currentState);

        yield return new WaitForSeconds(20f / 60f);
        inputManager.SwitchCurrentActionMap("Gameplay");
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
        inMap = true;

        levelTransitioner.StartLevelTransition(mapName);
        yield return new WaitForSeconds(40f / 60f);
        var asyncLoadLevel = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        levelTransitioner.EndLevelTransition();

        SetMapManagerFromScene();
        foreach (var level in currentState.GetAllOfTLType<TLLevel>())
        {
            if (level.GetLevelName() == levelName)
                currentState.GetPlayer().SetInitialPos(level.GetPosition());
        }
        foreach (var world in currentState.GetAllOfTLType<TLWorldPortal>())
        {
            if (world.GetWorldToTravelTo() == levelName)
                currentState.GetPlayer().SetInitialPos(world.GetPosition());
        }
        OnMapEnter?.Invoke(currentState);


        yield return new WaitForSeconds(20f / 60f);
        inputManager.SwitchCurrentActionMap("Gameplay");
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
        inMap = true;

        levelTransitioner.StartLevelTransition(mapName);
        yield return new WaitForSeconds(40f / 60f);
        var asyncLoadLevel = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        levelTransitioner.EndLevelTransition();

        SetMapManagerFromScene();
        OnMapEnter?.Invoke(currentState);

        Debug.Log("Pre Wait");
        yield return new WaitForSeconds(20f / 60f);
        inputManager.SwitchCurrentActionMap("Gameplay");
        Debug.Log("Finished Complete Level Coroutine");
    }

    public void OpenTitleScreen()
    {
        StartCoroutine(OpenTitleScreenAsync());
    }
    private IEnumerator OpenTitleScreenAsync()
    {
        levelTransitioner.StartLevelTransition("");
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

    public void PrintLevelsCompleted()
    {
        string result = "LEVELS COMPLETE:\n";
        foreach (var level in levelsCompleted)
        {
            result += level;
            result += ",\n";
        }
        Debug.Log(result);
    }
}
