using UnityEngine;

namespace Platformer {
    public class PlatformCollisionHandler : MonoBehaviour {

        Transform platform; // The platform, if any, we are on top of

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.CompareTag("Platform")) {
                // If the contact normal is poiting up, we've collided with the top of the platform
                ContactPoint contact = other.GetContact(0);
                if(contact.normal.y < 0.5f) return;

                platform = other.transform;
                transform.SetParent(platform);
            }
        }

        private void OnCollisionExit(Collision other) {
            if (other.transform.CompareTag("MovingPlatform")) {
                transform.SetParent(null);
                platform = null;
            }
        }
    }
}
