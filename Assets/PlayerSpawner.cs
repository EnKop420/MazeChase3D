using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public GameObject playerPrefab;
    public EnemyAI enemyAI; // Drag the EnemyAI object into this field in the Inspector

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkObject playerNetworkObject = player.GetComponent<NetworkObject>();

        playerNetworkObject.SpawnAsPlayerObject(OwnerClientId);
    }
}
