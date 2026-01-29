using UnityEngine;

public class PickupWeapon : MonoBehaviour
{
    [SerializeField] WeaponStat weapon;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pik = other.GetComponent<IPickup>();

        if(pik != null)
        {

            SoundManager.instance.PlaySound3D("pickupweapon", transform.position);
            weapon.ammoCur = weapon.ammoMax;
            pik.GetWeaponStats(weapon);
            Destroy(gameObject);
        }
    }
}
