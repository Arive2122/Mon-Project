using UnityEngine;
using UnityEngine.InputSystem;

public class Pistol : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 15f;

    // Time between shots.
    public float shootCooldown = 0.3f;

    // Tracks when the pistol can shoot again.
    private float nextShootTime = 0f;

    public void Shoot()
    {
        // Check cooldown.
        if (Time.time < nextShootTime)
        {
            return;
        }

        // Check required references.
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning(
                "Pistol: Bullet Prefab or Fire Point is missing."
            );

            return;
        }

        // Start cooldown.
        nextShootTime = Time.time + shootCooldown;

        // Create bullet.
        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        // Get bullet Rigidbody2D.
        Rigidbody2D rb =
            bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Fire in the direction the FirePoint faces.
            rb.linearVelocity =
                firePoint.right * bulletSpeed;
        }
    }
}