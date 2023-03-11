using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GunScript : MonoBehaviour, IEquipable
{
    [SerializeField] private EquipableType equipableType = EquipableType.HAND;
    [SerializeField] private string tooltipText = "Press E to equip gun";
    [SerializeField] private ObjectType objectType = ObjectType.EQUIPABLE;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float throwForce = 100f;
    private Vector3 localScale;
    private GroundCheck groundCheck;

    [SerializeField] private int bulletCount = 5;
    [SerializeField] private int maxBullets = 30;
    private AudioSource gunSound;
    [SerializeField] private AudioClip gunshot;
    [SerializeField] private AudioClip emptyGun;

    private Hand hand;

    public int BulletCount
    {
        get { return bulletCount; }
        set
        {
            bulletCount += value;
            SetGunSound();
            if (bulletCount > maxBullets)
                bulletCount = maxBullets;
        }
    }

    public EquipableType EquipableType { get { return equipableType; } }
    public string TooltipText { get { return tooltipText; } }
    public ObjectType ObjectType { get { return objectType; } }
    public Transform Transform { get { return transform; } }

    private void Awake()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
        localScale = transform.localScale;
        gunSound = GetComponent<AudioSource>();
    }

    public void OnEquip()
    {
        if(rigidbody != null)
            Destroy(rigidbody);
        DetermineHand();
        ManageEvent(SubMode.SUBSCRIBING);
        SetGunSound();
        EquipmentManager.Instance.SetGun(this, hand);
    }

    public void OnUnequip()
    {
        transform.parent = null;
        ManageEvent(SubMode.UNSUBSCRIBING);
        EquipmentManager.Instance.SetGun(null, hand);
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

    private void FireGunManually()
    {
        if(bulletCount > 0)
        {
            bulletCount -= 1;
        }
        else
        {
            SetGunSound();
        }
        gunSound.Play();
    }

    private void SetGunSound()
    {
        if (bulletCount > 0)
            gunSound.clip = gunshot;
        else
            gunSound.clip = emptyGun;
    }

    private void ManageEvent(SubMode mode)
    {
        if (mode == SubMode.SUBSCRIBING)
        {
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 += FireGunManually;
                Debug.Log("Sub left");
            }
            else
            {
                InputManager.Instance.OnFire2 += FireGunManually;
                Debug.Log("Sub right");
            }
        }
        else
        {
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 -= FireGunManually;
                EquipmentManager.Instance.UnsubscribeHand(hand);
                Debug.Log("Unsub left");
            }
            else
            {
                InputManager.Instance.OnFire2 -= FireGunManually;
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
