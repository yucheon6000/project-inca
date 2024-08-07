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
        gameObject.SetActive(true);
        base.Init(detectedObject);
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

    public override float TakeDamage(float attckAmount)
    {
        if (IsDead) return 0;

        // Apply the ability to the player.
        ability.OnSelectAbility();

        // Hide all ability cards.
        AbilityManager.Instance.HideAbilityCards();

        return base.TakeDamage(attckAmount);
    }

    protected override void OnDisappear()
    {
        base.OnDisappear();
        gameObject.SetActive(false);
    }
}
