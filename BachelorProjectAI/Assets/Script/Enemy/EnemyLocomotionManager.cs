using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLocomotionManager : MonoBehaviour
{
    EnemyManager enemyManager;
    // EnemyAnimatorManager enemyAnimatorManager

    public CharacterStat currentTarget;

    public LayerMask detectionLayer;


    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
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
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

        if (enemyManager.isPerformingAction) // if enemy is attacking or rolling
        {

        }
    }

}
