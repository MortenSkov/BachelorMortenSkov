using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : CharacterManager
{
    EnemyLocomotionManager enemyLocomotionManager;
    EnemyAnimatorManager enemyAnimatorManager;

    [HideInInspector]
    public bool isPerformingAction; // tells us if enemy is doing something -- moving, attacking, etc. etc.

    public EnemyAttackAction[] enemyAttacks;
    public EnemyAttackAction currentAttack;

    [Header("A.I Settings")]
    public float detectionRadius = 20;
    public float maximumDetectionAngle = 50; // the higher this value will expand the enemy's FOV
    public float minimumDetectionAngle = -50; // the lower this value will expand the enemy's FOV

    public float currentRecoveryTime = 0; // Time it takes before enemy can attack or perform any other kind of functionality


    private void Awake()
    {
        enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
        enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
    }

    private void Update()
    {
        HandleRecoveryTimer();
    }

    private void FixedUpdate()
    {
        HandleCurrentAction();
    }

    private void HandleCurrentAction()
    {
        if(enemyLocomotionManager.currentTarget != null)
        {
            enemyLocomotionManager.distanceFromTarget = Vector3.Distance(enemyLocomotionManager.currentTarget.transform.position, transform.position);
        }

        if (!enemyLocomotionManager.currentTarget) // handles detections and finds a potential target
        {
            enemyLocomotionManager.HandleDetection();
        }
        else if (enemyLocomotionManager.distanceFromTarget > enemyLocomotionManager.stoppingDistance)// if enemy DO have a target
        {
            enemyLocomotionManager.HandleMoveToTarget();
        }
        else if (enemyLocomotionManager.distanceFromTarget <= enemyLocomotionManager.stoppingDistance)
        {
            AttackTarget();
        }
    }

    private void HandleRecoveryTimer()
    {
        if(currentRecoveryTime > 0)
        {
            currentRecoveryTime -= Time.deltaTime;
        }

        if (isPerformingAction)
        {
            if(currentRecoveryTime <= 0)
            {
                isPerformingAction = false;
            }
        }
    }

    #region Attacks

    private void AttackTarget()
    {
        if (isPerformingAction)
            return;

        if (!currentAttack)
        {
            GetNewAttack();
        }
        else
        {
            isPerformingAction = true;
            currentRecoveryTime = currentAttack.recoveryTime;
            enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            currentAttack = null;
        }
    }

    private void GetNewAttack()
    {
        Vector3 targetDirection = enemyLocomotionManager.currentTarget.transform.position - transform.position;
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
        enemyLocomotionManager.distanceFromTarget = Vector3.Distance(enemyLocomotionManager.currentTarget.transform.position, transform.position);

        int maxScore = 0;

        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyAttackAction enemyAttackAction = enemyAttacks[i];

            if(enemyLocomotionManager.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack && enemyLocomotionManager.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
            {
                if(viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                {
                    maxScore += enemyAttackAction.attackScore;
                }
            }
        }

        int randomValue = Random.Range(0, maxScore);
        int tmpScore = 0;

        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyAttackAction enemyAttackAction = enemyAttacks[i];

            if (enemyLocomotionManager.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack && enemyLocomotionManager.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
            {
                if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                {
                    if (currentAttack != null)
                        return;

                    tmpScore += enemyAttackAction.attackScore;

                    if(tmpScore > randomValue)
                    {
                        currentAttack = enemyAttackAction;
                    }
                }
            }
        }
    }

    #endregion
}
