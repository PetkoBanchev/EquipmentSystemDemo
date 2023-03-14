using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RockScript : MonoBehaviour, IEquipable
{
    #region Private variables
    [SerializeField] private EquipableType equipableType = EquipableType.HAND;
    [SerializeField] private string tooltipText = "Press E to equip rock";
    [SerializeField] private ObjectType objectType = ObjectType.EQUIPABLE;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float throwForce = 500f;

    private Vector3 localScale;
    private GroundCheck groundCheck;
    private Hand hand;
    #endregion

    #region Public properties
    public EquipableType EquipableType { get { return equipableType; } }
    public string TooltipText { get { return tooltipText; } }
    public ObjectType ObjectType { get { return objectType; } }
    public Transform Transform { get { return transform; } }
    #endregion

    #region Private methods
    private void Awake()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
        localScale = transform.localScale;
    }

    /// <summary>
    /// Unsubcribes from the event. Removes the object from the parent.
    /// Adds a rigidbody to give it force to simulate the item being thrown.
    /// Removes the rigidbody once it hits the ground.
    /// Restores the original scale. (Could not figure out why the scale distortions happened.)
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Increases the throw force to create a strong throw effect.
    /// Uses the same coroutine as the unequip interaction.
    /// </summary>
    private void ThrowRock()
    {
        throwForce = 500f;
        StartCoroutine(Throw());
    }

    /// <summary>
    /// Subscribes or unsubscribes to respective events depending in which hand the object is equiped.
    /// </summary>
    /// <param name="mode"></param>
    private void ManageEvent(SubMode mode)
    {
        if (mode == SubMode.SUBSCRIBING)
        {
            if (hand == Hand.LEFT)
                InputManager.Instance.OnFire1 += ThrowRock;
            else
                InputManager.Instance.OnFire2 += ThrowRock;
        }
        else
        {
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 -= ThrowRock;
                EquipmentManager.Instance.UnsubscribeHand(hand);
            }
            else
            {
                InputManager.Instance.OnFire2 -= ThrowRock;
                EquipmentManager.Instance.UnsubscribeHand(hand);
            }
        }
    }

    /// <summary>
    /// Determines in which hand the object is equiped
    /// </summary>
    private void DetermineHand()
    {
        if (transform.parent.parent.name == "Left Hand")
            hand = Hand.LEFT;
        else
            hand = Hand.RIGHT;
    }
    #endregion

    #region Public methods
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
    #endregion
}
