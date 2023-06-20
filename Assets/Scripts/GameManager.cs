using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField] public InputActionReference moveUp, moveDown, moveRight, moveLeft, undo, reset;
    [SerializeField] public GameObject wallPrefab;
    [SerializeField] public GameObject plantPrefab;
    [SerializeField] public string[] wallNames;

    public static GameManager Inst;

    public GameState initialGameState;
    public GameState currentState;

    private void Awake()
    {
        Inst = this; // TODO fix
    }

    private void Start()
    {
        var tileMap = FindObjectOfType<Tilemap>();
        BoundsInt bounds = tileMap.cellBounds;
        foreach (Vector3Int tilePos in tileMap.cellBounds.allPositionsWithin)
        {
            foreach (var wallName in wallNames)
            {
                if (tileMap.GetTile(tilePos) != null && wallName.Equals(tileMap.GetTile(tilePos).name))
                    Instantiate(wallPrefab, tileMap.CellToLocal(tilePos), Quaternion.identity);
            }
        }
        
        initialGameState = new GameState(FindObjectsByType<TLObject>(FindObjectsSortMode.None));
        currentState = initialGameState;
    }

    public void DEBUG(string message)
    {
        print(message);
    }
}
