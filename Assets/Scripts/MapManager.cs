using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System;


public class MapManager : MonoBehaviour
{
    [SerializeField] public string[] pathNames;

    [SerializeField] public InputActionReference moveUp, moveDown, moveRight, moveLeft, enter;
    public event Action OnMapEnter;
    public event Action<MoveAction> OnPlayerMove;
    public event Action<List<Vector2Int>> OnPathsUnlock;

    public GameState currentState;
<<<<<<< Updated upstream
    private Dictionary<string, List<Vector2Int>> exitToPathsUnlocked;
=======
    public Dictionary<string, List<List<Vector2Int>>> lvlToUnlockedPaths;
>>>>>>> Stashed changes

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

    public void LoadMap(Vector2Int playerPos)
    {
        LoadMap();
        currentState.Move(currentState.GetPlayer(), playerPos);
        OnPlayerMove?.Invoke(new MoveAction(playerPos, playerPos, Vector2Int.right, currentState.GetPlayer(), currentState));
    }

    public void LoadMap()
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

        currentState = new GameState(TlObjectList);

        exitToPathsUnlocked = new Dictionary<string, List<Vector2Int>>();
        foreach (var lvl in currentState.GetAllTLLevels())
        {
            foreach (var pair in lvl.exitToPathsUnlocked)
            {
                exitToPathsUnlocked.Add(pair.Key, pair.Value);
                if (GameManager.Inst.levelsCompleted.ContainsKey(lvl.levelName) && GameManager.Inst.levelsCompleted[lvl.levelName] == true)
                {
                    foreach (var path in pair.Value)
                    {
                        if (currentState.GetPathAtPos(path) != null)
                            currentState.GetPathAtPos(path).unlocked = true;
                    }
                }
            }
        }

        OnMapEnter?.Invoke();
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
            GameManager.Inst.OpenMap(currentState.GetWorldAtPos(goalPos).worldName);
        }

        bool noLevelInFront = currentState.GetLevelAtPos(goalPos) == null;
        bool noPathInFront = currentState.GetPathAtPos(goalPos) == null || !currentState.GetPathAtPos(goalPos).unlocked;
        if (noLevelInFront && noPathInFront)
        {
            OnPlayerMove?.Invoke(new MoveAction(curPos, curPos, moveDir, player, currentState));
            return;
        }

        OnPlayerMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, player, currentState));
        currentState.MoveRelative(player, moveDir);
    }

    private void EnterLevel(InputAction.CallbackContext obj)
    {
        TLLevel level = currentState.GetLevelAtPos(currentState.GetPlayer().curPos);
        if (level != null)
        {
            GameManager.Inst.OpenLevel(level);
        }
    }

<<<<<<< Updated upstream
    public void UnlockLevelExit(string levelExit)
    {
        foreach (var pathPos in exitToPathsUnlocked[levelExit])
=======
    public void CompleteLevel(string currentLevel, int doorExited)
    {
        foreach (var pathPos in lvlToUnlockedPaths[currentLevel][doorExited])
>>>>>>> Stashed changes
        {
            if (currentState.GetPathAtPos(pathPos) != null)
            {
                currentState.GetPathAtPos(pathPos).unlocked = true;
            }
            else if (currentState.GetLevelAtPos(pathPos) != null)
            {
                currentState.GetLevelAtPos(pathPos).unlocked = true;
            }
        }
<<<<<<< Updated upstream
        OnPathsUnlock?.Invoke(exitToPathsUnlocked[levelExit]);
=======
        OnPathsUnlock?.Invoke(lvlToUnlockedPaths[currentLevel][0]);
>>>>>>> Stashed changes
    }
};
