using UnityEngine;

namespace BuggedGames.ColonyWars.Terrain {
    [ExecuteAlways]
    public class TerrainBehaviour : MonoBehaviour {
        [Range(1, 255)] public int MovementPenalty;
    }
}