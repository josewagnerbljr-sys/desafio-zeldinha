using UnityEngine;
using System;

namespace Zeldinha.Player
{
    /// <summary>
    /// Gerencia vida, stamina e status do jogador.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [Header("Vida")]
        public float maxHealth = 100f;
        public float currentHealth;

        [Header("Stamina")]
        public float maxStamina = 100f;
        public float currentStamina;
        public float staminaRegenRate = 15f;
        public float staminaRegenDelay = 1.2f;

        [Header("Status")]
        public int attackPower = 10;
        public float defense = 0f;
        public bool isInvulnerable = false;

        float lastStaminaUseTime;

        public event Action<float, float> OnHealthChanged;
        public event Action<float, float> OnStaminaChanged;
        public event Action OnDeath;

        void Awake()
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
        }

        void Update()
        {
            RegenerateStamina();
        }

        void RegenerateStamina()
        {
            if (currentStamina >= maxStamina) return;
            if (Time.time - lastStaminaUseTime < staminaRegenDelay) return;

            currentStamina = Mathf.Min(currentStamina + staminaRegenRate * Time.deltaTime, maxStamina);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }

        public bool TryUseStamina(float amount)
        {
            if (currentStamina < amount) return false;

            currentStamina -= amount;
            lastStaminaUseTime = Time.time;
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
            return true;
        }

        public void TakeDamage(float amount)
        {
            if (isInvulnerable || currentHealth <= 0) return;

            float finalDamage = Mathf.Max(0, amount - defense);
            currentHealth = Mathf.Max(0, currentHealth - finalDamage);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                OnDeath?.Invoke();
                Systems.GameManager.Instance?.PlayerDied();
            }
        }

        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void RestoreStamina(float amount)
        {
            currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }

        public void IncreaseMaxHealth(float amount)
        {
            maxHealth += amount;
            currentHealth += amount;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void IncreaseMaxStamina(float amount)
        {
            maxStamina += amount;
            currentStamina += amount;
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }

        public bool IsAlive => currentHealth > 0;
        public float HealthPercent => currentHealth / maxHealth;
        public float StaminaPercent => currentStamina / maxStamina;
    }
}
