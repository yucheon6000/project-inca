using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_MorningGlory_Speaker : DamagableEnemy
{
    public override void Init(DetectedObject detectedObject = null)
    {
        transform.localScale = Vector3.one * Random.Range(0.4f, 0.7f);
        scaleEffector.SaveOriginalScale(true);
        base.Init(detectedObject);
    }
}
