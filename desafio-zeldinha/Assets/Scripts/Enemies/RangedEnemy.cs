using UnityEngine;

namespace Zeldinha.Enemies
{
    /// <summary>
    /// Inimigo à distância que atira projéteis.
    /// </summary>
    public class RangedEnemy : EnemyBase
    {
        [Header("Ranged")]
        public GameObject projectilePrefab;
        public Transform firePoint;
        public float preferredDistance = 6f;
        public float projectileSpeed = 12f;

        protected override void Update()
        {
            if (isDead || player == null) return;

            float dist = Vector3.Distance(transform.position, player.position);

            switch (CurrentState)
            {
                case EnemyState.Idle:
                    if (dist <= detectionRange) ChangeState(EnemyState.Follow);
                    break;

                case EnemyState.Follow:
                    if (dist <= preferredDistance && dist > attackRange)
                    {
                        // Mantém distância e atira
                        KeepDistance();
                        TryAttack();
                    }
                    else if (dist > detectionRange * 1.3f)
                    {
                        ChangeState(EnemyState.Idle);
                    }
                    else
                    {
                        FollowPlayer();
                    }
                    break;

                case EnemyState.Attack:
                    if (dist > preferredDistance * 1.2f) ChangeState(EnemyState.Follow);
                    else TryAttack();
                    break;
            }
        }

        void KeepDistance()
        {
            Vector3 dir = (transform.position - player.position).normalized;
            dir.y = 0;
            rb.velocity = new Vector3(dir.x * moveSpeed * 0.6f, rb.velocity.y, dir.z * moveSpeed * 0.6f);

            // Olha para o jogador
            Vector3 lookDir = (player.position - transform.position).normalized;
            lookDir.y = 0;
            if (lookDir.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 8f * Time.deltaTime);
            }
        }

        protected override void TryAttack()
        {
            if (Time.time - lastAttackTime < attackCooldown) return;
            if (projectilePrefab == null || firePoint == null) return;

            lastAttackTime = Time.time;
            if (anim != null) anim.SetTrigger("Attack");

            // Instancia projétil
            var proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            var rbProj = proj.GetComponent<Rigidbody>();
            if (rbProj != null)
            {
                Vector3 dir = (player.position + Vector3.up * 0.8f - firePoint.position).normalized;
                rbProj.velocity = dir * projectileSpeed;
            }

            // O projétil deve ter um script que aplica dano no Player
            var dmg = proj.GetComponent<Projectile>();
            if (dmg != null) dmg.damage = damage;
        }
    }

    /// <summary>
    /// Script simples para o projétil.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        public float damage = 10f;
        public float lifetime = 4f;

        void Start()
        {
            Destroy(gameObject, lifetime);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var stats = other.GetComponent<Player.PlayerStats>();
                stats?.TakeDamage(damage);
                Destroy(gameObject);
            }
            else if (!other.isTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}
