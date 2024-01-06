using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TGB.SniperVsRunner
{
    public class TurretController : MonoBehaviour
    {
        private GameObject baseTurret;
        private GameObject barrel;
        private GameObject explosionFireBase;

        public Camera playerCamera;

        public float fov = 60f;
        public bool invertCamera = false;
        public bool cameraCanMove = true;
        public float mouseSensitivity = 2f;
        public float maxLookAngle = 90f;
        public float minLookAngle = -30f;
        private float zoomValue = 1;

        // Crosshair
        public bool lockCursor = true;

        public GameObject SniperSight;
        private RawImage SniperSightRI;

        // Internal Variables
        private float yaw = 0.0f;
        private float pitch = 0.0f;
        private Image crosshairObject;

        public bool enableZoom = true;
        public bool holdToZoom = false;
        private KeyCode zoomKey = KeyCode.Mouse1;
        public float zoomFOV = 30f;
        public float zoomStepTime = 5f;

        // Internal Variables
        private bool isZoomed = false;

        private bool turretReady = false;

        internal void AttachTurret(GameObject baseTurret, GameObject barrel, GameObject explosionFireBase, Transform projectileStartPoint)
        {
            this.baseTurret = baseTurret;
            this.barrel = barrel;
            this.explosionFireBase = explosionFireBase;
            turretReady = true;

            explosionFireBase.GetComponent<ParticleSystem>().Stop();

            // Also change the fire mode
            GetComponent<NetworkedFireSniper>().isTurret = true;
            GetComponent<NetworkedFireSniper>().projectileMount = projectileStartPoint;
            GetComponent<NetworkedFireSniper>().explosion = explosionFireBase.GetComponent<ParticleSystem>();
        }

        private void Awake()
        {
            // Set internal variables
            playerCamera.fieldOfView = fov;
        }

        void Start()
        {
            SniperSightRI = SniperSight.GetComponent<RawImage>();
            SniperSightRI.color = new Color(1, 1, 1, 0);

            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        float camRotation;
        float t = 1.0f;

        private void Update()
        {
            if (turretReady)
            {
                // Control camera movement
                if (cameraCanMove)
                {
                    yaw = baseTurret.transform.localEulerAngles.z + Input.GetAxis("Mouse X") * mouseSensitivity;

                    if (!invertCamera)
                    {
                        pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
                    }
                    else
                    {
                        // Inverted Y
                        pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
                    }

                    // Clamp pitch between lookAngle
                    pitch = Mathf.Clamp(pitch, minLookAngle, maxLookAngle);

                    //transform.localEulerAngles = new Vector3(0, yaw, 0);
                    //playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
                    baseTurret.transform.localEulerAngles = new Vector3(0, 0, yaw);
                    barrel.transform.localEulerAngles = new Vector3(pitch, 0, 0);
                }

                if (enableZoom)
                {
                    // Changes isZoomed when key is pressed
                    // Behavior for toogle zoom
                    if (Input.GetKeyDown(zoomKey) && !holdToZoom)
                    {
                        if (!isZoomed)
                        {
                            isZoomed = true;
                        }
                        else
                        {
                            isZoomed = false;
                        }
                        t = 0.0f;
                    }

                    // Changes isZoomed when key is pressed
                    // Behavior for hold to zoom
                    if (holdToZoom)
                    {
                        if (Input.GetKeyDown(zoomKey))
                        {
                            if (!isZoomed)
                            {
                                t = 0.0f;
                            }
                            isZoomed = true;
                        }
                        else if (Input.GetKeyUp(zoomKey))
                        {
                            if (isZoomed)
                            {
                                t = 0.0f;
                            }
                            isZoomed = false;
                        }
                    }

                    // Lerps camera.fieldOfView to allow for a smooth transistion
                    if (isZoomed)
                    {
                        t += Time.deltaTime;

                        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV / zoomValue, zoomStepTime * Time.deltaTime);

                        float alpha = Mathf.Lerp(0, 1, t);
                        if (t > 1.0f)
                        {
                            alpha = 1;
                        }
                        SniperSightRI.color = new Color(1, 1, 1, alpha);
                    }
                    else if (!isZoomed)
                    {
                        t += Time.deltaTime;

                        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, zoomStepTime * Time.deltaTime);

                        float alpha = Mathf.Lerp(1, 0, t);
                        if (t > 1.0f)
                        {
                            alpha = 0;
                        }
                        SniperSightRI.color = new Color(1, 1, 1, alpha);
                    }

                    zoomValue += Input.mouseScrollDelta.y;

                    if (zoomValue < 1f)
                    {
                        zoomValue = 1f;
                    }
                    else if (zoomValue > 16f)
                    {
                        zoomValue = 16f;
                    }
                }
            }
        }
    }
}