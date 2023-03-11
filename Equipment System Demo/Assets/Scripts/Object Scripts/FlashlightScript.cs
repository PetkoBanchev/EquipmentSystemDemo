using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FlashlightScript : MonoBehaviour, IEquipable
{
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

    public EquipableType EquipableType { get { return equipableType; } }
    public string TooltipText { get { return tooltipText; } }
    public ObjectType ObjectType { get { return objectType; } }
    public Transform Transform { get { return transform; } }

    private void Awake()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
        localScale = transform.localScale; // Caching the local scale
    }



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

    private void ToggleFlashlight()
    {
        if (isLightOn)
            spotlight.SetActive(false);
        else
            spotlight.SetActive(true);

        isLightOn = !isLightOn;
    }

    private void DetermineHand()
    {
        if (transform.parent.parent.name == "Left Hand")
            hand = Hand.LEFT;
        else
            hand = Hand.RIGHT;
    }

    private void ManageEvent(SubMode mode)
    {
        if (mode == SubMode.SUBSCRIBING)
        {
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 += ToggleFlashlight;
                Debug.Log("Sub left");
            }
            else
            {
                InputManager.Instance.OnFire2 += ToggleFlashlight;
                Debug.Log("Sub right");
            }
        }
        else
        {
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 -= ToggleFlashlight;
                EquipmentManager.Instance.UnsubscribeHand(hand);
                Debug.Log("Unsub left");
            }
            else
            {
                InputManager.Instance.OnFire2 -= ToggleFlashlight;
                EquipmentManager.Instance.UnsubscribeHand(hand);
                Debug.Log("Unsub right");
            }
        }
    }
}
