using UnityEngine;
using BuggedOutGames.ColonyWars.Events;

namespace BuggedOutGames.ColonyWars.Movement {
    public class MovementUI : MonoBehaviour {
        
        public GameObject movementIndicator;

        private void Start() {
            EventManager.Instance.MoveCommandEvent += HandleMoveCommandEvent;
        }

        private void HandleMoveCommandEvent(object sender, MoveCommand move) {
            DisplayMovementIndicator(move.Destination);
        }

        private void DisplayMovementIndicator(Vector2 position) {
            var instantiatedGameObject = Instantiate(movementIndicator, new Vector3(position.x, position.y, -1), Quaternion.identity);
            Destroy(instantiatedGameObject, 0.1f);
        }
    }
}