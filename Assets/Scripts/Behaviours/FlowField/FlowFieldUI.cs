﻿using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using BuggedOutGames.ColonyWars.Events;

namespace BuggedOutGames.ColonyWars.PathFinding {
    public class FlowFieldUI : MonoBehaviour {
        private void Start() {
            EventManager.Instance.MoveCommandEvent += HandleMoveCommandEvent;
        }

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        private void HandleMoveCommandEvent(object sender, MoveCommand move) {
            FlowFieldManager.Instance.DisplayFlowField(move.Destination);
        }
    }
}