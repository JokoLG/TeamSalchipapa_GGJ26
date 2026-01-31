using UnityEngine;
using UnityEngine.AI;

public class EnemigoAI : MonoBehaviour
{
    public Transform Player;
    private NavMeshAgent agente;

    void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        agente.updateRotation = false;
        agente.updateUpAxis = false;
        agente.baseOffset = 0f;
    }

    void Update()
    {
        // Lock destination to Z = 0
        Vector3 dest = Player.position;
        dest.z = 0f;
        agente.SetDestination(dest);
    }

    void LateUpdate()
    {
        // Lock actual transform position to Z = 0
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}
