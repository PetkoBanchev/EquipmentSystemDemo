using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AmmoScript : MonoBehaviour, IEquipable
{
    [SerializeField] private EquipableType equipableType = EquipableType.HAND;
    [SerializeField] private string tooltipText = "Press E to equip ammo clip";
    [SerializeField] private ObjectType objectType = ObjectType.EQUIPABLE;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float throwForce = 100f;
    private Vector3 localScale;
    private GroundCheck groundCheck;

    [SerializeField, Range(0,30)] private int bulletCount = 5;
    private AudioSource reloadSound;

    private Hand hand;

    public int BulletCount
    {
        get { return bulletCount; }
        set { bulletCount = value; }
    }

    public EquipableType EquipableType { get { return equipableType; } }
    public string TooltipText { get { return tooltipText; } }
    public ObjectType ObjectType { get { return objectType; } }
    public Transform Transform { get { return transform; } }

    private void Awake()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
        localScale = transform.localScale;
        reloadSound = GetComponent<AudioSource>();
    }

    public void OnEquip()
    {
        if (rigidbody != null)
            Destroy(rigidbody);
        DetermineHand();
        ManageEvent(SubMode.SUBSCRIBING);
    }

    public void OnUnequip()
    {
        transform.parent = null;
        ManageEvent(SubMode.UNSUBSCRIBING);
        rigidbody = transform.AddComponent<Rigidbody>();
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
        transform.localScale = localScale;
    }

    private IEnumerator Reload(IEquipable _gun)
    {
        reloadSound.Play();
        yield return new WaitForSeconds(.5f);
        _gun.Transform.GetComponent<GunScript>().BulletCount = bulletCount;
        ManageEvent(SubMode.UNSUBSCRIBING);
        Destroy(this.gameObject);
    }

    private void ReloadGun()
    {
        var gun = EquipmentManager.Instance.GetGun(hand);
        if (gun != null)
            StartCoroutine(Reload(gun));
    }

    private void ManageEvent(SubMode mode)
    {
        if (mode == SubMode.SUBSCRIBING)
        {
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 += ReloadGun;
                Debug.Log("Sub left");
            }
            else
            {
                InputManager.Instance.OnFire2 += ReloadGun;
                Debug.Log("Sub right");
            }
        }
        else
        {
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 -= ReloadGun;
                EquipmentManager.Instance.UnsubscribeHand(hand);
                Debug.Log("Unsub left");
            }
            else
            {
                InputManager.Instance.OnFire2 -= ReloadGun;
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
