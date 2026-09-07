using Unity.Netcode;

public class LobbyManager : NetworkBehaviour
{
    public NetworkVariable<int> playerCount = new NetworkVariable<int>(0);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientChanged;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientChanged;

            playerCount.Value = NetworkManager.Singleton.ConnectedClients.Count;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientChanged;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientChanged;
        }
    }

    void HandleClientChanged(ulong clientId)
    {
        playerCount.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }
}