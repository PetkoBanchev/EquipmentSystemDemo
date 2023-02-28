using UnityEngine;

public interface ISelectionDetermination
{
    public void CheckSelection(Ray _ray);
    public ISelectable GetSelection();
}
