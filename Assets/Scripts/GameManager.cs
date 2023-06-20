using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField] public InputActionReference moveUp, moveDown, moveRight, moveLeft, undo, reset;
    [SerializeField] public Object wallPrefab;
    [SerializeField] public string[] wallNames;

    public static GameManager Inst;

    public GameState initialGameState;

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
            print(tilePos);
            print(tileMap.CellToLocal(tilePos));
            foreach (var wallName in wallNames)
            {
                if (tileMap.GetTile(tilePos) != null && wallName.Equals(tileMap.GetTile(tilePos).name))
                    Instantiate(wallPrefab, tileMap.CellToLocal(tilePos), Quaternion.identity);
            }
        }
        
        initialGameState = new GameState(FindObjectsByType<TLObject>(FindObjectsSortMode.None));
    }

    public void DEBUG(string message)
    {
        print(message);
    }
}
