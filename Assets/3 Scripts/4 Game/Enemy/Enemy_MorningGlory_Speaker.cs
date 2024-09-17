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
        LookAtPlayer(true);
        base.Init(detectedObject);
    }

    public void ForceKill(float time)
    {
        Invoke(nameof(ForceKill), time);
    }

    public void PlayTakeDamageEffect(float time)
    {
        Invoke(nameof(PlayTakeDamageEffect_), time);
    }

    private void PlayTakeDamageEffect_()
    {
        if (gameObject.activeSelf == false) return;

        PlayAudioClip(AudioTypeForChracter.TakeDamage0);
        emissionEffector.Play(0.4f, 10f, 0f, Color.red);
    }
}
