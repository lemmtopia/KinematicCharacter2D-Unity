using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Reference to my kinematic character
    private KinematicCharacter2D kinematicCharacter;

    private float move;

    void Start()
    {
        // Copy stats
        kinematicCharacter = GetComponent<KinematicCharacter2D>();
    }

    void FixedUpdate()
    {
        // Move based on move float.
        kinematicCharacter.Move(move);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<float>();
        //Debug.Log("Move Input: " + move);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Jump if we performed the action
        if (context.performed)
        {
            kinematicCharacter.Jump();
        }

        // Cancel the jump if we are jumping and we canceled the action
        if (context.canceled && !kinematicCharacter.isFalling)
        {
            kinematicCharacter.JumpCancel();
        }
    }
}
