using UnityEngine;

namespace Zeldinha.Enemies
{
    /// <summary>
    /// Inimigo corpo-a-corpo padrão (melhorado do MeleeCreature original).
    /// </summary>
    public class MeleeEnemy : EnemyBase
    {
        [Header("Melee específico")]
        public float chargeSpeedMultiplier = 1.4f;
        public float chargeDistance = 4f;

        protected override void FollowPlayer()
        {
            float dist = Vector3.Distance(transform.position, player.position);

            // Se estiver na distância de carga, acelera
            float speed = moveSpeed;
            if (dist < chargeDistance && dist > attackRange)
            {
                speed *= chargeSpeedMultiplier;
            }

            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            rb.velocity = new Vector3(dir.x * speed, rb.velocity.y, dir.z * speed);

            if (dir.magnitude > 0.1f)
            {
                Quaternion look = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, 10f * Time.deltaTime);
            }
        }
    }
}
