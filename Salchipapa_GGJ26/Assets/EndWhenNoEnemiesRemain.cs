using UnityEngine;
using UnityEngine.SceneManagement;

public class EndWhenBossesGone : MonoBehaviour
{
    [SerializeField] private EneCom enemy1;
    [SerializeField] private EneCom enemy2;
    [SerializeField] private EneCom enemy3;
    [SerializeField] private string sceneToLoad = "TheEnd";

    private bool sceneLoaded = false;

    void Update()
    {
        if (sceneLoaded) return;

        if (enemy1 == null && enemy2 == null && enemy3 == null)
        {
            sceneLoaded = true;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}