using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorManager : AnimatorManager
{
    EnemyLocomotionManager enemyLocomotionManager;
    DamageCollider damageCollider;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyLocomotionManager = GetComponentInParent<EnemyLocomotionManager>();
        damageCollider = GetComponentInChildren<DamageCollider>();
    }

    private void OnAnimatorMove()
    {
        float delta = Time.deltaTime;
        enemyLocomotionManager.rb.drag = 0;
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        enemyLocomotionManager.rb.velocity = velocity;
    }

    public void OpenDamageCollider()
    {
        damageCollider.EnableDamageCollider();
    }

    public void CloseDamageCollider()
    {
        damageCollider.DisableDamageCollider();
    }

    public void EnableCombo()
    {
        anim.SetBool("canDoCombo", true);
    }

    public void DisableCombo()
    {
        anim.SetBool("canDoCombo", false);
    }

}
