using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("[Normal Weapon]")]
    [SerializeField]
    private Transform normalWeaponsTransform;
    private PlayerWeapon[] normalWeapons;
    private int currentNormalWeaponIndex = 0;
    private PlayerWeapon currentNormalWeapon = null;

    [Header("[Global Weapon]")]
    [SerializeField]
    private Transform globalWeaponsTransform;
    private PlayerWeapon[] globalWeapons;

    private void Start()
    {
        InitWeapons();

        IncaInputManager.Instance.SetDefaultCursorColor(Color.green);
        IncaInputManager.Instance.SetActiveCursorColor(Color.red);
    }

    private void InitWeapons()
    {
        // Normal weapons
        normalWeapons = normalWeaponsTransform.GetComponentsInChildren<PlayerWeapon>();

        foreach (var weapon in normalWeapons)
        {
            weapon.Init(Player.Instance);
            weapon.gameObject.SetActive(false);
        }

        currentNormalWeapon = normalWeapons[currentNormalWeaponIndex];
        currentNormalWeapon.gameObject.SetActive(true);
        currentNormalWeapon.SetAim();

        // Global weapons
        globalWeapons = globalWeaponsTransform.GetComponentsInChildren<PlayerWeapon>();
        foreach (var weapon in globalWeapons)
            weapon.Init(Player.Instance);
    }

    private void Update()
    {
        UpdateChangeWeapon();
    }

    private void UpdateChangeWeapon()
    {
        if (IncaInput.GetButtonDown(IncaButtonCode.A) == false) return;

        currentNormalWeapon.gameObject.SetActive(false);

        currentNormalWeaponIndex++;
        if (currentNormalWeaponIndex >= normalWeapons.Length)
            currentNormalWeaponIndex = 0;

        currentNormalWeapon = normalWeapons[currentNormalWeaponIndex];
        currentNormalWeapon.gameObject.SetActive(true);

        currentNormalWeapon.SetAim();
    }
}
