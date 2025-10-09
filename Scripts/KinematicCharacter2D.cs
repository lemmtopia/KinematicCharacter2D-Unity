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
    public bool hasLandedThisFrame;

    // Rigidbody2D - MUST BE KINEMATIC!!!
    private Rigidbody2D _rb;

    // My movement vector for this frame.
    private Vector2 _motion;

    // Coyote time
    private float _coyoteTime;

    // Jump buffer
    private float _jumpBuffer;

    void Start()
    {
        // Getting my rb
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
    }

    void FixedUpdate()
    {
        // My position before moving this frame
        Vector2 myPos = _rb.position;

        // Fall
        _motion.y -= stats.gravity * Time.deltaTime;

        // Horizontal collision
        if (CollideAt(myPos.x + (_motion.x * Time.deltaTime), myPos.y, solidMask))
        {
            while (!CollideAt(myPos.x + (Mathf.Sign(_motion.x) * Time.deltaTime), myPos.y, solidMask))
            {
                myPos.x += (Mathf.Sign(_motion.x) * Time.deltaTime);
            }

            _motion.x = 0;
        }
        myPos.x += _motion.x * Time.deltaTime;

        // Vertical Collision
        if (CollideAt(myPos.x, myPos.y + (_motion.y * Time.deltaTime), solidMask))
        {
            while (!CollideAt(myPos.x, myPos.y + (Mathf.Sign(_motion.y) * Time.deltaTime), solidMask))
            {
                myPos.y += (Mathf.Sign(_motion.y) * Time.deltaTime);
            }

            _motion.y = 0;
        }
        myPos.y += _motion.y * Time.deltaTime;

        // Update my position based on myPos
        _rb.MovePosition(myPos);

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
        hasLandedThisFrame = false;

        // If I'm moving
        if (_motion.x != 0)
        {
            // Change isFacingRight
            isFacingRight = (_motion.x > 0) ? true : false;
        }

        // Checking if my feet are colliding
        if (CollideAt(myPos.x, myPos.y - (1f / 8f), solidMask))
        {
            // Set coyote time
            _coyoteTime = stats.coyoteTimeMax;

            if (!isGrounded)
			{
                hasLandedThisFrame = true;
			}

            isGrounded = true;
            isFalling = false;
        }
        else
        {
            isGrounded = false;     // Not on ground

            // Check if I'm falling
            if (_motion.y < 0)
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
        _coyoteTime -= Time.deltaTime;
        _jumpBuffer -= Time.deltaTime;
        if (_jumpBuffer > 0 && _coyoteTime > 0)
        {
            _motion.y = stats.jumpHeight;

            _jumpBuffer = 0;
            _coyoteTime = 0;
        }
    }

    public void Move(float move)
    {
        // Accelerate
        if (isGrounded)
        {
            _motion.x += move * stats.groundAcceleration * Time.deltaTime;
        }
        else
        {
            if (!stats.hasAirControl)
            {
                return;
            }

            _motion.x += move * stats.airAcceleration * Time.deltaTime;
        }

        // Decelerate
        if (move == 0)
        {
            if (isGrounded)
            {
                _motion.x = Mathf.Lerp(_motion.x, 0, stats.groundDeceleration * Time.deltaTime);
            }
            else
            {
                _motion.x = Mathf.Lerp(_motion.x, 0, stats.airDeceleration * Time.deltaTime);
            }
        }
        
        // Limit our motion
        _motion.x = Mathf.Clamp(_motion.x, -stats.moveSpeed, stats.moveSpeed);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            // Jump
            _motion.y = stats.jumpHeight;

            _jumpBuffer = 0;
            _coyoteTime = 0;
        }
        else
        {
            // Try to jump later
            _jumpBuffer = stats.jumpBufferMax;
        }
    }

    public void JumpCancel()
    {
        if (stats.jumpCancelDivisor <= 1)
        {
            return;
        }

        if (_motion.y > stats.jumpHeight / stats.jumpCancelDivisor)
        {
            _motion.y /= stats.jumpCancelDivisor;
        }
    }
}
