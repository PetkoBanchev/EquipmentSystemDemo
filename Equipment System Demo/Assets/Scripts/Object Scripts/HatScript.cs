using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HatScript : MonoBehaviour, IEquipable
{
    #region Private variables
    [SerializeField] private EquipableType equipableType = EquipableType.HEAD;
    [SerializeField] private string tooltipText = "Press E to equip hat";
    [SerializeField] private ObjectType objectType = ObjectType.EQUIPABLE;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float throwForce = 500f;

    private Vector3 localScale;
    private GroundCheck groundCheck;
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
    #endregion

    #region Public methods
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
    #endregion
}
