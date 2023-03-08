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

    private void Awake()
    {
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
                selection.Transform.SetParent(leftHand);
                selection.Transform.localPosition = Vector3.zero;
                selection.Transform.localRotation = Quaternion.identity;
                selection.Transform.GetComponent<IEquipable>().OnEquip();

                InputManager.Instance.OnLeftHandUnequip += UnequipLeftHand; // Subscribes to the unequip event only after an item is equiped
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
                selection.Transform.SetParent(rightHand);
                selection.Transform.localPosition = Vector3.zero;
                selection.Transform.localRotation = Quaternion.identity;
                selection.Transform.GetComponent<IEquipable>().OnEquip();

                InputManager.Instance.OnRightHandUnequip += UnequipRightHand; // Subscribes to the unequip event only after an item is equiped
            }
        }
    }

    private void UnequipLeftHand()
    {
        leftHand.GetChild(1).GetComponent<IEquipable>().OnUnequip();
        InputManager.Instance.OnLeftHandUnequip -= UnequipLeftHand; // Unsubscribes from the event, so we can't trigger it on an empty hand
    }

    private void UnequipRightHand()
    {
        rightHand.GetChild(1).GetComponent<IEquipable>().OnUnequip();
        InputManager.Instance.OnRightHandUnequip -= UnequipRightHand; // Unsubscribes from the event, so we can't trigger it on an empty hand
    }
}
