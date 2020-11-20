using System;
using UnityEngine;
using BuggedOutGames.ColonyWars.Units;

namespace BuggedOutGames.ColonyWars.Events {
    public class MoveCommand : EventArgs {
        public MoveCommand(UnitBehaviour unit, Vector2 destination) {
            Unit = unit;
            Destination = destination;
        }

        public UnitBehaviour Unit { get; }
        public Vector2 Destination { get; }
    }
}