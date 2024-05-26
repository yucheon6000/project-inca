using System;
using TMPro;
using UnityEngine;

public class PlayerHpUI : MonoBehaviour
{
    [SerializeField]
    private CharacterStatus playerStatus;

    [SerializeField]
    private TextMeshProUGUI hpTMP;

    private void Start()
    {
        playerStatus.OnChangeCurrentHp.AddListener(OnChangeHp);

        OnChangeHp(playerStatus.CurrentHp, 0);
    }

    public void OnChangeHp(int currentHp, int prevHp)
    {
        hpTMP.text = String.Format("HP: {0}", currentHp.ToString());
    }
}
