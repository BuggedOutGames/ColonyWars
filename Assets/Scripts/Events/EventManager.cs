using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

using BuggedGames.ColonyWars.Units;

namespace BuggedGames.ColonyWars.Events {
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


        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        public void OnMoveCommand(UnitBehaviour unit, Vector2 destination) {
            var handler = MoveCommandEvent;
            handler?.Invoke(this, new MoveCommand(unit, destination));
        }

        public void OnUnitEnteredVision(UnitBehaviour unit) {
            var handler = UnitEnteredVisionEvent;
            handler?.Invoke(this, unit);
        }

        public void OnUnitExitedVision(UnitBehaviour unit) {
            var handler = UnitExitedVisionEvent;
            handler?.Invoke(this, unit);
        }

        public void OnSelectUnit(UnitBehaviour unit) {
            var handler = SelectUnitEvent;
            handler?.Invoke(this, unit);
        }

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        public void OnDeSelectUnit(UnitBehaviour unit) {
            var handler = DeSelectUnitEvent;
            handler?.Invoke(this, unit);
        }

        public void OnStartMouseSelectionBoxEvent(Rect bounds) {
            var handler = StartMouseSelectionBoxEvent;
            handler?.Invoke(this, bounds);
        }

        public void OnStopMouseSelectionBoxEvent() {
            var handler = StopMouseSelectionBoxEvent;
            handler?.Invoke(this, null);
        }
    }
}