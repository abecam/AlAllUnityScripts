using UnityEngine;
using Mirror;

namespace TGB.SniperVsRunner
{
    public class SelectTypeOfPlayer : NetworkBehaviour
    {
        public GameObject thePlayer;
        public NetworkedFireSniper theSniperFireScript;
        bool isSniper = false;

        // Start is called before the first frame update
        void Start()
        {
            // Check the altitude to see which kind of player we are: up is sniper, down is runner.
            if (thePlayer.transform.position.y > 10)
            {
                isSniper = true;
            }
            else
            {
                isSniper = false;
                theSniperFireScript.enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
