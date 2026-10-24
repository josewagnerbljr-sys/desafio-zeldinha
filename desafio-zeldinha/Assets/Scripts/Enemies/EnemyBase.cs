using UnityEngine;
using System.Collections;

namespace Zeldinha.Enemies
{
    /// <summary>
    /// Classe base para todos os inimigos.
    /// </summary>
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("Stats")]
        public float maxHealth = 50f;
        public float currentHealth;
        public float damage = 10f;
        public float moveSpeed = 3f;
        public float detectionRange = 8f;
        public float attackRange = 1.5f;
        public float attackCooldown = 1.5f;

        [Header("Drops")]
        public Items.ItemData[] possibleDrops;
        public float dropChance = 0.4f;
        public GameObject lightFragmentPrefab; // chance especial de dropar fragmento

        [Header("Feedback")]
        public float knockbackForce = 5f;
        public float hurtDuration = 0.4f;

        protected Transform player;
        protected Rigidbody rb;
        protected Animator anim;
        protected float lastAttackTime;
        protected bool isDead;
        protected bool isHurt;

        public enum EnemyState { Idle, Follow, Attack, Hurt, Dead }
        public EnemyState CurrentState { get; protected set; } = EnemyState.Idle;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
            rb = GetComponent<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
        }

        protected virtual void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        protected virtual void Update()
        {
            if (isDead || player == null) return;

            float dist = Vector3.Distance(transform.position, player.position);

            switch (CurrentState)
            {
                case EnemyState.Idle:
                    if (dist <= detectionRange) ChangeState(EnemyState.Follow);
                    break;
                case EnemyState.Follow:
                    if (dist <= attackRange) ChangeState(EnemyState.Attack);
                    else if (dist > detectionRange * 1.3f) ChangeState(EnemyState.Idle);
                    else FollowPlayer();
                    break;
                case EnemyState.Attack:
                    if (dist > attackRange * 1.2f) ChangeState(EnemyState.Follow);
                    else TryAttack();
                    break;
            }
        }

        protected virtual void FollowPlayer()
        {
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            rb.velocity = new Vector3(dir.x * moveSpeed, rb.velocity.y, dir.z * moveSpeed);

            if (dir.magnitude > 0.1f)
            {
                Quaternion look = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, 8f * Time.deltaTime);
            }
        }

        protected virtual void TryAttack()
        {
            if (Time.time - lastAttackTime < attackCooldown) return;
            lastAttackTime = Time.time;

            // Animação de ataque
            if (anim != null) anim.SetTrigger("Attack");

            // Aplica dano se ainda estiver perto
            if (Vector3.Distance(transform.position, player.position) <= attackRange * 1.1f)
            {
                var stats = player.GetComponent<Player.PlayerStats>();
                var combat = player.GetComponent<Player.PlayerCombat>();

                // Tenta parry
                if (combat != null && combat.TryParry())
                {
                    // Inimigo sofre contra-ataque leve
                    TakeDamage(damage * 0.5f, player.position);
                    return;
                }

                stats?.TakeDamage(damage);
            }
        }

        public virtual void TakeDamage(float amount, Vector3 attackerPos)
        {
            if (isDead || isHurt) return;

            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                Die();
                return;
            }

            StartCoroutine(HurtRoutine(attackerPos));
        }

        protected virtual IEnumerator HurtRoutine(Vector3 attackerPos)
        {
            isHurt = true;
            ChangeState(EnemyState.Hurt);

            // Knockback
            Vector3 knockDir = (transform.position - attackerPos).normalized;
            knockDir.y = 0.3f;
            rb.AddForce(knockDir * knockbackForce, ForceMode.Impulse);

            if (anim != null) anim.SetTrigger("Hurt");

            yield return new WaitForSeconds(hurtDuration);
            isHurt = false;
            ChangeState(EnemyState.Follow);
        }

        protected virtual void Die()
        {
            isDead = true;
            ChangeState(EnemyState.Dead);

            if (anim != null) anim.SetTrigger("Die");

            // Desativa colisão e movimento
            var col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;

            DropItems();
            Destroy(gameObject, 2.5f);
        }

        protected virtual void DropItems()
        {
            if (possibleDrops != null && possibleDrops.Length > 0 && Random.value <= dropChance)
            {
                var drop = possibleDrops[Random.Range(0, possibleDrops.Length)];
                if (drop != null && drop.worldPrefab != null)
                {
                    var go = Instantiate(drop.worldPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                    var pickup = go.GetComponent<Items.ItemPickup>();
                    if (pickup != null) pickup.itemData = drop;
                }
            }

            // Chance pequena de fragmento
            if (lightFragmentPrefab != null && Random.value < 0.08f)
            {
                Instantiate(lightFragmentPrefab, transform.position + Vector3.up, Quaternion.identity);
            }
        }

        protected void ChangeState(EnemyState newState)
        {
            CurrentState = newState;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
