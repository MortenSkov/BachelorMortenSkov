using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : AnimatorManager
{
    PlayerManager playerManager;
    InputHandler inputHandler;
    PlayerLocomotion playerLocomotion;
    private int vertical;
    private int horizontal;
    public bool canRotate;

    DamageCollider damageCollider;

    public void Initialize()
    {
        playerManager = GetComponentInParent<PlayerManager>();
        anim = GetComponent<Animator>();
        inputHandler = GetComponentInParent<InputHandler>();
        playerLocomotion = GetComponentInParent<PlayerLocomotion>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
        damageCollider = GetComponentInChildren<DamageCollider>();
    }

    public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
    {
        // Vertical
        float v = 0;

        if(verticalMovement > 0 && verticalMovement < 0.55f)
        {
            v = 0.5f;
        }
        else if(verticalMovement > 0.55f)
        {
            v = 1;
        }
        else if(verticalMovement < 0 && verticalMovement > -0.55f)
        {
            v = -0.5f;
        }
        else if(verticalMovement < -0.55f)
        {
            v = -1;
        }
        else
        {
            v = 0;
        }

        // Horizontal
        float h = 0;

        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            h = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            h = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            h = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            h = -1;
        }
        else
        {
            h = 0;
        }

        if (isSprinting)
        {
            v = 2;
            h = horizontalMovement;
        }

        anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
        anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
    }

    public void OpenDamageCollider()
    {
        damageCollider.EnableDamageCollider();
    }

    public void CloseDamageCollider()
    {
        damageCollider.DisableDamageCollider();
    }

    public void CanRotate()
    {
        canRotate = true;
    }

    public void StopRotation()
    {
        canRotate = false;
    }

    public void EnableCombo()
    {
        anim.SetBool("canDoCombo", true);
    }

    public void DisableCombo()
    {
        anim.SetBool("canDoCombo", false);
    }

    private void OnAnimatorMove()
    {
        if (playerManager.isInteracting == false)
            return;

        float delta = Time.deltaTime;
        playerLocomotion.rigidbody.drag = 0;
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        playerLocomotion.rigidbody.velocity = velocity;
    }

}
