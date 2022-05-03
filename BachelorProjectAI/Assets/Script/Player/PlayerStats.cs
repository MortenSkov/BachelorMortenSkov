using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int healthLevel = 10;
    public int maxHealth;
    public int currentHealth;

    public int staminaLevel = 10;
    public int maxStamina;
    public int currentStamina;

    public HealthBar healthBar;
    public StaminaBar staminaBar;

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

        maxStamina = SetMaxStaminaFromStaminaLevel();
        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
    }

    private int SetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    private int SetMaxStaminaFromStaminaLevel()
    {
        maxStamina = staminaLevel * 10;
        return maxStamina;
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

    public void TakeStaminaDamage(int damage)
    {
        currentStamina -= damage;

        staminaBar.SetCurrentStamina(currentStamina);
    }


}
