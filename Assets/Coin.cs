using Unity.Netcode;
using UnityEngine;

public class Coin : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return; // Only handle collisions on the server
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Only handle collisions on the server

        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerCoinCollection>()?.CollectCoinServerRpc();
            DespawnCoinServerRpc(); // Call the Server RPC
        }
    }

    [ServerRpc(RequireOwnership = false)] // Allow server-side calls without ownership
    private void DespawnCoinServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}
