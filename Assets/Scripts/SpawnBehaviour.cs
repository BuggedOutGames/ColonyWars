using UnityEngine;

public class SpawnBehaviour : MonoBehaviour {

    public GameObject unit;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            SpawnUnit();
        }
    }

    void SpawnUnit() {
        Vector2 position = Utils.Instance.GetMousePositionInWorld();
        Instantiate(unit, new Vector3(position.x, position.y, -1), Quaternion.identity);
    }
}
