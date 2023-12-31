using UnityEngine;
using Mirror;

namespace TGB.SniperVsRunner
{
    internal class Spawner
    {
        [ServerCallback]
        internal static void InitialSpawn()
        {
            for (int i = 0; i < 10; i++)
                SpawnReward();
        }

        [ServerCallback]
        internal static void SpawnReward()
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-19, 20), 1, Random.Range(-19, 20));
            NetworkServer.Spawn(Object.Instantiate(NetworkRoomManagerExt.singleton.rewardPrefab, spawnPosition, Quaternion.identity));
        }
    }
}
