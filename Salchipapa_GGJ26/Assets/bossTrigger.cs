using UnityEngine;

public class bossTrigger : MonoBehaviour
{
    public BGM_Manager bgm;

    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private int spawnCount = 9;

    private bool activated = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Player1"))
        {
            activated = true;

            if (bgm != null)
                bgm.PlayBGM("boss");

            if (prefabToSpawn != null)
            {
                for (int i = 0; i < spawnCount; i++)
                {
                    Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
                }
            }
        }
    }
}