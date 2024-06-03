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
    protected Enemy target = null;

    [Header("Effects")]
    [SerializeField]
    protected Transform shootEffectSpawnTransform;
    [SerializeField]
    protected GameObject shootEffect;
    [SerializeField]
    protected GameObject hitEffect;

    [Header("Audios")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip shootAudioClip;

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
        Enemy enemy = hitInfo.collider.GetComponent<Enemy>();
        if (enemy == null)
            enemy = hitInfo.collider.GetComponentInParent<Enemy>();

        if (enemy == null)
        {
            target?.OnHoverEnd();
            target = null;
            return false;
        }

        // If enemy is hitable
        if (enemy.IsInteractableType(InteractableType.Hitable))
        {
            if (target == null)
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

        return false;
    }

    protected abstract void SpawnShootEffect();

    private void SpawnHitEffect()
    {
        Instantiate(hitEffect, hitPoint, Quaternion.identity);
    }

    public void TriggerShoot()
    {
        SpawnShootEffect();

        audioSource.PlayOneShot(shootAudioClip);

        if (target == null) return;

        target.Hit(1);

        SpawnHitEffect();
    }
}
