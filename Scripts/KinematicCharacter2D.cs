using UnityEngine;

public class KinematicCharacter2D : MonoBehaviour
{
    [Header("References")]
    public KinematicCharacterStats2D stats;
    [SerializeField] private LayerMask solidMask;
    [SerializeField] private BoxCollider2D hitbox;

    [Header("Status Flags")]
    public bool isGrounded;
    public bool isFalling;
    public bool isFacingRight;

    // Rigidbody2D - MUST BE KINEMATIC!!!
    private Rigidbody2D rb;

    // My movement vector for this frame.
    private Vector2 motion;

    // Coyote time
    private float coyoteTime;

    // Jump buffer
    private float jumpBuffer;

    void Start()
    {
        // Getting my rb
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
    }

    void FixedUpdate()
    {
        // My position before moving this frame
        Vector2 myPos = rb.position;

        // Fall
        motion.y -= stats.gravity * Time.deltaTime;

        // Horizontal collision
        if (CollideAt(myPos.x + (motion.x * Time.deltaTime), myPos.y, solidMask))
        {
            while (!CollideAt(myPos.x + (Mathf.Sign(motion.x) * Time.deltaTime), myPos.y, solidMask))
            {
                myPos.x += (Mathf.Sign(motion.x) * Time.deltaTime);
            }

            motion.x = 0;
        }
        myPos.x += motion.x * Time.deltaTime;

        // Vertical Collision
        if (CollideAt(myPos.x, myPos.y + (motion.y * Time.deltaTime), solidMask))
        {
            while (!CollideAt(myPos.x, myPos.y + (Mathf.Sign(motion.y) * Time.deltaTime), solidMask))
            {
                myPos.y += (Mathf.Sign(motion.y) * Time.deltaTime);
            }

            motion.y = 0;
        }
        myPos.y += motion.y * Time.deltaTime;

        // Update my position based on myPos
        rb.MovePosition(myPos);

        UpdateStatusFlags(myPos);

        HandleJumpingOutsideGround();
    }

    // Basically a Unity version og GMS2's place_meeting()
    private bool CollideAt(float xPos, float yPos, LayerMask mask)
    {
        Vector2 pos = new Vector2(xPos + hitbox.offset.x, yPos + hitbox.offset.y);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, hitbox.size, 0, mask);
        return (colliders.Length > 0);
    }

    // Updates the public instance status variables (isFalling, isGrounded etc.)
    private void UpdateStatusFlags(Vector2 myPos)
    {
        // If I'm moving
        if (motion.x != 0)
        {
            // Change isFacingRight
            isFacingRight = (motion.x > 0) ? true : false;
        }

        // Checking if my feet are colliding
        if (CollideAt(myPos.x, myPos.y - (1f / 8f), solidMask))
        {
            // Set coyote time
            coyoteTime = stats.coyoteTimeMax;

            isGrounded = true;
            isFalling = false;
        }
        else
        {
            isGrounded = false;     // Not on ground

            // Check if I'm falling
            if (motion.y < 0)
            {
                isFalling = true;
            }
            else
            {
                isFalling = false;
            }
        }
    }

    private void HandleJumpingOutsideGround()
    {
        coyoteTime -= Time.deltaTime;
        jumpBuffer -= Time.deltaTime;
        if (jumpBuffer > 0 && coyoteTime > 0)
        {
            motion.y = stats.jumpHeight;

            jumpBuffer = 0;
            coyoteTime = 0;
        }
    }

    public void Move(float move)
    {
        // Accelerate
        if (isGrounded)
        {
            motion.x += move * stats.groundAcceleration * Time.deltaTime;
        }
        else
        {
            motion.x += move * stats.airAcceleration * Time.deltaTime;
        }

        // Decelerate
        if (move == 0)
        {
            if (isGrounded)
            {
                motion.x = Mathf.Lerp(motion.x, 0, stats.groundDeceleration * Time.deltaTime);
            }
            else
            {
                motion.x = Mathf.Lerp(motion.x, 0, stats.airDeceleration * Time.deltaTime);
            }
        }
        
        // Limit our motion
        motion.x = Mathf.Clamp(motion.x, -stats.moveSpeed, stats.moveSpeed);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            // Jump
            motion.y = stats.jumpHeight;

            jumpBuffer = 0;
            coyoteTime = 0;
        }
        else
        {
            // Try to jump later
            jumpBuffer = stats.jumpBufferMax;
        }
    }

    public void JumpCancel()
    {
        if (motion.y > stats.jumpHeight / stats.jumpCancelDivisor)
        {
            motion.y /= stats.jumpCancelDivisor;
        }
    }
}
