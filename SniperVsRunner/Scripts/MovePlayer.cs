using System;
using UnityEngine;

namespace TGB.SniperVsRunner
{
    public class MovePlayer : MonoBehaviour
    {
        public GameObject thePlayer;
        public GameObject theJoin;
        public GameObject thePlayerCapsule; // Mesh and collision

        public void TeleportHere(Transform turretSpawnPoint)
        {
            thePlayer.GetComponent<FirstPersonController>().enabled = false;

            thePlayer.transform.parent = turretSpawnPoint.transform;
            thePlayer.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            theJoin.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            Camera.main.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            // Also inform the player to change mode 
            thePlayer.GetComponent<TurretController>().enabled = true;
            Destroy(thePlayer.GetComponent<Rigidbody>());

            thePlayerCapsule.SetActive(false);
        }

        internal void AttachTurret(GameObject baseTurret, GameObject barrel, GameObject explosionFireBase, Transform projectileStartPoint)
        {   
            thePlayer.GetComponent<TurretController>().AttachTurret(baseTurret, barrel, explosionFireBase, projectileStartPoint);
        }
    }
}