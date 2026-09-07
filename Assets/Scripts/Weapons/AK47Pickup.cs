using UnityEngine;
using Platformer.Mechanics;

public class AK47Pickup : MonoBehaviour
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

        // Equip ONLY the AK47.
        weaponController.EquipAK47();

        Debug.Log(
            "Player picked up AK47!"
        );

        Destroy(gameObject);
    }
}