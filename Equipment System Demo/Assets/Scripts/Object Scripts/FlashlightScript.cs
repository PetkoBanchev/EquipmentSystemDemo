using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FlashlightScript : MonoBehaviour, IEquipable
{
    #region Private variables
    [SerializeField] private EquipableType equipableType = EquipableType.HAND;
    [SerializeField] private string tooltipText = "Press E to equip flashlight";
    [SerializeField] private ObjectType objectType = ObjectType.EQUIPABLE;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float throwForce = 100f;

    private Vector3 localScale;
    private GroundCheck groundCheck;
    private Hand hand;

    private bool isLightOn = false;
    [SerializeField] private GameObject spotlight;
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
        localScale = transform.localScale; // Caching the local scale
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
        rigidbody.AddForce(transform.forward * throwForce);
        while (!groundCheck.IsGrounded())
        {
            yield return new WaitForFixedUpdate();
        }
        rigidbody.freezeRotation = true;
        Destroy(rigidbody, 1f);
        transform.localScale = localScale; // reseting to the original local scale to prevent distortion of the object
    }

    /// <summary>
    /// Toggles the spotlight to simulate a flashlight interaction.
    /// </summary>
    private void ToggleFlashlight()
    {
        if (isLightOn)
            spotlight.SetActive(false);
        else
            spotlight.SetActive(true);

        isLightOn = !isLightOn;
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

    /// <summary>
    /// Subscribes or unsubscribes to respective events depending in which hand the object is equiped.
    /// </summary>
    /// <param name="mode"></param>
    private void ManageEvent(SubMode mode)
    {
        if (mode == SubMode.SUBSCRIBING)
        {
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 += ToggleFlashlight;
            }
            else
            {
                InputManager.Instance.OnFire2 += ToggleFlashlight;
            }
        }
        else
        {
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 -= ToggleFlashlight;
                EquipmentManager.Instance.UnsubscribeHand(hand);
            }
            else
            {
                InputManager.Instance.OnFire2 -= ToggleFlashlight;
                EquipmentManager.Instance.UnsubscribeHand(hand);
            }
        }
    }
    #endregion

    #region Public methods
    public void OnEquip()
    {
        DetermineHand();
        if (rigidbody != null)
            Destroy(rigidbody);
        ManageEvent(SubMode.SUBSCRIBING);
    }

    public void OnUnequip()
    {
        transform.parent = null;
        rigidbody = transform.AddComponent<Rigidbody>();
        ManageEvent(SubMode.UNSUBSCRIBING);
        if (isLightOn)
            ToggleFlashlight();
        StartCoroutine(Throw());
    }
    #endregion
}
