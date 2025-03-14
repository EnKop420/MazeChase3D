using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : NetworkBehaviour
{
    public enum EnemyState { Patrolling, Chasing }
    public EnemyState currentState = EnemyState.Patrolling;
    public SphereCollider detectionCollider;

    public float patrolSpeed = 3f;
    public float chaseSpeed = 5f;

    private NavMeshAgent navMeshAgent;
    private Vector3 patrolDestination;
    public float patrolRadius = 5f;
    private Transform targetPlayer;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetRandomPatrolDestination();
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
        }
    }

    void Patrol()
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            SetRandomPatrolDestination();
        }
        navMeshAgent.speed = patrolSpeed;
    }

    void SetRandomPatrolDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1);
        patrolDestination = hit.position;

        navMeshAgent.SetDestination(patrolDestination);
    }

    void Chase()
    {
        if (targetPlayer != null)
        {
            navMeshAgent.SetDestination(targetPlayer.position);
            navMeshAgent.speed = chaseSpeed;
        }
        else
        {
            currentState = EnemyState.Patrolling;
            SetRandomPatrolDestination();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if this is the server and the collider they entered was the detectionCollider
        if(!IsServer) return;

        if (other == detectionCollider)
        {
            Debug.Log("Its Sphere");
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered detection range");
            targetPlayer = other.transform;
            currentState = EnemyState.Chasing;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;

        if(other == detectionCollider)
        {
            Debug.Log("Its Sphere");
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited detection range");
            targetPlayer = null;
            currentState = EnemyState.Patrolling;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collided with enemy");
            Application.Quit();
        }
    }
}

