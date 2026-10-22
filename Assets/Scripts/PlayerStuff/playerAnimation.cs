using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class playerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float attackCooldown;
    [SerializeField] private AudioClip attackClip;
    private string currentAnimation = "";
    private Coroutine coolDownCoroutine;
    private InputAction attackAction;
    private bool attacked = false;
    public bool Attacked
    {
        get => attacked;
        set
        {
            if (value != attacked)
            {
                if (value)
                {
                    coolDownCoroutine = StartCoroutine(CoolDown(attackCooldown));
                    attacked = true;
                }
                else
                {
                    attacked = value;
                }
            }
        }
    }
    private void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
    }
    
    private void Update()
    {
        if (attackAction.WasPressedThisFrame() || attackAction.IsPressed())
        {
            Attacked = true;
        }
        CheckAnimation();
    }
    private void CheckAnimation()
    { 
        if (Attacked)
        {
            ChangeAnimation("Attack", 0f);
        }
        else
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
    IEnumerator CoolDown(float threshold)
    {
        float elapsedTime = 0f;
        SoundFXManager.Instance.PlaySoundFXclip(attackClip, transform);
        while (elapsedTime < threshold)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Attacked = false;
        coolDownCoroutine = null;
    }
    public void ComboPoint()
    {
        Debug.Log("ComboPoint");
    }
}
