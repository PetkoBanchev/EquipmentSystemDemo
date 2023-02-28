using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour, IEquipable
{
    [SerializeField] private EquipableType equipableType = EquipableType.HAND;

    [SerializeField] private string tooltipText = "Press E to equip gun";

    [SerializeField] private ObjectType objectType = ObjectType.EQUIPABLE;

    public EquipableType EquipableType { get { return equipableType; } }
    public string TooltipText { get { return tooltipText; } }
    public ObjectType ObjectType { get { return objectType; } }
    public Transform Transform { get { return transform; } }

    public void OnEquip()
    {
        Debug.Log("Gun equiped");
    }

    public void OnUnequip()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
