using UnityEngine;
using Mirror;

namespace TGB.SniperVsRunner
{
    public class PauseUnPause : NetworkBehaviour
    {
        public FirstPersonController controlScript;
        // Start is called before the first frame update
        void Start()
        {
            if (!isLocalPlayer)
            {
                controlScript.enabled = false;
            }
        }

        int coolDown = 0;
        // Update is called once per frame
        void Update()
        {
            if (coolDown == 0 && isLocalPlayer)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
                {   
                    controlScript.enabled = !controlScript.enabled;
                    Cursor.lockState = (controlScript.enabled)?CursorLockMode.Locked : CursorLockMode.None;

                    coolDown = 30; // Prevent switching back immediately
                }
            }
            if (coolDown > 0)
            {
                coolDown--;
            }
        }
    }
}
