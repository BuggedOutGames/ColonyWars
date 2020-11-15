using System;
using UnityEngine;

public class EventManager {

    private static EventManager _instance;
    public static EventManager Instance => _instance ?? (_instance = new EventManager());
    
    public event EventHandler<MoveCommand> MoveCommandEvent;
    public event EventHandler<UnitBehaviour> UnitEnteredVisionEvent;
    public event EventHandler<UnitBehaviour> UnitExitedVisionEvent;
    public event EventHandler<UnitBehaviour> SelectUnitEvent;
    public event EventHandler<UnitBehaviour> DeSelectUnitEvent;
    public event EventHandler<Rect> StartMouseSelectionBoxEvent;
    public event EventHandler StopMouseSelectionBoxEvent;


    public void OnMoveCommand(UnitBehaviour unit, Vector2 destination) {
        EventHandler<MoveCommand> handler = MoveCommandEvent;
        handler?.Invoke(this, new MoveCommand(unit, destination));
    }

    public void OnUnitEnteredVision(UnitBehaviour unit) {
        EventHandler<UnitBehaviour> handler = UnitEnteredVisionEvent;
        handler?.Invoke(this, unit);
    }

    public void OnUnitExitedVision(UnitBehaviour unit) {
        EventHandler<UnitBehaviour> handler = UnitExitedVisionEvent;
        handler?.Invoke(this, unit);
    }

    public void OnSelectUnit(UnitBehaviour unit) {
        EventHandler<UnitBehaviour> handler = SelectUnitEvent;
        handler?.Invoke(this, unit);
    }
    
    public void OnDeSelectUnit(UnitBehaviour unit) {
        EventHandler<UnitBehaviour> handler = DeSelectUnitEvent;
        handler?.Invoke(this, unit);
    }

    public void OnStartMouseSelectionBoxEvent(Rect bounds) {
        EventHandler<Rect> handler = StartMouseSelectionBoxEvent;
        handler?.Invoke(this, bounds);
    }
    
    public void OnStopMouseSelectionBoxEvent() {
        EventHandler handler = StopMouseSelectionBoxEvent;
        handler?.Invoke(this, null);
    }
}
