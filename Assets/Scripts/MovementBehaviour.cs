using UnityEngine;

public class MovementBehaviour : MonoBehaviour {

    private static readonly int Walking = Animator.StringToHash("walking");

    [Range(1, 10)]
    public float movementSpeed;

    private Rigidbody2D rigidBody;
    private Animator animator;
    private Vector2? moveDestination;
    private float adjustedMoveSpeed;
    private float adjustedRotationSpeed;
    
    private void Start() {
        adjustedMoveSpeed = movementSpeed / 2;
        adjustedRotationSpeed = adjustedMoveSpeed * 250;
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        EventManager.Instance.MoveCommandEvent += HandleMoveCommandEvent;
    }

    private void OnDestroy() {
        EventManager.Instance.MoveCommandEvent -= HandleMoveCommandEvent;
    }

    private void FixedUpdate() {
        if (moveDestination.HasValue) {
            MoveTowards(moveDestination.Value);
        }
    }

    private void HandleMoveCommandEvent(object sender, MoveCommand moveCommand) {
        if (moveCommand.Unit.gameObject.Equals(gameObject)) {
            moveDestination = moveCommand.Destination;
            StartWalkAnimation();
        }
    }

    private void MoveTowards(Vector2 target) {
        RotateTowards(target);
        MoveForward();
        if (Vector2.Distance(transform.position, target) < 0.1) {
            moveDestination = null;
            StopWalkAnimation();
        }
    }

    private void RotateTowards(Vector2 target) {
        Vector2 direction = (target - (Vector2) transform.position).normalized;
        Quaternion newRotation = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, adjustedRotationSpeed * Time.deltaTime);
    }

    private void MoveForward() {
        rigidBody.MovePosition(transform.position + transform.up * (adjustedMoveSpeed * Time.deltaTime));
    }

    private void StartWalkAnimation() => animator.SetBool(Walking, true);
    private void StopWalkAnimation() => animator.SetBool(Walking, false);
}
