using UnityEngine;

public interface ISelectable
{
    public string TooltipText { get; }
    public ObjectType ObjectType { get; }
    public Transform Transform { get; }

}
