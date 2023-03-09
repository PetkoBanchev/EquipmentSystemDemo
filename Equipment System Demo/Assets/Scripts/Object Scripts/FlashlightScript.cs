using System.Collections;
using System.Collections.Generic;
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

    private bool isLightOn = false;
    [SerializeField] private GameObject spotlight;

    public EquipableType EquipableType { get { return equipableType; } }
    public string TooltipText { get { return tooltipText; } }
    public ObjectType ObjectType { get { return objectType; } }
    public Transform Transform { get { return transform; } }

    private void Awake()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
        localScale = transform.localScale;
    }



    public void OnEquip()
    {
        if (rigidbody != null)
            Destroy(rigidbody);
        InputManager.Instance.OnFire1 += ToggleFlashlight;
    }

    public void OnUnequip()
    {
        transform.parent = null;
        rigidbody = transform.AddComponent<Rigidbody>();
        InputManager.Instance.OnFire1 -= ToggleFlashlight;
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
        transform.localScale = localScale;
    }

    private void ToggleFlashlight()
    {
        if (isLightOn)
        {
            isLightOn = false;
            spotlight.SetActive(false);
        }
        else
        {
            isLightOn = true;
            spotlight.SetActive(true);
        }
    }
}
