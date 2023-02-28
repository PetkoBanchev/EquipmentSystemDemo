using System;
using UnityEngine;

public class TooltipSelectionResponse : MonoBehaviour, ISelectionResponse
{

    public event Action<string> OnSelection;
    public event Action OnDeselection;

    public void OnSelect(ISelectable _selection)
    {
        if (_selection != null)
            OnSelection?.Invoke(_selection.TooltipText);
    }
    public void OnDeselect(ISelectable _selection)
    {
        if (_selection != null)
            OnDeselection?.Invoke();
    }
}
