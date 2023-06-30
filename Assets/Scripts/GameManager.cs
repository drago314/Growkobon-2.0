using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] public PlayerInput inputManager;
    [SerializeField] public string[] wallNames;
    [SerializeField] public string[] doorNames;
    [SerializeField] public string[] potNames;
    [SerializeField] public GeneralAnimator animator;

    public static GameManager Inst;
    public MovementManager movementManager;

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
        if (SceneManager.GetActiveScene().name.Contains("Level"))
            SetMovementManagerFromScene();
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

        movementManager.initialGameState = new GameState(TlObjectList);
        movementManager.stateList.Add(movementManager.initialGameState);
        movementManager.currentState = new GameState(movementManager.initialGameState);
    }

    public void StartLevel(int levelNumber)
    {
        inputManager.SwitchCurrentActionMap("Gameplay");
        LoadScene("World 1 Level " + levelNumber);
        SetMovementManagerFromScene();
    }

    public void FinishLevel()
    {
        inputManager.SwitchCurrentActionMap("World Map");
        LoadScene("World 1 Map");
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
