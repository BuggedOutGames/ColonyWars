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
    [Range(1, 10)] 
    public float obstacleAvoidanceFactor;
    
    private Rigidbody2D rigidBody;
    private Animator animator;
    private FlowField flowField;
    
    private Vector2 separationSmoothVector;
    private Vector2 alignmentSmoothVector;
    private Vector2 cohesionSmoothVector;
    private Vector2 destinationSmoothVector;
    private Vector2 obstacleAvoidanceSmoothVector;

    private float rotationSpeed;
    private float adjustedSeparationFactor;
    private float adjustedAlignmentFactor;
    private float adjustedCohesionFactor;
    private float adjustedDestinationFactor;
    private float adjustedObstacleAvoidanceFactor;
    
    private readonly List<Transform> nearbyUnits = new List<Transform>();
    private readonly List<Transform> obstacles = new List<Transform>();
    
    private void Start() {
        rotationSpeed = movementSpeed * 250;
        adjustedSeparationFactor = separationFactor * 100;
        adjustedAlignmentFactor = alignmentFactor * 100;
        adjustedCohesionFactor = cohesionFactor * 100;
        adjustedDestinationFactor = destinationFactor * 50;
        adjustedObstacleAvoidanceFactor = obstacleAvoidanceFactor * 20;
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
            } else if (other.gameObject.CompareTag("ObstacleTrigger")) {
                obstacles.Add(other.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.transform.parent != transform) {
            if (other.gameObject.CompareTag("FlockTrigger")) {
                nearbyUnits.Remove(other.transform);
            } else if (other.gameObject.CompareTag("ObstacleTrigger")) {
                obstacles.Remove(other.transform);
            }
        }
    }
    
    private void FixedUpdate() {
        if (flowField != null) {
            var direction = Cohesion() + Alignment() + Separation() + Destination() + ObstacleAvoidance();;
            RotateTowards(direction);
            MoveForward();
            if (flowField.GetFlowDirection(transform.position) == Vector2.zero) {
                flowField = null;
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
        Vector2 destinationSteer = (Vector2) transform.position + flowField.GetFlowDirection(transform.position) * adjustedDestinationFactor;
        return SmoothedSteer(destinationSteer, ref destinationSmoothVector);
    }

    private Vector2 ObstacleAvoidance() {
        Vector2 averageObstaclePosition = GetAverageObstaclePosition();
        Vector2 avoidanceSteer = (Vector2) transform.position - averageObstaclePosition;
        Vector2 adjustedAvoidanceSteer = avoidanceSteer * adjustedObstacleAvoidanceFactor;
        return SmoothedSteer(adjustedAvoidanceSteer, ref obstacleAvoidanceSmoothVector);
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
            return GetAverageTransformPosition(nearbyUnits);
        }
    }
    
    private Vector2 GetAverageObstaclePosition() {
        if (obstacles.Count == 0) {
            return transform.position;
        } else {
            return GetAverageTransformPosition(obstacles);
        }
    }

    private Vector2 GetAverageTransformPosition(List<Transform> transforms) {
        return transforms
            .Select(t => (Vector2) t.position)
            .Aggregate(Vector2.zero,
                (position, nextPosition) => position + nextPosition, 
                (position) => position / transforms.Count);
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
            flowField = FlowFieldManager.Instance.GetFlowField(moveCommand.Destination);
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
