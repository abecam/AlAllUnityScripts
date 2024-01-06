using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace TGB.SniperVsRunner
{
    public class SpawnPlayerHere : NetworkBehaviour
    {
        public GameObject baseTurret;
        public GameObject barrel;
        public GameObject explosionFireBase;
        public Transform projectileStartPoint;

        [Command(requiresAuthority = false)]
        public void TeleportPlayer()
        {
            Camera.main.GetComponent<MovePlayer>().TeleportHere(this.transform);
            Camera.main.GetComponent<MovePlayer>().AttachTurret(baseTurret, barrel, explosionFireBase, projectileStartPoint);
        }
    }
}
