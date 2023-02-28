using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectInteractionManager : MonoBehaviour
{
    private SelectionManager selectionManager;

    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform head;

    private void Awake()
    {
        selectionManager = GetComponent<SelectionManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)) 
        {
            var selection = selectionManager.GetSelection();
            if (selection != null)
            {
                if(selection.ObjectType == ObjectType.INTERACTIVE)
                {
                    selection.Transform.GetComponent<IInteractive>().TriggerInteraction();
                }
                else if(selection.ObjectType == ObjectType.EQUIPABLE)
                {
                    selection.Transform.SetParent(leftHand);
                    selection.Transform.localPosition = Vector3.zero;
                    selection.Transform.localRotation = Quaternion.identity;
                    selection.Transform.GetComponent<IEquipable>().OnEquip();
                }
            }
        }

        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.E))
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
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Debug.Log(rightHand.childCount);
                rightHand.GetChild(1).GetComponent<IEquipable>().OnUnequip();
            }
            else
            {
                leftHand.GetChild(1).GetComponent<IEquipable>().OnUnequip();
            }
        }

    }
}
