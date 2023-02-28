using UnityEngine;

public interface IEquipable : ISelectable
{
    public EquipableType EquipableType { get; }
    public void OnEquip();
    public void OnUnequip();

}
