using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gate Guardian/Weapon Information")]
public class WeaponInformation : ScriptableObject
{
    [Header("Aim")]
    [SerializeField]
    private Sprite aimSprite;
    public Sprite AimSprite => aimSprite;

    [Header("Ammo")]
    [SerializeField]
    private int maxAmmo;
    public int MaxAmmo => maxAmmo;

    [Header("Reload")]
    [SerializeField]
    private float reloadDelay;
    public float ReloadDelay => reloadDelay;

    [Header("Delay")]
    [SerializeField]
    private float shootDelay;
    public float ShootDelay => shootDelay;

    [Header("Hitscan (Targeting)")]
    [SerializeField]
    private WeaponInformationDetail weaponInfoForTargeting;
    public WeaponInformationDetail WeaponInfoForTargeting => weaponInfoForTargeting;

    [Header("Non-Targeting")]
    [SerializeField]
    private WeaponInformationDetail weaponInfoForNonTargeting;
    public WeaponInformationDetail WeaponInfoForNonTargeting => weaponInfoForNonTargeting;

    [Serializable]
    public class WeaponInformationDetail
    {
        [SerializeField]
        private int power;
        public int Power => power;
        [SerializeField]
        private GameObject projectilePrefab;
        public GameObject ProjectilePrefab => projectilePrefab;
        [SerializeField]
        private GameObject hitEffectPrefab;
        public GameObject HitEffectPrefab => hitEffectPrefab;
    }
}
