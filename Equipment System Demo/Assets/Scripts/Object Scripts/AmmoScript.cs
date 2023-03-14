using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AmmoScript : MonoBehaviour, IEquipable
{
    #region Private variables
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
    #endregion

    #region Public properties
    public int BulletCount
    {
        get { return bulletCount; }
        set { bulletCount = value; }
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
        reloadSound = GetComponent<AudioSource>();
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
    /// <summary>
    /// Reloads the gun and destroys the game object. 
    /// A small the delay is used to make sure the reload sound is played fully.
    /// </summary>
    /// <param name="_gun"></param>
    /// <returns></returns>
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
                InputManager.Instance.OnFire1 += ReloadGun;
            }
            else
            {
                InputManager.Instance.OnFire2 += ReloadGun;
            }
        }
        else
        {
            if (hand == Hand.LEFT)
            {
                InputManager.Instance.OnFire1 -= ReloadGun;
                EquipmentManager.Instance.UnsubscribeHand(hand);
            }
            else
            {
                InputManager.Instance.OnFire2 -= ReloadGun;
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
    #endregion
}
