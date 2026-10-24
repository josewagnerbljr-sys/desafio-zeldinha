using UnityEngine;

namespace Zeldinha.Items
{
    /// <summary>
    /// Baú que entrega itens ao ser aberto.
    /// </summary>
    public class Chest : Systems.Interactable
    {
        public ItemData[] contents;
        public GameObject openVisual;
        public GameObject closedVisual;
        public bool isLocked = false;
        public bool requiresKey = false;

        bool isOpen;

        public override void Interact()
        {
            if (isOpen) return;

            if (isLocked || requiresKey)
            {
                // Verifica se tem chave no inventário
                var inv = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player.Inventory>();
                if (inv == null || inv.GetQuantity(ItemType.Key) <= 0)
                {
                    Debug.Log("Baú trancado! Precisa de uma chave.");
                    return;
                }
                // Consome a chave (opcional)
            }

            Open();
        }

        void Open()
        {
            isOpen = true;
            if (closedVisual != null) closedVisual.SetActive(false);
            if (openVisual != null) openVisual.SetActive(true);

            var inv = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player.Inventory>();
            if (inv != null && contents != null)
            {
                foreach (var item in contents)
                {
                    if (item != null)
                        inv.AddItem(item, 1);
                }
            }

            Debug.Log("Baú aberto!");
        }
    }
}
