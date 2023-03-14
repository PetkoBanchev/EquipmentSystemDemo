using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.4f;

    public bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, groundDistance, groundMask); ;
    }
}
