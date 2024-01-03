using UnityEngine;
using Mirror;

namespace TGB.SniperVsRunner
{
    public class GotPressed : NetworkBehaviour
    {
        [SyncVar] public bool isActivated = false;
        [SyncVar] public bool isWorking = true;
        public bool isTurretButton = false;
        public GameObject parentToSwitchOn;
        public ActivateWhenOthersPressed siblingToInform;
        public SpawnPlayerHere playerSpawner;

        public void WasPressed()
        {
            cmdActivated();
        }

        [Command(requiresAuthority = false)]
        void cmdActivated()
        {
            if (isWorking)
            {
                isActivated = true;
                GetComponent<Renderer>().material.color = new Color(0, 1, 0);
                GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green);

                if (parentToSwitchOn != null)
                {
                    parentToSwitchOn.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
                }
                if (siblingToInform != null)
                {
                    siblingToInform.ExternalButtonPressed();
                }
                if (isTurretButton)
                {
                    playerSpawner.TeleportPlayer();
                }
            }
        }

    }
}
