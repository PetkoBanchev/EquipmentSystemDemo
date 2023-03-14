using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HatScript : MonoBehaviour, IEquipable
{
    [SerializeField] private EquipableType equipableType = EquipableType.HEAD;
    [SerializeField] private string tooltipText = "Press E to equip hat";
    [SerializeField] private ObjectType objectType = ObjectType.EQUIPABLE;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float throwForce = 500f;

    private Vector3 localScale;
    private GroundCheck groundCheck;

    public EquipableType EquipableType { get { return equipableType; } }
    public string TooltipText { get { return tooltipText; } }
    public ObjectType ObjectType { get { return objectType; } }
    public Transform Transform { get { return transform; } }

    private void Awake()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
        localScale = transform.localScale;
    }

    public void OnEquip()
    {
        if (rigidbody != null)
            Destroy(rigidbody);
    }

    public void OnUnequip()
    {
        throwForce = 50f;
        StartCoroutine(Throw());
    }

    private IEnumerator Throw()
    {
        transform.parent = null;
        rigidbody = transform.AddComponent<Rigidbody>();
        rigidbody.AddForce(transform.forward * throwForce);
        while (!groundCheck.IsGrounded())
        {
            yield return new WaitForFixedUpdate();
        }
        rigidbody.freezeRotation = true;
        Destroy(rigidbody);
        transform.localScale = localScale;
    }
    
}
