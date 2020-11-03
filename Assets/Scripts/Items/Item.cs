using System;
using UnityEngine;

namespace Items
{
    [Serializable]
    public enum ItemType
    {
        Protection,
        Weapon,
        Armor,
        InventoryUpgrade,
        PowerUp,
        Other,
    }

    [Serializable]
    public enum PowerUpType
    {
        Health,
        Hunger,
        Hygiene,
        Damage,
        Speed
    }
    [CreateAssetMenu(fileName = "Item",menuName = "Item", order = 2)]
    public class Item : ScriptableObject
    {
        public string itemName;
        [TextArea(1,3)]
        public string description;
        public Sprite icon;
        public int spaceRequired;
        public ItemType type;
        public int damage;
        public float fireRate;
        public AttackMode mode;
        public float range;
        public GameObject projectile;
        [Range(0,1)]
        public float toughness;
        [Range(0,1)]
        public float protectionLevel;
        public int backpackCapacity;
        public float powerUpCooldown;
        public PowerUpType powerUpType;
        public float powerUpMultiplier = 1;
    }
}