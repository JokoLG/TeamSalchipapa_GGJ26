using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [Header("Configuracion")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float defaultFadeDuration = 1.5f;

    private bool isMuted = false;
    private float currentMaxVolume = 1f; // Para recordar el volumen del ScriptableObject
    private Coroutine fadeCoroutine;
    private MusicData currentMusicData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Aseguramos que el AudioSource esté listo
            if (bgmSource == null) bgmSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Ahora recibe el ScriptableObject en lugar de solo el AudioClip
    public void ReproducirBGM(MusicData music)
    {
        if (music == null || bgmSource.clip == music.clip) return;

        currentMusicData = music;

        ConfigurarSource(music);

        bgmSource.loop = music.loop;
        bgmSource.Play();
    }

    public void CambiarBGM(MusicData newMusic, float duration = -1f)
    {
        if (newMusic == null || bgmSource.clip == newMusic.clip) return;

        float fadeTime = duration > 0 ? duration : defaultFadeDuration;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeTransition(newMusic, fadeTime));
    }

    private void ConfigurarSource(MusicData data)
    {
        bgmSource.clip = data.clip;
        bgmSource.loop = data.loop;
        currentMaxVolume = data.volume;
        // Si no está muteado, aplicamos el volumen del objeto
        bgmSource.volume = isMuted ? 0 : currentMaxVolume;
    }

    public void AlternarMute()
    {
        isMuted = !isMuted;
        bgmSource.volume = isMuted ? 0 : currentMaxVolume;
    }

    private IEnumerator FadeTransition(MusicData nextMusic, float duration)
    {
        // 1. Fade Out
        if (bgmSource.isPlaying)
        {
            float startVol = bgmSource.volume;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVol, 0, t / duration);
                yield return null;
            }
        }

        // 2. Cambio de Clip y Configuración
        ConfigurarSource(nextMusic);
        bgmSource.Play();

        // 3. Fade In
        if (!isMuted)
        {
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(0, currentMaxVolume, t / duration);
                yield return null;
            }
            bgmSource.volume = currentMaxVolume;
        }
    }

    public void DetenerYCambiar(MusicData nuevaMusica, float fadeCustom = -1f)
    {
        if (nuevaMusica == null) return;

        // Si ya está sonando esa canción, no hacemos nada
        if (currentMusicData != null && currentMusicData.clip == nuevaMusica.clip) return;

        // Usamos el Fade que ya programaste para una transición suave
        CambiarBGM(nuevaMusica, fadeCustom);
    }

    private void Update()
    {
        if (bgmSource.isPlaying && currentMusicData != null && currentMusicData.loop)
        {
            if (bgmSource.time >= currentMusicData.LoopEnd)
            {
                float exceso = bgmSource.time - currentMusicData.LoopEnd;
                bgmSource.time = currentMusicData.LoopStart + exceso;
                Debug.Log($"Bucle BGM: {currentMusicData.clip.name} desde {currentMusicData.LoopStart}s");
            }
        }
    }
}
