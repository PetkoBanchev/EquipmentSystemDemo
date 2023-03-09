using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public event Action OnFire1; // Left Mouse button down
    public event Action OnFire2; // Right Mouse button down
    public event Action OnFire1Released; // Left Mouse button up
    public event Action OnFire2Released; // Right Mouse button up
    public event Action OnInteractWithObject; // E 
    public event Action OnRightHandEquip; // Shift + E
    public event Action OnLeftHandUnequip; // Q
    public event Action OnRightHandUnequip; // Shift + Q

    #region Singleton
    private static InputManager instance;
    public static InputManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnFire1?.Invoke();

        if (Input.GetMouseButtonDown(1))
            OnFire2?.Invoke();

        if (Input.GetMouseButtonUp(0))
            OnFire1Released?.Invoke();

        if (Input.GetMouseButtonUp(1))
            OnFire2Released?.Invoke();

        if (Input.GetKeyDown(KeyCode.E) && !Input.GetKey(KeyCode.LeftShift))
            OnInteractWithObject?.Invoke();

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.E)) // Uses GetKey for the shift to prevent firing the OnInteractWithObject event
            OnRightHandEquip?.Invoke();

        if (Input.GetKeyDown(KeyCode.Q) && !Input.GetKey(KeyCode.LeftShift))
            OnLeftHandUnequip?.Invoke();

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q)) // Uses GetKey for the shift to prevent firing the OnLeftHandUnequip event
            OnRightHandUnequip?.Invoke();
    }
}
