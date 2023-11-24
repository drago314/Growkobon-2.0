using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneralAnimator : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private GameObject clearPlantPrefab;
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private GameObject worldExitPrefab;

    public event System.Action<Vector2Int> OnLevelUnlock;
    private Tilemap pathTilemap;

    private void Start()
    {
        GameManager.Inst.movementManager.OnPlantGrow += GrowPlant;
        GameManager.Inst.OnLevelEnter += GenerateLevel;
        GameManager.Inst.movementManager.OnResetEnd += GenerateLevel;
        GameManager.Inst.movementManager.OnUndoEnd += GenerateLevel;
        GameManager.Inst.OnMapEnter += GenerateMap;
        GameManager.Inst.mapManager.OnPathsUnlock += UnlockPaths;
    }

    private void OnDestroy()
    {
        GameManager.Inst.movementManager.OnPlantGrow -= GrowPlant;
        GameManager.Inst.OnLevelEnter -= GenerateLevel;
        GameManager.Inst.movementManager.OnResetEnd -= GenerateLevel;
        GameManager.Inst.movementManager.OnUndoEnd -= GenerateLevel;
        GameManager.Inst.OnMapEnter -= GenerateMap;
        GameManager.Inst.mapManager.OnPathsUnlock -= UnlockPaths;
    }

    private void InstantiatePlayer(TLPlayer player)
    {
        var playerAnimator = Instantiate(playerPrefab, new Vector3Int(player.curPos.x, player.curPos.y, 0), Quaternion.identity).GetComponent<PlayerAnimator>();
        playerAnimator.PlayerFaceDir(player.directionFacing);
    }

    private void GrowPlant(GrowAction grow)
    {
        var plantAnimator = Instantiate(clearPlantPrefab, new Vector3Int(grow.newPos.x, grow.newPos.y, 0), Quaternion.identity).GetComponent<PlantAnimator>();
        plantAnimator.Grow(grow.moveDir);
    }

    private void InstantiatePlant(TLPlant plant)
    {
        var plantAnimator = Instantiate(plantPrefab, new Vector3Int(plant.curPos.x, plant.curPos.y, 0), Quaternion.identity).GetComponent<PlantAnimator>();
        plantAnimator.Instantiate();
    }

    private void InstantiateLevel(TLLevel level)
    {
        var levelAnimator = Instantiate(levelPrefab, new Vector3Int(level.curPos.x, level.curPos.y, 0), Quaternion.identity).GetComponent<LevelAnimator>();
        levelAnimator.Instantiate();
    }

    private void InstantiateWorldExit(TLWorldPortal world)
    {
        Instantiate(worldExitPrefab, new Vector3Int(world.curPos.x, world.curPos.y, 0), Quaternion.identity);
    }

    private void GenerateLevel()
    {
        // TODO Add MoveableTLObject class
        var TLSignatures = FindObjectsByType<TLSignature>(FindObjectsSortMode.None);
        foreach (var TLSig in TLSignatures)
        {
            if (TLSig is MoveableObjectSignature)
                Destroy(TLSig.gameObject);
        }

        foreach (var TLObj in GameManager.Inst.movementManager.currentState.GetAllTLObjects())
        {
            if (TLObj is TLPlayer)
                InstantiatePlayer((TLPlayer)TLObj);
            if (TLObj is TLPlant)
                InstantiatePlant((TLPlant)TLObj);
        }
    }

    private void GenerateMap()
    {
        var tileMaps = FindObjectsOfType<Tilemap>();
        foreach (var tileMap in tileMaps)
        {
            if (tileMap.tag.Equals("Path Tilemap"))
            {
                pathTilemap = tileMap;
            }
        }

        var TLSignatures = FindObjectsByType<TLSignature>(FindObjectsSortMode.None);
        foreach (var TLSig in TLSignatures)
        {
            Destroy(TLSig.gameObject);
        }

        foreach (var TLObj in GameManager.Inst.mapManager.currentState.GetAllTLObjects())
        {
            if (TLObj is TLPlayer)
                InstantiatePlayer((TLPlayer)TLObj);
            if (TLObj is TLLevel)
                InstantiateLevel((TLLevel)TLObj);
            if (TLObj is TLPath)
            {
                Vector3Int pos = pathTilemap.WorldToCell(new Vector3Int(TLObj.curPos.x, TLObj.curPos.y, 0));
                PathTile tile = (PathTile) pathTilemap.GetTile(pos);
                tile.unlocked = ((TLPath)TLObj).unlocked;
                pathTilemap.RefreshTile(pos);
            }      
            if (TLObj is TLWorldPortal)
                InstantiateWorldExit((TLWorldPortal)TLObj);
        }
    }

    private void UnlockPaths(List<Vector2Int> posList)
    {
        List<Vector3Int> tilesToUnlock = new List<Vector3Int>();

        foreach (var pos in posList)
        {
            tilesToUnlock.Add(pathTilemap.WorldToCell(new Vector3Int(pos.x, pos.y, 0)));
        }

        StartCoroutine(UnlockPathCorutine(tilesToUnlock));
    }

    private IEnumerator UnlockPathCorutine(List<Vector3Int> tilesToUnlock)
    {
        yield return new WaitForSeconds(0.5f);
        List<List<Vector3Int>> unlockPosOrder = new List<List<Vector3Int>>();

        for (int i = 0; i < tilesToUnlock.Count; i++)
        {
            Vector2Int tilePos = new Vector2Int(tilesToUnlock[i].x, tilesToUnlock[i].y);
            if (i == 0)
            {
                unlockPosOrder.Add(new List<Vector3Int> { tilesToUnlock[i] });
                continue;
            }

            bool foundPair = false;
            for (int j = unlockPosOrder.Count - 1; j >= 0; j--)
            {
                foreach (var possiblePairVector3 in unlockPosOrder[j])
                {
                    Vector2Int possiblePair = new Vector2Int(possiblePairVector3.x, possiblePairVector3.y);
                    if ((tilePos + Vector2Int.up).Equals(possiblePair) || (tilePos + Vector2Int.down).Equals(possiblePair) || (tilePos + Vector2Int.right).Equals(possiblePair) || (tilePos + Vector2Int.left).Equals(possiblePair))
                    {
                        if (j == unlockPosOrder.Count - 1)
                            unlockPosOrder.Add(new List<Vector3Int> { tilesToUnlock[i] });
                        else
                            unlockPosOrder[j + 1].Add(tilesToUnlock[i]);

                        foundPair = true;
                        break;
                    }
                }

                if (foundPair)
                    break;
            }
            
            if (!foundPair)
            {
                unlockPosOrder[0].Add(tilesToUnlock[i]);
            }
        }

        foreach (var bunch in unlockPosOrder)
        {
            foreach (var tile in bunch)
            {
                Vector2Int curPos = new Vector2Int(tile.x, tile.y);

                if (pathTilemap.GetTile(tile) != null && pathTilemap.GetTile(tile) is PathTile)
                {
                    (pathTilemap.GetTile(tile) as PathTile).unlocked = true;
                    pathTilemap.RefreshTile(tile);
                }
                if (GameManager.Inst.mapManager.currentState.GetLevelAtPos(curPos) != null)
                {
                    OnLevelUnlock?.Invoke(curPos);
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
        // List<Vector2Int> pastPosList = new List<Vector2Int>();
        //  List<Vector2Int> curPosList = new List<Vector2Int>();
        // curPosList.Add(curPos);
        /*foreach (var newPos in tilesToUnlock)
    {
        foreach (var pastPos in pastPosList)
        {
            Vector2Int newPos2 = new Vector2Int(newPos.x, newPos.y); ;
            if (!(curPos + Vector2Int.up).Equals(newPos) && !(curPos + Vector2Int.down).Equals(newPos) && !(curPos + Vector2Int.right).Equals(newPos) && !(curPos + Vector2Int.left).Equals(newPos)
                && ((pastPos + Vector2Int.up).Equals(newPos) || (pastPos + Vector2Int.left).Equals(newPos) || (pastPos + Vector2Int.right).Equals(newPos) || (pastPos + Vector2Int.down).Equals(newPos)))
            {
                if (pathTilemap.GetTile(newPos) != null && pathTilemap.GetTile(newPos) is PathTile)
                {
                    (pathTilemap.GetTile(newPos) as PathTile).unlocked = true;
                    pathTilemap.RefreshTile(newPos);
                }
                else if (GameManager.Inst.mapManager.currentState.GetLevelAtPos(newPos2) != null)
                {
                    OnLevelUnlock?.Invoke(newPos2);
                }
                curPosList.Add(newPos2);
                tilesToUnlock.Remove(newPos);
            }
            break;
        }
    }
    pastPosList = curPosList;*/
}
