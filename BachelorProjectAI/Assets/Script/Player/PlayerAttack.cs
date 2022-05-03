using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    AnimationHandler animHandler;
    InputHandler inputHandler;
    public string lastAttack;

    private void Awake()
    {
        animHandler = GetComponentInChildren<AnimationHandler>();
        inputHandler = GetComponent<InputHandler>();
    }

    public void HandleWeaponCombo()
    {
        if (inputHandler.comboFlag)
        {
            animHandler.anim.SetBool("canDoCombo", false);

            if (lastAttack == "LightAttack_01")
            {
                animHandler.PlayTargetAnimation("LightAttack_02", true);
            }
        }
    }

    public void HandleLightAttack(string lightAttackName)
    {
        animHandler.PlayTargetAnimation(lightAttackName, true);
        lastAttack = lightAttackName;
    }

    public void HandleHeavyAttack(string heavyAttackName)
    {
        animHandler.PlayTargetAnimation(heavyAttackName, true);
        lastAttack = heavyAttackName;
    }
    
}
