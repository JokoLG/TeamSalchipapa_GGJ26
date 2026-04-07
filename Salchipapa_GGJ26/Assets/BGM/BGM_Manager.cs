using UnityEngine;
using System.Collections.Generic;

public class BGM_Manager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource srcLoop;

    [Header("BGM Tracks")]
    public AudioClip Walk;
    public AudioClip Hit_1, Hit_2, Death, SwitchWeapon;

    private Dictionary<string, AudioClip> sfx;

    void Awake()
    {
        srcLoop.playOnAwake = false;
        srcLoop.loop = true;

        sfx = new Dictionary<string, AudioClip>()
        {
            { "Walk", Walk },
            { "Hit_1", Hit_1 },
            { "Hit_2", Hit_2 },
            { "Death", Death },
            { "SwitchWeapon", SwitchWeapon },
        };
    }

    public void PlayBGM(string id, float volume = 1f)
    {
        if (!sfx.TryGetValue(id, out AudioClip clip) || clip == null)
        {
            Debug.LogWarning($"BGM not found: {id}");
            return;
        }

        if (srcLoop.isPlaying && srcLoop.clip == clip) return;

        srcLoop.clip = clip;
        srcLoop.volume = volume;
        srcLoop.Play();
    }

    public void StopBGM()
    {
        srcLoop.Stop();
        srcLoop.volume = 1f;
    }
}