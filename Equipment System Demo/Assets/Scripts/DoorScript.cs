using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour, IInteractive
{
    [SerializeField] private string tooltipText = "Press E to open the door";
    [SerializeField] private ObjectType objectType = ObjectType.INTERACTIVE;

    [SerializeField] private bool isDoorOpen = false;

    public string TooltipText { get { return tooltipText; } }
    public ObjectType ObjectType { get { return objectType; } }
    public Transform Transform { get { return transform; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerInteraction()
    {
        ToggleDoor();
    }

    private void ToggleDoor()
    {
        if (!isDoorOpen) 
        {
            Debug.Log("Opened Door");
            transform.parent.Rotate(0, -90, 0);
            tooltipText = "Press E to close the door";
        }
        else
        {
            Debug.Log("Closed Door");
            transform.parent.Rotate(0, 90, 0);
            tooltipText = "Press E to open the door";
        }

        isDoorOpen= !isDoorOpen;
    }
}
