using UnityEngine;

namespace TGB.SniperVsRunner
{
    public class ButtonPressAction : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            // Only if the game is on

            {
                Vector3 touchPos = Input.mousePosition;

                // Check if the user has clicked
                bool aTouch = Input.GetMouseButtonDown(0);

                if (aTouch)
                {
                    // Debug.Log( "Moused moved to point " + touchPos );

                    CheckBoard(touchPos);
                }
            }
        }

        void CheckBoard(Vector3 touchPos)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touchPos);

            if (Physics.Raycast(ray, out hit, 2))
            {
                Transform objectHit = hit.transform;

                if (objectHit.gameObject.GetComponent<GotPressed>() != null)
                {
                    objectHit.gameObject.GetComponent<GotPressed>().WasPressed();
                }
            }
        }
    }
}
