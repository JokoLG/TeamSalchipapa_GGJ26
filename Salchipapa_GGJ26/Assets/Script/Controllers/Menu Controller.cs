using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Configuración de Audio")]
    [SerializeField] private MusicData menuMusic; // Arrastra aquí tu ScriptableObject "Menu BGM"

    private void Start()
    {
        // Al iniciar el menú, le pedimos al Manager que reproduzca la música
        if (BGMManager.Instance != null && menuMusic != null)
        {
            BGMManager.Instance.ReproducirBGM(menuMusic);
        }
    }

    // Función para el botón "Jugar"
    public void OnClickPlay()
    {
        SceneManager.LoadScene("Scena_Figth");
    }

    public void OnClickExit()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}
