using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public Pistol pistol;
    public AK47 ak47;
    public Sniper sniper;

    // ==========================================
    // PLAYER NUMBER
    // ==========================================

    public int playerNumber = 1;

    // ==========================================
    // GET SHOOT KEY
    // ==========================================

    private KeyCode GetShootKey()
    {
        switch (playerNumber)
        {
            case 1:
                return KeyCode.F;

            case 2:
                return KeyCode.RightControl;

            case 3:
                return KeyCode.O;

            case 4:
                return KeyCode.Keypad0;

            default:
                return KeyCode.F;
        }
    }

    // ==========================================
    // UPDATE
    // ==========================================

    private void Update()
    {
        KeyCode shootKey = GetShootKey();

        // ======================================
        // AK47
        // ======================================

        if (
            ak47 != null &&
            ak47.gameObject.activeInHierarchy
        )
        {
            if (Input.GetKey(shootKey))
            {
                ak47.Shoot();
            }

            return;
        }

        // ======================================
        // SNIPER
        // ======================================

        if (
            sniper != null &&
            sniper.gameObject.activeInHierarchy
        )
        {
            if (Input.GetKeyDown(shootKey))
            {
                sniper.Shoot();
            }

            return;
        }

        // ======================================
        // PISTOL
        // ======================================

        if (
            pistol != null &&
            pistol.gameObject.activeInHierarchy
        )
        {
            if (Input.GetKeyDown(shootKey))
            {
                pistol.Shoot();
            }
        }
    }

    // ==========================================
    // EQUIP PISTOL
    // ==========================================

    public void EquipPistol()
    {
        DisableAllWeapons();

        if (pistol != null)
        {
            pistol.gameObject.SetActive(true);

            Debug.Log(
                "Player " +
                playerNumber +
                " equipped Pistol"
            );
        }
    }

    // ==========================================
    // EQUIP AK47
    // ==========================================

    public void EquipAK47()
    {
        DisableAllWeapons();

        if (ak47 != null)
        {
            ak47.gameObject.SetActive(true);

            Debug.Log(
                "Player " +
                playerNumber +
                " equipped AK47"
            );
        }
    }

    // ==========================================
    // EQUIP SNIPER
    // ==========================================

    public void EquipSniper()
    {
        DisableAllWeapons();

        if (sniper != null)
        {
            sniper.gameObject.SetActive(true);

            Debug.Log(
                "Player " +
                playerNumber +
                " equipped Sniper"
            );
        }
    }

    // ==========================================
    // DISABLE ALL WEAPONS
    // ==========================================

    private void DisableAllWeapons()
    {
        if (pistol != null)
        {
            pistol.gameObject.SetActive(false);
        }

        if (ak47 != null)
        {
            ak47.gameObject.SetActive(false);
        }

        if (sniper != null)
        {
            sniper.gameObject.SetActive(false);
        }
    }
}