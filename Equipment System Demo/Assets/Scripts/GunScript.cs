using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GunScript : MonoBehaviour, IEquipable
{
    [SerializeField] private EquipableType equipableType = EquipableType.HAND;

    [SerializeField] private string tooltipText = "Press E to equip gun";

    [SerializeField] private ObjectType objectType = ObjectType.EQUIPABLE;

    [SerializeField] private Rigidbody rigidbody;

    public EquipableType EquipableType { get { return equipableType; } }
    public string TooltipText { get { return tooltipText; } }
    public ObjectType ObjectType { get { return objectType; } }
    public Transform Transform { get { return transform; } }

    public void OnEquip()
    {
        Debug.Log("Gun equiped");
        Destroy(rigidbody);
    }

    public void OnUnequip()
    {
        Debug.Log("Gun unequiped");
        transform.parent = null;
        rigidbody = transform.AddComponent<Rigidbody>();
        rigidbody.AddForce(Vector3.forward * 2f, ForceMode.Impulse);
        Destroy(rigidbody,1f);
    }

    private void Awake()
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
