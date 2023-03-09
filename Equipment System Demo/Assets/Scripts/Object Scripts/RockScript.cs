using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RockScript : MonoBehaviour, IEquipable
{
    [SerializeField] private EquipableType equipableType = EquipableType.HAND;
    [SerializeField] private string tooltipText = "Press E to equip rock";
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
        InputManager.Instance.OnFire1 += ThrowRock;
    }

    public void OnUnequip()
    {
        StartCoroutine(Throw());
    }

    private IEnumerator Throw()
    {
        transform.parent = null;
        rigidbody = transform.AddComponent<Rigidbody>();
        InputManager.Instance.OnFire1 -= ThrowRock;
        rigidbody.AddForce(transform.forward * throwForce);
        while (!groundCheck.IsGrounded())
        {
            yield return new WaitForFixedUpdate();
        }
        rigidbody.freezeRotation = true;
        Destroy(rigidbody, 1f);
        transform.localScale = localScale;
    }

    private void ThrowRock()
    {
        StartCoroutine(Throw());
    }
}
