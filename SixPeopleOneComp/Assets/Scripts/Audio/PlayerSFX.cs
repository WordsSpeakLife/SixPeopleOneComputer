using Unity.Mathematics;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [SerializeField] private SoundLibrary sfxLibrary;
    [SerializeField] private AudioSource sfxSource;
    public static PlayerSFX instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }

        else
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            float volume;
            sfxSource.outputAudioMixerGroup.audioMixer.GetFloat("Player SFX", out volume);
            volume = Mathf.Clamp(Mathf.Pow(10, (volume * 0.05f)) * sfxSource.volume, 0.0f, 1.0f);
            AudioSource.PlayClipAtPoint(clip, pos, volume);
        }
    }

    public void PlaySound3D(string soundName, Vector3 pos)
    {
        PlaySound3D(sfxLibrary.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        sfxSource.PlayOneShot(sfxLibrary.GetClipFromName(soundName));
    }
}