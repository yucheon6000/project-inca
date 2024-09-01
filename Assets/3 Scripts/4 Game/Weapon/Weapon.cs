using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("[@Debug]")]
    [SerializeField]
    private WeaponInformation information;

    [Space]
    [SerializeField]
    private int currentAmmoInMagazine;
    private float timer = 0;

    public bool IsReloading { get; private set; } = false;
    public bool HasAmmo => currentAmmoInMagazine > 0;
    public bool ShouldReload => !HasAmmo;
    private bool CanShoot() => HasAmmo && timer > information.ShootDelay;

    public void Init(WeaponInformation information)
    {
        this.information = information;
        currentAmmoInMagazine = information.MaxAmmo;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (IsReloading)
        {
            UpdateReload();
            return;
        }

        if (ShouldReload /* && Shaking controller? */)
            Reload();

        if (CanShoot() && IncaInput.GetButton(IncaButtonCode.RightTrigger))
            Shoot();
    }

    private void UpdateReload()
    {
        if (timer < information.ReloadDelay) return;

        // If finish reload.
        currentAmmoInMagazine = information.MaxAmmo;
        timer = information.ShootDelay;
        IsReloading = false;
    }

    private void Reload()
    {
        timer = 0;
        IsReloading = true;
    }

    protected void Shoot()
    {
        if (IncaInput.TargetGameObject != null && IncaInput.TargetGameObject.TryGetComponent<DamagableEnemy>(out DamagableEnemy enemy))
        {
            enemy.TakeDamage(information.WeaponInfoForTargeting.Power);
            GameObject clone = Instantiate(information.WeaponInfoForTargeting.HitEffectPrefab, IncaInput.HitPoint, Quaternion.identity);
        }
        else
        {
            Instantiate(information.WeaponInfoForNonTargeting.ProjectilePrefab,
                        IncaData.UserRightHandPosition,
                        Quaternion.LookRotation(IncaData.UserRightHandTrasnform.forward),
                        IncaData.UserCarTransform);
        }

        timer = 0;
    }
}
