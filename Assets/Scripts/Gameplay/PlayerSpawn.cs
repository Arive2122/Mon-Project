using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    public class PlayerSpawn : Simulation.Event<PlayerSpawn>
    {
        PlatformerModel model =
            Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            Debug.Log("PLAYER SPAWN EXECUTED");

            var player = model.player;

            if (player == null)
            {
                Debug.LogError("PLAYER NOT FOUND!");
                return;
            }

            // Find SpawnPoint
            GameObject spawnObject =
                GameObject.Find("SpawnPoint");

            if (spawnObject == null)
            {
                Debug.LogError(
                    "SpawnPoint not found! " +
                    "Name your spawn object exactly: SpawnPoint"
                );

                return;
            }

            Debug.Log(
                "SPAWNING PLAYER AT: " +
                spawnObject.transform.position
            );

            // Reset movement
            player.velocity = Vector2.zero;

            // Reset gravity
            player.gravityModifier = 1f;

            // Restore full health
            if (player.health != null)
            {
                player.health.ResetHealth();
            }

            // Teleport player
            player.Teleport(
                spawnObject.transform.position
            );

            // Enable collider
            if (player.collider2d != null)
            {
                player.collider2d.enabled = true;
            }

            // Reset jump state
            player.jumpState =
                PlayerController.JumpState.Grounded;

            // Reset animation
            if (player.animator != null)
            {
                player.animator.SetBool(
                    "dead",
                    false
                );

                player.animator.SetBool(
                    "grounded",
                    true
                );
            }

            // Play respawn sound
            if (
                player.audioSource != null &&
                player.respawnAudio != null
            )
            {
                player.audioSource.PlayOneShot(
                    player.respawnAudio
                );
            }

            // Camera follows player again
            if (model.virtualCamera != null)
            {
                model.virtualCamera.Follow =
                    player.transform;

                model.virtualCamera.LookAt =
                    player.transform;
            }

            // Enable controls immediately
            player.controlEnabled = true;

            Debug.Log("PLAYER RESPAWN COMPLETE");
        }
    }
}