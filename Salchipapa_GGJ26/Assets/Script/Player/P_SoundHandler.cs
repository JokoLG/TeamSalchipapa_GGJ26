using UnityEngine;
using System.Collections.Generic;

public class P_SoundHandler : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource src;
    public AudioSource srcLoop;

    [Header("Sound Effects")]
    [Header("└─General")]
    public AudioClip Walk;
    public AudioClip Hit_1, Hit_2, Death;
    [Header("└─Odysseus")]
    public AudioClip OdyHit_1;
    public AudioClip OdyHit_2, OdyDeath, OdySlash;
    [Header("└─Witch")]
    public AudioClip WitchHit_1;
    public AudioClip WitchHit_2, WitchDeath, WitchFireBall;
    [Header("└─Shark")]
    public AudioClip SharkHit_1;
    public AudioClip SharkHit_2, SharkDeath;
    public AudioClip SharkCharge, SharkCast, SharkLoop, SharkCollide, SharkBreak;

    private Dictionary<string, AudioClip> sfx;

    void Awake()
    {
        src.playOnAwake = false;
        src.loop = false;

        srcLoop.playOnAwake = false;
        srcLoop.loop = true;

        // Build lookup table once
        sfx = new Dictionary<string, AudioClip>()
        {
            { "Walk", Walk },
            { "Hit_1", Hit_1 },
            { "Hit_2", Hit_2 },
            { "Death", Death },

            { "OdyHit_1", OdyHit_1 },
            { "OdyHit_2", OdyHit_2 },
            { "OdyDeath", OdyDeath },
            { "OdySlash", OdySlash },

            { "WitchHit_1", WitchHit_1 },
            { "WitchHit_2", WitchHit_2 },
            { "WitchDeath", WitchDeath },
            { "WitchFireBall", WitchFireBall },

            { "SharkHit_1", SharkHit_1 },
            { "SharkHit_2", SharkHit_2 },
            { "SharkDeath", SharkDeath },
            { "SharkCharge", SharkCharge },
            { "SharkCast", SharkCast },
            { "SharkLoop", SharkLoop },
            { "SharkCollide", SharkCollide },
            { "SharkBreak", SharkBreak }
        };
    }

    public void Play(string id, float volume = 1f)
    {
        Debug.Log($"SFX: {id}");
        if (!sfx.TryGetValue(id, out AudioClip clip) || clip == null)
        {
            Debug.LogWarning($"SFX not found: {id}");
            return;
        }

        src.PlayOneShot(clip, volume);
    }

    public void PlayLoop(string id, float volume = 1f)
    {
        if (!sfx.TryGetValue(id, out AudioClip clip) || clip == null)
        {
            Debug.LogWarning($"SFX not found: {id}");
            return;
        }

        if (srcLoop.isPlaying && srcLoop.clip == clip) return;

        srcLoop.clip = clip;
        srcLoop.volume = volume;
        srcLoop.Play();
    }

    public void StopLoop()
    {
        srcLoop.Stop();
        srcLoop.volume = 1f;
    }
}