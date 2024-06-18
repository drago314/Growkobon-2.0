using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantCreationAnimator : MonoBehaviour
{
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private GameObject clearPlantPrefab;

    private void Start()
    {
        GameManager.Inst.movementManager.OnPlantGrow += GrowPlant;  
    }

    private void OnDestroy()
    {
        GameManager.Inst.movementManager.OnPlantGrow -= GrowPlant;
    }

    private void GrowPlant(GrowAction grow)
    {
        var plantAnimator = Instantiate(clearPlantPrefab, new Vector3Int(grow.newPos.x, grow.newPos.y, 0), Quaternion.identity).GetComponent<PlantAnimator>();
        plantAnimator.Instantiate(grow.state);
        plantAnimator.Grow(grow.moveDir);
    }
}