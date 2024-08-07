using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected abstract bool ThisIsMyEnemy(Character character);

    protected abstract void Attack(Character target);

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Character>(out Character character) == false) return;

        if (ThisIsMyEnemy(character) == false) return;

        Attack(character);
    }
}
