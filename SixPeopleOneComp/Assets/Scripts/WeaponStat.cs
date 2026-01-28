using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu]
public class WeaponStat : ScriptableObject
{
    [Range(0, 20)] public int shootDamage;
    [Range(0, 500)] public float shootDistance;
    [Range(0.1f, 3)] public float shootRate;
    [Range(0, 10)] public float shootSpeed;
    [SerializeField] GameObject bullet;

    public bool isTri;

    public Sprite weaponIcon;

    public int ammoCur;
    [Range(5, 50)] public int ammoMax;
}
