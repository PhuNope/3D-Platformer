using UnityEngine;

namespace Platformer {
    public class GroundChecker : MonoBehaviour {

        [SerializeField] float groundDistance = 0.08f;
        [SerializeField] LayerMask groundLayers;

        public bool IsGrouned { get; private set; }

        private void Update() {
            IsGrouned = Physics.SphereCast(transform.position, groundDistance, Vector3.down, out _, groundDistance, groundLayers);
        }
    }
}
