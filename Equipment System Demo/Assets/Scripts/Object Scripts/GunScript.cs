using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GunScript : MonoBehaviour, IEquipable
{
    #region Private variables
    [SerializeField] private EquipableType equipableType = EquipableType.HAND;
    [SerializeField] private string tooltipText = "Press E to equip gun";
    [SerializeField] private ObjectType objectType = ObjectType.EQUIPABLE;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float throwForce = 100f;
    private Vector3 localScale;
    private GroundCheck groundCheck;

    [SerializeField, Range(0, 30)] private int bulletCount = 5;
    [SerializeField, Range(0, 30)] private int maxBullets = 30;
    private GunMode gunMode = GunMode.MANUAL;
    private bool isAutomaticFireOn = false;
    private AudioSource gunSound;
    [SerializeField] private AudioClip gunshot;
    [SerializeField] private AudioClip emptyGun;
    private Hand hand;
    #endregion

    #region Public properties
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
    #endregion

    #region Private methods

    private void Awake()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
        localScale = transform.localScale;
        gunSound = GetComponent<AudioSource>();
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
        transform.localScale = localScale;
    }

    private void FireGunManually()
    {
        if(bulletCount > 0)
            bulletCount--;
        else
            SetGunSound();
        gunSound.Play();
    }

    /// <summary>
    /// Triggers the automatic fire if there is at least 1 bullet in the gun.
    /// </summary>
    private void FireGunAutomatically()
    {
        isAutomaticFireOn = true;
        if (bulletCount > 0)
            StartCoroutine(AutomaticFire());
        else
            SetGunSound();
        gunSound.Play();
    }

    /// <summary>
    /// Fires the gun automatically. Used a coroutine to add delay between the shots.
    /// Check the summary of TurnOffAutomaticFire for more info
    /// </summary>
    /// <returns></returns>
    private IEnumerator AutomaticFire()
    {
        while (isAutomaticFireOn)
        {
            if (bulletCount > 0)
                bulletCount--;
            else
                SetGunSound();
            gunSound.Play();
            yield return new WaitForSeconds(.1f);
        }
    }
    
    /// <summary>
    /// Changes the sound to an empty clip when the bullets deplete
    /// </summary>
    private void SetGunSound()
    {
        if (bulletCount > 0)
            gunSound.clip = gunshot;
        else
            gunSound.clip = emptyGun;
    }

    /// <summary>
    /// Subscribes or unsubscribes to respective events depending in which hand the object is equiped.
    /// </summary>
    /// <param name="mode"></param>
    private void ManageEvent(SubMode mode)
    {
        if (mode == SubMode.SUBSCRIBING)
        {
            InputManager.Instance.OnToggleGunMode += ToggleGunMode;
            if (hand == Hand.LEFT)
                InputManager.Instance.OnFire1 += FireGunManually;
            else
                InputManager.Instance.OnFire2 += FireGunManually;
        }
        else
        {
            InputManager.Instance.OnToggleGunMode -= ToggleGunMode;
            if (gunMode == GunMode.AUTOMATIC)
                ToggleGunMode();
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 -= FireGunManually;
                EquipmentManager.Instance.UnsubscribeHand(hand);
            }
            else
            {
                InputManager.Instance.OnFire2 -= FireGunManually;
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

    /// <summary>
    /// This is subscribed to the OnFire1Released or OnFire2Released event based on the hand in which the gun is equiped.
    /// The events are fired when the respective mouse button is released. The the boolean is set to false.
    /// This in turn breaks the while loop in the AutomaticFire coroutine.
    /// </summary>
    private void TurnOffAutomaticFire()
    {
        isAutomaticFireOn = false;
    }

    private void ToggleGunMode()
    {
        if (gunMode == GunMode.MANUAL)
            gunMode = GunMode.AUTOMATIC;
        else
        {
            gunMode = GunMode.MANUAL;
            TurnOffAutomaticFire();
        }
        ChangeGunModeTo();
    }

    /// <summary>
    /// Subscribes and unsubcribes from the respective events based on the gun mode and the hand in which the gun is equiped.
    /// </summary>
    private void ChangeGunModeTo()
    {
        if(gunMode == GunMode.AUTOMATIC)
        {
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 -= FireGunManually;
                InputManager.Instance.OnFire1 += FireGunAutomatically;
                InputManager.Instance.OnFire1Released += TurnOffAutomaticFire;
            }
            else
            {
                InputManager.Instance.OnFire2 -= FireGunManually;
                InputManager.Instance.OnFire2 += FireGunAutomatically;
                InputManager.Instance.OnFire2Released += TurnOffAutomaticFire;
            }
        }
        else
        {
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 -= FireGunAutomatically;
                InputManager.Instance.OnFire1 += FireGunManually;
                InputManager.Instance.OnFire1Released -= TurnOffAutomaticFire;
            }
            else
            {
                InputManager.Instance.OnFire2 -= FireGunAutomatically;
                InputManager.Instance.OnFire2 += FireGunManually;
                InputManager.Instance.OnFire2Released -= TurnOffAutomaticFire;
            }
        }
    }
    #endregion

    #region Public methods
    public void OnEquip()
    {
        if(rigidbody != null)
            Destroy(rigidbody);
        DetermineHand();
        ManageEvent(SubMode.SUBSCRIBING);
        SetGunSound();
        EquipmentManager.Instance.SetGun(this, hand); // Sends a referrence, so the ammo clip can reload this gun.
    }

    public void OnUnequip()
    {
        transform.parent = null;
        ManageEvent(SubMode.UNSUBSCRIBING);
        EquipmentManager.Instance.SetGun(null, hand); // Sends a null referrence, so the ammo clip is unusable when there is no gun equipped.
        rigidbody = transform.AddComponent<Rigidbody>();
        StartCoroutine(Throw());
    }
    #endregion

}
