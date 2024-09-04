using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpUI : MonoBehaviour
{
    [SerializeField]
    private CharacterStatus playerStatus;

    [SerializeField]
    private Image barImage;
    [SerializeField]
    private Transform needleTransform;
    [SerializeField]
    private float animationTime;
    [SerializeField]
    private AnimationCurve animationCurve;

    private float hpPercent = 0;
    private float needleTargetAngleZ
        => Mathf.Lerp(180f, 0f, hpPercent);

    private void Start()
    {
        playerStatus.OnChangeCurrentHp.AddListener(OnChangeHp);

        OnChangeHp(playerStatus.CurrentHp, 0);
    }

    public void OnChangeHp(float currentHp, float prevHp)
    {
        hpPercent = currentHp / playerStatus.MaxHp;
        StopAllCoroutines();
        StartCoroutine(UpdateHpUiRoutine());
    }

    private IEnumerator UpdateHpUiRoutine()
    {
        float timer = 0;
        float progress = 0;

        float originalPercent = barImage.fillAmount;
        float originalRotZ = needleTransform.eulerAngles.z;

        while (progress <= 1)
        {
            timer += Time.deltaTime;
            progress = timer / animationTime;

            float ani = animationCurve.Evaluate(progress);

            barImage.fillAmount = Mathf.Lerp(originalPercent, hpPercent, ani);

            float z = Mathf.Lerp(originalRotZ, needleTargetAngleZ, ani);

            needleTransform.rotation = Quaternion.Euler(0, 0, z);

            yield return null;
        }
    }
}
