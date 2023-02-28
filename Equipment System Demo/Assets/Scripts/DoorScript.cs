using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour, IInteractive
{
    [SerializeField] private string tooltipText = "Press E to open the door";
    [SerializeField] private ObjectType objectType = ObjectType.INTERACTIVE;

    public string TooltipText
    {
        get { return tooltipText; }
    }

    public ObjectType ObjectType
    {
        get { return objectType; }
    }

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
        Debug.Log("Open Door");
    }
}
