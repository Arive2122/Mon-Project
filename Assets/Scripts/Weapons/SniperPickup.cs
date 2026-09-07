using UnityEngine;
using Platformer.Mechanics;

public class SniperPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player =
            other.GetComponent<PlayerController>();

        if (player == null)
        {
            return;
        }

        PlayerWeaponController weaponController =
            other.GetComponent<PlayerWeaponController>();

        if (weaponController == null)
        {
            return;
        }

        weaponController.EquipSniper();

        Debug.Log("Player picked up Sniper!");

        Destroy(gameObject);
    }
}