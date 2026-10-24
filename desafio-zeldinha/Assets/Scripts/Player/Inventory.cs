using UnityEngine;
using System.Collections.Generic;
using System;

namespace Zeldinha.Player
{
    /// <summary>
    /// Sistema de inventário simples do jogador.
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        [Serializable]
        public class InventorySlot
        {
            public Items.ItemData item;
            public int quantity;
        }

        public List<InventorySlot> items = new List<InventorySlot>();
        public int maxSlots = 12;

        public event Action OnInventoryChanged;

        PlayerStats stats;

        void Awake()
        {
            stats = GetComponent<PlayerStats>();
        }

        void Update()
        {
            // Atalhos rápidos
            if (Input.GetKeyDown(KeyCode.Alpha1)) UseItemByType(Items.ItemType.HealthPotion);
            if (Input.GetKeyDown(KeyCode.Alpha2)) UseItemByType(Items.ItemType.StaminaPotion);
        }

        public bool AddItem(Items.ItemData item, int amount = 1)
        {
            if (item == null) return false;

            // Stack se possível
            foreach (var slot in items)
            {
                if (slot.item == item && slot.quantity < item.maxStack)
                {
                    int canAdd = Mathf.Min(amount, item.maxStack - slot.quantity);
                    slot.quantity += canAdd;
                    amount -= canAdd;
                    if (amount <= 0)
                    {
                        OnInventoryChanged?.Invoke();
                        return true;
                    }
                }
            }

            // Novo slot
            while (amount > 0 && items.Count < maxSlots)
            {
                int add = Mathf.Min(amount, item.maxStack);
                items.Add(new InventorySlot { item = item, quantity = add });
                amount -= add;
            }

            OnInventoryChanged?.Invoke();
            return amount <= 0;
        }

        public bool UseItemByType(Items.ItemType type)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item.itemType == type)
                {
                    return UseItemAt(i);
                }
            }
            return false;
        }

        public bool UseItemAt(int index)
        {
            if (index < 0 || index >= items.Count) return false;

            var slot = items[index];
            if (slot.item == null || slot.quantity <= 0) return false;

            // Aplica efeito
            ApplyItemEffect(slot.item);

            slot.quantity--;
            if (slot.quantity <= 0)
            {
                items.RemoveAt(index);
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        void ApplyItemEffect(Items.ItemData item)
        {
            switch (item.itemType)
            {
                case Items.ItemType.HealthPotion:
                    stats.Heal(item.value);
                    break;
                case Items.ItemType.StaminaPotion:
                    stats.RestoreStamina(item.value);
                    break;
                case Items.ItemType.LightFragment:
                    Systems.GameManager.Instance?.CollectLightFragment();
                    // Bônus permanente
                    if (Systems.GameManager.Instance.lightFragmentsCollected == 1)
                        stats.IncreaseMaxHealth(25f);
                    else if (Systems.GameManager.Instance.lightFragmentsCollected == 2)
                        stats.IncreaseMaxStamina(30f);
                    else if (Systems.GameManager.Instance.lightFragmentsCollected == 3)
                        stats.attackPower += 8;
                    break;
            }
        }

        public int GetQuantity(Items.ItemType type)
        {
            int total = 0;
            foreach (var slot in items)
            {
                if (slot.item != null && slot.item.itemType == type)
                    total += slot.quantity;
            }
            return total;
        }
    }
}
