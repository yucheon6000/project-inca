using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public abstract class GameController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField]
    protected Camera targetCamera;

    [Header("UI")]
    [SerializeField]
    protected RectTransform rectTransform;
    [SerializeField]
    protected Image image;
    [SerializeField]
    protected Material materialGreen;
    [SerializeField]
    protected Material materialRed;
    [SerializeField]
    protected AnimationCurve sizeChangingCurve;

    [Header("Target Layermask")]
    [SerializeField]
    protected LayerMask targetLayermask;

    [Header("Hit Information (for debug)")]
    [SerializeField]
    protected Vector3 hitPoint = Vector3.zero;
    [SerializeField]
    protected DamagableEnemy target = null;
    protected bool hasTarget => target != null;

    [Header("Effects")]
    [SerializeField]
    protected Transform shootEffectSpawnTransform;

    [Header("Audios")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip shootAudioClip;

    [Header("Weapon")]
    [SerializeField]
    protected List<WeaponInformation> weaponInformations;
    [SerializeField]
    protected WeaponInformation currentWeapon;
    protected int weaponIndex = 0;
    protected float shootTimer = 0;
    protected bool isPressingShootButton = false;

    protected bool CheckTarget(Ray ray)
    {
        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, 100000, targetLayermask);

        // If ray didn't hit anything
        if (!hit)
        {
            target?.OnHoverEnd();
            target = null;
            return false;
        }

        // Find enemy
        DamagableEnemy enemy = hitInfo.collider.GetComponent<DamagableEnemy>();
        if (enemy == null)
            enemy = hitInfo.collider.GetComponentInParent<DamagableEnemy>();

        if (enemy == null)
        {
            target?.OnHoverEnd();
            target = null;
            return false;
        }

        // If enemy is hitable
        if (false)//enemy.IsInteractableType(InteractableType.Hitable))
        {
            if (hasTarget == false)
            {
                enemy.OnHoverStart();
            }
            else if (target != enemy)
            {
                target.OnHoverEnd();
                enemy.OnHoverStart();
            }

            hitPoint = hitInfo.point;
            target = enemy;
            return true;
        }

        target?.OnHoverEnd();
        target = null;
        return false;
    }

    protected abstract void SpawnShootEffect();

    private void SpawnHitEffect()
    {
        Instantiate(currentWeapon.WeaponInfoForTargeting.HitEffectPrefab, hitPoint, Quaternion.identity);
    }

    public void TriggerShoot(int power)
    {
        SpawnShootEffect();

        audioSource.PlayOneShot(shootAudioClip);

        if (hasTarget == false) return;

        target.TakeDamage(power);

        SpawnHitEffect();
    }
}
