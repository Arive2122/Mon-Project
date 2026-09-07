using UnityEngine;
using Platformer.Mechanics;

public class LocalMultiplayerManager : MonoBehaviour
{
    [Header("Player Prefab")]
    public GameObject playerPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Number Of Players")]
    [Range(1, 4)]
    public int numberOfPlayers = 4;

    private void Start()
    {
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        if (playerPrefab == null)
        {
            Debug.LogError(
                "LocalMultiplayerManager: Player Prefab is missing!"
            );

            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError(
                "LocalMultiplayerManager: No Spawn Points assigned!"
            );

            return;
        }

        int playersToSpawn =
            Mathf.Min(
                numberOfPlayers,
                spawnPoints.Length
            );

        for (int i = 0; i < playersToSpawn; i++)
        {
            if (spawnPoints[i] == null)
            {
                Debug.LogWarning(
                    "LocalMultiplayerManager: Spawn Point " +
                    (i + 1) +
                    " is missing!"
                );

                continue;
            }

            GameObject player = Instantiate(
                playerPrefab,
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );

            player.name =
                "Player" + (i + 1);

            // ==========================================
            // PLAYER CONTROLLER
            // ==========================================

            PlayerController controller =
                player.GetComponent<PlayerController>();

            if (controller != null)
            {
                controller.playerNumber =
                    i + 1;
            }
            else
            {
                Debug.LogError(
                    "Player prefab does not have " +
                    "PlayerController!"
                );
            }

            // ==========================================
            // WEAPON CONTROLLER
            // ==========================================

            PlayerWeaponController weaponController =
                player.GetComponent<PlayerWeaponController>();

            if (weaponController != null)
            {
                weaponController.playerNumber =
                    i + 1;
            }
            else
            {
                Debug.LogWarning(
                    "Player prefab does not have " +
                    "PlayerWeaponController!"
                );
            }

            Debug.Log(
                "Spawned Player " +
                (i + 1) +
                " at SpawnPoint " +
                (i + 1)
            );
        }
    }
}