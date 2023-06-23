using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralAnimator : MonoBehaviour
{
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject plantPrefab;
    [SerializeField] public GameObject potPrefab;

    private void Start()
    {
        gameObject.GetComponent<MovementManager>().OnPlantGrow += GrowPlant;
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

    public void InstantiatePot(TLPot pot)
    {
        Instantiate(potPrefab, new Vector3Int(pot.curPos.x, pot.curPos.y, 0), Quaternion.identity);
    }    
}
