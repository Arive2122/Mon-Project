using Platformer.Core;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    public class PlayerDeath : Simulation.Event<PlayerDeath>
    {
        PlatformerModel model =
            Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            Debug.Log("PLAYER DEATH EXECUTED");

            var player = model.player;

            if (player == null)
            {
                Debug.LogError("PLAYER NOT FOUND!");
                return;
            }

            // Kill player
            if (
                player.health != null &&
                player.health.IsAlive
            )
            {
                player.health.Die();
            }

            // Stop movement
            player.velocity = Vector2.zero;

            // Disable controls
            player.controlEnabled = false;

            // Disable collider
            if (player.collider2d != null)
            {
                player.collider2d.enabled = false;
            }

            // Animation
            if (player.animator != null)
            {
                player.animator.SetTrigger("hurt");
                player.animator.SetBool("dead", true);
            }

            // Stop camera following dead player
            if (model.virtualCamera != null)
            {
                model.virtualCamera.Follow = null;
                model.virtualCamera.LookAt = null;
            }

            // Play death sound
            if (
                player.audioSource != null &&
                player.ouchAudio != null
            )
            {
                player.audioSource.PlayOneShot(
                    player.ouchAudio
                );
            }

            Debug.Log("RESPAWN SCHEDULED");

            // Respawn after 2 seconds
            Simulation.Schedule<PlayerSpawn>(2f);
        }
    }
}