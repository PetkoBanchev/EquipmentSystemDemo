using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private SelectionManager selectionManager;

    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform head;

    private bool isLeftHandEquiped = false;
    private bool isRightHandEquiped = false;
    private bool isHeadEquiped = false;

    private IEquipable LeftHandGun;
    private IEquipable RightHandGun;

    private static EquipmentManager instance;
    public static EquipmentManager Instance { get { return instance; } }

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

    private void EquipRightHand()
    {
        var selection = selectionManager.GetSelection();
        if (selection != null)
        {
            if (selection.ObjectType == ObjectType.EQUIPABLE)
            {
                if (isRightHandEquiped)
                    UnequipRightHand();
                EquipItem(selection.Transform, rightHand);
                InputManager.Instance.OnRightHandUnequip += UnequipRightHand; // Subscribes to the unequip event only after an item is equiped
                isRightHandEquiped = true;
            }
        }
    }

    // Helper method
    private void EquipItem(Transform itemTransform, Transform parentTransform)
    {
        itemTransform.SetParent(parentTransform);
        itemTransform.localPosition = Vector3.zero;
        itemTransform.localRotation = Quaternion.identity;
        itemTransform.GetComponent<IEquipable>().OnEquip();
    }

    private void UnequipLeftHand()
    {
        leftHand.GetChild(1).GetComponent<IEquipable>().OnUnequip();
        InputManager.Instance.OnLeftHandUnequip -= UnequipLeftHand; // Unsubscribes from the event, so we can't trigger it on an empty hand
        isLeftHandEquiped = false;
    }

    private void UnequipRightHand()
    {
        rightHand.GetChild(1).GetComponent<IEquipable>().OnUnequip();
        InputManager.Instance.OnRightHandUnequip -= UnequipRightHand; // Unsubscribes from the event, so we can't trigger it on an empty hand
        isRightHandEquiped = false;
    }

    private void UnequipHead()
    {
        head.GetChild(1).GetComponent<IEquipable>().OnUnequip();
        InputManager.Instance.OnHeadUnequip -= UnequipHead;
        isHeadEquiped = false;
    }

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

    public IEquipable GetGun(Hand hand)
    {
        if (hand == Hand.LEFT)
            return RightHandGun;
        else
            return LeftHandGun;
    }

    public void SetGun(IEquipable gun, Hand hand)
    {
        if (hand == Hand.LEFT)
            LeftHandGun = gun;
        else
            RightHandGun = gun;
    }
}
