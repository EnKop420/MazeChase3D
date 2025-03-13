using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinSpawner : NetworkBehaviour
{
    public GameObject coinPrefab;
    public int numberOfCoinsToSpawn = 10;
    public GameObject spawnPlane; // Assign the plane in the Inspector
    public int maxSpawnAttempts = 100;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        for (int i = 0; i < numberOfCoinsToSpawn; i++)
        {
            SpawnCoins();
        }
    }

    void SpawnCoins()
    {
        if (spawnPlane == null)
        {
            Debug.LogError("Spawn plane is not assigned!");
            return;
        }

        SpawnCoinServerRpc();
    }

    [ServerRpc] // Only runs on the server when called from a client
    private void SpawnCoinServerRpc()
    {
        Vector3 randomPosition = GetRandomPositionOnPlane();
        GameObject newCoin = Instantiate(coinPrefab, randomPosition, Quaternion.identity);
        newCoin.GetComponent<NetworkObject>().Spawn(); // Make sure the coin is spawned properly
        //while (true)
        //{
        //    GameObject testCoin = Instantiate(coinPrefab, randomPosition, Quaternion.identity);
        //    if (!IsCollidingWithWalls(testCoin))
        //    {

        //        return;
        //    }
        //}
    }

    Vector3 GetRandomPositionOnPlane()
    {
        MeshRenderer planeRenderer = spawnPlane.GetComponent<MeshRenderer>();
        if (planeRenderer == null)
        {
            Debug.LogError("Spawn plane must have a MeshRenderer!");
            return Vector3.zero;
        }

        Bounds planeBounds = planeRenderer.bounds;
        float x = Random.Range(planeBounds.min.x, planeBounds.max.x);
        float z = Random.Range(planeBounds.min.z, planeBounds.max.z);
        float y = planeBounds.center.y; // Keep the y-coordinate at the plane's height

        return new Vector3(x, y, z);
    }

    bool IsCollidingWithWalls(GameObject obj)
    {
        Collider objCollider = obj.GetComponent<Collider>();
        if (objCollider == null)
        {
            Debug.LogError("Coin prefab must have a collider!");
            return true;
        }

        Collider[] hitColliders = Physics.OverlapBox(objCollider.bounds.center, objCollider.bounds.extents, obj.transform.rotation);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != obj && hitCollider.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }
}
