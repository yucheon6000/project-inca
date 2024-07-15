using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static string animation_enemy_idle = "Idle";
    public static string animation_enemy_hit = "Hit";
    public static string animation_enemy_take_damage = "Take Damage";
    public static string animation_enemy_death = "Death";
    public static string animation_enemy_attack = "Attack";

    public static class Animation
    {
        public const string ENEMY_ANIMATION_ID = "animation";
        public const int ENEMY_ANIMATION_IDLE = 1;
        public const int ENEMY_ANIMATION_MOVE = 2;
        public const int ENEMY_ANIMATION_ATTACK = 3;
        public const int ENEMY_ANIMATION_TAKE_DAMAGE = 4;
        public const int ENEMY_ANIMATION_DIE = 5;
    }
}
