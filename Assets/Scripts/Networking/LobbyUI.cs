using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [Header("Networking")]
    public LanDiscovery lanDiscovery;
    public LobbyManager lobbyManager;

    public float discoveryTimeout = 2f;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject lobbyPanel;

    [Header("Player Slots")]
    public GameObject playerSlot1;
    public GameObject playerSlot2;
    public GameObject playerSlot3;
    public GameObject playerSlot4;

    [Header("Lobby UI")]
    public TMP_Text playerCountText;
    public Button fightButton;

    private bool hasStartedNetwork = false;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        Debug.Log("LobbyUI started.");

        // Main menu visible
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("LobbyUI: Main Menu Panel is NOT assigned!");
        }

        // Lobby hidden
        if (lobbyPanel != null)
        {
            lobbyPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("LobbyUI: Lobby Panel is NOT assigned!");
        }

        // Fight button visible but disabled
        if (fightButton != null)
        {
            fightButton.gameObject.SetActive(true);
            fightButton.interactable = false;
        }
        else
        {
            Debug.LogError("LobbyUI: Fight Button is NOT assigned!");
        }

        UpdatePlayerSlots(0);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (NetworkManager.Singleton != null &&
            NetworkManager.Singleton.IsListening)
        {
            int count =
                NetworkManager.Singleton.ConnectedClients.Count;

            UpdatePlayerSlots(count);

            if (fightButton != null)
            {
                fightButton.gameObject.SetActive(true);

                fightButton.interactable =
                    NetworkManager.Singleton.IsHost &&
                    count >= 2 &&
                    count <= 4;
            }
        }
    }

    // =========================================================
    // LOCAL MULTIPLAYER BUTTON
    // =========================================================

    public void OnLocalMultiplayerClicked()
    {
        Debug.Log("LOCAL MULTIPLAYER CLICKED");

        // Prevent starting networking twice
        if (hasStartedNetwork)
        {
            Debug.Log("Network has already been started.");
            return;
        }

        // -----------------------------------------------------
        // Hide Main Menu
        // -----------------------------------------------------

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
            Debug.Log("Main Menu Panel hidden.");
        }
        else
        {
            Debug.LogError("Main Menu Panel is NULL!");
        }

        // -----------------------------------------------------
        // Show Lobby
        // -----------------------------------------------------

        if (lobbyPanel != null)
        {
            lobbyPanel.SetActive(true);
            Debug.Log("Lobby Panel shown.");
        }
        else
        {
            Debug.LogError("Lobby Panel is NULL!");
        }

        // -----------------------------------------------------
        // Start LAN discovery
        // -----------------------------------------------------

        hasStartedNetwork = true;

        StartCoroutine(TryAutoConnect());
    }

    // =========================================================
    // LAN AUTO CONNECT
    // =========================================================

    private IEnumerator TryAutoConnect()
    {
        Debug.Log("Starting LAN discovery...");

        if (lanDiscovery == null)
        {
            Debug.LogError(
                "LobbyUI: LanDiscovery is NOT assigned!"
            );

            yield break;
        }

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError(
                "LobbyUI: NetworkManager.Singleton is NULL!"
            );

            yield break;
        }

        // Start listening for an existing host
        lanDiscovery.StartListening();

        float elapsed = 0f;
        string hostIP = null;

        while (elapsed < discoveryTimeout)
        {
            hostIP = lanDiscovery.CheckForHost();

            if (!string.IsNullOrEmpty(hostIP))
            {
                Debug.Log(
                    "Existing host found at: " + hostIP
                );

                break;
            }

            elapsed += Time.deltaTime;

            yield return null;
        }

        lanDiscovery.StopListening();

        // -----------------------------------------------------
        // Existing host found
        // -----------------------------------------------------

        if (!string.IsNullOrEmpty(hostIP))
        {
            JoinHost(hostIP);
        }
        else
        {
            // -------------------------------------------------
            // No host found → become host
            // -------------------------------------------------

            StartNewHost();
        }
    }

    // =========================================================
    // JOIN EXISTING HOST
    // =========================================================

    private void JoinHost(string hostIP)
    {
        Debug.Log(
            "Attempting to join host: " + hostIP
        );

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError(
                "NetworkManager.Singleton is NULL!"
            );

            return;
        }

        UnityTransport transport =
            NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (transport == null)
        {
            Debug.LogError(
                "UnityTransport component not found!"
            );

            return;
        }

        transport.ConnectionData.Address = hostIP;

        bool started =
            NetworkManager.Singleton.StartClient();

        if (started)
        {
            Debug.Log(
                "Joined existing lobby at: " + hostIP
            );
        }
        else
        {
            Debug.LogError(
                "Failed to start Network Client!"
            );
        }
    }

    // =========================================================
    // START NEW HOST
    // =========================================================

    private void StartNewHost()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError(
                "NetworkManager.Singleton is NULL!"
            );

            return;
        }

        bool started =
            NetworkManager.Singleton.StartHost();

        if (started)
        {
            Debug.Log(
                "No existing lobby found."
            );

            Debug.Log(
                "Hosting a new lobby."
            );

            if (lanDiscovery != null)
            {
                lanDiscovery.StartBroadcasting();

                Debug.Log(
                    "LAN host broadcasting started."
                );
            }
        }
        else
        {
            Debug.LogError(
                "Failed to start Network Host!"
            );
        }
    }

    // =========================================================
    // UPDATE PLAYER SLOTS
    // =========================================================

    private void UpdatePlayerSlots(int playerCount)
    {
        if (playerSlot1 != null)
        {
            playerSlot1.SetActive(playerCount >= 1);
        }

        if (playerSlot2 != null)
        {
            playerSlot2.SetActive(playerCount >= 2);
        }

        if (playerSlot3 != null)
        {
            playerSlot3.SetActive(playerCount >= 3);
        }

        if (playerSlot4 != null)
        {
            playerSlot4.SetActive(playerCount >= 4);
        }

        if (playerCountText != null)
        {
            playerCountText.text =
                playerCount + " / 4 Players";
        }
    }

    // =========================================================
    // FIGHT BUTTON
    // =========================================================

    public void OnFightClicked()
    {
        Debug.Log("FIGHT BUTTON CLICKED");

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError(
                "NetworkManager not found!"
            );

            return;
        }

        // Only host can start the match
        if (!NetworkManager.Singleton.IsHost)
        {
            Debug.Log(
                "Only the host can start the game."
            );

            return;
        }

        int playerCount =
            NetworkManager.Singleton
                .ConnectedClients.Count;

        if (playerCount < 2)
        {
            Debug.Log(
                "Need at least 2 players!"
            );

            return;
        }

        if (playerCount > 4)
        {
            Debug.Log(
                "Maximum 4 players allowed!"
            );

            return;
        }

        Debug.Log(
            "FIGHT! Starting game..."
        );

        // Map scene loading will be added here.
    }

    // =========================================================
    // EXIT BUTTON
    // =========================================================

    public void OnExitClicked()
    {
        Debug.Log("EXIT BUTTON CLICKED");

#if UNITY_EDITOR
        // Stop Play Mode when testing inside Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
