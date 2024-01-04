using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace TGB.SniperVsRunner
{
    public class CreateNPCs : NetworkBehaviour
    {
        // Create NPCs on a grid separated by 1,1
        public GameObject NPCPrefab;
        public int nbOfNPCs = 20;
        public int nbInRow = 10;
        public GameObject targetParent;

        // Start is called before the first frame update
        void Start()
        {
            if (isServer)
            { 
                addNpcs();
            }
        }

        [Command(requiresAuthority = false)]
        void addNpcs()
        {
            Vector3 initialPos = GetComponent<Transform>().position;

            for (int iNPC = 0; iNPC < nbOfNPCs; iNPC++)
            {
                Vector3 position = new Vector3(initialPos.x + (iNPC % nbInRow), initialPos.y, initialPos.z + (iNPC / nbInRow));
                GameObject npc = Instantiate(NPCPrefab, position, Quaternion.identity);
                npc.transform.parent = this.transform;
                npc.GetComponent<FindTarget>().targetsParent = targetParent;
                npc.GetComponent<FindTarget>().PrepareTargets();
                NetworkServer.Spawn(npc);
            }
        }
    }
}
