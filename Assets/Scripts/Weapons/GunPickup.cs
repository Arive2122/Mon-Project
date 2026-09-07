using UnityEngine;
using Platformer.Mechanics;

public class GunPickup : MonoBehaviour
{
    public enum WeaponType
    {
        Pistol,
        AK47,
        Sniper
    }

    [Header("Weapon Given By This Pickup")]
    public WeaponType weaponType;

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

        switch (weaponType)
        {
            case WeaponType.Pistol:

                weaponController.EquipPistol();

                Debug.Log(
                    "Player picked up Pistol!"
                );

                break;


            case WeaponType.AK47:

                weaponController.EquipAK47();

                Debug.Log(
                    "Player picked up AK47!"
                );

                break;


            case WeaponType.Sniper:

                weaponController.EquipSniper();

                Debug.Log(
                    "Player picked up Sniper!"
                );

                break;
        }

        Destroy(gameObject);
    }
}