using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class GameController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    protected RectTransform rectTransform;
    [SerializeField]
    protected Image image;
    [SerializeField]
    protected Material materialGreen;
    [SerializeField]
    protected Material materialRed;
    [SerializeField]
    protected AnimationCurve sizeChangingCurve;

    [Header("Target Layermask")]
    [SerializeField]
    protected LayerMask targetLayermask;
}
