using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

namespace TGB.SniperVsRunner
{
    public class NetworkedFireSniper : NetworkBehaviour
    {
        private ILogger myLogger = Debug.unityLogger;

        [Header("Components")]
        public TextMeshPro healthBar;
        // TODO: Particles or FX for shoot
        public ParticleSystem explosion;
        public Camera theCamera;

        [Header("Firing")]
        public KeyCode shootKey = KeyCode.Space;
        public GameObject projectilePrefab;
        public Transform projectileMount;

        [Header("Stats")]
        [SyncVar] public int health = 4;
        [SyncVar] public int lostCount = 0;

        PlayerScore playerScore;

        private void Start()
        {
            explosion.Stop();
            if (!isLocalPlayer)
            {
                theCamera.enabled = false;
            }
            playerScore = GetComponent<PlayerScore>();
        }

        void Update()
        {
            // always update health bar.
            // (SyncVar hook would only update on clients, not on server)
            healthBar.text = new string('-', health);

            // take input from focused window only
            if (!Application.isFocused) return;

            // movement for local player
            if (isLocalPlayer)
            {
                // shoot
                if (Input.GetKeyDown(shootKey))
                {
                    CmdFire();
                }
            }
        }

        internal void scoreOneDeath()
        {
            playerScore.score++;
            lostCount--;
            if (lostCount < 0)
                lostCount = 0;
        }

        // this is called on the server
        [Command]
        void CmdFire()
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileMount.position, projectileMount.rotation);
            projectile.GetComponent<NetworkProjectile>().parent = this;
            NetworkServer.Spawn(projectile);
            RpcOnFire();
        }

        // this is called on the tank that fired for all observers
        [ClientRpc]
        void RpcOnFire()
        {
            // Trigger FX
            explosion.Play();
        }

        [ServerCallback]
        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<NetworkProjectile>() != null)
            {
                myLogger.Log("OnTriggerEnter", "--- Got hit by projectile");
                --health;
                if (health == 0)
                {
                    //NetworkServer.Destroy(gameObject);

                    // To do: increase score, do not destroy object
                    other.gameObject.GetComponent<NetworkProjectile>().scoreOneDeath();

                    lostCount++;
                    if (lostCount > 16)
                    {
                        lostCount = 16;
                    }

                    // Back to full health + bonus
                    health = 4 + lostCount;
                }
            }
        }
    }
}
