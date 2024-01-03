using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace TGB.SniperVsRunner
{
    public class ActivateWhenOthersPressed : NetworkBehaviour
    {
        public List<GotPressed> buttonsToActivate;

        // Start is called before the first frame update
        void Start()
        {
            GetComponent<GotPressed>().isWorking = false;
        }

        // One button was pressed, check if we can go green
        [Command(requiresAuthority = false)]
        public void ExternalButtonPressed()
        {
            bool allPressed = true;

            foreach (GotPressed oneButton in buttonsToActivate)
            {
                if (!oneButton.GetComponent<GotPressed>().isActivated)
                {
                    allPressed = false;

                    break;
                }
            }

            if (allPressed)
            {
                GetComponent<GotPressed>().isWorking = true;

                GetComponent<Renderer>().material.color = new Color(0, 1, 0);
                GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green);
            }
        }
    }
}
