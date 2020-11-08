using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementBehaviour : MonoBehaviour {

    private static readonly int Walking = Animator.StringToHash("walking");

    [Range(1, 10)]
    public float movementSpeed;
    [Range(1, 10)]
    public float separationFactor;
    [Range(1, 10)]
    public float alignmentFactor;
    [Range(1, 10)]
    public float cohesionFactor;
    [Range(1, 10)]
    public float destinationFactor;
    
    private Rigidbody2D rigidBody;
    private Animator animator;
    private Vector2? moveDestination;
    
    private Vector2 separationSmoothVector;
    private Vector2 alignmentSmoothVector;
    private Vector2 cohesionSmoothVector;
    private Vector2 destinationSmoothVector;

    private float rotationSpeed;
    private float adjustedSeparationFactor;
    private float adjustedAlignmentFactor;
    private float adjustedCohesionFactor;
    private float adjustedDestinationFactor;

    private readonly List<Transform> nearbyUnits = new List<Transform>();

    private void Start() {
        rotationSpeed = movementSpeed * 250;
        adjustedSeparationFactor = separationFactor * 100;
        adjustedAlignmentFactor = alignmentFactor * 100;
        adjustedCohesionFactor = cohesionFactor * 100;
        adjustedDestinationFactor = destinationFactor * 2;
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        EventManager.Instance.MoveCommandEvent += HandleMoveCommandEvent;
    }

    private void OnDestroy() {
        EventManager.Instance.MoveCommandEvent -= HandleMoveCommandEvent;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.parent != transform) {
            if (other.gameObject.CompareTag("FlockTrigger")) {
                nearbyUnits.Add(other.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.transform.parent != transform) {
            if (other.gameObject.CompareTag("FlockTrigger")) {
                nearbyUnits.Remove(other.transform);
            }
        }
    }
    
    private void FixedUpdate() {
        if (moveDestination.HasValue) {
            Vector2 direction = Cohesion() + Alignment() + Separation() + Destination();
            RotateTowards(direction);
            MoveForward();
            if (Vector2.Distance(transform.position, moveDestination.Value) < 1) {
                moveDestination = null;
                StopWalkAnimation();
            }
        }
    }
    
    private Vector2 Cohesion() {
        Vector2 averageNeighborPosition = GetAverageNeighborPosition();
        Vector2 cohesionSteer = averageNeighborPosition - (Vector2) transform.position;
        Vector2 adjustedCohesionSteer = cohesionSteer * adjustedCohesionFactor;
        return SmoothedSteer(adjustedCohesionSteer, ref cohesionSmoothVector);
    }

    private Vector2 Alignment() {
        Vector2 alignmentSteer = GetAverageNeighborHeading();
        Vector2 adjustedAlignmentSteer = alignmentSteer * adjustedAlignmentFactor;
        return SmoothedSteer(adjustedAlignmentSteer, ref alignmentSmoothVector);
    }

    private Vector2 Separation() {
        Vector2 averageNeighborPosition = GetAverageNeighborPosition();
        Vector2 separationSteer = (Vector2) transform.position - averageNeighborPosition;
        Vector2 adjustedSeparationSteer = separationSteer * adjustedSeparationFactor;
        return SmoothedSteer(adjustedSeparationSteer, ref separationSmoothVector);
    }

    private Vector2 Destination() {
        Vector2 destinationSteer = moveDestination.Value - (Vector2) transform.position;
        Vector2 adjustedDestinationSteer = destinationSteer * adjustedDestinationFactor;
        return SmoothedSteer(adjustedDestinationSteer, ref destinationSmoothVector);
    }

    private Vector2 SmoothedSteer(Vector2 steerVector, ref Vector2 velocityVector) {
        return Vector2.SmoothDamp(
            transform.up, steerVector, ref velocityVector, 0.1f
        );
    }
    
    private Vector2 GetAverageNeighborPosition() {
        if (nearbyUnits.Count == 0) {
            return transform.position;
        } else {
            return nearbyUnits
                .Select(unit => (Vector2) unit.position)
                .Aggregate(Vector2.zero,
                    (position, nextPosition) => position + nextPosition, 
                    (position) => position / nearbyUnits.Count);
        }
    }

    private Vector2 GetAverageNeighborHeading() {
        if (nearbyUnits.Count == 0) {
            return transform.up;
        } else {
            return nearbyUnits
                .Select(unit => (Vector2) unit.transform.up)
                .Aggregate(Vector2.zero,
                    (heading, nextHeading) => heading + nextHeading,
                    (heading) => heading / nearbyUnits.Count);
        }
    }

    private void HandleMoveCommandEvent(object sender, MoveCommand moveCommand) {
        if (moveCommand.Unit.gameObject.Equals(gameObject)) {
            moveDestination = moveCommand.Destination;
            StartWalkAnimation();
        }
    }
    
    private void MoveForward() {
        rigidBody.MovePosition(transform.position + transform.up * (movementSpeed * Time.fixedDeltaTime));
    }

    private void RotateTowards(Vector2 target) {
        Vector2 direction = (target - (Vector2) transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        rigidBody.MoveRotation(newRotation);
    }

    private void StartWalkAnimation() => animator.SetBool(Walking, true);
    private void StopWalkAnimation() => animator.SetBool(Walking, false);
}
