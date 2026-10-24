using UnityEngine;

namespace Zeldinha.Systems
{
    /// <summary>
    /// Base para objetos interativos (baús, NPCs, portas).
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        public string prompt = "Pressione E para interagir";
        public float interactRange = 2.5f;

        protected bool playerInRange;
        Transform player;

        protected virtual void Start()
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        protected virtual void Update()
        {
            if (player == null) return;

            float dist = Vector3.Distance(transform.position, player.position);
            playerInRange = dist <= interactRange;

            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }

        public virtual void Interact()
        {
            Debug.Log($"Interagiu com {gameObject.name}");
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
    }
}
