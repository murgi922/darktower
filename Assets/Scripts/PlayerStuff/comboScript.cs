
using UnityEngine;
using UnityEngine.InputSystem;

public class comboScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool comboPerStart = false;
    private InputAction attackAction;
    private bool attacking = false;
    private bool didCombo = false;
    private void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
    }
    private void Update()
    {
        if (!didCombo)
        {
            didCombo = attackAction.IsInProgress() || attackAction.WasPressedThisFrame();
        }
        if (attackAction.WasPressedThisFrame() && !attacking)
        {
            attacking = true;
            Attack();
        }
        if (comboPerStart)
        {
            if (didCombo)
            {
                PerformCombo();
            }
        }
    }
    public void NullPeriod()
    {
        attackAction.Disable();
        didCombo = false;
    }
    public void TakeComboInput()
    {
        attackAction.Enable();
    }
    public void ComboPeriodStart()
    {
        comboPerStart = true;
 
    }
    public void PerformCombo()
    {
        attackAction.Disable();
        animator.CrossFadeInFixedTime("Attack 1", 0.05f);
        didCombo = false;
    }
    public void ComboFinished()
    {
        comboPerStart = false;
        attackAction.Enable();
        attacking = false;
    }
    public void Attack()
    {
        animator.CrossFadeInFixedTime("Attack", 0.05f);
    }
}
