using UnityEngine;

namespace TGB.SniperVsRunner
{
    public class MovePlayer : MonoBehaviour
    {
        public GameObject thePlayer;
        public GameObject thePlayerCapsule; // Mesh and collision

        public void TeleportHere(Vector3 position, Quaternion rotation)
        {
            thePlayer.transform.SetPositionAndRotation(position, rotation);

            // Also inform the player to change mode
            thePlayer.GetComponent<FirstPersonController>().enabled = false;
            thePlayer.GetComponent<TurretController>().enabled = true;
            Destroy(thePlayer.GetComponent<Rigidbody>());

            thePlayerCapsule.SetActive(false);
        }
    }
}