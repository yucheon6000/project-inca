using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

[RequireComponent(typeof(WeaponStatus))]
public class Weapon : MonoBehaviour
{
    private Character Owner { get; set; }
    public WeaponStatus Status { get; private set; }

    [Header("[Projectiles]")]
    [SerializeField]
    protected List<Projectile> projectiles;

    [Header("[@Debug]")]
    [SerializeField]
    private int currentAmmoInMagazine;
    [SerializeField]
    protected float timer = 0;

    public bool IsReloading { get; private set; } = false;
    public bool HasAmmo => currentAmmoInMagazine > 0;
    public bool ShouldReload => !IsReloading && !HasAmmo;

    protected bool IsReadyToShoot() => !IsReloading && HasAmmo && timer > Status.CurrentShootDelay;
    protected virtual bool IsShootInputReceived() => false;

    private void Awake()
    {
        Status = GetComponent<WeaponStatus>();
    }

    public virtual void Init(Character owner)
    {
        Owner = owner;

        if (Status == null)
            Status = GetComponent<WeaponStatus>();


        Status.Init();
        currentAmmoInMagazine = Status.CurrentAmmoPerMagazine;
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

        if (IsReadyToShoot() && IsShootInputReceived())
            Shoot();
    }

    private void UpdateReload()
    {
        if (timer < Status.CurrentReloadDelay) return;

        // If finish reload.
        currentAmmoInMagazine = Status.CurrentAmmoPerMagazine;
        timer = Status.CurrentShootDelay;
        IsReloading = false;
    }

    private void Reload()
    {
        timer = 0;
        IsReloading = true;
    }

    protected virtual void Shoot()
    {
        timer = 0;
    }
}
