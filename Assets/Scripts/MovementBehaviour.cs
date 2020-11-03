using UnityEngine;

public class MovementBehaviour : MonoBehaviour {

    public float movementSpeed;
    private Vector2? moveDestination;
    
    private void Start() {
        EventManager.Instance.MoveCommandEvent += HandleMoveCommandEvent;
    }

    private void OnDestroy() {
        EventManager.Instance.MoveCommandEvent -= HandleMoveCommandEvent;
    }

    private void Update() {
        if (moveDestination.HasValue) {
            MoveTowards(moveDestination.Value, movementSpeed * Time.deltaTime);
        }
    }

    private void HandleMoveCommandEvent(object sender, MoveCommand moveCommand) {
        if (moveCommand.Unit.gameObject.Equals(gameObject)) {
            moveDestination = moveCommand.Destination;
        }
    }

    private void MoveTowards(Vector2 target, float distance) {
        Vector2 newPosition = Vector2.MoveTowards(transform.position, target, distance);
        gameObject.GetComponent<Rigidbody2D>().MovePosition(newPosition);
        if (Vector2.Distance(transform.position, target) < 1) {
            moveDestination = null;
        }
    }
}
