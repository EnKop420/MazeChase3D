using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    public NetworkManager networkManager;
    public Button startHostButton;
    public Button joinButton;
    public Button cancelButton;

    public GameObject startMenuCanvas;
    public GameObject waitingForClientCanvas;

    private int maxPlayers = 2; // Set the minimum and maximum number of players

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startHostButton.onClick.AddListener(StartHost);
        waitingForClientCanvas.SetActive(false); // Ensure it's disabled initially.
        joinButton.onClick.AddListener(StartClient);
        cancelButton.onClick.AddListener(CloseHost);
    }

    void StartHost()
    {
        if (networkManager != null)
        {
            networkManager.StartHost();
            CloseMenusAndLoadGame();

            // Subscribe to the client connect event
            networkManager.OnClientConnectedCallback += OnClientConnected;
            networkManager.OnClientDisconnectCallback += OnClientDisconnected; //For disconnections.
        }
        else
        {
            Debug.LogError("Network Manager not assigned!");
        }
    }

    void StartClient()
    {
        if (networkManager != null)
        {
            networkManager.StartClient(); // Start client on localhost.
            networkManager.OnClientConnectedCallback += OnClientConnectedClient;
            networkManager.OnClientDisconnectCallback += OnClientDisconnectClient;
        }
        else
        {
            Debug.LogError("Network Manager not assigned!");
        }
    }

    void OnDestroy()
    {
        if (networkManager != null)
        {
            networkManager.OnClientConnectedCallback -= OnClientConnected;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
            networkManager.OnClientConnectedCallback -= OnClientConnectedClient;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnectClient;
        }
    }

    void CloseHost()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
            networkManager.OnClientConnectedCallback -= OnClientConnected;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
            startMenuCanvas.SetActive(true);
            waitingForClientCanvas.SetActive(false);
        }
    }

    void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (NetworkManager.Singleton.ConnectedClients.Count > maxPlayers)
            {
                // Reject the connection if the maximum number of players is exceeded
                NetworkManager.Singleton.DisconnectClient(clientId);
                Debug.Log($"Client {clientId} rejected. Max players reached.");
            }
            else if (clientId != NetworkManager.Singleton.LocalClientId)
            {
                // A client has joined!
                if (NetworkManager.Singleton.ConnectedClients.Count == maxPlayers)
                {
                    networkManager.OnClientConnectedCallback -= OnClientConnected; // Unsubscribe.
                    networkManager.OnClientDisconnectCallback -= OnClientDisconnected; //Unsubscribe.
                    CloseMenusAndLoadGame();
                }
            }
        }
    }

    void OnClientDisconnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (NetworkManager.Singleton.ConnectedClients.Count < maxPlayers)
            {
                networkManager.OnClientConnectedCallback += OnClientConnected;
            }
        }
    }

    void CloseMenusAndLoadGame()
    {
        startMenuCanvas.SetActive(false);
        waitingForClientCanvas.SetActive(false);
        // Load your player spawning logic or scene management here.
        Debug.Log("Game Starting!");
    }

    void OnClientConnectedClient(ulong clientId)
    {
        if (NetworkManager.Singleton.IsClient && clientId == NetworkManager.Singleton.LocalClientId)
        {
            networkManager.OnClientConnectedCallback -= OnClientConnectedClient;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnectClient;
            waitingForClientCanvas.SetActive(false);
            Debug.Log("Connected to host!");
            CloseMenusAndLoadGame();
        }
    }

    void OnClientDisconnectClient(ulong clientId)
    {
        if (NetworkManager.Singleton.IsClient && clientId == NetworkManager.Singleton.LocalClientId)
        {
            networkManager.OnClientConnectedCallback -= OnClientConnectedClient;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnectClient;
            waitingForClientCanvas.SetActive(false);
            startMenuCanvas.SetActive(true);
            Debug.Log("Disconnected from host.");
        }
    }
}
