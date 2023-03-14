using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Private variables
    private SelectionManager selectionManager;

    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform head;

    private bool isLeftHandEquiped = false;
    private bool isRightHandEquiped = false;
    private bool isHeadEquiped = false;

    private IEquipable LeftHandGun;
    private IEquipable RightHandGun;
    #endregion

    #region Singleton
    private static EquipmentManager instance;
    public static EquipmentManager Instance { get { return instance; } }
    #endregion

    #region Private methods
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        selectionManager = GetComponent<SelectionManager>();
    }

    private void Start()
    {
        InputManager.Instance.OnInteractWithObject += InteractWithObject;
        InputManager.Instance.OnRightHandEquip += EquipRightHand;
    }

    /// <summary>
    /// Triggered by the OnInteractWithObject event (The E key)
    /// Determines the type of object. If it's interactive it triggers the interaction.
    /// If it's equipable it equips in the appropriate place and subscribes it to the proper unequip event.
    /// </summary>
    private void InteractWithObject()
    {
        var selection = selectionManager.GetSelection();
        if (selection != null)
        {
            if (selection.ObjectType == ObjectType.INTERACTIVE)
            {
                selection.Transform.GetComponent<IInteractive>().TriggerInteraction();
            }
            else if (selection.ObjectType == ObjectType.EQUIPABLE)
            {
                var equipable = selection.Transform.GetComponent<IEquipable>();

                if (equipable.EquipableType == EquipableType.HAND) 
                {
                    if (isLeftHandEquiped)
                        UnequipLeftHand();
                    EquipItem(selection.Transform, leftHand);
                    InputManager.Instance.OnLeftHandUnequip += UnequipLeftHand; // Subscribes to the unequip event only after an item is equiped
                    isLeftHandEquiped = true;
                }
                else if (equipable.EquipableType == EquipableType.HEAD)
                {
                    if(isHeadEquiped) 
                        UnequipHead();
                    EquipItem(selection.Transform, head);
                    InputManager.Instance.OnHeadUnequip += UnequipHead; // Subscribes to the unequip event only after an item is equiped
                    isHeadEquiped = true;
                }
            }
        }
    }

    /// <summary>
    /// Triggered by the OnRightHandEquip event (Shift + E)
    /// Equips the object to the right hand and subscribes it to the unequip event.
    /// </summary>
    private void EquipRightHand()
    {
        var selection = selectionManager.GetSelection();
        if (selection != null)
        {
            if (selection.ObjectType == ObjectType.EQUIPABLE)
            {
                if (selection.Transform.GetComponent<IEquipable>().EquipableType == EquipableType.HEAD) // Prevents head objects being equipped to the right hand
                    return;
                if (isRightHandEquiped)
                    UnequipRightHand();
                EquipItem(selection.Transform, rightHand);
                InputManager.Instance.OnRightHandUnequip += UnequipRightHand; // Subscribes to the unequip event only after an item is equiped
                isRightHandEquiped = true;
            }
        }
    }

    /// <summary>
    /// Helper method to set the correct parent of the equiped item
    /// And reset its transform
    /// </summary>
    /// <param name="itemTransform"></param>
    /// <param name="parentTransform"></param>
    private void EquipItem(Transform itemTransform, Transform parentTransform)
    {
        itemTransform.SetParent(parentTransform);
        itemTransform.localPosition = Vector3.zero;
        itemTransform.localRotation = Quaternion.identity;
        itemTransform.GetComponent<IEquipable>().OnEquip();
    }

    /// <summary>
    /// Unequips the left hand and unsubscribes from the unequip event
    /// </summary>
    private void UnequipLeftHand()
    {
        leftHand.GetChild(1).GetComponent<IEquipable>().OnUnequip();
        InputManager.Instance.OnLeftHandUnequip -= UnequipLeftHand; // Unsubscribes from the event, so we can't trigger it on an empty hand
        isLeftHandEquiped = false;
    }

    /// <summary>
    /// Unequips the right hand and unsubscribes from the unequip event
    /// </summary>
    private void UnequipRightHand()
    {
        rightHand.GetChild(1).GetComponent<IEquipable>().OnUnequip();
        InputManager.Instance.OnRightHandUnequip -= UnequipRightHand; // Unsubscribes from the event, so we can't trigger it on an empty hand
        isRightHandEquiped = false;
    }

    /// <summary>
    /// Unequips the head and unsubscribes from the unequip event
    /// </summary>
    private void UnequipHead()
    {
        head.GetChild(1).GetComponent<IEquipable>().OnUnequip();
        InputManager.Instance.OnHeadUnequip -= UnequipHead;
        isHeadEquiped = false;
    }

    #endregion

    #region Public methods
    /// <summary>
    /// Public method to help unsubcribe a given hand remotely.
    /// This is applicable to single use items. (Rock, ammo clip, etc.)
    /// </summary>
    /// <param name="_hand"></param>
    public void UnsubscribeHand(Hand _hand)
    {
        if(_hand == Hand.LEFT)
        {
            InputManager.Instance.OnLeftHandUnequip -= UnequipLeftHand;
            isLeftHandEquiped = false;
        }
        else
        {
            InputManager.Instance.OnRightHandUnequip -= UnequipRightHand;
            isRightHandEquiped = false;
        }
    }

    /// <summary>
    /// Returns a referrence to the gun in the opposite hand of the ammo holding hand. (If the ammo is in the left hand it returns a referrence to the gun in the right hand) 
    /// If the hand doesn't have a gun equiped it returns null.
    /// This is used for the ammo clip
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    public IEquipable GetGun(Hand hand)
    {
        if (hand == Hand.LEFT)
            return RightHandGun;
        else
            return LeftHandGun;
    }

    /// <summary>
    /// Caches a referrence to the gun in the selected hand.
    /// On unequip it nullifies the referrence.
    /// This is also used for the ammo clip
    /// </summary>
    /// <param name="gun"></param>
    /// <param name="hand"></param>
    public void SetGun(IEquipable gun, Hand hand)
    {
        if (hand == Hand.LEFT)
            LeftHandGun = gun;
        else
            RightHandGun = gun;
    }

    #endregion
}
