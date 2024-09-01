using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    private GameObject weaponPrefab;
    [SerializeField]
    private List<WeaponInformation> weaponInformation;
    private List<Weapon> weapons = new List<Weapon>();
    private int currentWeaponIndex = 0;
    private Weapon currentWeapon = null;

    private void Awake()
    {
        InitWeapons();

        currentWeapon = weapons[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);
    }

    private void InitWeapons()
    {
        foreach (WeaponInformation info in weaponInformation)
        {
            GameObject clone = Instantiate(weaponPrefab, this.transform);
            Weapon weapon = clone.GetComponent<Weapon>();
            weapon.Init(info);
            weapons.Add(weapon);
            clone.SetActive(false);
        }
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
        if (currentWeaponIndex >= weapons.Count)
            currentWeaponIndex = 0;

        currentWeapon = weapons[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);
    }
}
