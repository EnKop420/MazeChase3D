using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerCoinCollection : NetworkBehaviour
{
    private NetworkVariable<int> coinCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public int CoinCount => coinCount.Value; // Public getter for coin count

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Optional: UI setup or other client-specific initialization
        }
    }

    [ServerRpc]
    public void CollectCoinServerRpc()
    {
        coinCount.Value++;
        Debug.Log($"Player {OwnerClientId} collected a coin. Total coins: {coinCount.Value}");
    }

    private void OnGUI()
    {
        if (IsOwner)
        {
            GUI.Label(new Rect(10, 10, 200, 30), $"Coins: {coinCount.Value}");
        }
    }
}
