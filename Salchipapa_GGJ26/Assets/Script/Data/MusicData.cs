using UnityEngine;

[CreateAssetMenu(fileName = "NewMusicData", menuName = "Audio/MusicData")]
public class MusicData : ScriptableObject
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public float LoopStart = 0.0f, LoopEnd = 0.0f;

    public bool loop = true;

    // Podemos agregar un nombre clave para buscarlo si fuera necesario
    public string trackName;
}
