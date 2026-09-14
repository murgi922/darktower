using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundFXobject;
    public static SoundFXManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void PlaySoundFXclip(AudioClip audioClip, Transform spawnTransform, float volume, float pitch)
    {
        AudioSource audioSource = Instantiate(soundFXobject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.pitch = Mathf.Clamp(pitch, -3f, 3f);
        audioSource.Play();
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }
    public void PlayRandomSoundFXclip(AudioClip[] audioClip, Transform spawnTransform, float volume, float pitch)
    {
        AudioSource audioSource = Instantiate(soundFXobject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip[Random.Range(0, audioClip.Length)];
        audioSource.volume = volume;
        audioSource.pitch = Mathf.Clamp(pitch, -3f, 3f);
        audioSource.Play();
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }
}
