using UnityEngine;

namespace FlowField {
    [ExecuteInEditMode]
    public class FlowFieldBehaviour : MonoBehaviour {
        [Range(1, 255)]
        public int Cost;
    }
}
