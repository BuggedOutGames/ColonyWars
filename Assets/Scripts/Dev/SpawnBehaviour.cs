using System;
using UnityEngine;

public class SpawnBehaviour : MonoBehaviour {

    public GameObject unit;

    private Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        if (Application.isEditor) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                SpawnUnit();
            }
        }
    }

    private void SpawnUnit() {
        Vector2 position = ProjectionUtil.GetPositionInWorld(mainCamera, Input.mousePosition);
        Instantiate(unit, new Vector3(position.x, position.y, -1), Quaternion.identity);
    }
}
