using UnityEngine;

public abstract class Ability : ScriptableObject
{
    [Header("[Ability]")]
    [Multiline]
    [SerializeField]
    protected string abilityName;
    public string AbilityName => abilityName;
    [Multiline]
    [SerializeField]
    protected string abilityDescription;
    public string AbilityDescription => abilityDescription;
    [SerializeField]
    protected Sprite abilityIconSprite;
    public Sprite AbilityIconSprite => abilityIconSprite;

    public abstract void OnSelectAbility();
}
