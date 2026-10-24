using UnityEngine;

namespace Zeldinha.Items
{
    public enum ItemType
    {
        HealthPotion,
        StaminaPotion,
        LightFragment,
        Key,
        Weapon,
        Shield,
        Misc
    }

    /// <summary>
    /// ScriptableObject que define um item do jogo.
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Zeldinha/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        [TextArea] public string description;
        public ItemType itemType;
        public Sprite icon;
        public GameObject worldPrefab; // modelo 3D quando dropado no chão
        public int maxStack = 1;
        public float value; // quanto cura / quanto dano / etc.
        public bool isConsumable = true;
    }
}
