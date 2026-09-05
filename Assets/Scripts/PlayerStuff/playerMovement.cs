using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class playerMovement : MonoBehaviour
{
    #region Variables
    [Header("Basic Movement")]
    [SerializeField] private float moveForce;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float maxSpeed;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform root;
    private Vector2 moveValue;
    private float moveValueAbs;
    private InputAction moveAction;

    [Header("Jump")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float castRadius;
    [SerializeField] private float castLength;
    [SerializeField] private float initialJumpForce;
    [SerializeField] private float contJumpForce;
    [SerializeField] private float contJumpDuration;
    [SerializeField] private float jumpDelay;
    [SerializeField] private PhysicsMaterial2D PhysicsMaterial2D;
    [SerializeField, Range(0f, 0.3f)] private float coyoteTime;
    [SerializeField, Range(0f, 1f)] private float airSpeedMultiplier;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float jumpCooldown;
    private float jumpCoolTime = 0f;
    private float tempAirSpeedMultiplier = 0f;
    private float airSpeedMultiplierTimeSpan = 0.3f;
    private Coroutine IncreaserCoroutine;
    private Coroutine JumpCoroutine;
    private InputAction jumpAction;
    private bool hasJumped;
    private bool isGrounded;
    RaycastHit2D hit2D;

    public Action JumpDelegate;
    public bool HasJumped
    {
        get => hasJumped;
        set
        {
            if (value !=  hasJumped)
            {
                if (value == true)
                {
                    jumpCoolTime = 0f;
                    JumpDelegate?.Invoke();
                    hasJumped = value;
                }
                else
                {
                    hasJumped = value;
                }
            }
        }
    }
    private float eltime = 0f;
    public bool IsGrounded
    {
        get => isGrounded;
        set
        {
            eltime += Time.fixedDeltaTime;
            if (value) eltime = 0f;
            if (value != isGrounded)
            {
                if (value)
                {
                    rb.sharedMaterial = null;
                    isGrounded = value;
                }
                else
                {
                    if (eltime > coyoteTime)
                    {
                        isGrounded = value;
                    }
                    rb.sharedMaterial = PhysicsMaterial2D;

                }
            }
        }
    }
    #endregion
    #region Built In Functions
    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        if (moveAction != null) moveAction.Enable();
        jumpAction = InputSystem.actions.FindAction("Jump");
        if (jumpAction != null) jumpAction.Enable();
    }
    private void OnEnable()
    {
        JumpDelegate += HandleJump;
        jumpCoolTime = jumpCooldown;
    }
    private void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        JumpDelegate -= HandleJump; 
    }
    private void FixedUpdate()
    {
        Move();
        bool shouldJump = JumpCooledDown();
        if (IsGrounded && (jumpAction.WasPressedThisFrame() || jumpAction.IsInProgress()) && shouldJump) HasJumped = true;
        else HasJumped = false;
    }
    #endregion
    #region Movement Functions
    private void Move()
    {
        moveValue.x = moveAction.ReadValue<Vector2>().x;
        if (moveValue.x < 0f)
        {
            orientation.eulerAngles = new Vector3(0f, 180f, 0f);
            root.eulerAngles = new Vector3(0f, 0f, root.eulerAngles.z);
        }
        else if (moveValue.x > 0f)
        {
            orientation.eulerAngles = new Vector3(0f, 0f, 0f);
            root.eulerAngles = new Vector3(0f, 180f, root.eulerAngles.z);
        }
            moveValueAbs = Mathf.Abs(moveValue.x);
        moveValue.y = 0f;
        moveValue = (new Vector2(hit2D.normal.y, - hit2D.normal.x)) * moveValue.x;
        float tempAirSpeed = GroundCheck();
        rb.AddForce(moveValue * moveForce * tempAirSpeed, ForceMode2D.Force);
        if (moveValue.magnitude <= 0.01f)
        {
            rb.AddForce(new Vector2(moveAction.ReadValue<Vector2>().x * tempAirSpeed, 0f) * moveForce, ForceMode2D.Force);
        }
        LimitSpeed(maxSpeed);
    }
    private float GroundCheck()
    {
        hit2D = Physics2D.CircleCast(transform.position, castRadius, Vector2.down, castLength, whatIsGround);
        if (hit2D && hit2D.normal.y > 0.5f)
        {
            IsGrounded = true;
            if (tempAirSpeedMultiplier != 0f)
            {
                airSpeedMultiplier = tempAirSpeedMultiplier;
            }
            if (IncreaserCoroutine != null)
            {
                StopCoroutine(IncreaserCoroutine);
                IncreaserCoroutine = null; 
            }
            //Normal Force
            //rb.AddForce(-(rb.gravityScale * rb.mass * Physics2D.gravity), ForceMode2D.Force);
            return 1f;
        }
        else
        {
            IsGrounded = false;
            if (IncreaserCoroutine == null)
            {
                tempAirSpeedMultiplier = airSpeedMultiplier;
                if (moveValueAbs > 0.9f)
                {
                    return 1f;
                }
                IncreaserCoroutine = StartCoroutine(JumpMultiplierIncreaser(airSpeedMultiplier, airSpeedMultiplierTimeSpan));
            }
            return airSpeedMultiplier;
        }
    }
    private void LimitSpeed(float speed)
    {
        float flatVel = rb.linearVelocity.x;
        if (Mathf.Abs(flatVel) > speed) rb.linearVelocity = new Vector2(Mathf.Clamp(flatVel, -speed, speed), rb.linearVelocity.y);
    }
    #endregion
    #region Jump Mechanics
    private void HandleJump()
    {
        if (JumpCoroutine != null)
        {
            StopCoroutine(JumpCoroutine);
            StartCoroutine(Jump());
        }
        else JumpCoroutine = StartCoroutine(Jump());
    }
    private bool JumpCooledDown()
    {
        jumpCoolTime += Time.fixedDeltaTime;
        Mathf.Clamp(jumpCoolTime, 0f, jumpCooldown);
        if (jumpCoolTime >= jumpCooldown)
        {
            return true;
        }
        return false;
    }
    IEnumerator Jump()
    {
        yield return new WaitForSeconds(jumpDelay);
        float temp = contJumpForce;
        float jumpForceTemp = temp;
        rb.linearVelocityY = 0f;
        rb.AddForceY(initialJumpForce, ForceMode2D.Impulse);
        float elapsedTime = 0f;
        while (elapsedTime < contJumpDuration)
        {
            elapsedTime += Time.fixedDeltaTime;
            if (jumpAction.IsInProgress())
            {
                rb.AddForceY(jumpForceTemp, ForceMode2D.Force);
                jumpForceTemp = Mathf.Lerp(temp, 0f, jumpCurve.Evaluate(elapsedTime / contJumpDuration));
            }
            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator JumpMultiplierIncreaser(float multiplier, float timeSpan)
    {
        float elapsedTime = 0f;
        while (elapsedTime < timeSpan)
        {
            yield return new WaitUntil(() => moveValueAbs > 0.1f);
            elapsedTime += Time.fixedDeltaTime;
            airSpeedMultiplier = Mathf.Lerp(multiplier, 1f, elapsedTime / timeSpan);
            yield return new WaitForFixedUpdate();
        }
        
    }
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (Vector3.down * castLength), castRadius);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(rb.totalForce.x, rb.totalForce.y, 0f));
    }
}
