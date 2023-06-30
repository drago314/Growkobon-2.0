using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralAnimator : MonoBehaviour
{
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject plantPrefab;

    private void Start()
    {
        gameObject.GetComponent<MovementManager>().OnPlantGrow += GrowPlant;
        GameManager.Inst.movementManager.OnResetEnd += GenerateState;
        GameManager.Inst.movementManager.OnUndoEnd += GenerateState;
    }

    private void OnDestroy()
    {
        GameManager.Inst.movementManager.OnResetEnd -= GenerateState;
        GameManager.Inst.movementManager.OnUndoEnd -= GenerateState;
    }

    public void InstantiatePlayer(TLPlayer player)
    {
        var playerAnimator = Instantiate(playerPrefab, new Vector3Int(player.curPos.x, player.curPos.y, 0), Quaternion.identity).GetComponent<PlayerAnimator>();
        playerAnimator.PlayerFaceDir(player.directionFacing);
    }

    public void GrowPlant(GrowAction grow)
    {
        var plantAnimator = Instantiate(plantPrefab, new Vector3Int(grow.newPos.x, grow.newPos.y, 0), Quaternion.identity).GetComponent<PlantAnimator>();
        plantAnimator.Grow(grow.moveDir);
    }

    public void InstantiatePlant(TLPlant plant)
    {
        var plantAnimator = Instantiate(plantPrefab, new Vector3Int(plant.curPos.x, plant.curPos.y, 0), Quaternion.identity).GetComponent<PlantAnimator>();
        plantAnimator.Instantiate();
    }

    private void GenerateState()
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
}
