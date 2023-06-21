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
    [SerializeField] public string[] wallNames;

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


        var TlObjectList = new List<TLObject>();

        var tileMap = FindObjectOfType<Tilemap>();
        BoundsInt bounds = tileMap.cellBounds;
        foreach (Vector3Int tilePos in tileMap.cellBounds.allPositionsWithin)
        {
            foreach (var wallName in wallNames)
            {
                if (tileMap.GetTile(tilePos) != null && wallName.Equals(tileMap.GetTile(tilePos).name))
                {
                    Vector3 pos = tileMap.CellToLocal(tilePos);
                    TlObjectList.Add(new TLWall(new Vector2Int((int) pos.x, (int) pos.y)));
                }
            }
        }

        var TLAnimators = FindObjectsByType<TLAnimator>(FindObjectsSortMode.None);
        foreach (var TLanim in TLAnimators)
        {
            Vector2Int pos = new Vector2Int((int)TLanim.gameObject.transform.position.x, (int)TLanim.gameObject.transform.position.y);
            if (TLanim is PlayerAnimator)
                TlObjectList.Add(new TLPlayer(pos));
            if (TLanim is PlantAnimator)
                TlObjectList.Add(new TLPlant(pos));
        }

        initialGameState = new GameState(TlObjectList);
        stateList.Add(initialGameState);
        currentState = new GameState(initialGameState);
    }

    public void GenerateCurrentState()
    {
        var TLAnimators = FindObjectsByType<TLAnimator>(FindObjectsSortMode.None);
        foreach (var TLanim in TLAnimators)
        {
            Destroy(TLanim.gameObject);
        }

        foreach (var TLObj in currentState.GetAllTLObjects())
        {
            if (TLObj is TLPlayer)
                Instantiate(playerPrefab, new Vector3(TLObj.curPos.x, TLObj.curPos.y, 0), Quaternion.identity);
            if (TLObj is TLPlant)
                Instantiate(plantPrefab, new Vector3(TLObj.curPos.x, TLObj.curPos.y, 0), Quaternion.identity);
        }

        stateList.Add(currentState);
        currentState = new GameState(currentState);
    }

    public void DEBUG(string message)
    {
        print(message);
    }
}
