using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Random = UnityEngine.Random;

namespace TGB.SniperVsRunner
{
    public class NetworkProjectile : NetworkBehaviour
    {
        private ILogger myLogger = Debug.unityLogger;

        public float destroyAfter = 2;
        public Rigidbody rigidBody;
        public float force = 1000;
        public NetworkedFireSniper parent;
        public bool isTurretBullet = false;
        public List<ParticleSystem> explosions;
        private ParticleSystem chosenExplosion;
        private float timer=1000; // Will disappear after a while anyway
        internal GameObject[] players;

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfter);
        }

        // set velocity for server and client. this way we don't have to sync the
        // position, because both the server and the client simulate it.
        void Start()
        {
            rigidBody.AddForce(transform.forward * force);
            if (isTurretBullet)
            {
                foreach (ParticleSystem particle in explosions)
                {
                    particle.Stop();
                }
                int chosenExplosionIndex = (int)(explosions.Count * Random.value);

                chosenExplosion = explosions[chosenExplosionIndex];

                if (players == null)
                {
                    myLogger.Log("No player yet, null returned");
                }
                else if (players.Length == 0)
                {
                    myLogger.Log("No player yet, length 0 returned");
                }
            }
        }

        // destroy for everyone on the server
        [Server]
        void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        
        // ServerCallback because we don't want a warning
        // if OnTriggerEnter is called on the client
        [ServerCallback]
        void OnTriggerEnter(Collider co) => Explode();

        [Server]
        private void Explode()
        {
            if (isTurretBullet)
            {
                chosenExplosion.Play();
                timer = 15;
                // Now inform all players (including player) about their damage and push them accordingly
                foreach (GameObject player in players)
                {
                    if (player.GetComponent<Rigidbody>() != null)
                    { 
                        Vector3 pushForce = (player.transform.position - transform.position);
                        player.GetComponent<Rigidbody>().AddForce(pushForce);
                    }
                }
            }
            else
            {
                DestroySelf();
            }
        }

        internal void scoreOneDeath()
        {
            parent.scoreOneDeath();
        }

        private void Update()
        {
            if (isTurretBullet)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    DestroySelf();
                }
            }
        }
    }
}