using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBehaviour : MonoBehaviour {

    public UnitBehaviour unit;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            SpawnUnit();
        }
    }

    void SpawnUnit() {
        Vector2 position = Utils.instance.GetMousePositionInWorld();
        UnitBehaviour spawnedUnit = Instantiate(unit, new Vector3(position.x, position.y, -1), Quaternion.identity);
        spawnedUnit.SetSelected(false);
    }
}
