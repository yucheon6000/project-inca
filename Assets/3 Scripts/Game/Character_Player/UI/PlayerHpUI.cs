using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpUI : MonoBehaviour
{
    [SerializeField]
    private CharacterStatus playerStatus;

    [SerializeField]
    private List<Image> images;

    private void Start()
    {
        playerStatus.OnChangeCurrentHp.AddListener(OnChangeHp);

        OnChangeHp(playerStatus.CurrentHp, 0);
    }

    public void OnChangeHp(int currentHp, int prevHp)
    {
        UpdateUI(currentHp);
    }

    private void UpdateUI(int hp)
    {
        int lastIndex = hp - 1;
        for (int i = 0; i < images.Count; ++i)
        {
            Image img = images[i];

            if (i > lastIndex)
                img.color = new Color(0, 0, 0, 0);
            else
                img.color = new Color(1, 1, 1, 1);

        }
    }
}
