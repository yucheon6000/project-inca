using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    private bool initOnStart = false;
    private bool initNormalWeapons = false;
    private bool initGlobalWeapon = false;

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
        if (initOnStart)
            InitAllWeapons();
    }

    private void InitAllWeapons()
    {
        InitNormalWeapons();
        InitGlobalWeapon();
    }

    public void InitNormalWeapons()
    {
        if (initNormalWeapons) return;

        IncaInputManager.Instance.SetDefaultCursorColor(Color.green);
        IncaInputManager.Instance.SetActiveCursorColor(Color.red);

        // Normal weapons
        normalWeapons = normalWeaponsTransform.GetComponentsInChildren<PlayerWeapon>();

        foreach (var weapon in normalWeapons)
        {
            weapon.Init(Player.Instance);
            weapon.Uninstall();
        }

        currentNormalWeapon = normalWeapons[currentNormalWeaponIndex];
        currentNormalWeapon.Install();

        initNormalWeapons = true;
    }

    private void InitGlobalWeapon()
    {
        if (initGlobalWeapon) return;

        // Global weapons
        globalWeapons = globalWeaponsTransform.GetComponentsInChildren<PlayerWeapon>();
        foreach (var weapon in globalWeapons)
        {
            weapon.Init(Player.Instance);
            weapon.Install();
        }

        initGlobalWeapon = true;
    }

    private void Update()
    {
        if (!initNormalWeapons) return;

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
        currentNormalWeapon.Install();
    }
}
