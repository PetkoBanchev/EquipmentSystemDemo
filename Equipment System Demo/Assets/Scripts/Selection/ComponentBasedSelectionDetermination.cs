using UnityEngine;

public class ComponentBasedSelectionDetermination : MonoBehaviour, ISelectionDetermination
{
    private ISelectable selection;

    public void CheckSelection(Ray _ray)
    {
        selection = null;

        if (Physics.Raycast(_ray, out var hit, 3f))
        {
            var _selection = hit.transform.GetComponent<ISelectable>();
            if (_selection != null)
                selection = _selection;
        }
    }

    public ISelectable GetSelection()
    {
        return selection;
    }
}
