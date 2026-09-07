using UnityEngine;

public class AK47 : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float bulletSpeed = 18f;

    // Time between bullets
    public float fireCooldown = 0.1f;

    private float nextFireTime;

    public void Shoot()
    {
        // Cooldown check
        if (Time.time < nextFireTime)
        {
            return;
        }

        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning(
                "AK47: Bullet Prefab or Fire Point is missing."
            );

            return;
        }

        nextFireTime =
            Time.time + fireCooldown;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Rigidbody2D rb =
            bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity =
                firePoint.right * bulletSpeed;
        }
    }
}