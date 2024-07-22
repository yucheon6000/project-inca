using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gate Guardian/Weapon Information")]
public class WeaponInformation : ScriptableObject
{
    [SerializeField]
    private int power;
    public int Power => power;
    [SerializeField]
    private float shootDelay;
    public float ShootDelay => shootDelay;
    [SerializeField]
    private GameObject projectileEffectPrefab;
    public GameObject ProjectileEffectPrefab => projectileEffectPrefab;
    [SerializeField]
    private GameObject hitEffectPrefab;
    public GameObject HitEffectPrefab => hitEffectPrefab;
}
