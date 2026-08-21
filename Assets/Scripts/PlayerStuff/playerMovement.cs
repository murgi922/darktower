using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    [Header("Basic Movement")]
    [SerializeField] private float moveForce;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float maxSpeed;
    private InputAction moveAction;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        if (moveAction != null) moveAction.Enable();
    }
    private void OnDisable()
    {
        moveAction?.Disable();
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void Move()
    {
        Vector2 moveValue;
        moveValue.x = moveAction.ReadValue<Vector2>().x;
        moveValue.y = 0f;
        rb.AddForce(moveValue * moveForce, ForceMode2D.Force);
        LimitSpeed(maxSpeed);
    }
    private void LimitSpeed(float speed)
    {
        float flatVel = rb.linearVelocity.x;
        if (Mathf.Abs(flatVel) > speed) rb.linearVelocity = new Vector2(Mathf.Clamp(flatVel, -speed, speed), rb.linearVelocity.y);
    }
}
