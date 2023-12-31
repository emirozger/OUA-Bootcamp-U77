using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovementAdvanced : MonoBehaviour
{

    #region veriables
    [Header("Movement")]
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallrunSpeed;
    public float climbSpeed;
    public float vaultSpeed;
    public float airMinSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Dashing")]
    public float dashDistance;
    public float dashDuration;
    public float dashCooldown = 3f;
    private bool dashing;
    private Vector3 dashDirection;
    private bool dashReady = true;
    public float dashShakeDuration;
    public float dashShakeStrength;

    [Header("Keybinds")]
    public KeyCode dashKey = KeyCode.E;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public LayerMask obstacleLayer;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("References")]
    public Climbing climbingScript;
    public PlayerCam cam;

    public Transform orientation;
    public Vector3 startPos;
    public float horizontalInput;
    public float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    public MovementState state;
    public enum MovementState
    {
        freeze,
        unlimited,
        walking,
        sprinting,
        wallrunning,
        climbing,
        vaulting,
        crouching,
        sliding,
        air
    }

    public bool sliding;
    public bool crouching;
    public bool wallrunning;
    public bool climbing;
    public bool vaulting;

    public bool freeze;
    public bool unlimited;

    public bool restricted;
    bool wasGrounded = false;

    #endregion
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;

    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        if (!wasGrounded && grounded)
        {
            AudioManager.Instance.PlayOneShot("Grounded");
        }
        wasGrounded = grounded;
        MyInput();
        SpeedControl();
        StateHandler();
        LastDash();

        if (state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()

    {
        MovePlayer();
    }
    private void LastDash()
    {
        if (Input.GetKeyDown(dashKey))
        {
            if (dashReady && !dashing)
            {
                if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
                {
                    DashForwardRight();
                }
                else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
                {
                    DashForwardLeft();
                }
                else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
                {
                    DashBackwardLeft();
                }
                else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
                {
                    DashBackwardRight();
                }
                else if (Input.GetKey(KeyCode.W))
                {
                    DashForward();
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    DashLeft();
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    DashBackward();
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    DashRight();
                }
                else
                    DashForward();
            }
        }
    }
    private void DashLeft()
    {
        if (!dashing)
        {
            dashDirection = -orientation.right;
            StartDash();
        }
    }

    private void DashRight()
    {
        if (!dashing)
        {
            dashDirection = orientation.right;
            StartDash();
        }
    }

    private void DashBackward()
    {
        if (!dashing)
        {
            dashDirection = -orientation.forward;
            StartDash();
        }
    }
    private void DashForward()


    {
        if (!dashing)
        {
            dashDirection = orientation.forward;
            StartDash();
        }
    }
    private void DashForwardLeft()
    {
        if (!dashing)
        {
            dashDirection = (orientation.forward - orientation.right).normalized;
            StartDash();
        }
    }
    private void DashBackwardRight()
    {
        if (!dashing)
        {
            dashDirection = (-orientation.forward + orientation.right).normalized;
            StartDash();
        }
    }
    private void DashBackwardLeft()
    {
        if (!dashing)
        {
            dashDirection = (-orientation.forward - orientation.right).normalized;
            StartDash();
        }
    }
    private void DashForwardRight()
    {
        if (!dashing)
        {
            dashDirection = (orientation.forward + orientation.right).normalized;
            StartDash();
        }
    }
    private void StartDash()
    {
        AudioManager.Instance.PlayOneShot("Dash");
        dashDirection.y = 0f;
        dashDirection.Normalize();
        dashing = true;
        dashReady = false;

        RaycastHit hit;
        Vector3 dashDestination = transform.position + dashDirection * dashDistance;

        if (Physics.Raycast(transform.position, dashDirection, out hit, dashDistance, obstacleLayer))
        {
            dashDestination = hit.point;

            //StopDash();
        }

        transform.DOMove(dashDestination, dashDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(StopDash)
            .SetDelay(dashCooldown);

        //cam.DoFov(90, .5f);

        cam.transform.DOShakePosition(dashShakeDuration, dashShakeStrength);
    }
    private void ResetDashCooldown()
    {
        dashReady = true;
    }

    private void StopDash()
    {
        dashing = false;
        ResetDashCooldown();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey) && horizontalInput == 0 && verticalInput == 0 && !sliding)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            crouching = true;
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

            crouching = false;
        }
    }

    bool keepMomentum;
    private void StateHandler()
    {

        if (freeze)
        {

            state = MovementState.freeze;
            rb.velocity = Vector3.zero;
            desiredMoveSpeed = 0f;
        }


        else if (unlimited)
        {

            state = MovementState.unlimited;
            desiredMoveSpeed = 9f;
        }


        else if (vaulting)
        {

            state = MovementState.vaulting;
            desiredMoveSpeed = vaultSpeed;
        }


        else if (climbing)
        {
            cam.DoFov(90, 1);
            state = MovementState.climbing;
            desiredMoveSpeed = climbSpeed;
        }


        else if (wallrunning)
        {
            cam.DoFov(90, 1);
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }


        else if (sliding)
        {
            cam.DoFov(90, 1);
            state = MovementState.sliding;
            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
                keepMomentum = true;
            }

            else
                desiredMoveSpeed = sprintSpeed;
        }


        else if (crouching)
        {
            cam.DoFov(70, 1);
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }


        else if (grounded && Input.GetKey(sprintKey))
        {
            cam.DoFov(90, 1);
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }


        else if (grounded)
        {
            cam.DoFov(70, 1);
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }


        else
        {
            state = MovementState.air;

            if (moveSpeed < airMinSpeed)
            {
                desiredMoveSpeed = airMinSpeed;
            }

        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;


        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {

        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        if (climbingScript.exitingWall) return;
        if (restricted) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;


        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }

        }


        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }



        if (!wallrunning) rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {

        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }


        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        AudioManager.Instance.PlayOneShot("Jump");
        exitingSlope = true;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Respawn"))
        {
            transform.position = startPos;
        }
    }
}
