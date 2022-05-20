using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : CharacterManager
{
    EnemyLocomotionManager enemyLocomotionManager;

    [HideInInspector]
    public bool isPerformingAction; // tells us if enemy is doing something -- moving, attacking, etc. etc.

    [Header("A.I Settings")]
    public float detectionRadius = 20;
    public float maximumDetectionAngle = 50; // the higher this value will expand the enemy's FOV
    public float minimumDetectionAngle = -50; // the lower this value will expand the enemy's FOV



    private void Awake()
    {
        enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
    }

    private void Update()
    {
        HandleCurrentAction();
    }

    private void HandleCurrentAction()
    {
        if (!enemyLocomotionManager.currentTarget) // handles detections and finds a potential target
        {
            enemyLocomotionManager.HandleDetection();
        }
    }


}
