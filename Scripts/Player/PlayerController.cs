using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Reference to my kinematic character
    private KinematicCharacter2D kinematicCharacter;

    void Start()
    {
        // Copy stats
        kinematicCharacter = GetComponent<KinematicCharacter2D>();
    }

    void Update()
    {
        float move = ((Input.GetKey(KeyCode.RightArrow) ? 1 : 0)) - ((Input.GetKey(KeyCode.LeftArrow) ? 1 : 0));
        kinematicCharacter.Move(move);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            kinematicCharacter.Jump();
        }

        if (Input.GetKeyUp(KeyCode.Z) && !kinematicCharacter.isFalling)
        {
            kinematicCharacter.JumpCancel();
        }
    }
}
