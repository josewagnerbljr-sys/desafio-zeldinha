using UnityEngine;
using System.Collections;

namespace Zeldinha.Boss
{
    /// <summary>
    /// Boss final - Lich Lord Malakar com 3 fases.
    /// </summary>
    public class LichBossController : Enemies.EnemyBase
    {
        [Header("Boss Settings")]
        public float phase2Threshold = 0.66f;
        public float phase3Threshold = 0.33f;
        public GameObject[] summonPrefabs;
        public Transform[] summonPoints;
        public GameObject ritualVFX;
        public GameObject superAttackVFX;

        int currentPhase = 1;
        bool isPerformingSpecial;

        protected override void Start()
        {
            base.Start();
            maxHealth = 300f;
            currentHealth = maxHealth;
            damage = 18f;
            moveSpeed = 2.8f;
            detectionRange = 20f;
            attackRange = 2.5f;
            attackCooldown = 1.8f;

            Systems.GameManager.Instance?.SetState(Systems.GameManager.GameState.BossFight);
        }

        protected override void Update()
        {
            if (isDead || isPerformingSpecial || player == null) return;

            // Atualiza fase
            float hpPercent = currentHealth / maxHealth;
            if (hpPercent <= phase3Threshold && currentPhase < 3)
            {
                currentPhase = 3;
                StartCoroutine(EnterPhase3());
            }
            else if (hpPercent <= phase2Threshold && currentPhase < 2)
            {
                currentPhase = 2;
                StartCoroutine(EnterPhase2());
            }

            base.Update();
        }

        protected override void TryAttack()
        {
            if (isPerformingSpecial) return;
            if (Time.time - lastAttackTime < attackCooldown) return;

            lastAttackTime = Time.time;

            // Escolhe ataque baseado na fase
            int roll = Random.Range(0, 100);
            if (currentPhase >= 2 && roll < 30)
            {
                StartCoroutine(RitualAttack());
            }
            else if (currentPhase >= 3 && roll < 50)
            {
                StartCoroutine(SuperAttack());
            }
            else
            {
                // Ataque normal
                if (anim != null) anim.SetTrigger("Attack");
                StartCoroutine(NormalAttackHit());
            }
        }

        IEnumerator NormalAttackHit()
        {
            yield return new WaitForSeconds(0.4f);
            if (Vector3.Distance(transform.position, player.position) <= attackRange * 1.3f)
            {
                player.GetComponent<Player.PlayerStats>()?.TakeDamage(damage);
            }
        }

        IEnumerator RitualAttack()
        {
            isPerformingSpecial = true;
            if (anim != null) anim.SetTrigger("Ritual");
            if (ritualVFX != null) ritualVFX.SetActive(true);

            yield return new WaitForSeconds(1.2f);

            // Invoca minions
            if (summonPrefabs != null && summonPoints != null)
            {
                for (int i = 0; i < Mathf.Min(2, summonPoints.Length); i++)
                {
                    var prefab = summonPrefabs[Random.Range(0, summonPrefabs.Length)];
                    if (prefab != null)
                        Instantiate(prefab, summonPoints[i].position, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(0.8f);
            if (ritualVFX != null) ritualVFX.SetActive(false);
            isPerformingSpecial = false;
        }

        IEnumerator SuperAttack()
        {
            isPerformingSpecial = true;
            if (anim != null) anim.SetTrigger("Super");
            if (superAttackVFX != null) superAttackVFX.SetActive(true);

            yield return new WaitForSeconds(0.9f);

            // Área de dano
            Collider[] hits = Physics.OverlapSphere(transform.position, 6f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    hit.GetComponent<Player.PlayerStats>()?.TakeDamage(damage * 1.8f);
                }
            }

            yield return new WaitForSeconds(0.6f);
            if (superAttackVFX != null) superAttackVFX.SetActive(false);
            isPerformingSpecial = false;
        }

        IEnumerator EnterPhase2()
        {
            isPerformingSpecial = true;
            // Efeito visual de fase
            yield return new WaitForSeconds(1f);
            attackCooldown *= 0.85f;
            moveSpeed *= 1.15f;
            isPerformingSpecial = false;
            Debug.Log("Lich entra na Fase 2!");
        }

        IEnumerator EnterPhase3()
        {
            isPerformingSpecial = true;
            yield return new WaitForSeconds(1.2f);
            attackCooldown *= 0.75f;
            damage *= 1.25f;
            isPerformingSpecial = false;
            Debug.Log("Lich entra na Fase 3 - Forma Enfurecida!");
        }

        protected override void Die()
        {
            base.Die();
            Systems.GameManager.Instance?.BossDefeated();
            Debug.Log("Lich Lord Malakar foi derrotado! A ilha está salva.");
        }
    }
}
