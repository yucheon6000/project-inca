using System.Collections;
using System.Collections.Generic;
using Inca;
using TMPro;
using UnityEngine;

public class SpeedUI : MonoBehaviour
{
    private TextMeshProUGUI speedText;

    private void Awake()
    {
        speedText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        speedText.text = $"{Mathf.FloorToInt(IncaData.Speed)} <size=60><i>km/h</i></size>";
    }
}
