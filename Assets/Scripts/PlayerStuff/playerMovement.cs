using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class playerMovement : MonoBehaviour
{

    [Header("Basic Movement")]
    [SerializeField] private float moveForce;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float maxSpeed;
    private InputAction moveAction;

    [Header("Jump")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float castRadius;
    [SerializeField] private float castLength;
    [SerializeField] private float initialJumpForce;
    [SerializeField] private float contJumpForce;
    [SerializeField] private float contJumpDuration;
    [SerializeField] private float jumpDelay;
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
        GroundCheck();
    }
    private void Move()
    {
        Vector2 moveValue;
        moveValue.x = moveAction.ReadValue<Vector2>().x;
        moveValue.y = 0f;
        moveValue = (new Vector2(hit2D.normal.y, - hit2D.normal.x)) * moveValue.x;
        debugDir = hit2D.normal;
        rb.AddForce(moveValue * moveForce, ForceMode2D.Force);
        LimitSpeed(maxSpeed);
        Debug.Log(hit2D.normal.y);
    }
    private void GroundCheck()
    {
        
        hit2D = Physics2D.CircleCast(transform.position, castRadius, Vector2.down, castLength, whatIsGround);
        if (hit2D && hit2D.normal.y > 0.5f)
        {
            isGrounded = true;
            rb.AddForce(Vector2.up * rb.mass * rb.gravityScale, ForceMode2D.Force);
        }
        else isGrounded = false;
        Debug.Log(isGrounded);
    }
    private void LimitSpeed(float speed)
    {
        float flatVel = rb.linearVelocity.x;
        if (Mathf.Abs(flatVel) > speed) rb.linearVelocity = new Vector2(Mathf.Clamp(flatVel, -speed, speed), rb.linearVelocity.y);
    }
    private void HandleJump()
    {
        if (JumpCoroutine != null)
        {
            StopCoroutine(JumpCoroutine);
            StartCoroutine(Jump());
        }
        else JumpCoroutine = StartCoroutine(Jump());
    }
    IEnumerator Jump()
    {
        yield return new WaitForSeconds(jumpDelay);
        rb.AddForceY(initialJumpForce, ForceMode2D.Impulse);
        float elapsedTime = 0f;
        while (elapsedTime < contJumpDuration)
        {
            elapsedTime += Time.fixedDeltaTime;
            rb.AddForceY(contJumpForce, ForceMode2D.Force);
            yield return new WaitForFixedUpdate();
        }
    }
    private Vector2 debugDir;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (Vector3.down * castLength), castRadius);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(debugDir.x, debugDir.y, 0f));
    }
}
