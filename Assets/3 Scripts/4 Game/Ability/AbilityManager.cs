using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }

    [SerializeField]
    private Ability[] abilities;
    [SerializeField]
    private AbilityCard[] abilityCards;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        foreach (AbilityCard card in abilityCards)
            card.SetActivate(false);
    }

    [ContextMenu("Show Ability Cards")]
    public void ShowAbilityCards()
    {
        foreach (AbilityCard card in abilityCards)
        {
            card.SetAbility(abilities[Random.Range(0, abilities.Length)]);
            card.Init();
        }
    }

    [ContextMenu("Hide Ability Cards")]
    public void HideAbilityCards()
    {
        foreach (AbilityCard card in abilityCards)
            card.ForceKill();
    }
}
