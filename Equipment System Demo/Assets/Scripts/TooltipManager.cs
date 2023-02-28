using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI tooltipText;
    [SerializeField] TooltipSelectionResponse tooltipSelectionResponse;

    private void Awake()
    {
        tooltipSelectionResponse.OnSelection += ShowTooltip;
        tooltipSelectionResponse.OnDeselection += HideTooltip;
    }

    private void ShowTooltip(string _text)
    {
        tooltipText.text = _text;
    }

    private void HideTooltip()
    {
        tooltipText.text = " ";
    }
}
