using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStatus : MonoBehaviour
{
    [Header("[[Projectile Status]]")]
    private WeaponStatus ownerStatus;

    [Header("[Attack]")]
    [SerializeField]
    private float maxAttack;
    public float MaxAttack => maxAttack;
    [SerializeField]
    private float defaultAttack;
    public float DefaultAttack => defaultAttack;
    [SerializeField]
    private float currentAttack;
    public float CurrentAttack => currentAttack;

    [Header("[Move Speed]")]
    [SerializeField]
    private float maxMoveSpeed;
    public float MaxMoveSpeed => maxMoveSpeed;
    [SerializeField]
    private float defaultMoveSpeed;
    public float DefaultMoveSpeed => defaultMoveSpeed;
    [SerializeField]
    private float currentMoveSpeed;
    public float CurrentMoveSpeed => currentMoveSpeed;

    public void Init(WeaponStatus ownerStatus)
    {
        this.ownerStatus = ownerStatus;

        currentAttack = Mathf.Clamp(defaultAttack * ownerStatus.CurrentAttackMultiplier, 0, maxAttack);
        currentMoveSpeed = Mathf.Clamp(defaultMoveSpeed * ownerStatus.CurrentMoveSpeedMultiplier, 0, maxMoveSpeed);
    }
}
