using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [System.Serializable]
    public struct SceneMusic
    {
        public string sceneName;
        public MusicData music;
    }

    [Header("Configuración de Música por Escena")]
    public List<SceneMusic> playlist;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var item in playlist)
        {
            if (item.sceneName == scene.name)
            {
                BGMManager.Instance.DetenerYCambiar(item.music);
                break;
            }
        }
    }

    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;
}
