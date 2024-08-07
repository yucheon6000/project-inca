using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gate Guardian/Ability/PlayerStatusAbility")]
public class PlayerStatusAbility : Ability
{
    public enum PlayerStatusAbilityType { IncreaseMaxHp, IncreaseAttackPower, IncreaseAttackSpeed }

    [SerializeField]
    private PlayerStatusAbilityType type;

    [SerializeField]
    [Tooltip("Default is perecnt-based.")]
    private bool constant = false;

    [SerializeField]
    private float amount;

    public override void OnSelectAbility()
    {
        PlayerStatus status = Player.Instance.Status;

        switch (type)
        {
            case PlayerStatusAbilityType.IncreaseMaxHp:
                status.IncreaseMaxHp(constant ? amount : (status.MaxHp * amount));
                Player.Instance.Status.IncreaseHp(int.MaxValue);
                break;

            case PlayerStatusAbilityType.IncreaseAttackPower:
                status.IncreaseAttack(constant ? amount : (status.CurrentAttack * amount));
                break;

            case PlayerStatusAbilityType.IncreaseAttackSpeed:
                status.IncreaseAttackSpeed(constant ? amount : (status.CurrentAttackSpeed * amount));
                break;
        }
    }
}
