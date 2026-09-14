using Unity.Properties;
using UnityEngine;

public class playerSounds : MonoBehaviour
{
    [Header("Footstep")]
    [SerializeField, Range(0f, 1f)] private float footVol; 
    [SerializeField] private Transform lLegIk;
    [SerializeField] private Transform rLegIk;
    [SerializeField] private float rayLength;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField, Range(-3f, 3f)] private float minPitch;
    [SerializeField, Range(-3f, 3f)] private float maxPitch;
    private bool isTouchGroundL;
    public bool IsTouchGroundL
    {
        get => isTouchGroundL;
        set
        {
            if (isTouchGroundL != value)
            {
                if (value)
                {
                    SoundFXManager.Instance.PlayRandomSoundFXclip(footstepSounds, transform, footVol, Random.Range(minPitch, maxPitch));
                    isTouchGroundL = value;
                }
                else
                {
                    isTouchGroundL = value;
                }
            }
        }
    }
    private bool isTouchGroundR;
    public bool IsTouchGroundR
    {
        get => isTouchGroundR;
        set
        {
            if (isTouchGroundR != value)
            {
                if (value)
                {
                    SoundFXManager.Instance.PlayRandomSoundFXclip(footstepSounds, transform, footVol, Random.Range(minPitch, maxPitch));
                    isTouchGroundR = value;
                }
                else
                {
                    isTouchGroundR = value;
                }
            }
        }
    }

    [Header("Movement")]
    [SerializeField, Range(0f, 1f)] private float jumpVol;
    [SerializeField] private playerMovement playerMovement;
    private bool hasJumped;
    public bool HasJumped
    {
        get => hasJumped;
        set
        {
            if (hasJumped != value)
            {
                if (!value)
                {
                    SoundFXManager.Instance.PlaySoundFXclip(jumpSound, transform, jumpVol, 1f);
                }
                hasJumped = value;
            }
        }
    }
    private void Update()
    {
        IsTouchGroundL = CheckLeg(lLegIk.position);
        IsTouchGroundR = CheckLeg(rLegIk.position);
        HasJumped = playerMovement.HasJumped;
    }
    private bool CheckLeg(Vector2 position)
    {
        if (Physics2D.Raycast(position, Vector2.down, rayLength, whatIsGround))
            return true;
        else return false;
    }
    private void OnValidate()
    {
        if (maxPitch < minPitch) maxPitch = minPitch;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(rLegIk.position, rLegIk.position + (Vector3.down * rayLength));
        Gizmos.DrawLine(lLegIk.position, lLegIk.position + (Vector3.down * rayLength));
    }
}
