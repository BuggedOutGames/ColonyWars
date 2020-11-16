using System;
using UnityEngine;

using BuggedGames.ColonyWars.Units;

namespace BuggedGames.ColonyWars.Events {
    public class MoveCommand : EventArgs {
        public MoveCommand(UnitBehaviour unit, Vector2 destination) {
            Unit = unit;
            Destination = destination;
        }

        public UnitBehaviour Unit { get; }
        public Vector2 Destination { get; }
    }
}