using UnityEngine;

public class sound : MonoBehaviour
{
    [SerializeField] private AudioClip swordSwingSound;
    public void playSlashSound()
    {
        SoundFXManager.Instance.PlaySoundFXclip(swordSwingSound, transform, 1, 1);
    }
}
