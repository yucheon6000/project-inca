using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStatus : MonoBehaviour
{
    [Header("[[Weapon]]")]

    /*
    [Header("[Level]")]
    [SerializeField]
    private int currentLevel = 0;
    */

    [Header("[Ammo Per Magazine]")]
    [SerializeField]
    private int maxAmmoPerMagazine = 50;
    [SerializeField]
    private int defaultAmmoPerMagazine = 30;
    [SerializeField]
    private int currentAmmoPerMagazine;
    public int CurrentAmmoPerMagazine => currentAmmoPerMagazine;

    [Header("[Shoot Delay]")]
    [SerializeField]
    private float maxShootDelay = 5f;
    [SerializeField]
    private float defaultShootDelay = 0.5f;
    [SerializeField]
    private float currentShootDelay;
    public float CurrentShootDelay => currentShootDelay;

    [Header("[Reload Delay]")]
    [SerializeField]
    private float maxReloadDelay = 5f;
    [SerializeField]
    private float defaultReloadDelay = 1f;
    [SerializeField]
    private float currentReloadDelay;
    public float CurrentReloadDelay => currentReloadDelay;

    [Space]
    [Header("[[Projectile]]")]
    [Header("[Attack Multiplier]")]
    [SerializeField]
    private float maxAttackMultiplier = 5f;
    [SerializeField]
    private float defaultAttackMultiplier = 1f;
    [SerializeField]
    private float currentAttackMultiplier;
    public float CurrentAttackMultiplier => currentAttackMultiplier;

    [Header("[Move Speed Multiplier]")]
    [SerializeField]
    private float maxMoveSpeedMultiplier = 5f;
    [SerializeField]
    private float defaultMoveSpeedMultiplier = 1f;
    [SerializeField]
    private float currentMoveSpeedMultiplier;
    public float CurrentMoveSpeedMultiplier => currentMoveSpeedMultiplier;

    public void Init()
    {
        currentAmmoPerMagazine = Mathf.Clamp(defaultAmmoPerMagazine, 0, maxAmmoPerMagazine);
        currentShootDelay = Mathf.Clamp(defaultShootDelay, 0, maxShootDelay);
        currentReloadDelay = Mathf.Clamp(defaultReloadDelay, 0, maxReloadDelay);
        currentAttackMultiplier = Mathf.Clamp(defaultAttackMultiplier, 0, maxAttackMultiplier);
        currentMoveSpeedMultiplier = Mathf.Clamp(defaultMoveSpeedMultiplier, 0, maxMoveSpeedMultiplier);
    }
}
