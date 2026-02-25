using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void GameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void CreditsScene()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Salí del juego");
    }
}
