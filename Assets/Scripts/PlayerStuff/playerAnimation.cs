using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class playerAnimation : MonoBehaviour
{
    #region Miscellaneous
    [SerializeField] private playerMovement playerMovement;
    private InputAction moveAction;
    private InputAction jumpAction;
    [SerializeField] private Animator animator;
    private string currentAnimation = "";
    private bool isMoving = false;
    public bool IsMoving
    {
        get => isMoving;
        set
        {
            if (value != isMoving)
            {
                isMoving = value;
            }
        }
    }
    private void CheckMovement()
    {
        if (Mathf.Abs(moveAction.ReadValue<Vector2>().x) > 0.0f) IsMoving = true;
        else IsMoving = false;  

    }
    #endregion
    #region Attack
    [SerializeField] private AudioClip attackClip;
    private InputAction attackAction;
    private bool attack1 = false;
    public bool Attack1
    {
        get => attack1;
        set
        {
            if (value != attack1)
            {
                attack1 = value;
            }
        }
    }
    private bool comboWindowOpen = false;
    private bool buffer = false;
    public bool Buffer
    {
        get => buffer;
        set
        {
            if (buffer != value)
            { 
                buffer = value;
            }
        }
    }
    private bool attack2 = false;
    public bool Attack2
    {
        get => attack2;
        set
        {
            if (value != attack2)
            {
                attack2 = value;
            }
        }
    }

    public void ActivateDeadzone()
    {
        jumpAction.Disable();
        moveAction.Disable();
        comboWindowOpen = false;
    }
    public void ComboWindowOpened1()
    {
        jumpAction.Enable();
        moveAction.Enable();
        comboWindowOpen = true;
        Attack2 = Buffer;
        if (Attack2) Attack1 = false;
    }
    public void AttackEnd1()
    {
        Attack1 = false;
        Buffer = false;

        jumpAction.Enable();
        moveAction.Enable();
    }
    public void ComboWindowOpened2()
    {
        jumpAction.Enable();
        moveAction.Enable();
    }
    public void AttackEnd2()
    {
        Attack2 = false;

        jumpAction.Enable();
        moveAction.Enable();
    }
    #region Attack Cooldown
    [SerializeField] private float attackCooldown;
    private bool attack = false;
    public bool Attack
    {
        get => attack;
        set
        {
            if (value != attack)
            {
                if (value)
                {
                    attack = true;
                    StartCoroutine(AtkCooldwn(attackCooldown));
                }
                else
                {
                    attack = false;
                }
            }
        }
    }
    IEnumerator AtkCooldwn(float limit)
    {
        float elapsedTime = 0f;
        attackAction.Disable();
        while (elapsedTime < limit)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        attackAction.Enable();
    }
    #endregion
    #endregion
    #region Jump
    private bool jumped = false;
    public bool Jumped
    {
        get => jumped;
        set
        {
            if (value != jumped)
            {
                if (value) jumped = true;
                else jumped = false;
            }
        }
    }
    public void JumpEnd()
    {
        Jumped = false;
    }
    #endregion
    #region Built-in func
    private void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }
    
    private void Update()
    {
        if (attackAction.WasPressedThisFrame())
        {
            Attack = true;
        }
        else Attack = false;
        if (Attack2)
        {
            Attack1 = false;
            Buffer = false;
        }
        if (Attack1)
        {
            if (attackAction.WasPressedThisFrame())
            {
                if (!comboWindowOpen)
                {
                    Buffer = true;
                }
                else
                {
                    Attack2 = true;
                }
            }
        }
        CheckMovement();
        if (IsMoving)
        {
            Attack1 = false;
            Attack2 = false;
        }
        if (!Attack1 && !Attack2)
        {
            if (attackAction.WasPressedThisFrame())
            {
                Attack1 = true;
            }
        }
        
        CheckAnimation();
        if (playerMovement.HasJumped) Jumped = true;
    }
    #endregion
    #region Check/Change Animation
    private void CheckAnimation()
    { 
        if (Attack1 && playerMovement.IsGrounded)
        {
            ChangeAnimation("Attack1", 0f);
        }
        else if (Attack2 && playerMovement.IsGrounded)
        {
            ChangeAnimation("Attack2", 0f);
        }
        else if (Jumped)
        {
            ChangeAnimation("Jump", 0f);
        }
        else if (!playerMovement.IsGrounded)
        {
            ChangeAnimation("Falling", 0f);
        }
        else if (IsMoving && playerMovement.IsGrounded)
        {
            ChangeAnimation("Walk", 0f);
        }
        else if (playerMovement.IsGrounded)
        {
            ChangeAnimation("Idle", 0f);
        }
    }
    private void ChangeAnimation(string animation, float crossFade = 0.2f)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            animator.CrossFade(animation, crossFade);
        }
    }
    #endregion
}
