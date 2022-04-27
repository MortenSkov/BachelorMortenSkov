using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    AnimationHandler animHandler;

    private void Awake()
    {
        animHandler = GetComponentInChildren<AnimationHandler>();
    }

    public void HandleLightAttack(string lightAttackName)
    {
        animHandler.PlayTargetAnimation(lightAttackName, true);
    }

    public void HandleHeavyAttack(string heavyAttackName)
    {
        animHandler.PlayTargetAnimation(heavyAttackName, true);
    }
    
}
