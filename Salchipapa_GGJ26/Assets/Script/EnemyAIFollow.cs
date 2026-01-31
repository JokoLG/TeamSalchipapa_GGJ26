using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemigoAI : MonoBehaviour
{
    public Transform Player;
    private NavMeshAgent agente;

    private void Awake(){
        agente = GetComponent<NavMeshAgent>();
    }
    private void Start(){
        agente.updateRotation = false;
        agente.updateUpAxis = false;
    }
    private void Update(){
        agente.SetDestination(Player.position);
    }
}
