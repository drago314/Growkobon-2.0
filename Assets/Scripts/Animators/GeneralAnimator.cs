using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralAnimator : MonoBehaviour
{
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject plantPrefab;
    [SerializeField] public GameObject levelPrefab;

    private void Start()
    {
        gameObject.GetComponent<MovementManager>().OnPlantGrow += GrowPlant;
        GameManager.Inst.movementManager.OnResetEnd += GenerateLevel;
        GameManager.Inst.movementManager.OnUndoEnd += GenerateLevel;
        GameManager.Inst.OnMapEnter += GenerateMap;
    }

    private void OnDestroy()
    {
        GameManager.Inst.movementManager.OnResetEnd -= GenerateLevel;
        GameManager.Inst.movementManager.OnUndoEnd -= GenerateLevel;
        GameManager.Inst.OnMapEnter -= GenerateMap;
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
        var TLSignatures = FindObjectsByType<TLSignature>(FindObjectsSortMode.None);
        foreach (var TLSig in TLSignatures)
        {
            if (TLSig is MoveableObjectSignature)
                Destroy(TLSig.gameObject);
        }

        foreach (var TLObj in GameManager.Inst.mapManager.currentState.GetAllTLObjects())
        {
            if (TLObj is TLPlayer)
                InstantiatePlayer((TLPlayer)TLObj);
            if (TLObj is TLLevel)
                InstantiateLevel((TLLevel)TLObj);
        }
    }
}
