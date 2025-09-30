using UnityEngine;

[CreateAssetMenu(menuName = "Kinematic Character Stats 2D")]
public class KinematicCharacterStats2D : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundAcceleration;
    public float groundDeceleration;
    public float airAcceleration;
    public float airDeceleration;
    public float gravity;
    public float jumpHeight;
    public float jumpCancelDivisor = 1;
    public float coyoteTimeMax = 0.08f;
    public float jumpBufferMax = 0.06f;
    public bool hasAirControl;
}
