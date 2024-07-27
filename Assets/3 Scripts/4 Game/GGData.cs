using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GGData
{
    public static Transform PlayerTransform => GameManager.Instance.Player;
    public static Vector3 PlayerPosition => PlayerTransform.position;
}
