using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] public InputActionReference moveUp, moveDown, moveRight, moveLeft, enter;
    public event System.Action<MoveAction> OnPlayerMove;
    public event System.Action<string> OnLevelComplete;

    public GameState currentState;
    public Dictionary<string, List<Vector2Int>> exitToPathsUnlocked;

    private void Start()
    {
        moveUp.action.performed += MoveUp;
        moveDown.action.performed += MoveDown;
        moveRight.action.performed += MoveRight;
        moveLeft.action.performed += MoveLeft;
        enter.action.performed += EnterLevel;
    }

    private void OnDestroy()
    {
        moveUp.action.performed -= MoveUp;
        moveDown.action.performed -= MoveDown;
        moveRight.action.performed -= MoveRight;
        moveLeft.action.performed -= MoveLeft;
        enter.action.performed -= EnterLevel;
    }


    private void MoveUp(InputAction.CallbackContext obj)
    {
        Move(Vector2Int.up);
    }

    private void MoveDown(InputAction.CallbackContext obj)
    {
        Move(Vector2Int.down);
    }
    private void MoveRight(InputAction.CallbackContext obj)
    {
        Move(Vector2Int.right);
    }
    private void MoveLeft(InputAction.CallbackContext obj)
    {
        Move(Vector2Int.left);
    }

    private void Move(Vector2Int moveDir)
    {
        //print(currentState.ToString());
        /*foreach (var kvp in lvlToUnlockedPaths)
        {
            print(kvp.Key + " ");
            print(kvp.Value.Count);
            foreach (var item in kvp.Value)
            {
                print(item.x + " " + item.y);
            }
        }
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
                    if (tileMap.GetTile(tilePos) != null && tileMap.GetTile(tilePos) is PathTile)
                    {
                        print(tileMap.CellToLocal(tilePos) + " " + ((PathTile) tileMap.GetTile(tilePos)).unlocked);
                    }
                }
            }
        }*/


        TLPlayer player = currentState.GetPlayer();
        Vector2Int curPos = currentState.GetPosOf(player);
        Vector2Int goalPos = curPos + moveDir;

        if (currentState.GetWorldAtPos(goalPos) != null)
        {
            GameManager.Inst.OpenMap(currentState.GetWorldAtPos(goalPos).worldToTravelTo, currentState.GetWorldAtPos(goalPos).posToTravelTo);
        }

        bool wallBlocking = currentState.GetWallAtPos(goalPos) != null;
        if (wallBlocking)
        {
            OnPlayerMove?.Invoke(new MoveAction(curPos, curPos, moveDir, player, currentState));
            return;
        }

        OnPlayerMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, player, currentState));
        currentState.MoveRelative(player, moveDir);
        // OnPlayerMove?.Invoke(new MoveAction());
    }

    private void EnterLevel(InputAction.CallbackContext obj)
    {
        TLLevel level = currentState.GetLevelAtPos(currentState.GetPlayer().curPos);
        if (level != null)
        {
            GameManager.Inst.OpenLevel(level.levelName);
        }
    }

    public void CompleteLevel(string levelName)
    {
        OnLevelComplete?.Invoke(levelName);
        // TODO UNLOCK THINGS
    }
};
