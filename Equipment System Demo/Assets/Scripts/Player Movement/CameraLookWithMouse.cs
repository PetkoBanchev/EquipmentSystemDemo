using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookWithMouse : MonoBehaviour
{
    #region Private variables
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerBody;

    private float xRotation = 0f;
    private bool isCursorVisible = false;
    #endregion

    #region Private methods
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
        
        if(Input.GetKeyDown(KeyCode.C))
            ToggleCursor();
    }

    private void ToggleCursor()
    {
        if (isCursorVisible)
            Cursor.visible = false;
        else
            Cursor.visible = true;
        isCursorVisible = !isCursorVisible;
    }
    #endregion
}
