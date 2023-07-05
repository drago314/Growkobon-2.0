using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneralAnimator : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private GameObject levelPrefab;

    public event System.Action<Vector2Int> OnLevelUnlock;
    private Tilemap pathTilemap;
    private List<Vector3Int> tilesToUnlock;
    bool corutineInAction;

    private void Start()
    {
        gameObject.GetComponent<MovementManager>().OnPlantGrow += GrowPlant;
        GameManager.Inst.movementManager.OnResetEnd += GenerateLevel;
        GameManager.Inst.movementManager.OnUndoEnd += GenerateLevel;
        GameManager.Inst.OnMapEnter += GenerateMap;
        GameManager.Inst.OnMapLoad += GenerateMap;
        GameManager.Inst.mapManager.OnPathUnlock += UnlockPath;
    }

    private void OnDestroy()
    {
        GameManager.Inst.movementManager.OnResetEnd -= GenerateLevel;
        GameManager.Inst.movementManager.OnUndoEnd -= GenerateLevel;
        GameManager.Inst.OnMapEnter -= GenerateMap;
        GameManager.Inst.OnMapLoad -= GenerateMap;
        GameManager.Inst.mapManager.OnPathUnlock -= UnlockPath;
    }

    private void InstantiatePlayer(TLPlayer player)
    {
        var playerAnimator = Instantiate(playerPrefab, new Vector3Int(player.curPos.x, player.curPos.y, 0), Quaternion.identity).GetComponent<PlayerAnimator>();
        playerAnimator.PlayerFaceDir(player.directionFacing);
    }

    private void GrowPlant(GrowAction grow)
    {
        var plantAnimator = Instantiate(plantPrefab, new Vector3Int(grow.newPos.x, grow.newPos.y, 0), Quaternion.identity).GetComponent<PlantAnimator>();
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
        }
    }

    private void UnlockPath(Vector2Int pos)
    {
        if (tilesToUnlock == null)
            tilesToUnlock = new List<Vector3Int>();

        tilesToUnlock.Add(pathTilemap.WorldToCell(new Vector3Int(pos.x, pos.y, 0)));


        if (!corutineInAction)
        {
            corutineInAction = true;
            StartCoroutine(UnlockPathCorutine());
        }
    }

    private IEnumerator UnlockPathCorutine()
    {
        while (tilesToUnlock.Count != 0)
        {
            if (pathTilemap.GetTile(tilesToUnlock[0]) != null && pathTilemap.GetTile(tilesToUnlock[0]) is PathTile)
            {
                (pathTilemap.GetTile(tilesToUnlock[0]) as PathTile).unlocked = true;
                pathTilemap.RefreshTile(tilesToUnlock[0]);
            }
            else if (GameManager.Inst.mapManager.currentState.GetLevelAtPos(new Vector2Int(tilesToUnlock[0].x, tilesToUnlock[0].y)) != null)
            {
                OnLevelUnlock?.Invoke(new Vector2Int(tilesToUnlock[0].x, tilesToUnlock[0].y));
            }

            tilesToUnlock.RemoveAt(0);
            yield return new WaitForSeconds(0.2f);
        }
        corutineInAction = false;
    }
}
