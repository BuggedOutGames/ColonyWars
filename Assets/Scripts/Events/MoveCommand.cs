using System;
using Behaviours;
using UnityEngine;

namespace Events {
    public class MoveCommand : EventArgs {
        public MoveCommand(UnitBehaviour unit, Vector2 destination) {
            Unit = unit;
            Destination = destination;
        }

        public UnitBehaviour Unit { get; }
        public Vector2 Destination { get; }
    }
}