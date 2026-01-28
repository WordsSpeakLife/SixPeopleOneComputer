using UnityEngine;

public class PickupWeapon : MonoBehaviour
{
    [SerializeField] WeaponStat weapon;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pik = other.GetComponent<IPickup>();

        if(pik != null)
        {
            weapon.ammoCur = weapon.ammoMax;
            pik.GetWeaponStats(weapon);
            Destroy(gameObject);
        }
    }
}
