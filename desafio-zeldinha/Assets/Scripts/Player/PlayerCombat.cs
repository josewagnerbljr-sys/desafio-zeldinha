using UnityEngine;
using System.Collections;

namespace Zeldinha.Player
{
    /// <summary>
    /// Sistema de combate do jogador: combo de 3 golpes + escudo/parry.
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Ataque")]
        public Transform attackPoint;
        public float attackRange = 1.2f;
        public LayerMask enemyLayers;
        public float[] comboDamage = { 12f, 16f, 25f };
        public float[] comboStaminaCost = { 8f, 10f, 15f };
        public float comboResetTime = 0.8f;

        [Header("Escudo / Parry")]
        public float parryWindow = 0.25f;
        public float parryCooldown = 0.6f;
        public float shieldStaminaCostPerSecond = 18f;
        public GameObject shieldVisual;

        [Header("Feedback")]
        public float hitStopDuration = 0.08f;

        PlayerStats stats;
        PlayerController controller;
        Animator anim;

        int comboIndex = 0;
        float lastAttackTime;
        bool isAttacking;
        bool isShielding;
        bool isParrying;
        float lastParryTime;

        public bool IsAttacking => isAttacking;
        public bool IsShielding => isShielding;

        void Awake()
        {
            stats = GetComponent<PlayerStats>();
            controller = GetComponent<PlayerController>();
            anim = GetComponentInChildren<Animator>();
        }

        void Update()
        {
            if (Systems.GameManager.Instance?.CurrentState != Systems.GameManager.GameState.Playing) return;
            if (!stats.IsAlive) return;

            HandleAttackInput();
            HandleShieldInput();

            // Reset de combo
            if (Time.time - lastAttackTime > comboResetTime)
            {
                comboIndex = 0;
            }
        }

        void HandleAttackInput()
        {
            if (Input.GetMouseButtonDown(0) && !isAttacking && !isShielding)
            {
                if (stats.TryUseStamina(comboStaminaCost[comboIndex]))
                {
                    StartCoroutine(PerformAttack());
                }
            }
        }

        IEnumerator PerformAttack()
        {
            isAttacking = true;
            lastAttackTime = Time.time;

            // Trigger animação (configure no Animator: Attack1, Attack2, Attack3)
            if (anim != null)
            {
                anim.SetTrigger("Attack" + (comboIndex + 1));
            }

            // Pequeno delay para o hitbox bater no frame certo da animação
            yield return new WaitForSeconds(0.15f);

            // Detecta inimigos
            Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
            foreach (var hit in hits)
            {
                var enemy = hit.GetComponentInParent<Enemies.EnemyBase>();
                if (enemy != null)
                {
                    enemy.TakeDamage(comboDamage[comboIndex], transform.position);
                    StartCoroutine(HitStop());
                }
            }

            comboIndex = (comboIndex + 1) % 3;
            yield return new WaitForSeconds(0.25f);
            isAttacking = false;
        }

        void HandleShieldInput()
        {
            bool wantShield = Input.GetMouseButton(1);

            if (wantShield && !isAttacking)
            {
                if (stats.TryUseStamina(shieldStaminaCostPerSecond * Time.deltaTime))
                {
                    isShielding = true;
                    if (shieldVisual != null) shieldVisual.SetActive(true);

                    // Janela de parry no início do escudo
                    if (!isParrying && Time.time - lastParryTime > parryCooldown)
                    {
                        StartCoroutine(ParryWindow());
                    }
                }
                else
                {
                    isShielding = false;
                    if (shieldVisual != null) shieldVisual.SetActive(false);
                }
            }
            else
            {
                isShielding = false;
                if (shieldVisual != null) shieldVisual.SetActive(false);
            }
        }

        IEnumerator ParryWindow()
        {
            isParrying = true;
            lastParryTime = Time.time;
            // Visual de parry (brilho, etc.)
            yield return new WaitForSeconds(parryWindow);
            isParrying = false;
        }

        public bool TryParry()
        {
            if (isParrying)
            {
                // Parry bem sucedido → i-frames + possível contra-ataque
                StartCoroutine(ParrySuccess());
                return true;
            }
            return false;
        }

        IEnumerator ParrySuccess()
        {
            stats.isInvulnerable = true;
            // Aqui você pode dar um boost de stamina ou trigger de animação de contra-ataque
            yield return new WaitForSeconds(0.4f);
            stats.isInvulnerable = false;
        }

        IEnumerator HitStop()
        {
            Time.timeScale = 0.1f;
            yield return new WaitForSecondsRealtime(hitStopDuration);
            Time.timeScale = 1f;
        }

        void OnDrawGizmosSelected()
        {
            if (attackPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(attackPoint.position, attackRange);
            }
        }
    }
}
