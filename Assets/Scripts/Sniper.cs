using UnityEngine;

public class Sniper : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float bulletSpeed = 30f;

    // 1 shot every 1 second
    public float fireCooldown = 1f;

    private float nextFireTime;

    public void Shoot()
    {
        if (Time.time < nextFireTime)
        {
            return;
        }

        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning(
                "Sniper: Bullet Prefab or Fire Point is missing."
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