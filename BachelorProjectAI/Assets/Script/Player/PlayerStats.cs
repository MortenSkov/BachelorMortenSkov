using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int healthLevel = 10;
    public int maxHealth;
    public int currentHealth;

    public HealthBar healthBar;

    AnimationHandler animHandler;

    private void Awake()
    {
        animHandler = GetComponentInChildren<AnimationHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private int SetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetCurrentHealth(currentHealth);

        animHandler.PlayTargetAnimation("Damage_01", true);

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            animHandler.PlayTargetAnimation("Dead_01", true);
            // HANDLE PLAYER DEATH
        }
    }


}
