using System.Collections;
using System.Collections.Generic;
using Inca;
using TMPro;
using UnityEngine;

public class AbilityCard : DamagableEnemy
{
    [Header("Ability Card")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private TextMeshPro nameText;
    [SerializeField]
    private TextMeshPro descriptionText;

    private Ability ability;

    public override void Init(DetectedObject detectedObject = null)
    {
        SetActivate(true);
        base.Init(detectedObject);
    }

    public void SetActivate(bool value)
        => gameObject.SetActive(value);

    protected override void InitStateMachine()
    {
        base.InitStateMachine();
        states[EnemyState.Die] = new State_Die(this);
    }

    /// <summary>
    /// Change the text and image of this card depending on its abilities.
    /// </summary>
    public void SetAbility(Ability ability)
    {
        this.ability = ability;

        spriteRenderer.sprite = ability.AbilityIconSprite;

        nameText.text = ability.AbilityName;
        descriptionText.text = ability.AbilityDescription;
    }

    public override float TakeDamage(float attackAmount)
    {
        if (IsDead) return 0;

        // Apply the ability to the player.
        ability.OnSelectAbility();

        // Hide all ability cards.
        AbilityManager.Instance.HideAbilityCards();

        return base.TakeDamage(attackAmount);
    }

    public class State_Die : EnemyState_Die
    {
        AbilityCard owner;

        public State_Die(Enemy entity) : base(entity)
            => owner = (AbilityCard)entity;

        public override void OnDisappear(Enemy entity)
        {
            // base.OnDisappear(entity); => Deactivate 막기 위해서
            owner.SetActivate(false);
        }
    }
}
