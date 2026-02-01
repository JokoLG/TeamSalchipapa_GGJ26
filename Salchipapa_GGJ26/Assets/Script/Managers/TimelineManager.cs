using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineManager : MonoBehaviour
{
    private PlayableDirector director;

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    void Start()
    {

        if (director == null)
        {
            Debug.LogError("PlayableDirector component not found on TimelineManager GameObject.");
        }
    }

    public void EmpezarSecuencia()
    {
        if (director != null && director.playableAsset != null)
            director.Play();
        else
            Debug.LogError("PlayableDirector or PlayableAsset is null. Cannot play timeline.");
    }

    public void PausarTimeline()
    {
        if (director != null)
            director.Pause();
    }

    public void ResumirTimeline()
    {
        if (director != null)
            director.Play();
    }
}
