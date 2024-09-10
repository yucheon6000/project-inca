using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    None, Spawn, Idle, Move, Attack, TakeDamage, Die,       // Default states
    Fly, Fall, Attack1,                                     // Specific states
    Global
}

/*
public enum AudioType
{
    Spawn,
    Idle,
    Move,
    Attack0, Attack1, Attack2, Attack3,
    TakeDamage0, TakeDamage1, TakeDamage2, TakeDamage3,
    Die,
}
*/

public static class EnemyAnimation
{
    public const string Spawn = "Spawn";
    public const string Idle = "Idle";
    public const string Move = "Move";
    public const string Attack0 = "Attack 0";
    public const string Attack1 = "Attack 1";
    public const string Attack2 = "Attack 2";
    public const string Attack3 = "Attack 3";
    public const string TkaeDamage = "Tkae Damage";
    public const string Die = "Die";


}