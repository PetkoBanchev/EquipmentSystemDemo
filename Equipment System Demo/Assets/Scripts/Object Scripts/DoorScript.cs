using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour, IInteractive
{
    #region Private variables
    [SerializeField] private string tooltipText = "Press E to open the door";
    [SerializeField] private ObjectType objectType = ObjectType.INTERACTIVE;
    [SerializeField] private bool isDoorOpen = false;
    #endregion

    #region Public properties
    public string TooltipText { get { return tooltipText; } }
    public ObjectType ObjectType { get { return objectType; } }
    public Transform Transform { get { return transform; } }
    #endregion
    public void TriggerInteraction()
    {
        ToggleDoor();
    }

    private void ToggleDoor()
    {
        if (!isDoorOpen) 
        {
            transform.parent.Rotate(0, -90, 0);
            tooltipText = "Press E to close the door";
        }
        else
        {
            transform.parent.Rotate(0, 90, 0);
            tooltipText = "Press E to open the door";
        }

        isDoorOpen= !isDoorOpen;
    }
}
