using UnityEngine;

namespace Zeldinha.Items
{
    /// <summary>
    /// Componente colocado em objetos do mundo que podem ser coletados.
    /// </summary>
    public class ItemPickup : MonoBehaviour
    {
        public ItemData itemData;
        public int quantity = 1;
        public float bobSpeed = 2f;
        public float bobHeight = 0.2f;
        public float rotateSpeed = 50f;

        Vector3 startPos;

        void Start()
        {
            startPos = transform.position;
        }

        void Update()
        {
            // Animação simples de flutuação + rotação
            float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPos.x, newY, startPos.z);
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            var inventory = other.GetComponent<Player.Inventory>();
            if (inventory != null && inventory.AddItem(itemData, quantity))
            {
                // Feedback de coleta
                Debug.Log($"Coletado: {itemData.itemName} x{quantity}");
                Destroy(gameObject);
            }
        }
    }
}
