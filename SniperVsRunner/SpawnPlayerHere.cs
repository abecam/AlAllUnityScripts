using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace TGB.SniperVsRunner
{
    public class SpawnPlayerHere : NetworkBehaviour
    {
        [Command(requiresAuthority = false)]
        public void TeleportPlayer()
        {
            Camera.main.GetComponent<MovePlayer>().TeleportHere(GetComponent<Transform>().position, GetComponent<Transform>().rotation);
        }
    }
}
