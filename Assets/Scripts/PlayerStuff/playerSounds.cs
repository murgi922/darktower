using Unity.Properties;
using UnityEngine;

public class playerSounds : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField, Range(-3f, 3f)] private float minPitch;
    [SerializeField, Range(-3f, 3f)] private float maxPitch;

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
                    SoundFXManager.Instance.PlaySoundFXclip(jumpSound, player, jumpVol, 1f);
                }
                hasJumped = value;
            }
        }
    }
    private void Update()
    {
        HasJumped = playerMovement.HasJumped;
    }

    private void OnValidate()
    {
        if (maxPitch < minPitch) maxPitch = minPitch;
    }
}