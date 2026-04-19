using UnityEngine;
using UnityEngine.SceneManagement;

public class ToMain : MonoBehaviour
{
    [SerializeField] string whereTo = "MenuPrincipal";

    void Update()
    {
        // Check for space key OR left mouse click
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(whereTo);
        }
    }
}