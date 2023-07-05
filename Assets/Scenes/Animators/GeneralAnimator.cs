using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneralAnimator : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private GameObject levelPrefab;
    private Tilemap pathTilemap;

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
        Vector3Int pos3 = pathTilemap.WorldToCell(new Vector3Int(pos.x, pos.y, 0));
        PathTile tile = (PathTile)pathTilemap.GetTile(pos3);
        tile.unlocked = true;
        pathTilemap.RefreshTile(pos3);
    }
}
