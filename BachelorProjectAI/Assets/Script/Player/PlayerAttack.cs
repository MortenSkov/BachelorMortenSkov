using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    AnimationHandler animHandler;
    InputHandler inputHandler;
    PlayerStats playerStats;
    public string lastAttack;

    private void Awake()
    {
        animHandler = GetComponentInChildren<AnimationHandler>();
        inputHandler = GetComponent<InputHandler>();
        playerStats = GetComponent<PlayerStats>();
    }

    public void HandleWeaponCombo()
    {
        if (inputHandler.comboFlag)
        {
            animHandler.anim.SetBool("canDoCombo", false);

            if (lastAttack == "LightAttack_01")
            {
                animHandler.PlayTargetAnimation("LightAttack_02", true);
                playerStats.TakeStaminaDamage(20);
            }
        }
    }

    public void HandleLightAttack(string lightAttackName)
    {
        animHandler.PlayTargetAnimation(lightAttackName, true);
        lastAttack = lightAttackName;
        playerStats.TakeStaminaDamage(20);
    }

    public void HandleHeavyAttack(string heavyAttackName)
    {
        animHandler.PlayTargetAnimation(heavyAttackName, true);
        lastAttack = heavyAttackName;
    }
    
}
