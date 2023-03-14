using System.Collections;
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

    private Hand hand;


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
        DetermineHand();
        ManageEvent(SubMode.SUBSCRIBING);
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
        ManageEvent(SubMode.UNSUBSCRIBING);
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

    private void ThrowRock()
    {
        throwForce = 500f;
        StartCoroutine(Throw());
    }

    private void ManageEvent(SubMode mode)
    {
        if (mode == SubMode.SUBSCRIBING)
        {
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 += ThrowRock;
                Debug.Log("Sub left");
            }
            else
            {
                InputManager.Instance.OnFire2 += ThrowRock;
                Debug.Log("Sub right");
            }
        }
        else
        {
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 -= ThrowRock;
                EquipmentManager.Instance.UnsubscribeHand(hand);
                Debug.Log("Unsub left");
            }
            else
            {
                InputManager.Instance.OnFire2 -= ThrowRock;
                EquipmentManager.Instance.UnsubscribeHand(hand);
                Debug.Log("Unsub right");
            }
        }
    }

    private void DetermineHand()
    {
        if (transform.parent.parent.name == "Left Hand")
            hand = Hand.LEFT;
        else
            hand = Hand.RIGHT;
    }
}
