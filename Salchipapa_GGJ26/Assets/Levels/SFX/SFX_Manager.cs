using UnityEngine;
using System.Collections.Generic;

public class SFX_Manager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource src;
    public AudioSource srcLoop;

    [Header("Sound Effects")]
    [Header("└─Masks")]
    public AudioClip Obt_Ody;
    public AudioClip Obt_Witch, Obt_Shark;

    [Header("└─Misc")]
    public AudioClip CompletePuzzle;
    public AudioClip SpotLightON, BrickBreak, BrickFall, CrowdCheer;

    [Header("└─Enemies")]
    public AudioClip SmallHit;
    public AudioClip SmallDeath, SmallAttack, BigHit, BigDeath, BigAttack;

    [Header("└─Cyclops")]
    public AudioClip CycStun;
    public AudioClip CycAtk, CycHurt, Block, CycDeath;

    private Dictionary<string, AudioClip> sfx;

    void Awake()
    {
        src.playOnAwake = false;
        src.loop = false;

        srcLoop.playOnAwake = false;
        srcLoop.loop = true;

        sfx = new Dictionary<string, AudioClip>()
        {
            // Masks
            { "Obt_Ody", Obt_Ody },
            { "Obt_Witch", Obt_Witch },
            { "Obt_Shark", Obt_Shark },

            // Misc
            { "CompletePuzzle", CompletePuzzle },
            { "SpotLightON", SpotLightON },
            { "BrickBreak", BrickBreak },
            { "BrickFall", BrickFall },
            { "CrowdCheer", CrowdCheer },

            // Enemies
            { "SmallHit", SmallHit },
            { "SmallDeath", SmallDeath },
            { "SmallAttack", SmallAttack },
            { "BigHit", BigHit },
            { "BigDeath", BigDeath },
            { "BigAttack", BigAttack },

            // Cyclops
            { "CycStun", CycStun },
            { "CycAtk", CycAtk },
            { "CycDeath", CycDeath },
            { "CycHurt", CycHurt }
        };
    }

    public void Play(string id, float volume = 0.5f)
    {
        if (!sfx.TryGetValue(id, out AudioClip clip) || clip == null)
        {
            Debug.LogWarning($"SFX not found: {id}");
            return;
        }

        src.PlayOneShot(clip, volume);
    }

    public void PlayLoop(string id, float volume = 0.5f)
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
        src.Stop();
        srcLoop.volume = 1f;
    }
}