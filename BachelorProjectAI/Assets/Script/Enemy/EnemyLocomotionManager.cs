using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLocomotionManager : MonoBehaviour
{
    EnemyManager enemyManager;
    EnemyAnimatorManager enemyAnimatorManager;
    NavMeshAgent navMeshAgent;
    [HideInInspector] public Rigidbody rb;

    public float movementSpeed = 5;

    public CharacterStat currentTarget;
    public LayerMask detectionLayer;

    public float distanceFromTarget; // how far away we are from our current target 
    public float stoppingDistance = 1f; // minimum distance away we need to before, before we stop running around 'randomly'

    public float rotationSpeed = 15;


    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
        enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        navMeshAgent.enabled = false;
        rb.isKinematic = false;
    }

    public void HandleDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].transform.TryGetComponent<CharacterStat>(out var characterStats))
            {
                // CHECK FOR TEAM ID

                Vector3 targetDirection = characterStats.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
                {
                    currentTarget = characterStats;
                }
            }
        }
    }

    public void HandleMoveToTarget()
    {
        Vector3 targetDirection = currentTarget.transform.position - transform.position;
        distanceFromTarget = Vector3.Distance(currentTarget.transform.position, transform.position);
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

        if (enemyManager.isPerformingAction) // if enemy is performing an action, stop its movement
        {
            enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            navMeshAgent.enabled = false;
        }
        else
        {
            if(distanceFromTarget > stoppingDistance)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            }
            else if (distanceFromTarget <= stoppingDistance)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            }
        }

        HandleRotationTowardsTarget();

        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;
    }

    private void HandleRotationTowardsTarget()
    {
        if (enemyManager.isPerformingAction) // Rotate manually
        {
            Vector3 direction = currentTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if(direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed / Time.deltaTime);
        }
        else // Rotate with pathfinding (navmesh)
        {
            Vector3 relativeDIrection = transform.InverseTransformDirection(navMeshAgent.desiredVelocity);
            Vector3 targetVelocity = rb.velocity;

            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(currentTarget.transform.position);
            rb.velocity = targetVelocity;
            transform.rotation = Quaternion.Slerp(transform.rotation, navMeshAgent.transform.rotation, rotationSpeed / Time.deltaTime);
        }
    }

}
