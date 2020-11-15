using System;
using UnityEngine;

public class MoveCommand : EventArgs {
    public UnitBehaviour Unit { get; }
    public Vector2 Destination { get; }

    public MoveCommand(UnitBehaviour unit, Vector2 destination) {
        Unit = unit;
        Destination = destination;
    }
}
