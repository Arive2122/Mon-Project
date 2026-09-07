using UnityEngine;
using Platformer.Mechanics;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;

    private float lastTeleportTime = -1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Find PlayerController on this object OR its parent
        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player == null)
        {
            return;
        }

        // Prevent instant teleporting back
        if (Time.time < lastTeleportTime + 0.5f)
        {
            return;
        }

        if (linkedPortal == null)
        {
            Debug.LogWarning("Portal: Linked Portal is not assigned!");
            return;
        }

        Debug.Log("PORTAL: Player detected!");

        // Move the player's root object
        Transform playerTransform = player.transform;

        Rigidbody2D playerRb =
            player.GetComponent<Rigidbody2D>();

        Vector3 destination =
            linkedPortal.transform.position;

        if (playerRb != null)
        {
            playerRb.position = destination;
            playerRb.linearVelocity = Vector2.zero;
        }
        else
        {
            playerTransform.position = destination;
        }

        // Prevent immediate teleport back
        lastTeleportTime = Time.time;
        linkedPortal.lastTeleportTime = Time.time;

        Debug.Log("PORTAL: Player teleported!");
    }
}