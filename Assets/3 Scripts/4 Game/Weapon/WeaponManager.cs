using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private Weapon[] weapons;
    private int currentWeaponIndex = 0;
    private Weapon currentWeapon = null;

    private void Start()
    {
        InitWeapons();
    }

    private void InitWeapons()
    {
        weapons = GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons)
        {
            weapon.Init(Player.Instance);
            weapon.gameObject.SetActive(false);
        }

        currentWeapon = weapons[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);
    }

    private void Update()
    {
        UpdateChangeWeapon();
    }

    private void UpdateChangeWeapon()
    {
        if (IncaInput.GetButtonDown(IncaButtonCode.A) == false) return;

        currentWeapon.gameObject.SetActive(false);

        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Length)
            currentWeaponIndex = 0;

        currentWeapon = weapons[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);
    }
}
